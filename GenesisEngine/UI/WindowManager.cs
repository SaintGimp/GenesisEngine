using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using StructureMap;

namespace GenesisEngine
{
    public interface IWindowManager
    {
        void ShowAllWindows();
    }

    public class WindowManager : IWindowManager, IDisposable
    {
        IContainer _container;
        IScreenCustodian<SettingsView, SettingsViewModel> _settingsCustodian;
        IScreenCustodian<StatisticsView, StatisticsViewModel> _statisticsCustodian;
        // TODO: use SynchronizationContext instead?
        Dispatcher _windowDispatcher;

        public WindowManager(IContainer container)
        {
            _container = container;
        }

        public void ShowAllWindows()
        {
            StartUIThread();

            _windowDispatcher.Invoke(() =>
            {
                // We pull these out of the container here instead of doing normal
                // constructor injection because we need them to be created on this thread.
                _settingsCustodian = _container.GetInstance<IScreenCustodian<SettingsView, SettingsViewModel>>();
                _statisticsCustodian = _container.GetInstance<IScreenCustodian<StatisticsView, StatisticsViewModel>>();
            });

            _windowDispatcher.Invoke((Action)(() =>
            {
                _settingsCustodian.ShowInactive();
                _statisticsCustodian.ShowInactive();
            }));
        }

        void StartUIThread()
        {
            var dispatcherCreatedEvent = new ManualResetEvent(false);
            var thread = new Thread(() =>
            {
                _windowDispatcher = Dispatcher.CurrentDispatcher;
                dispatcherCreatedEvent.Set();

                Dispatcher.Run();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();

            dispatcherCreatedEvent.WaitOne();
        }

        public void Dispose()
        {
            if (_windowDispatcher != null)
            {
                _windowDispatcher.InvokeShutdown();
            }
        }
    }
}
