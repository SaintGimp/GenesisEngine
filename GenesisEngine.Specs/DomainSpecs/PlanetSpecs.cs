using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Machine.Specifications;
using Microsoft.Xna.Framework;
using Rhino.Mocks;

namespace GenesisEngine.Specs.DomainSpecs
{
    [Subject(typeof(Planet))]
    public class when_the_planet_is_initialized : PlanetContext
    {
        Because of = () =>
            _planet.Initialize(_location, _radius);

        It should_initialize_the_renderer = () =>
            _planetRenderer.AssertWasCalled(x => x.Initialize(_radius));

        It should_create_terrain = () =>
            _terrainFactory.AssertWasCalled(x => x.Create(_radius));
    }

    [Subject(typeof(Planet))]
    public class when_the_planet_is_drawn : PlanetContext
    {
        public static DoubleVector3 _cameraLocation;
        public static Matrix _viewMatrix;
        public static Matrix _projectionMatrix;
        public static ICamera _camera;

        Establish context = () =>
        {
            _cameraLocation = DoubleVector3.Up;
            _viewMatrix = Matrix.Identity;
            _projectionMatrix = Matrix.Identity;

            _camera = MockRepository.GenerateStub<ICamera>();
            _camera.Location = _cameraLocation;
            _camera.Stub(x => x.OriginBasedViewTransformation).Return(_viewMatrix);
            _camera.Stub(x => x.ProjectionTransformation).Return(_projectionMatrix);

            _planet.Initialize(_location, _radius);
        };

        Because of = () =>
            _planet.Draw(_camera);

        It should_draw_the_planet = () =>
            _planetRenderer.AssertWasCalled(x => x.Draw(_location, _cameraLocation, _viewMatrix, _projectionMatrix));

        It should_draw_the_terrain = () =>
            _terrain.AssertWasCalled(x => x.Draw(_cameraLocation, _viewMatrix, _projectionMatrix));
    }

    [Subject(typeof(Planet))]
    public class when_the_planet_is_updated : PlanetContext
    {
        Establish context = () =>
            _planet.Initialize(_location, _radius);

        Because of = () =>
            _planet.Update(new TimeSpan(), DoubleVector3.Up);

        It should_update_the_terrain = () =>
            _terrain.AssertWasCalled(x => x.Update(Arg<TimeSpan>.Is.Anything, Arg.Is(DoubleVector3.Up), Arg.Is(_location), Arg<ClippingPlanes>.Is.Anything));
    }

    [Subject(typeof(Planet))]
    public class when_getting_the_ground_height_for_a_location : PlanetContext
    {
        public static DoubleVector3 _observerLocation;
        public static double _height;

        Establish context = () =>
        {
            _observerLocation = DoubleVector3.Forward * 100;
            _generator.Stub(x => x.GetHeight(DoubleVector3.Forward, 19, 8000)).Return(1234);

            _planet.Initialize(_location, _radius);
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
        public static IHeightfieldGenerator _generator;
        public static Statistics _statistics;
        public static IPlanet _planet;

        Establish context = () =>
        {
            _radius = 100;
            _location = DoubleVector3.Zero;

            _planetRenderer = MockRepository.GenerateStub<IPlanetRenderer>();

            _terrain = MockRepository.GenerateStub<ITerrain>();
            _terrainFactory = MockRepository.GenerateStub<ITerrainFactory>();
            _terrainFactory.Stub(x => x.Create(_radius)).Return(_terrain);

            _generator = MockRepository.GenerateStub<IHeightfieldGenerator>();

            _statistics = new Statistics();

            _planet = new Planet(_planetRenderer, _terrainFactory, _generator, _statistics);
        };
    }
}
