using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public class Planet : IPlanet
    {
        private DoubleVector3 _location;
        private double _radius;
        private IPlanetRenderer _renderer;
        private ITerrainFactory _terrainFactory;
        readonly IHeightfieldGenerator _generator;
        readonly Statistics _statistics;
        private ITerrain _terrain;
        ClippingPlanes _clippingPlanes;

        public Planet(IPlanetRenderer renderer, ITerrainFactory terrainFactory, IHeightfieldGenerator generator, Statistics statistics)
        {
            _renderer = renderer;
            _terrainFactory = terrainFactory;
            _generator = generator;
            _statistics = statistics;

            _clippingPlanes = new ClippingPlanes();
        }

        public void Initialize(DoubleVector3 location, double radius)
        {
            _location = location;
            _radius = radius;

            CreateTerrain();
            
            _renderer.Initialize(_radius);
        }

        private void CreateTerrain()
        {
            _terrain = _terrainFactory.Create(_radius);
        }

        public void Update(TimeSpan elapsedTime, DoubleVector3 cameraLocation)
        {
            _terrain.Update(elapsedTime, cameraLocation, _location, _clippingPlanes);

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
