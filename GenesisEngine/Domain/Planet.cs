using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public class Planet : IPlanet
    {
        DoubleVector3 _location;
        readonly double _radius;
        readonly ITerrain _terrain;
        readonly IPlanetRenderer _renderer;
        readonly IHeightfieldGenerator _generator;
        readonly Statistics _statistics;
        
        ClippingPlanes _clippingPlanes;

        public Planet(DoubleVector3 location, double radius, ITerrain terrain, IPlanetRenderer renderer, IHeightfieldGenerator generator, Statistics statistics)
        {
            _location = location;
            _radius = radius;
            
            _terrain = terrain;
            _renderer = renderer;
            _generator = generator;
            _statistics = statistics;

            _clippingPlanes = new ClippingPlanes();
        }

        public void Update(DoubleVector3 cameraLocation)
        {
            _terrain.Update(cameraLocation, _location, _clippingPlanes);

            UpdateStatistics(cameraLocation);
        }

        void UpdateStatistics(DoubleVector3 cameraLocation)
        {
            _statistics.CameraAltitude = DoubleVector3.Distance(_location, cameraLocation) - _radius;
        }

        public void Draw(ICamera camera)
        {
            SetCameraClippingPlanes(camera);

            _renderer.Draw(_location, camera.Location, camera.OriginBasedViewTransformation, camera.ProjectionTransformation);
            _terrain.Draw(camera.Location, camera.OriginBasedViewTransformation, camera.ProjectionTransformation);
        }

        void SetCameraClippingPlanes(ICamera camera)
        {
            // TODO: we could probably do a better job of estimating
            // what the near plane should be so that it optimizes the
            // z buffer while not accidentally clipping anything we
            // care about.
            float nearPlane;
            if (_clippingPlanes.Near < 50)
            {
                nearPlane = 2f;    
            }
            else if (_clippingPlanes.Near < 20000)
            {
                nearPlane = (float) _clippingPlanes.Near * 0.50f;
            }
            else
            {
                nearPlane = (float)_clippingPlanes.Near * 0.90f;
            }

            float farPlane = (float) _clippingPlanes.Far * 1.1f;
            
            camera.SetClippingPlanes(nearPlane, farPlane);
        }

        public double GetGroundHeight(DoubleVector3 observerLocation)
        {
            // TODO: should probably delegate this responsibility to the terrain object
            // TODO: what about water?
            var planetUnitVector = DoubleVector3.Normalize(observerLocation - _location);
            var height = _generator.GetHeight(planetUnitVector, 19, 8000);
            return _radius + height;
        }
    }
}
