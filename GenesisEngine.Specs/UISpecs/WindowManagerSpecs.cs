using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using NSubstitute;
using StructureMap;

namespace GenesisEngine.Specs.UISpecs
{
    [Subject(typeof(WindowManager))]
    public class when_showing_all_windows : WindowManagerContext
    {
        Because of = () =>
            _windowManager.ShowAllWindows();

        It should_show_the_settings_window = () =>
            _settingsCustodian.Received().ShowInactive();

        It should_show_the_statistics_window = () =>
            _statisticsCustodian.Received().ShowInactive();

        Cleanup after = () =>
            _windowManager.Dispose();
    }

    public class WindowManagerContext
    {
        public static IContainer _container;
        public static IScreenCustodian<SettingsView, SettingsViewModel> _settingsCustodian;
        public static IScreenCustodian<StatisticsView, StatisticsViewModel> _statisticsCustodian;
        public static WindowManager _windowManager;

        Establish context = () =>
        {
            _container = Substitute.For<IContainer>();
            _settingsCustodian = Substitute.For<IScreenCustodian<SettingsView, SettingsViewModel>>();
            _statisticsCustodian = Substitute.For<IScreenCustodian<StatisticsView, StatisticsViewModel>>();

            _container.GetInstance<IScreenCustodian<SettingsView, SettingsViewModel>>().Returns(_settingsCustodian);
            _container.GetInstance<IScreenCustodian<StatisticsView, StatisticsViewModel>>().Returns(_statisticsCustodian);

            _windowManager = new WindowManager(_container);
        };
    }
}
