using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Machine.Specifications;
using Rhino.Mocks;
using Microsoft.Xna.Framework;

namespace GenesisEngine.Specs.DomainSpecs
{
    // TODO: the QuadNode class is a flagrant SRP violation and it shows in these tests.
    // The class needs to be refactored and the tests needs to be extended to fully
    // cover everything.

    [Subject(typeof(QuadNode))]
    public class when_a_node_is_initialized : QuadNodeContext
    {
        Because of = () =>
            _node.Initialize(_radius, Vector3.Up, Vector3.Backward, Vector3.Right, _extents, 7);

        It should_initialize_the_mesh = () =>
            _mesh.AssertWasCalled(x => x.Initialize(_radius, Vector3.Up, Vector3.Backward, Vector3.Right, _extents, 7));

        It should_increment_the_node_count_statistic = () =>
            _statistics.NumberOfQuadNodes.ShouldEqual(1);

        It should_increment_the_node_level_count_statistic = () =>
            _statistics.NumberOfQuadNodesAtLevel[7].ShouldEqual(1);

        It should_remember_its_level = () =>
            _node.Level.ShouldEqual(7);
    }

    [Subject(typeof(QuadNode))]
    public class when_a_leaf_node_is_updated : QuadNodeContext
    {
        Establish context = () =>
            InitializeNodeAsLeaf();

        Because of = () =>
            _node.Update(DoubleVector3.Up, DoubleVector3.Down);

        It should_update_the_mesh = () =>
            _mesh.AssertWasCalled(x => x.Update(DoubleVector3.Up, DoubleVector3.Down));
    }

    [Subject(typeof(QuadNode))]
    public class when_split_is_recommended_for_a_leaf_node : QuadNodeContext
    {
        Establish context = () =>
        {
            InitializeNodeAsLeaf();
            ConfigureStrategyForSplit();
        };

        Because of = () =>
            _node.Update(DoubleVector3.Zero, DoubleVector3.Zero);

        It should_split_into_subquads = () =>
            _node.Subnodes.Count.ShouldEqual(4);

        It should_create_subquads_at_the_next_level = () =>
        {
            foreach (var subnode in _node.Subnodes)
            {
                subnode.AssertWasCalled(x => x.Initialize(Arg<double>.Is.Anything, Arg<DoubleVector3>.Is.Anything, Arg<DoubleVector3>.Is.Anything, Arg<DoubleVector3>.Is.Anything, Arg<QuadNodeExtents>.Is.Anything, Arg.Is(6)));
            }
        };

        It should_create_subquads_smaller_than_this_quad;
        //{
        //    foreach (var subnode in _node.Subnodes)
        //    {
        //        // TODO: need to determine that the extents for the subnode are smaller than this one
        //    }
        //};
    }

    [Subject(typeof(QuadNode))]
    public class when_split_is_recommended_for_a_nonleaf_node : QuadNodeContext
    {
        Establish context = () =>
        {
            InitializeNodeAsNonleaf();
            ConfigureStrategyForSplit();
        };

        Because of = () =>
            _node.Update(DoubleVector3.Zero, DoubleVector3.Zero);

        It should_not_split_again = () =>
            _quadNodeFactory.AssertWasCalled(x => x.Create(), c => c.Repeat.Times(4));
    }

    [Subject(typeof(QuadNode))]
    public class when_split_is_recommended_but_a_split_is_already_in_progress : QuadNodeContext
    {
        Establish context = () =>
        {
            InitializeNodeAsLeaf();
            _node.ConfigureAsSplitInProgress();
            ConfigureStrategyForSplit();
        };

        Because of = () =>
            _node.Update(DoubleVector3.Zero, DoubleVector3.Zero);

        It should_not_split_again = () =>
            _quadNodeFactory.AssertWasNotCalled(x => x.Create());
    }

    [Subject(typeof(QuadNode))]
    public class when_split_is_recommended_but_a_merge_is_in_progress : QuadNodeContext
    {
        Establish context = () =>
        {
            InitializeNodeAsLeaf();
            _node.ConfigureAsSplitInProgress();
            ConfigureStrategyForMerge();
        };

        Because of = () =>
            _node.Update(DoubleVector3.Zero, DoubleVector3.Zero);

        It should_not_split = () =>
            _quadNodeFactory.AssertWasNotCalled(x => x.Create());
    }

    [Subject(typeof(QuadNode))]
    public class when_a_merge_is_recommended_for_a_nonleaf_node : QuadNodeContext
    {
        Establish context = () =>
        {
            InitializeNodeAsNonleaf();
            ConfigureStrategyForMerge();
        };

        Because of = () =>
            _node.Update(DoubleVector3.Zero, DoubleVector3.Zero);

        It should_remove_subnodes = () =>
            _node.Subnodes.Count.ShouldEqual(0);

        It should_dispose_subnodes = () =>
        {
            foreach (var subnode in _node.Subnodes)
            {
                ((IDisposable)subnode).AssertWasCalled(x => x.Dispose());
            }
        };
    }

    [Subject(typeof(QuadNode))]
    public class when_a_merge_is_recommended_for_a_leaf_node : QuadNodeContext
    {
        Establish context = () =>
        {
            InitializeNodeAsLeaf();
            ConfigureStrategyForMerge();
        };

        Because of = () =>
            _node.Update(DoubleVector3.Zero, DoubleVector3.Zero);

        It should_not_attempt_to_merge = () =>
            _node.WasMergeStarted.ShouldBeFalse();

        It should_dispose_subnodes = () =>
        {
            foreach (var subnode in _node.Subnodes)
            {
                ((IDisposable)subnode).AssertWasCalled(x => x.Dispose());
            }
        };
    }

    [Subject(typeof(QuadNode))]
    public class when_merge_is_recommended_but_a_split_is_in_progress : QuadNodeContext
    {
        Establish context = () =>
        {
            InitializeNodeAsNonleaf();
            _node.ConfigureAsSplitInProgress();
            ConfigureStrategyForMerge();
        };

        Because of = () =>
            _node.Update(DoubleVector3.Zero, DoubleVector3.Zero);

        It should_not_merge = () =>
            _node.WasMergeStarted.ShouldBeFalse();
    }

    [Subject(typeof(QuadNode))]
    public class when_merge_is_recommended_but_a_merge_is_already_in_progress : QuadNodeContext
    {
        Establish context = () =>
        {
            InitializeNodeAsNonleaf();
            _node.ConfigureAsMergeInProgress();
            ConfigureStrategyForMerge();
        };

        Because of = () =>
            _node.Update(DoubleVector3.Zero, DoubleVector3.Zero);

        It should_not_merge_again = () =>
            _node.WasMergeStarted.ShouldBeFalse();
    }

    [Subject(typeof(QuadNode))]
    public class when_a_nonleaf_node_is_updated : QuadNodeContext
    {
        Establish context = () =>
            InitializeNodeAsNonleaf();

        Because of = () =>
            _node.Update(DoubleVector3.Zero, DoubleVector3.Zero);

        It should_update_subquads = () =>
        {
            foreach (var subnode in _node.Subnodes)
            {
                subnode.AssertWasCalled(x => x.Update(DoubleVector3.Zero, DoubleVector3.Zero));
            }
        };
    }

    // TODO: update statistics

    [Subject(typeof(QuadNode))]
    public class when_a_leaf_node_is_drawn : QuadNodeContext
    {
        public static DoubleVector3 _cameraLocation;
        public static BoundingFrustum _viewFrustum;
        public static Matrix _viewMatrix;
        public static Matrix _projectionMatrix;

        Establish context = () =>
        {
            _cameraLocation = DoubleVector3.Up;
            _viewFrustum = new BoundingFrustum(Matrix.Identity);
            _viewMatrix = Matrix.Identity;
            _projectionMatrix = Matrix.Identity;

            InitializeNodeAsLeaf();
        };

        Because of = () =>
            _node.Draw(_cameraLocation, _viewFrustum, _viewMatrix, _projectionMatrix);

        It should_draw_the_node = () =>
            _renderer.AssertWasCalled(x => x.Draw(_location, _cameraLocation, _viewMatrix, _projectionMatrix));

        It should_draw_the_mesh = () =>
            _mesh.AssertWasCalled(x => x.Draw(_cameraLocation, _viewFrustum, _viewMatrix, _projectionMatrix));
    }
    
    [Subject(typeof(QuadNode))]
    public class when_a_nonleaf_node_is_drawn : QuadNodeContext
    {
        public static DoubleVector3 _cameraLocation;
        public static BoundingFrustum _viewFrustum;
        public static Matrix _viewMatrix;
        public static Matrix _projectionMatrix;

        Establish context = () =>
        {
            _mesh.Stub(x => x.IsAboveHorizonToCamera).Return(true);

            _cameraLocation = DoubleVector3.Up;
            _viewFrustum = new BoundingFrustum(Matrix.Identity);
            _viewMatrix = Matrix.Identity;
            _projectionMatrix = Matrix.Identity;

            InitializeNodeAsNonleaf();
        };

        Because of = () =>
            _node.Draw(_cameraLocation, _viewFrustum, _viewMatrix, _projectionMatrix);

        It should_not_draw_the_node = () =>
            _renderer.AssertWasNotCalled(x => x.Draw(Arg<DoubleVector3>.Is.Anything, Arg<DoubleVector3>.Is.Anything, Arg<Matrix>.Is.Anything, Arg<Matrix>.Is.Anything));

        It should_not_draw_the_mesh = () =>
            _mesh.AssertWasNotCalled(x => x.Draw(Arg<DoubleVector3>.Is.Anything, Arg<BoundingFrustum>.Is.Anything, Arg<Matrix>.Is.Anything, Arg<Matrix>.Is.Anything));

        It should_draw_the_subnodes_in_the_correct_location = () =>
        {
            foreach (var subnode in _node.Subnodes)
            {
                subnode.AssertWasCalled(x => x.Draw(_cameraLocation, _viewFrustum, _viewMatrix, _projectionMatrix));
            }
        };
    }

    [Subject(typeof(QuadNode))]
    public class when_a_leaf_node_is_disposed : QuadNodeContext
    {
        Establish context = () =>
            InitializeNodeAsLeaf();

        Because of = () =>
            _node.Dispose();

        It should_dispose_the_renderer = () =>
            ((IDisposable)_renderer).AssertWasCalled(x => x.Dispose());

        It should_dispose_the_mesh = () =>
            ((IDisposable)_mesh).AssertWasCalled(x => x.Dispose());

        It should_decrement_the_number_of_nodes = () =>
            _statistics.NumberOfQuadNodes.ShouldEqual(0);
    }

    [Subject(typeof(QuadNode))]
    public class when_a_nonleaf_node_is_disposed : QuadNodeContext
    {
        Establish context = () =>
            InitializeNodeAsNonleaf();

        Because of = () =>
            _node.Dispose();

        It should_dispose_the_renderer = () =>
            ((IDisposable)_renderer).AssertWasCalled(x => x.Dispose());

        It should_dispose_the_mesh = () =>
            ((IDisposable) _mesh).AssertWasCalled(x => x.Dispose());

        It should_dispose_the_subnodes = () =>
        {
            foreach (var subnode in _node.Subnodes)
            {
                ((IDisposable)subnode).AssertWasCalled(x => x.Dispose());
            }
        };

        It should_decrement_the_number_of_nodes = () =>
            _statistics.NumberOfQuadNodes.ShouldEqual(0);
    }

    public class QuadNodeContext
    {
        public static readonly float _radius = 10;
        public static DoubleVector3 _location = DoubleVector3.Up * _radius;
        public static QuadNodeExtents _extents = new QuadNodeExtents(-1.0, 1.0, -1.0, 1.0);

        public static IQuadMesh _mesh;
        public static IQuadNodeFactory _quadNodeFactory;
        public static ISplitMergeStrategy _splitMergeStrategy;
        public static IQuadNodeRenderer _renderer;
        public static Statistics _statistics;

        public static TestableQuadNode _node;

        Establish context = () =>
        {
            _mesh = MockRepository.GenerateMock<IQuadMesh, IDisposable>();

            _quadNodeFactory = MockRepository.GenerateStub<IQuadNodeFactory>();
            _quadNodeFactory.Stub(x => x.Create()).Do((Func<IQuadNode>)(() => (IQuadNode)MockRepository.GenerateMock<IQuadNode, IDisposable>()));

            _splitMergeStrategy = MockRepository.GenerateStub<ISplitMergeStrategy>();

            // We're using a hand-rolled fake here because of a bug
            // in .Net that prevents mocking of multi-dimentional arrays:
            // http://code.google.com/p/moq/issues/detail?id=182#c0
            _renderer = MockRepository.GenerateMock<IQuadNodeRenderer, IDisposable>();

            _statistics = new Statistics();

            _node = new TestableQuadNode(_mesh, _quadNodeFactory, _splitMergeStrategy, _renderer, _statistics);
        };

        public static void InitializeNodeAsLeaf()
        {
            _node.Initialize(_radius, Vector3.Up, Vector3.Backward, Vector3.Right, _extents, 5);
        }

        public static void InitializeNodeAsNonleaf()
        {
            _node.Initialize(_radius, Vector3.Up, Vector3.Backward, Vector3.Right, _extents, 5);

            ConfigureStrategyForSplit();
            _node.Update(Vector3.Up, Vector3.Down);
        }

        public static void ConfigureStrategyForSplit()
        {
            _splitMergeStrategy.Stub(x => x.ShouldSplit(Arg<IQuadMesh>.Is.Anything, Arg<int>.Is.Anything)).Return(true).Repeat.Once();
        }

        public static void ConfigureStrategyForMerge()
        {
            _splitMergeStrategy.Stub(x => x.ShouldMerge(Arg<IQuadMesh>.Is.Anything)).Return(true).Repeat.Once();
        }
    }

    public class TestableQuadNode : QuadNode
    {
        public TestableQuadNode(IQuadMesh mesh, IQuadNodeFactory quadNodeFactory, ISplitMergeStrategy splitMergeStrategy, IQuadNodeRenderer renderer, Statistics statistics)
            : base(mesh, quadNodeFactory, splitMergeStrategy, new CurrentThreadTaskSchedulerFactory(), renderer, statistics)
        {
            // We create this version of the QuadNode with a task scheduler type that runs everything on the
            // current thread, i.e. everything is synchronous and there are no threading issues to worry about.
        }

        public IList<IQuadNode> Subnodes
        {
            get { return _subnodes; }
        }

        public double Width
        {
            get { return _extents.Width; }
        }

        public bool WasMergeStarted
        {
            get { return _backgroundMergeTask != null; }
        }

        public void ConfigureAsSplitInProgress()
        {
            _splitInProgress = true;
        }

        public void ConfigureAsMergeInProgress()
        {
            _mergeInProgress = true;
        }
    }
}
