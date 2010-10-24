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
            _statistics.Flush();
        }

        public void Draw()
        {
            _planet.Draw(_camera);
        }

        public void SetViewportSize(int width, int height)
        {
            float aspectRatio = width / (float)height;
            const float fieldOfView = MathHelper.Pi / 4;

            // TODO: better z-buffer handling?  Not sure what we can do to improve the mechanics of
            // the z-buffer itself from inside XNA.  We should test the (scaled and transformed) objects
            // in the view frustum in order to set a custom clipping plane for each frame, and/or do
            // multiple drawing passes with a different clipping plane set for each.
            // http://www.gamedev.net/community/forums/mod/journal/journal.asp?jn=263350&cmonth=10&cyear=2006
            // http://www.gamedev.net/community/forums/topic.asp?topic_id=565995
            // http://mynameismjp.wordpress.com/2010/03/22/attack-of-the-depth-buffer/
            // http://www.codermind.com/articles/Depth-buffer-tutorial.html
            // http://forum.beyond3d.com/showthread.php?t=52049
            // http://mynameismjp.wordpress.com/2010/03/22/attack-of-the-depth-buffer/
            // http://www.gamedev.net/community/forums/mod/journal/journal.asp?jn=503094&reply_id=3508347
            // http://www.gamedev.net/community/forums/mod/journal/journal.asp?jn=263350&reply_id=3513134
            // http://www.gamedev.net/community/forums/mod/journal/journal.asp?jn=503094&reply_id=3580850
            // http://www.gamedev.net/community/forums/mod/journal/journal.asp?jn=263350&reply_id=3643238
            // http://www.gamedev.net/community/forums/mod/journal/journal.asp?jn=263350&reply_id=3643238&PageSize=15&WhichPage=2
            // http://www.humus.name/index.php?ID=255
            
            _camera.SetProjectionParameters(fieldOfView, 1f, aspectRatio, 2f, (float)_settings.FarClippingPlaneDistance);
        }
    }
}
