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
            var planet = ObjectFactory.GetInstance<IPlanet>();
            planet.Initialize(location, radius);

            return planet;
        }
    }
}
