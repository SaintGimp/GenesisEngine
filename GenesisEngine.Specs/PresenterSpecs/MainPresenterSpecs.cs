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
    public class when_it_is_constructed : MainPresenterContext
    {
        It should_initialize_the_planet = () =>
            _planet.AssertWasCalled(x => x.Initialize(DoubleVector3.Zero, 0), s => s.IgnoreArguments());

        It should_show_all_ui_windows = () =>
            _windowManager.AssertWasCalled(x => x.ShowAllWindows());
    }

    [Subject(typeof(MainPresenter))]
    public class when_updating_is_enabled_and_the_presenter_is_updated : MainPresenterContext
    {
        Establish context = () =>
            _settings.ShouldUpdate = true;

        Because of = () =>
            _mainPresenter.Update(TimeSpan.FromMilliseconds(250));

        It should_update_the_planet = () =>
            _planet.AssertWasCalled(x => x.Update(TimeSpan.FromMilliseconds(250), _camera.Location));

        It update_the_framerate_statistic = () =>
            _statistics.FrameRate.ShouldEqual(4);
    }

    [Subject(typeof(MainPresenter))]
    public class when_updating_is_disabled_and_the_presenter_is_updated : MainPresenterContext
    {
        Establish context = () =>
            _settings.ShouldUpdate = false;

        Because of = () =>
            _mainPresenter.Update(TimeSpan.FromMilliseconds(250));

        It should_not_update_the_planet = () =>
            _planet.AssertWasNotCalled(x => x.Update(Arg<TimeSpan>.Is.Anything, Arg<DoubleVector3>.Is.Anything));

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
        };

        Because of = () =>
            _mainPresenter.Update(TimeSpan.FromMilliseconds(250));

        It should_update_the_planet = () =>
            _planet.AssertWasCalled(x => x.Update(TimeSpan.FromMilliseconds(250), _camera.Location));

        It should_turn_off_single_stepping = () =>
            _settings.ShouldSingleStep.ShouldBeFalse();

        It update_the_framerate_statistic = () =>
            _statistics.FrameRate.ShouldEqual(4);
    }

    [Subject(typeof(MainPresenter))]
    public class when_the_presenter_is_drawn : MainPresenterContext
    {
        Because of = () =>
            _mainPresenter.Draw();

        It should_draw_the_planet = () =>
            _planet.AssertWasCalled(x => x.Draw(_camera));
    }

    [Subject(typeof(MainPresenter))]
    public class when_the_viewport_size_is_set : MainPresenterContext
    {
        Because of = () =>
            _mainPresenter.SetViewportSize(640, 480);

        It should_set_camera_projection_parameters = () =>
            _camera.AssertWasCalled(x => x.SetProjectionParameters(Arg<float>.Is.Anything, Arg<float>.Is.Anything, Arg<float>.Is.Anything, Arg<float>.Is.Anything, Arg<float>.Is.Anything));
    }

    public class MainPresenterContext
    {
        public static IPlanet _planet;
        public static ICamera _camera;
        public static IWindowManager _windowManager;
        public static Statistics _statistics;
        public static ISettings _settings;
        public static MainPresenter _mainPresenter;

        Establish context = () =>
        {
            _planet = MockRepository.GenerateStub<IPlanet>();
            _camera = MockRepository.GenerateStub<ICamera>();
            _windowManager = MockRepository.GenerateStub<IWindowManager>();
            _statistics = new Statistics();
            _settings = MockRepository.GenerateStub<ISettings>();

            _mainPresenter = new MainPresenter(_planet, _camera, _windowManager, _statistics, _settings);
        };
    }
}
