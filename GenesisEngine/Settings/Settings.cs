using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public class Settings : ISettings,
                            IListener<ToggleDrawWireframeSetting>,
                            IListener<ToggleUpdateSetting>,
                            IListener<ToggleSingleStepSetting>,
                            IListener<IncreaseCameraSpeed>,
                            IListener<DecreaseCameraSpeed>
    {
        readonly IEventAggregator _eventAggregator;

        public Settings(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            ShouldUpdate = true;
            ShouldSingleStep = false;
            ShouldDrawWireframe = false;
            CameraStartingLocation = new DoubleVector3(0, PhysicalConstants.RadiusOfEarth * 1.002, 0);
            CameraStartingLookAt = CameraStartingLocation + DoubleVector3.Backward + DoubleVector3.Down * .2;
            CameraMoveSpeedPerSecond = PhysicalConstants.RadiusOfEarth / 1000;
            CameraMouseLookDamping = 300f;
            MaximumQuadNodeLevel = 19;
            ShowQuadBoundaries = true;
            FarClippingPlaneDistance = 10000000;
            ShouldDrawMeshBoundingBoxes = false;
        }

        bool _shouldUpdate;
        public bool ShouldUpdate
        {
            get { return _shouldUpdate; }
            set { SetFieldValue(ref _shouldUpdate, value); }
        }

        bool _shouldSingleStep;
        public bool ShouldSingleStep
        {
            get { return _shouldSingleStep; }
            set { SetFieldValue(ref _shouldSingleStep, value); }
        }

        bool _shouldDrawWireframe;
        public bool ShouldDrawWireframe
        {
            get { return _shouldDrawWireframe; }
            set { SetFieldValue(ref _shouldDrawWireframe, value); }
        }

        DoubleVector3 _cameraStartingLocation;
        public DoubleVector3 CameraStartingLocation
        {
            get { return _cameraStartingLocation; }
            set { SetFieldValue(ref _cameraStartingLocation, value); }
        }

        DoubleVector3 _cameraStartingLookAt;
        public DoubleVector3 CameraStartingLookAt
        {
            get { return _cameraStartingLookAt; }
            set { SetFieldValue(ref _cameraStartingLookAt, value); }
        }

        double _cameraMoveSpeedPerSecond;
        public double CameraMoveSpeedPerSecond
        {
            get { return _cameraMoveSpeedPerSecond; }
            set { SetFieldValue(ref _cameraMoveSpeedPerSecond, value); }
        }

        float _cameraMouseLookDamping;
        public float CameraMouseLookDamping
        {
            get { return _cameraMouseLookDamping; }
            set { SetFieldValue(ref _cameraMouseLookDamping, value); }
        }

        int _maximumQuadNodeLevel;
        public int MaximumQuadNodeLevel
        {
            get { return _maximumQuadNodeLevel; }
            set { SetFieldValue(ref _maximumQuadNodeLevel, value); }
        }

        bool _showQuadBoundaries;
        public bool ShowQuadBoundaries
        {
            get { return _showQuadBoundaries; }
            set { SetFieldValue(ref _showQuadBoundaries, value); }
        }

        double _farClippingPlaneDistance;
        public double FarClippingPlaneDistance
        {
            get { return _farClippingPlaneDistance; }
            set { SetFieldValue(ref _farClippingPlaneDistance, value); }
        }

        bool _shouldDrawMeshBoundingBoxes;
        public bool ShouldDrawMeshBoundingBoxes
        {
            get { return _shouldDrawMeshBoundingBoxes; }
            set { SetFieldValue(ref _shouldDrawMeshBoundingBoxes, value); }
        }

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

        void SetFieldValue<T>(ref T field, T value)
        {
            if (!field.Equals(value))
            {
                field = value;
                _eventAggregator.SendMessage(new SettingsChanged());
            }
        }
    }
}
