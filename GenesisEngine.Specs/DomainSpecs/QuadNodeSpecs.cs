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
    public class when_a_node_is_updated : QuadNodeContext
    {
        Establish context = () =>
            _node.Initialize(10, Vector3.Up, Vector3.Backward, Vector3.Right, _extents, 5);

        Because of = () =>
            _node.Update(DoubleVector3.Up * 11, DoubleVector3.Zero);

        It should_update_the_mesh = () =>
            _mesh.AssertWasCalled(x => x.Update(DoubleVector3.Up * 11, DoubleVector3.Zero));
    }

    [Subject(typeof(QuadNode))]
    public class when_a_leaf_node_is_updated_and_the_camera_is_close : QuadNodeContext
    {
        Establish context = () =>
        {
            _mesh.Stub(x => x.IsAboveHorizonToCamera).Return(true);

            _node.Initialize(10, Vector3.Up, Vector3.Backward, Vector3.Right, _extents, 5);
        };

        Because of = () =>
        {
            _node.Update(DoubleVector3.Up * 11, DoubleVector3.Zero);
            _node.WaitForSplitToComplete();
        };

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
    public class when_a_leaf_node_is_updated_more_than_once_and_the_camera_is_close : QuadNodeContext
    {
        Establish context = () =>
        {
            _mesh.Stub(x => x.IsAboveHorizonToCamera).Return(true);

            _node.Initialize(10, Vector3.Up, Vector3.Backward, Vector3.Right, _extents, 0);
            _node.Update(DoubleVector3.Up * 11, DoubleVector3.Zero);
        };

        Because of = () =>
        {
            _node.Update(DoubleVector3.Up * 11, DoubleVector3.Zero);
            _node.WaitForSplitToComplete();
        };

        It should_split_only_once = () =>
            _node.Subnodes.Count.ShouldEqual(4);
    }

    [Subject(typeof(QuadNode))]
    public class when_a_max_level_leaf_node_is_updated_and_the_camera_is_close : QuadNodeContext
    {
        Establish context = () =>
        {
            _mesh.Stub(x => x.IsAboveHorizonToCamera).Return(true);

            _node.Initialize(10, Vector3.Up, Vector3.Backward, Vector3.Right, _extents, 19);
            _node.Update(DoubleVector3.Up * 11, DoubleVector3.Zero);
        };

        Because of = () =>
            _node.Update(DoubleVector3.Up * 11, DoubleVector3.Zero);

        It should_not_split = () =>
            _node.Subnodes.Count.ShouldEqual(0);
    }

    [Subject(typeof(QuadNode))]
    public class when_a_nonleaf_node_is_updated_and_the_camera_is_far : QuadNodeContext
    {
        public static DoubleVector3 _nearCameraLocation;
        public static DoubleVector3 _farCameraLocation;

        Establish context = () =>
        {
            _mesh.Stub(x => x.IsAboveHorizonToCamera).Return(true);

            _nearCameraLocation = DoubleVector3.Up * 11;
            _farCameraLocation = DoubleVector3.Up * 15 * 10 * 2;

            _node.Initialize(10, Vector3.Up, Vector3.Backward, Vector3.Right, _extents, 0);
            _node.Update(_nearCameraLocation, DoubleVector3.Zero);
            _node.WaitForSplitToComplete();

            _mesh.Stub(x => x.CameraDistanceToWidthRatio).Return(2);
        };

        Because of = () =>
        {
            _node.Update(_farCameraLocation, DoubleVector3.Zero);
            _node.WaitForMergeToComplete();
        };

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
    public class when_a_nonleaf_node_is_updated_and_the_camera_is_near : QuadNodeContext
    {
        public static DoubleVector3 _cameraLocation;

        Establish context = () =>
        {
            _cameraLocation = DoubleVector3.Up;

            _node.Initialize(10, Vector3.Up, Vector3.Backward, Vector3.Right, _extents, 0);
            _node.Update(_cameraLocation, DoubleVector3.Zero);
        };

        Because of = () =>
            _node.Update(_cameraLocation, DoubleVector3.Zero);

        It should_update_subquads = () =>
        {
            foreach (var subnode in _node.Subnodes)
            {
                subnode.AssertWasCalled(x => x.Update(_cameraLocation, DoubleVector3.Zero));
            }
        };
    }

    // TODO: update statistics

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

            _node.Initialize(10, Vector3.Up, Vector3.Backward, Vector3.Right, new QuadNodeExtents(-1.0, 1.0, -1.0, 1.0), 0);
            _node.Update(DoubleVector3.Up, DoubleVector3.Zero);
            _node.WaitForSplitToComplete();
        };

        Because of = () =>
            _node.Draw(_cameraLocation, _viewFrustum, _viewMatrix, _projectionMatrix);

        It should_not_draw_the_node = () =>
            _renderer.DrawWasCalled.ShouldBeFalse();

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

            _node.Initialize(10, Vector3.Up, Vector3.Backward, Vector3.Right, new QuadNodeExtents(-1.0, 1.0, -1.0, 1.0), 0);
        };

        Because of = () =>
            _node.Draw(_cameraLocation, _viewFrustum, _viewMatrix, _projectionMatrix);

        It should_draw_the_node = () =>
            _renderer.DrawWasCalled.ShouldBeTrue();

        It should_draw_the_mesh = () =>
            _mesh.AssertWasCalled(x => x.Draw(_cameraLocation, _viewFrustum, _viewMatrix, _projectionMatrix));

        It should_draw_the_node_in_the_correct_location = () =>
            _renderer.Location.ShouldEqual(Vector3.Up * 10);
    }

    [Subject(typeof(QuadNode))]
    public class when_a_leaf_node_is_disposed : QuadNodeContext
    {
        Establish context = () =>
            _node.Initialize(_radius, Vector3.Up, Vector3.Backward, Vector3.Right, _extents, 0);

        Because of = () =>
            _node.Dispose();

        It should_dispose_the_renderer = () =>
            _renderer.DisposeWasCalled.ShouldBeTrue();

        It should_decrement_the_number_of_nodes = () =>
            _statistics.NumberOfQuadNodes.ShouldEqual(0);
    }

    [Subject(typeof(QuadNode))]
    public class when_a_nonleaf_node_is_disposed : QuadNodeContext
    {
        Establish context = () =>
        {
            _node.Initialize(_radius, Vector3.Up, Vector3.Backward, Vector3.Right, _extents, 0);
            _node.Update(DoubleVector3.Up, DoubleVector3.Zero);
        };

        Because of = () =>
            _node.Dispose();

        It should_dispose_the_renderer = () =>
            _renderer.DisposeWasCalled.ShouldBeTrue();

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
        public static readonly float _radius = 1;
        public static DoubleVector3 _location;
        public static QuadNodeExtents _extents = new QuadNodeExtents(-1.0, 1.0, -1.0, 1.0);

        public static IQuadMesh _mesh;
        public static IQuadNodeFactory _quadNodeFactory;
        public static MockQuadNodeRenderer _renderer;
        public static ISettings _settings;
        public static Statistics _statistics;

        public static TestableQuadNode _node;

        Establish context = () =>
        {
            _mesh = MockRepository.GenerateStub<IQuadMesh>();

            _quadNodeFactory = MockRepository.GenerateStub<IQuadNodeFactory>();
            _quadNodeFactory.Stub(x => x.Create()).Do((Func<IQuadNode>)(() => (IQuadNode)MockRepository.GenerateMock(typeof(IQuadNode), new Type[] { typeof(IDisposable) })));

            // We're using a hand-rolled fake here because of a bug
            // in .Net that prevents mocking of multi-dimentional arrays:
            // http://code.google.com/p/moq/issues/detail?id=182#c0
            _renderer = new MockQuadNodeRenderer();

            _settings = MockRepository.GenerateStub<ISettings>();
            _settings.MaximumQuadNodeLevel = 19;

            _statistics = new Statistics();

            _node = new TestableQuadNode(_mesh, _quadNodeFactory, _renderer, _settings, _statistics);
        };

        public static void AssertCornerIsProjected(Vector3 projectedVector, Vector3 normalVector, Vector3 uVector, Vector3 vVector)
        {
            var expectedVector = (normalVector * _radius) + (uVector * _radius) + (vVector * _radius);
            expectedVector.Normalize();
            expectedVector *= _radius;
            expectedVector -= normalVector * _radius;

            projectedVector.ShouldBeCloseTo(expectedVector);
        }
    }

    public class TestableQuadNode : QuadNode
    {
        public TestableQuadNode(IQuadMesh mesh, IQuadNodeFactory quadNodeFactory, IQuadNodeRenderer renderer, ISettings settings, Statistics statistics)
            : base(mesh, quadNodeFactory, renderer, settings, statistics)
        {
        }

        public IList<IQuadNode> Subnodes
        {
            get { return _subnodes; }
        }

        public double Width
        {
            get { return _extents.Width; }
        }

        public void WaitForSplitToComplete()
        {
            _splitCompletionTask.Wait();
        }

        public void WaitForMergeToComplete()
        {
            _backgroundMergeTask.Wait();
        }
    }

    public class MockQuadNodeRenderer : IQuadNodeRenderer, IDisposable
    {
        public bool InitializeWasCalled { get; private set; }
        public bool DrawWasCalled { get; private set; }
        public bool DisposeWasCalled { get; private set; }
        public Vector3 Location { get; private set; }
        public VertexPositionNormalColor[] Vertices { get; private set; }

        public virtual void Initialize(VertexPositionNormalColor[] vertices, int[] indices)
        {
            this.InitializeWasCalled = true;
            this.Vertices = vertices;
        }

        public virtual void Draw(DoubleVector3 location, DoubleVector3 cameraLocation, Matrix originBasedViewMatrix, Matrix projectionMatrix)
        {
            this.DrawWasCalled = true;
            this.Location = location;
        }

        public void Dispose()
        {
            DisposeWasCalled = true;
        }
    }
}
