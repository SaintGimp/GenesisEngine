using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public interface ITerrainFactory
    {
        ITerrain Create(double planetRadius);
    }
}
