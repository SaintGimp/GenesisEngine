using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Machine.Specifications;
using Rhino.Mocks;
using Microsoft.Xna.Framework;

namespace GenesisEngine.Specs.DomainSpecs
{
    [Subject(typeof(QuadMesh))]
    public class when_the_mesh_is_initialized : QuadMeshContext
    {
        Because of = () =>
            _mesh.Initialize(_radius, Vector3.Up, Vector3.Backward, Vector3.Right, _extents, 7);

        It should_initialize_the_renderer = () =>
            _renderer.InitializeWasCalled.ShouldBeTrue();

        It should_remember_its_level = () =>
            _mesh.Level.ShouldEqual(7);
    }

    [Subject(typeof(QuadMesh))]
    public class when_a_top_facing_mesh_creates_a_mesh : QuadMeshContext
    {
        Because of = () =>
            _mesh.Initialize(_radius, Vector3.Up, Vector3.Backward, Vector3.Right, _extents, 0);

        It should_project_center_point_into_spherical_mesh_space = () =>
        {
            var centerPosition = _renderer.Vertices[_renderer.Vertices.Length / 2].Position;
            centerPosition.ShouldBeCloseTo(Vector3.Zero);
        };

        It should_calculate_center_point_normal = () =>
        {
            var centerNormal = _renderer.Vertices[_renderer.Vertices.Length / 2].Normal;
            centerNormal.ShouldBeCloseTo(Vector3.Up);
        };

        It should_project_top_left_corner_into_spherical_mesh_space = () =>
        {
            var vertex = _renderer.Vertices[0].Position;
            AssertCornerIsProjected(vertex, Vector3.Up, Vector3.Left, Vector3.Forward);
        };

        It should_project_bottom_right_corner_into_spherical_mesh_space = () =>
        {
            var vertex = _renderer.Vertices[_renderer.Vertices.Length - 1].Position;
            AssertCornerIsProjected(vertex, Vector3.Up, Vector3.Backward, Vector3.Right);
        };

        // TODO: verify that it captures corner and center samples?
    }

    [Subject(typeof(QuadMesh))]
    public class when_a_mesh_is_below_the_horizon : QuadMeshContext
    {
        Establish context = () =>
            _mesh.Initialize(10, Vector3.Up, Vector3.Backward, Vector3.Right, _extents, 5);

        Because of = () =>
            _mesh.Update(new TimeSpan(), DoubleVector3.Down * 100, DoubleVector3.Zero, _clippingPlanes);

        It should_not_be_visible = () =>
            _mesh.IsVisibleToCamera.ShouldBeFalse();
    }

    [Subject(typeof(QuadMesh))]
    public class when_a_mesh_is_not_below_the_horizon : QuadMeshContext
    {
        Establish context = () =>
            _mesh.Initialize(10, Vector3.Up, Vector3.Backward, Vector3.Right, _extents, 5);

        Because of = () =>
            _mesh.Update(new TimeSpan(), DoubleVector3.Up * 100, DoubleVector3.Zero, _clippingPlanes);

        It should_be_visible = () =>
            _mesh.IsVisibleToCamera.ShouldBeTrue();
    }

    //[Subject(typeof(QuadMesh))]
    //public class when_a_mesh_is_updated : QuadMeshContext
    //{
    //    Establish context = () =>
    //    {
    //        _mesh.Initialize(10, Vector3.Up, Vector3.Backward, Vector3.Right, _extents, 0);
    //    };

    //    Because of = () =>
    //        _mesh.Update();
    //}

    [Subject(typeof(QuadMesh))]
    public class when_a_mesh_is_drawn : QuadMeshContext
    {
        public static DoubleVector3 _cameraLocation;
        public static Matrix _viewMatrix;
        public static Matrix _projectionMatrix;

        Establish context = () =>
        {
            _cameraLocation = DoubleVector3.Up;
            _viewMatrix = Matrix.Identity;
            _projectionMatrix = Matrix.Identity;

            _mesh.Initialize(10, Vector3.Up, Vector3.Backward, Vector3.Right, new QuadNodeExtents(-1.0, 1.0, -1.0, 1.0), 0);
        };

        Because of = () =>
            _mesh.Draw(_cameraLocation, _viewMatrix, _projectionMatrix);

        It should_draw_the_mesh = () =>
            _renderer.DrawWasCalled.ShouldBeTrue();

        It should_draw_the_mesh_in_the_correct_location = () =>
            _renderer.Location.ShouldEqual(Vector3.Up * 10);
    }

    [Subject(typeof(QuadMesh))]
    public class when_a_mesh_is_disposed : QuadMeshContext
    {
        Establish context = () =>
            _mesh.Initialize(_radius, Vector3.Up, Vector3.Backward, Vector3.Right, _extents, 0);

        Because of = () =>
            _mesh.Dispose();

        It should_dispose_the_renderer = () =>
            _renderer.DisposeWasCalled.ShouldBeTrue();
    }

    public class QuadMeshContext
    {
        public static readonly float _radius = 1;
        public static DoubleVector3 _location;
        public static QuadNodeExtents _extents = new QuadNodeExtents(-1.0, 1.0, -1.0, 1.0);

        public static IHeightfieldGenerator _generator;
        public static MockQuadMeshRenderer _renderer;
        public static ISettings _settings;
        public static Statistics _statistics;
        public static ClippingPlanes _clippingPlanes;

        public static QuadMesh _mesh;

        Establish context = () =>
        {
            _generator = MockRepository.GenerateStub<IHeightfieldGenerator>();

            // We're using a hand-rolled fake here because of a bug
            // in .Net that prevents mocking of multi-dimentional arrays:
            // http://code.google.com/p/moq/issues/detail?id=182#c0
            _renderer = new MockQuadMeshRenderer();

            _settings = MockRepository.GenerateStub<ISettings>();

            _statistics = new Statistics();

            _clippingPlanes = new ClippingPlanes();

            _mesh = new QuadMesh(_generator, _renderer, _settings);
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

    public class MockQuadMeshRenderer : IQuadMeshRenderer, IDisposable
    {
        public bool InitializeWasCalled { get; private set; }
        public bool DrawWasCalled { get; private set; }
        public bool DisposeWasCalled { get; private set; }
        public Vector3 Location { get; private set; }
        public VertexPositionNormalColored[] Vertices { get; private set; }

        public virtual void Initialize(VertexPositionNormalColored[] vertices, int[] indices)
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
