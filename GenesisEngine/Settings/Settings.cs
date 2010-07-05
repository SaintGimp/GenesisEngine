using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace GenesisEngine
{
    public class Settings : ISettings,
                            INotifyPropertyChanged,
                            IListener<ToggleDrawWireframeSetting>,
                            IListener<ToggleUpdateSetting>,
                            IListener<ToggleSingleStepSetting>,
                            IListener<IncreaseCameraSpeed>,
                            IListener<DecreaseCameraSpeed>
    {
        double _cameraMoveSpeedPerSecond;

        public Settings()
        {
            ShouldUpdate = true;
            ShouldSingleStep = false;
            ShouldDrawWireframe = false;
            CameraStartingLocation = new DoubleVector3(0, PhysicalConstants.RadiusOfEarth * 1.002, 0);
            CameraStartingLookAt = CameraStartingLocation + DoubleVector3.Backward + DoubleVector3.Down * .2;
            CameraMoveSpeedPerSecond = PhysicalConstants.RadiusOfEarth / 1000;
            CameraMouseLookDamping = 300f;
            ShouldOutlineQuads = false;
            MaximumQuadNodeLevel = 19;
            ShowQuadBoundaries = true;
            FarClippingPlaneDistance = 10000000;
        }

        public bool ShouldUpdate { get; set; }

        public bool ShouldSingleStep { get; set; }

        public bool ShouldDrawWireframe { get; set; }

        public DoubleVector3 CameraStartingLocation { get; set; }

        public DoubleVector3 CameraStartingLookAt { get; set; }

        public double CameraMoveSpeedPerSecond
        {
            get { return _cameraMoveSpeedPerSecond; }
            set
            {
                _cameraMoveSpeedPerSecond = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CameraMoveSpeedPerSecond"));
                }
            }
        }

        public float CameraMouseLookDamping { get; set; }

        public bool ShouldOutlineQuads { get; set; }

        public int MaximumQuadNodeLevel { get; set; }

        public bool ShowQuadBoundaries { get; set; }

        public double FarClippingPlaneDistance { get; set; }
        
        public void Handle(ToggleDrawWireframeSetting message)
        {
            ShouldDrawWireframe = !ShouldDrawWireframe;
        }

        public void Handle(ToggleUpdateSetting message)
        {
            ShouldUpdate = !ShouldUpdate;
        }

        public void Handle(ToggleSingleStepSetting message)
        {
            ShouldSingleStep = !ShouldSingleStep;
        }

        public void Handle(IncreaseCameraSpeed message)
        {
            CameraMoveSpeedPerSecond *= 2;
        }

        public void Handle(DecreaseCameraSpeed message)
        {
            CameraMoveSpeedPerSecond /= 2;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
