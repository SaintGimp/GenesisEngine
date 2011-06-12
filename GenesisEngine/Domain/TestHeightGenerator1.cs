using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public class TestHeightGenerator1 : IHeightGenerator
    {
        RidgedMultiFractalSpectralGenerator _generator = new RidgedMultiFractalSpectralGenerator(new SimplexNoiseGenerator());

        public double GetHeight(DoubleVector3 location, int level, double scale)
        {
            var noise = _generator.GetSpectralNoise(location, 40, level + 1, 2.0, 0.5);
            var height = noise * scale;
            return height;
        }
    }
}
