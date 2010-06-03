using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace GenesisEngine
{
    public class QuadNode : IQuadNode, IDisposable
    {
        // TODO: this class is a serious SRP violation and needs to be refactored ASAP!

        DoubleVector3 _locationRelativeToPlanet;
        double _planetRadius;
        DoubleVector3 _uVector;
        DoubleVector3 _vVector;
        DoubleVector3 _planeNormalVector;
        protected QuadNodeExtents _extents;

        bool _hasSubnodes = false;
        protected List<IQuadNode> _subnodes = new List<IQuadNode>();

        IQuadMesh _mesh;
        IQuadNodeFactory _quadNodeFactory;
        IQuadNodeRenderer _renderer;
        readonly ISettings _settings;
        readonly Statistics _statistics;

        public QuadNode(IQuadMesh mesh, IQuadNodeFactory quadNodeFactory, IQuadNodeRenderer renderer, ISettings settings, Statistics statistics)
        {
            _mesh = mesh;
            _quadNodeFactory = quadNodeFactory;
            _renderer = renderer;
            _settings = settings;
            _statistics = statistics;
        }

        public int Level { get; private set; }

        // TODO: push this data in through the constructor, probably in a QuadNodeDefintion class, and make
        // this method private.  Except that would do real work in construction.  Hmmm.  When we explode this class
        // into separate responsibilites, this problem may go away.
        public void Initialize(double planetRadius, DoubleVector3 planeNormalVector, DoubleVector3 uVector, DoubleVector3 vVector, QuadNodeExtents extents, int level)
        {
            _planetRadius = planetRadius;
            _planeNormalVector = planeNormalVector;
            _uVector = uVector;
            _vVector = vVector;
            _extents = extents;
            Level = level;

            _locationRelativeToPlanet = (_planeNormalVector) + (_uVector * ((_extents.West + (_extents.Width / 2.0)))) + (_vVector * ((_extents.North + (_extents.Width / 2.0))));
            _locationRelativeToPlanet = _locationRelativeToPlanet.ProjectUnitPlaneToUnitSphere() * _planetRadius;

            _mesh.Initialize(planetRadius, planeNormalVector, uVector, vVector, extents, level);

            _statistics.NumberOfQuadNodes++;
            _statistics.NumberOfQuadNodesAtLevel[Level]++;
        }

        public void Update(TimeSpan elapsedTime, DoubleVector3 cameraLocation, DoubleVector3 planetLocation, ClippingPlanes clippingPlanes)
        {
            _mesh.Update(elapsedTime, cameraLocation, planetLocation, clippingPlanes);

            // TODO: This algorithm could be improved to optimize the number of triangles that are drawn

            if (_mesh.IsVisibleToCamera && _mesh.CameraDistanceToWidthRatio < 1 && !_hasSubnodes && Level < _settings.MaximumQuadNodeLevel)
            {
                Split();
            }
            else if (_mesh.CameraDistanceToWidthRatio > 1.2 && _hasSubnodes)
            {
                Merge();
            }

            if (_hasSubnodes)
            {
                foreach (var subnode in _subnodes)
                {
                    subnode.Update(elapsedTime, cameraLocation, planetLocation, clippingPlanes);
                }
            }
        }

        private void Split()
        {
            var subextents = _extents.Split();

            foreach (var subextent in subextents)
            {
                var node = _quadNodeFactory.Create();
                node.Initialize(_planetRadius, _planeNormalVector, _uVector, _vVector, subextent, Level + 1);
                _subnodes.Add(node);
            }

            _hasSubnodes = true;
        }

        private void Merge()
        {
            DisposeSubNodes();
            _subnodes.Clear();
            _hasSubnodes = false;
        }

        void DisposeSubNodes()
        {
            foreach (var node in _subnodes)
            {
                ((IDisposable)node).Dispose();
            }
        }

        public void Draw(DoubleVector3 cameraLocation, Matrix originBasedViewMatrix, Matrix projectionMatrix)
        {
            if (!_mesh.IsVisibleToCamera)
            {
                return;
            }

            if (_hasSubnodes)
            {
                foreach (var subnode in _subnodes)
                {
                    subnode.Draw(cameraLocation, originBasedViewMatrix, projectionMatrix);
                }
            }
            else
            {
                _renderer.Draw(_locationRelativeToPlanet, cameraLocation, originBasedViewMatrix, projectionMatrix);
                _mesh.Draw(cameraLocation, originBasedViewMatrix, projectionMatrix);
            }
        }

        public void Dispose()
        {
            ((IDisposable)_renderer).Dispose();
            DisposeSubNodes();

            _statistics.NumberOfQuadNodes--;
            _statistics.NumberOfQuadNodesAtLevel[Level]--;
        }
    }
}

