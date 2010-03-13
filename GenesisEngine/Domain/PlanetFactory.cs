using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;

namespace GenesisEngine
{
    public class PlanetFactory : IPlanetFactory
    {
        readonly ITerrainFactory _terrainFactory;

        public PlanetFactory(ITerrainFactory terrainFactory)
        {
            _terrainFactory = terrainFactory;
        }

        public IPlanet Create(DoubleVector3 location, double radius)
        {
            var terrain = _terrainFactory.Create(radius);
            var renderer = ObjectFactory.GetInstance<IPlanetRenderer>();
            var generator = ObjectFactory.GetInstance<IHeightfieldGenerator>();
            var statistics = ObjectFactory.GetInstance<Statistics>();

            var planet = new Planet(location, radius, terrain, renderer, generator, statistics);

            return planet;
        }
    }
}
