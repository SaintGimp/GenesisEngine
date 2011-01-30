using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace GenesisEngine
{
    public class QuadNode : IQuadNode, IDisposable
    {
        DoubleVector3 _locationRelativeToPlanet;
        double _planetRadius;
        DoubleVector3 _uVector;
        DoubleVector3 _vVector;
        DoubleVector3 _planeNormalVector;
        protected QuadNodeExtents _extents;

        bool _hasSubnodes;
        protected bool _splitInProgress;
        protected bool _mergeInProgress;
        protected Task _splitCompletionTask;
        protected Task _backgroundMergeTask;
        protected CancellationTokenSource _cancellationTokenSource;
        protected List<IQuadNode> _subnodes = new List<IQuadNode>();

        readonly IQuadMesh _mesh;
        readonly IQuadNodeFactory _quadNodeFactory;
        readonly ISplitMergeStrategy _splitMergeStrategy;
        readonly ITaskSchedulerFactory _taskSchedulerFactory;
        readonly IQuadNodeRenderer _renderer;
        readonly Statistics _statistics;

        public QuadNode(IQuadMesh mesh, IQuadNodeFactory quadNodeFactory, ISplitMergeStrategy splitMergeStrategy, ITaskSchedulerFactory taskSchedulerFactory, IQuadNodeRenderer renderer, Statistics statistics)
        {
            _mesh = mesh;
            _quadNodeFactory = quadNodeFactory;
            _splitMergeStrategy = splitMergeStrategy;
            _taskSchedulerFactory = taskSchedulerFactory;
            _renderer = renderer;
            _statistics = statistics;
        }

        public int Level { get; private set; }

        // TODO: push this data in through the constructor, probably in a QuadNodeDefintion class, and make
        // this method private.  Except that would do real work in construction.  Hmmm.
        public void Initialize(double planetRadius, DoubleVector3 planeNormalVector, DoubleVector3 uVector, DoubleVector3 vVector, QuadNodeExtents extents, int level)
        {
            _planetRadius = planetRadius;
            _planeNormalVector = planeNormalVector;
            _uVector = uVector;
            _vVector = vVector;
            _extents = extents;
            Level = level;

            _locationRelativeToPlanet = (_planeNormalVector) + (_uVector * (_extents.North + (_extents.Width / 2.0))) + (_vVector * (_extents.West + (_extents.Width / 2.0)));
            _locationRelativeToPlanet = _locationRelativeToPlanet.ProjectUnitPlaneToUnitSphere() * _planetRadius;

            _mesh.Initialize(planetRadius, planeNormalVector, uVector, vVector, extents, level);

            Interlocked.Increment(ref _statistics.NumberOfQuadNodes);
            Interlocked.Increment(ref _statistics.NumberOfQuadNodesAtLevel[Level]);
        }

        public void Update(DoubleVector3 cameraLocation, DoubleVector3 planetLocation)
        {
            _mesh.Update(cameraLocation, planetLocation);

            if (ShouldSplit())
            {
                Split(cameraLocation, planetLocation);
            }
            else if (ShouldMerge())
            {
                Merge();
            }
            else if (ShouldCancelSplit())
            {
                CancelSplit();
            }

            UpdateSubnodes(cameraLocation, planetLocation);
        }

        bool ShouldSplit()
        {
            return IsSplittable() && _splitMergeStrategy.ShouldSplit(_mesh, Level);
        }

        bool IsSplittable()
        {
            return !_hasSubnodes && !(_splitInProgress || _mergeInProgress);
        }

        bool ShouldMerge()
        {
            return IsMergable() && _splitMergeStrategy.ShouldMerge(_mesh);
        }

        bool IsMergable()
        {
            return _hasSubnodes && !(_splitInProgress || _mergeInProgress);
        }

        bool ShouldCancelSplit()
        {
            return _splitInProgress && !_cancellationTokenSource.IsCancellationRequested && !_splitMergeStrategy.ShouldSplit(_mesh, Level);
        }

        void CancelSplit()
        {
            Interlocked.Increment(ref _statistics.NumberOfSplitsCanceledPerInterval);

            _cancellationTokenSource.Cancel();
        }

        void UpdateSubnodes(DoubleVector3 cameraLocation, DoubleVector3 planetLocation)
        {
            if (_hasSubnodes)
            {
                foreach (var subnode in _subnodes)
                {
                    subnode.Update(cameraLocation, planetLocation);
                }
            }
        }

        private void Split(DoubleVector3 cameraLocation, DoubleVector3 planetLocation)
        {
            // TODO: should we bother to write specs for the threading behavior?

            Interlocked.Increment(ref _statistics.NumberOfSplitsScheduledPerInterval);
            Interlocked.Increment(ref _statistics.NumberOfPendingSplits);

            CreateCancellationTokenSource();
            var splitTasks = CreateBackgroundSplitTasks(cameraLocation, planetLocation);
            CreateSplitCompletionTask(splitTasks);
        }

        void CreateCancellationTokenSource()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        List<Task<IQuadNode>> CreateBackgroundSplitTasks(DoubleVector3 cameraLocation, DoubleVector3 planetLocation)
        {
            // TODO: there's a problem with this algorithm.  If we need to split very deeply because for example
            // the camera is teleported to the surface, we only split one level at a time and wait for that level
            // to finish and for the next update sweep to occur before queuing splits for the next level.  There
            // may be wasted time in there (not clear yet).  We also spend time generating meshes for quads that
            // we know we don't need.  Ideally we'd jump straight to rendering meshes for the new leaves and worry
            // about rendering meshes for the intermediate nodes later when we need them.  This means we need a
            // general way to delay completing a merge (continuing to render its children in the meantime) until
            // a mesh is rendered for that node.  Once we have that behavior
            // we can maybe throw away the vertex buffers for all non-leaf meshes to save memory and regenerate them
            // as needed.  That would take more CPU overall but it's not time sensitive because we'd just keep
            // rendering the child nodes until we got around to it.  That would be a problem only if we end up
            // overloading the GPU but in many cases that wouldn't happen because the merging nodes would be behind
            // the camera as it travels and thus not rendered.
            // Potential problem: we don't know for sure if a node needs to be split until after we generate its mesh.

            _splitInProgress = true;
            var subextents = _extents.Split();

            return subextents.Select(extent => Task<IQuadNode>.Factory.StartNew(() =>
            {
                var node = _quadNodeFactory.Create();
                node.Initialize(_planetRadius, _planeNormalVector, _uVector, _vVector, extent, Level + 1);
                node.Update(cameraLocation, planetLocation);
                return node;
            }, _cancellationTokenSource.Token, TaskCreationOptions.None, _taskSchedulerFactory.CreateForLevel(Level))).ToList();
        }

        void CreateSplitCompletionTask(List<Task<IQuadNode>> tasks)
        {
            _splitCompletionTask = Task.Factory.ContinueWhenAll(tasks.ToArray(), finishedTasks =>
            {
                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    StoreCompletedSubnodes(finishedTasks);
                }
                else
                {
                    // TODO: this class is coupled too closely with the test-unfriendly TPL so we can't
                    // easily write tests for this kind of required behavior.  How to fix?
                    DisposeCompletedSubnodes(finishedTasks);
                }

                _splitInProgress = false;

                Interlocked.Decrement(ref _statistics.NumberOfPendingSplits);
            }, CancellationToken.None, TaskContinuationOptions.None, _taskSchedulerFactory.CreateForLevel(Level));
        }

        void StoreCompletedSubnodes(IEnumerable<Task<IQuadNode>> finishedTasks)
        {
            foreach (var task in finishedTasks)
            {
                _subnodes.Add(task.Result);
            }

            _hasSubnodes = true;
        }

        void DisposeCompletedSubnodes(IEnumerable<Task<IQuadNode>> finishedTasks)
        {
            foreach (var task in finishedTasks.Where(task => task.Status == TaskStatus.RanToCompletion))
            {
                ((IDisposable)task.Result).Dispose();
            }
        }

        private void Merge()
        {
            Interlocked.Increment(ref _statistics.NumberOfPendingMerges);

            // TODO: if a split is pending, cancel it
            _mergeInProgress = true;
            _hasSubnodes = false;

            CreateCancellationTokenSource();
            CreateBackgroundMergeTask();
        }

        void CreateBackgroundMergeTask()
        {
            _backgroundMergeTask = Task.Factory.StartNew(() =>
            {
                DisposeSubNodes();
                _subnodes.Clear();

                _mergeInProgress = false;

                Interlocked.Decrement(ref _statistics.NumberOfPendingMerges);
            }, _cancellationTokenSource.Token, TaskCreationOptions.None, _taskSchedulerFactory.Create());
        }

        void DisposeSubNodes()
        {
            // TODO: we have a race condition here where the parent node might
            // decide to dispose this node at the same time that this node is
            // being split.  We get an exception here because the _subNodes
            // collection is modified while we're iterating it.

            foreach (var node in _subnodes)
            {
                ((IDisposable)node).Dispose();
            }
        }

        public void Draw(DoubleVector3 cameraLocation, BoundingFrustum originBasedViewFrustum, Matrix originBasedViewMatrix, Matrix projectionMatrix)
        {
            if (_hasSubnodes)
            {
                // TODO: we'd like to stop traversing into subnodes if this node's mesh isn't visibile, but our
                // horizon culling algorithm isn't that great right now and the primary faces are so large that
                // sometimes all of their sample points are below the horizon even though we're above that face
                // and would want to draw its children.  For now, we'll scan all subnodes regardless.  The child
                // node's meshes will do visibility culling on an individual basis.
                foreach (var subnode in _subnodes)
                {
                    subnode.Draw(cameraLocation, originBasedViewFrustum, originBasedViewMatrix, projectionMatrix);
                }
            }
            else
            {
                _renderer.Draw(_locationRelativeToPlanet, cameraLocation, originBasedViewMatrix, projectionMatrix);
                _mesh.Draw(cameraLocation, originBasedViewFrustum, originBasedViewMatrix, projectionMatrix);
            }
        }

        public void Dispose()
        {
            ((IDisposable)_renderer).Dispose();
            ((IDisposable)_mesh).Dispose();
            DisposeSubNodes();

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }

            Interlocked.Decrement(ref _statistics.NumberOfQuadNodes);
            Interlocked.Decrement(ref _statistics.NumberOfQuadNodesAtLevel[Level]);
        }
    }
}

