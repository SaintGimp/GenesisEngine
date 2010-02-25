using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public interface INoiseGenerator
    {
        double GetNoise(DoubleVector3 location);
    }
}
