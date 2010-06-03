using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Machine.Specifications;
using Microsoft.Xna.Framework;
using Rhino.Mocks;

namespace GenesisEngine.Specs.PresenterSpecs
{
    [Subject(typeof(MainPresenter))]
    public class when_it_is_shown : MainPresenterContext
    {
        Because of = () =>
            _mainPresenter.Show();

        It should_create_at_least_one_planet = () =>
            _planetFactory.AssertWasCalled(x => x.Create(Arg<DoubleVector3>.Is.Anything, Arg<double>.Is.Anything));

        It should_attach_the_controller_to_a_planet = () =>
            _cameraController.AssertWasCalled(x => x.AttachToPlanet(Arg<IPlanet>.Is.Anything));

        It should_show_all_ui_windows = () =>
            _windowManager.AssertWasCalled(x => x.ShowAllWindows());
    }

    [Subject(typeof(MainPresenter))]
    public class when_updating_is_enabled_and_the_presenter_is_updated : MainPresenterContext
    {
        Establish context = () =>
        {
            _settings.ShouldUpdate = true;
            _mainPresenter.Show();
        };

        Because of = () =>
            _mainPresenter.Update(TimeSpan.FromMilliseconds(250));

        It should_update_the_planet = () =>
            _planet.AssertWasCalled(x => x.Update(_camera.Location));

        It update_the_framerate_statistic = () =>
            _statistics.FrameRate.ShouldEqual(4);
    }

    [Subject(typeof(MainPresenter))]
    public class when_updating_is_disabled_and_the_presenter_is_updated : MainPresenterContext
    {
        Establish context = () =>
        {
            _settings.ShouldUpdate = false;
            _mainPresenter.Show();
        };

        Because of = () =>
            _mainPresenter.Update(TimeSpan.FromMilliseconds(250));

        It should_not_update_the_planet = () =>
            _planet.AssertWasNotCalled(x => x.Update(Arg<DoubleVector3>.Is.Anything));

        It update_the_framerate_statistic = () =>
            _statistics.FrameRate.ShouldEqual(4);
    }

    [Subject(typeof(MainPresenter))]
    public class when_updating_is_disabled_and_single_step_is_enabled_and_the_presenter_is_updated : MainPresenterContext
    {
        Establish context = () =>
        {
            _settings.ShouldUpdate = false;
            _settings.ShouldSingleStep = true;
            _mainPresenter.Show();
        };

        Because of = () =>
            _mainPresenter.Update(TimeSpan.FromMilliseconds(250));

        It should_update_the_planet = () =>
            _planet.AssertWasCalled(x => x.Update(_camera.Location));

        It should_turn_off_single_stepping = () =>
            _settings.ShouldSingleStep.ShouldBeFalse();

        It update_the_framerate_statistic = () =>
            _statistics.FrameRate.ShouldEqual(4);
    }

    [Subject(typeof(MainPresenter))]
    public class when_the_presenter_is_drawn : MainPresenterContext
    {
        Establish context = () =>
            _mainPresenter.Show();

        Because of = () =>
            _mainPresenter.Draw();

        It should_draw_the_planet = () =>
            _planet.AssertWasCalled(x => x.Draw(_camera));
    }

    [Subject(typeof(MainPresenter))]
    public class when_the_viewport_size_is_set : MainPresenterContext
    {
        Establish context = () =>
            _mainPresenter.Show();

        Because of = () =>
            _mainPresenter.SetViewportSize(640, 480);

        It should_set_camera_projection_parameters = () =>
            _camera.AssertWasCalled(x => x.SetProjectionParameters(Arg<float>.Is.Anything, Arg<float>.Is.Anything, Arg<float>.Is.Anything, Arg<float>.Is.Anything, Arg<float>.Is.Anything));
    }

    public class MainPresenterContext
    {
        public static IPlanetFactory _planetFactory;
        public static IPlanet _planet;
        public static ICamera _camera;
        public static ICameraController _cameraController;
        public static IWindowManager _windowManager;
        public static Statistics _statistics;
        public static ISettings _settings;
        public static MainPresenter _mainPresenter;

        Establish context = () =>
        {
            _planet = MockRepository.GenerateStub<IPlanet>();
            _planetFactory = MockRepository.GenerateStub<IPlanetFactory>();
            _planetFactory.Stub(x => x.Create(Arg<DoubleVector3>.Is.Anything, Arg<double>.Is.Anything)).Return(_planet);
            _camera = MockRepository.GenerateStub<ICamera>();
            _cameraController = MockRepository.GenerateStub<ICameraController>();
            _windowManager = MockRepository.GenerateStub<IWindowManager>();
            _statistics = new Statistics();
            _settings = MockRepository.GenerateStub<ISettings>();

            _mainPresenter = new MainPresenter(_planetFactory, _camera, _cameraController, _windowManager, _statistics, _settings);
        };
    }
}
