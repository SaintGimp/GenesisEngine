using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public class MainPresenter
    {
        readonly IPlanetFactory _planetFactory;
        readonly ICamera _camera;
        readonly ICameraController _cameraController;
        readonly IWindowManager _windowManager;
        readonly Statistics _statistics;
        readonly ISettings _settings;

        IPlanet _planet;

        public MainPresenter(IPlanetFactory planetFactory, ICamera camera, ICameraController cameraController, IWindowManager windowManager, Statistics statistics, ISettings settings)
		{
            _planetFactory = planetFactory;
			_camera = camera;
            _cameraController = cameraController;
            _windowManager = windowManager;
            _statistics = statistics;

            _settings = settings;
            _settings.ShouldUpdate = true;
        }

        public void Show()
        {
            _planet = _planetFactory.Create(DoubleVector3.Zero, PhysicalConstants.RadiusOfEarth);
            _cameraController.AttachToPlanet(_planet);
            _windowManager.ShowAllWindows();
        }

        public void Update(TimeSpan elapsedTime)
		{
			if (_settings.ShouldUpdate || _settings.ShouldSingleStep)
			{
                _planet.Update(_camera.Location);
				
				_settings.ShouldSingleStep = false;
			}

            UpdateStatistics(elapsedTime);
		}

        void UpdateStatistics(TimeSpan elapsedTime)
        {
            _statistics.FrameRate =  (float)Math.Round(1000.0 / elapsedTime.TotalMilliseconds, 1);
        }

        public void Draw()
        {
            _planet.Draw(_camera);
        }

		public void SetViewportSize(int width, int height)
		{
			float aspectRatio = width / (float)height;
			const float fieldOfView = MathHelper.Pi / 4;
			
			_camera.SetProjectionParameters(fieldOfView, 1.0f, aspectRatio, 1f, 500000f);
		}
    }
}
