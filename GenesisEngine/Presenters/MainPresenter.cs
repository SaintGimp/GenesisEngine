using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public class MainPresenter
    {
        private IPlanet _planet;
		private ICamera _camera;
        private IWindowManager _windowManager;
        private Statistics _statistics;
        private ISettings _settings;

        public MainPresenter(IPlanet planet, ICamera camera, IWindowManager windowManager, Statistics statistics, ISettings settings)
		{
            _planet = planet;
            _planet.Initialize(DoubleVector3.Zero, PhysicalConstants.RadiusOfEarth);
			_camera = camera;
            _windowManager = windowManager;
            _statistics = statistics;
            _windowManager.ShowAllWindows();

            _settings = settings;
            _settings.ShouldUpdate = true;
        }

        public void Update(TimeSpan elapsedTime)
		{
			if (_settings.ShouldUpdate || _settings.ShouldSingleStep)
			{
                _planet.Update(elapsedTime, _camera.Location);
				
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
