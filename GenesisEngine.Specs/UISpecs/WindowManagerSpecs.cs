using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Rhino.Mocks;
using StructureMap;

namespace GenesisEngine.Specs.UISpecs
{
    [Subject(typeof(WindowManager))]
    public class when_showing_all_windows : WindowManagerContext
    {
        Because of = () =>
            _windowManager.ShowAllWindows();

        It should_show_the_settings_window = () =>
            _settingsCustodian.AssertWasCalled(x => x.ShowInactive());

        It should_show_the_statistics_window = () =>
            _statisticsCustodian.AssertWasCalled(x => x.ShowInactive());

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
            _container = MockRepository.GenerateStub<IContainer>();
            _settingsCustodian = MockRepository.GenerateStub<IScreenCustodian<SettingsView, SettingsViewModel>>();
            _statisticsCustodian = MockRepository.GenerateStub<IScreenCustodian<StatisticsView, StatisticsViewModel>>();

            _container.Stub(x => x.GetInstance<IScreenCustodian<SettingsView, SettingsViewModel>>()).Return(_settingsCustodian);
            _container.Stub(x => x.GetInstance<IScreenCustodian<StatisticsView, StatisticsViewModel>>()).Return(_statisticsCustodian);

            _windowManager = new WindowManager(_container);
        };
    }
}
