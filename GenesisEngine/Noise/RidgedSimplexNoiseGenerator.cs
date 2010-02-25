using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public class RidgedSimplexNoiseGenerator : INoiseGenerator
    {
        SimplexNoiseGenerator _sourceGenerator = new SimplexNoiseGenerator();

        public double GetNoise(DoubleVector3 location)
        {
            return 1 - Math.Abs(_sourceGenerator.GetNoise(location));
        }
    }
}
