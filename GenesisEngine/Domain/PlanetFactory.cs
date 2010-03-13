using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;

namespace GenesisEngine
{
    public class PlanetFactory : IPlanetFactory
    {
        public IPlanet Create(DoubleVector3 location, double radius)
        {
            var renderer = ObjectFactory.GetInstance<IPlanetRenderer>();
            var terrainFactory = ObjectFactory.GetInstance<ITerrainFactory>();
            var generator = ObjectFactory.GetInstance<IHeightfieldGenerator>();
            var statistics = ObjectFactory.GetInstance<Statistics>();
            
            var planet = new Planet(renderer, terrainFactory, generator, statistics);
            planet.Initialize(location, radius);

            return planet;
        }
    }
}
