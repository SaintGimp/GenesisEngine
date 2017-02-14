using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Machine.Specifications;
using Microsoft.Xna.Framework;
using NSubstitute;

namespace GenesisEngine.Specs.DomainSpecs
{
    [Subject(typeof(Planet))]
    public class when_the_planet_is_drawn : PlanetContext
    {
        public static DoubleVector3 _cameraLocation;
        public static BoundingFrustum _viewFrustum;
        public static Matrix _viewMatrix;
        public static Matrix _projectionMatrix;
        public static ICamera _camera;

        Establish context = () =>
        {
            _cameraLocation = DoubleVector3.Up;
            _viewFrustum = new BoundingFrustum(Matrix.Identity);
            _viewMatrix = Matrix.Identity;
            _projectionMatrix = Matrix.Identity;

            _camera = Substitute.For<ICamera>();
            _camera.Location = _cameraLocation;
            _camera.OriginBasedViewFrustum.Returns(_viewFrustum);
            _camera.OriginBasedViewTransformation.Returns(_viewMatrix);
            _camera.ProjectionTransformation.Returns(_projectionMatrix);
        };

        Because of = () =>
            _planet.Draw(_camera);

        It should_draw_the_planet = () =>
            _planetRenderer.Received().Draw(_location, _cameraLocation, _viewMatrix, _projectionMatrix);

        It should_draw_the_terrain = () =>
            _terrain.Received().Draw(_cameraLocation, _viewFrustum, _viewMatrix, _projectionMatrix);
    }

    [Subject(typeof(Planet))]
    public class when_the_planet_is_updated : PlanetContext
    {
        Because of = () =>
            _planet.Update(DoubleVector3.Up);

        It should_update_the_terrain = () =>
            _terrain.Received().Update(Arg.Is(DoubleVector3.Up), Arg.Is(_location));
    }

    [Subject(typeof(Planet))]
    public class when_getting_the_ground_height_for_a_location : PlanetContext
    {
        public static DoubleVector3 _observerLocation;
        public static double _height;

        Establish context = () =>
        {
            _observerLocation = DoubleVector3.Forward * 100;
            _generator.GetHeight(DoubleVector3.Forward, 19, 8000).Returns(1234);
        };

        Because of = () =>
            _height = _planet.GetGroundHeight(_observerLocation);

        It should_get_the_height_underneath_the_location = () =>
            _height.ShouldEqual(1234 + _radius);
    }

    public class PlanetContext
    {
        public static double _radius;
        public static DoubleVector3 _location;

        public static IPlanetRenderer _planetRenderer;
        public static ITerrainFactory _terrainFactory;
        public static ITerrain _terrain;
        public static IHeightGenerator _generator;
        public static ISettings _settings;
        public static Statistics _statistics;
        public static IPlanet _planet;

        Establish context = () =>
        {
            _radius = 100;
            _location = DoubleVector3.Zero;

            _terrain = Substitute.For<ITerrain>();
            _planetRenderer = Substitute.For<IPlanetRenderer>();
            _generator = Substitute.For<IHeightGenerator>();
            _settings = Substitute.For<ISettings>();
            _statistics = new Statistics();

            _planet = new Planet(_location, _radius, _terrain, _planetRenderer, _generator, _settings, _statistics);
        };
    }
}
