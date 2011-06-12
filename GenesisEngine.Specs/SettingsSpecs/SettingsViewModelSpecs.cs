using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using NSubstitute;

namespace GenesisEngine.Specs.SettingsSpecs
{
    public class when_the_update_option_is_changed_in_the_domain : SettingsViewModelDomainContext
    {
        public static bool _expectedValue;

        Establish context = () =>
            _expectedValue = !_settings.ShouldUpdate;

        Because of = () =>
        {
            _settings.ShouldUpdate = !_settings.ShouldUpdate;
            _viewModel.Handle(new SettingsChanged());
        };

        It should_notify_that_the_property_changed = () =>
           _changedPropertyName.ShouldEqual("ShouldUpdate");

        It should_reflect_the_new_value = () =>
            _viewModel.ShouldUpdate.ShouldEqual(_expectedValue);
    }

    public class when_the_update_option_is_changed_in_the_view : SettingsViewModelViewContext
    {
        public static bool _expectedValue;

        Establish context = () =>
            _expectedValue = !_viewModel.ShouldUpdate;

        Because of = () =>
            _viewModel.ShouldUpdate = !_viewModel.ShouldUpdate;

        It should_change_the_value_in_the_domain = () =>
            _settings.ShouldUpdate.ShouldEqual(_expectedValue);
    }

    public class when_the_single_step_option_is_changed_in_the_domain : SettingsViewModelDomainContext
    {
        public static bool _expectedValue;

        Establish context = () =>
            _expectedValue = !_settings.ShouldSingleStep;

        Because of = () =>
        {
            _settings.ShouldSingleStep = !_settings.ShouldSingleStep;
            _viewModel.Handle(new SettingsChanged());
        };

        It should_notify_that_the_property_changed = () =>
           _changedPropertyName.ShouldEqual("ShouldSingleStep");

        It should_reflect_the_new_value = () =>
            _viewModel.ShouldSingleStep.ShouldEqual(_expectedValue);
    }

    public class when_the_single_step_option_is_changed_in_the_view : SettingsViewModelViewContext
    {
        public static bool _expectedValue;

        Establish context = () =>
            _expectedValue = !_viewModel.ShouldSingleStep;

        Because of = () =>
            _viewModel.ShouldSingleStep = !_viewModel.ShouldSingleStep;

        It should_change_the_value_in_the_domain = () =>
            _settings.ShouldSingleStep.ShouldEqual(_expectedValue);
    }

    public class when_the_draw_wireframe_option_is_changed_in_the_domain : SettingsViewModelDomainContext
    {
        public static bool _expectedValue;

        Establish context = () =>
            _expectedValue = !_settings.ShouldDrawWireframe;

        Because of = () =>
        {
            _settings.ShouldDrawWireframe = !_settings.ShouldDrawWireframe;
            _viewModel.Handle(new SettingsChanged());
        };

        It should_notify_that_the_property_changed = () =>
           _changedPropertyName.ShouldEqual("ShouldDrawWireframe");

        It should_reflect_the_new_value = () =>
            _viewModel.ShouldDrawWireframe.ShouldEqual(_expectedValue);
    }

    public class when_the_draw_wireframe_option_is_changed_in_the_view : SettingsViewModelViewContext
    {
        public static bool _expectedValue;

        Establish context = () =>
            _expectedValue = !_viewModel.ShouldDrawWireframe;

        Because of = () =>
            _viewModel.ShouldDrawWireframe = !_viewModel.ShouldDrawWireframe;

        It should_change_the_value_in_the_domain = () =>
            _settings.ShouldDrawWireframe.ShouldEqual(_expectedValue);
    }

    public class when_the_camera_move_speed_is_changed_in_the_domain : SettingsViewModelDomainContext
    {
        Because of = () =>
        {
            _settings.CameraMoveSpeedPerSecond = 123;
            _viewModel.Handle(new SettingsChanged());
        };

        It should_notify_that_the_property_changed = () => 
           _changedPropertyName.ShouldEqual("CameraMoveSpeedPerSecond");

        It should_reflect_the_new_value = () =>
            _viewModel.CameraMoveSpeedPerSecond.ShouldEqual(123);
    }

    public class when_the_camera_move_speed_is_changed_in_the_view : SettingsViewModelViewContext
    {
        Because of = () =>
            _viewModel.CameraMoveSpeedPerSecond = 123;

        It should_change_the_value_in_the_domain = () =>
            _settings.CameraMoveSpeedPerSecond.ShouldEqual(123);
    }

    public class when_the_draw_mesh_bounding_box_option_is_changed_in_the_domain : SettingsViewModelDomainContext
    {
        public static bool _expectedValue;

        Establish context = () =>
            _expectedValue = !_settings.ShouldDrawMeshBoundingBoxes;

        Because of = () =>
        {
            _settings.ShouldDrawMeshBoundingBoxes = !_settings.ShouldDrawMeshBoundingBoxes;
            _viewModel.Handle(new SettingsChanged());
        };

        It should_notify_that_the_property_changed = () =>
           _changedPropertyName.ShouldEqual("ShouldDrawMeshBoundingBoxes");

        It should_reflect_the_new_value = () =>
            _viewModel.ShouldDrawMeshBoundingBoxes.ShouldEqual(_expectedValue);
    }

    public class when_the_draw_mesh_bounding_box_option_is_changed_in_the_view : SettingsViewModelViewContext
    {
        public static bool _expectedValue;

        Establish context = () =>
            _expectedValue = !_viewModel.ShouldDrawMeshBoundingBoxes;

        Because of = () =>
            _viewModel.ShouldDrawMeshBoundingBoxes = !_viewModel.ShouldDrawMeshBoundingBoxes;

        It should_change_the_value_in_the_domain = () =>
            _settings.ShouldDrawMeshBoundingBoxes.ShouldEqual(_expectedValue);
    }

    public class SettingsViewModelDomainContext
    {
        public static ISettings _settings;
        public static SettingsViewModel _viewModel;
        public static string _changedPropertyName;

        Establish context = () =>
        {
            _settings = Substitute.For<ISettings>();
            _viewModel = new SettingsViewModel(_settings);
            _viewModel.PropertyChanged += (s, e) => _changedPropertyName = e.PropertyName;
        };
    }

    public class SettingsViewModelViewContext
    {
        public static ISettings _settings;
        public static SettingsViewModel _viewModel;

        Establish context = () =>
        {
            _settings = Substitute.For<ISettings>();
            _viewModel = new SettingsViewModel(_settings);
        };
    }
}
