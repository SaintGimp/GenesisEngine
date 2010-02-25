using System;

namespace GenesisEngine
{
    public interface IPlanetFactory
    {
        IPlanet Create(GenesisEngine.DoubleVector3 location, double radius);
    }
}
