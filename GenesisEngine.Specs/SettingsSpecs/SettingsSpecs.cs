using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Machine.Specifications;
using Rhino.Mocks;
using StructureMap;

namespace GenesisEngine.Specs.SettingsSpecs
{
    [Subject(typeof(Settings))]
    public class when_a_toggle_draw_wireframe_message_is_sent : SettingsContext
    {
        static public bool _oldValue;

        Establish context = () =>
            _oldValue = _settings.ShouldDrawWireframe;

        Because of = () =>
            _settings.Handle(new ToggleDrawWireframeSetting());

        It should_toggle_the_draw_wireframe_setting = () =>
            _settings.ShouldDrawWireframe.ShouldNotEqual(_oldValue);
    }

    [Subject(typeof(Settings))]
    public class when_a_toggle_update_message_is_sent : SettingsContext
    {
        static public bool _oldValue;

        Establish context = () =>
            _oldValue = _settings.ShouldUpdate;

        Because of = () =>
            _settings.Handle(new ToggleUpdateSetting());

        It should_toggle_the_update_setting = () =>
            _settings.ShouldUpdate.ShouldNotEqual(_oldValue);
    }

    [Subject(typeof(Settings))]
    public class when_a_single_step_message_is_sent : SettingsContext
    {
        static public bool _oldValue;

        Establish context = () =>
            _oldValue = _settings.ShouldSingleStep;

        Because of = () =>
            _settings.Handle(new ToggleSingleStepSetting());

        It should_toggle_the_single_step_setting = () =>
            _settings.ShouldSingleStep.ShouldNotEqual(_oldValue);
    }

    [Subject(typeof(Settings))]
    public class when_an_increase_camera_speed_message_is_sent : SettingsContext
    {
        static public double _oldValue;

        Establish context = () =>
            _oldValue = _settings.CameraMoveSpeedPerSecond;

        Because of = () =>
            _settings.Handle(new IncreaseCameraSpeed());

        It should_increase_the_camera_speed = () =>
            _settings.CameraMoveSpeedPerSecond.ShouldBeGreaterThan(_oldValue);
    }

    [Subject(typeof(Settings))]
    public class when_a_decrease_camera_speed_message_is_sent : SettingsContext
    {
        static public double _oldValue;

        Establish context = () =>
            _oldValue = _settings.CameraMoveSpeedPerSecond;

        Because of = () =>
            _settings.Handle(new DecreaseCameraSpeed());

        It should_decrease_the_camera_speed = () =>
            _settings.CameraMoveSpeedPerSecond.ShouldBeLessThan(_oldValue);
    }

    public class SettingsContext
    {
        public static IEventAggregator _eventAggregator;
        public static Settings _settings;

        Establish context = () =>
        {
            _eventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            _settings = new Settings(_eventAggregator);
        };
    }
}