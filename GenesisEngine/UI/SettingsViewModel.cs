using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace GenesisEngine
{
    // TODO: implementing all settings by hand is pain.  It would be nice to have
    // something that would reflect over the settings object and automagically
    // build a view for all settings, though it's not clear now whether we might
    // want some custom transforms in there somewhere.

    public class SettingsViewModel : INotifyPropertyChanged, IListener<SettingsChanged>
    {
        readonly ISettings _settings;

        public SettingsViewModel(ISettings settings)
        {
            _settings = settings;
            UpdateFromDomain();
        }

        bool _shouldUpdate;
        public bool ShouldUpdate
        {
            get { return _shouldUpdate; }
            set
            {
                SetFieldValue(ref _shouldUpdate, value, "ShouldUpdate");
                _settings.ShouldUpdate = value;
            }
        }

        bool _shouldSingleStep;
        public bool ShouldSingleStep
        {
            get { return _shouldSingleStep; }
            set
            {
                SetFieldValue(ref _shouldSingleStep, value, "ShouldSingleStep");
                _settings.ShouldSingleStep = value;
            }
        }

        bool _shouldDrawWireframe;
        public bool ShouldDrawWireframe
        {
            get { return _shouldDrawWireframe; }
            set
            {
                SetFieldValue(ref _shouldDrawWireframe, value, "ShouldDrawWireframe");
                _settings.ShouldDrawWireframe = value;
            }
        }

        double _cameraMoveSpeedPerSecond;
        public double CameraMoveSpeedPerSecond
        {
            get { return _cameraMoveSpeedPerSecond; }
            set
            {
                SetFieldValue(ref _cameraMoveSpeedPerSecond, value, "CameraMoveSpeedPerSecond");
                _settings.CameraMoveSpeedPerSecond = value;
            }
        }

        bool _shouldDrawMeshBoundingBoxes;
        public bool ShouldDrawMeshBoundingBoxes
        {
            get { return _shouldDrawMeshBoundingBoxes; }
            set
            {
                SetFieldValue(ref _shouldDrawMeshBoundingBoxes, value, "ShouldDrawMeshBoundingBoxes");
                _settings.ShouldDrawMeshBoundingBoxes = value;
            }
        }

        void RaisePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Handle(SettingsChanged changedEvent)
        {
            UpdateFromDomain();
        }

        void UpdateFromDomain()
        {
            ShouldUpdate = _settings.ShouldUpdate;
            ShouldSingleStep = _settings.ShouldSingleStep;
            ShouldDrawWireframe = _settings.ShouldDrawWireframe;
            CameraMoveSpeedPerSecond = _settings.CameraMoveSpeedPerSecond;
            ShouldDrawMeshBoundingBoxes = _settings.ShouldDrawMeshBoundingBoxes;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void SetFieldValue<T>(ref T field, T value, string propertyName)
        {
            if (!field.Equals(value))
            {
                field = value;
                RaisePropertyChangedEvent(propertyName);
            }
        }
    }
}
