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
        readonly ISettings _settings;
        readonly Statistics _statistics;
        
        public Planet(DoubleVector3 location, double radius, ITerrain terrain, IPlanetRenderer renderer, IHeightfieldGenerator generator, ISettings settings, Statistics statistics)
        {
            _location = location;
            _radius = radius;
            
            _terrain = terrain;
            _renderer = renderer;
            _generator = generator;
            _settings = settings;
            _statistics = statistics;
        }

        public void Update(DoubleVector3 cameraLocation)
        {
            _terrain.Update(cameraLocation, _location);

            UpdateStatistics(cameraLocation);
        }

        void UpdateStatistics(DoubleVector3 cameraLocation)
        {
            _statistics.CameraAltitude = DoubleVector3.Distance(_location, cameraLocation) - _radius;
        }

        public void Draw(ICamera camera)
        {
            _renderer.Draw(_location, camera.Location, camera.OriginBasedViewTransformation, camera.ProjectionTransformation);
            _terrain.Draw(camera.Location, camera.OriginBasedViewFrustum, camera.OriginBasedViewTransformation, camera.ProjectionTransformation);
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
