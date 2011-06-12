using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GenesisEngine
{
    public class TestHeightGenerator2 : IHeightGenerator
    {
        ISpectralGenerator _varianceGenerator1 = new RidgedMultiFractalSpectralGenerator2(new SimplexNoiseGenerator());
        ISpectralGenerator _varianceGenerator2 = new RidgedMultiFractalSpectralGenerator(new SimplexNoiseGenerator());
        static int _sampleCount = 0;

        public double GetHeight(DoubleVector3 location, int level, double scale)
        {
            var heightVariance1 = _varianceGenerator1.GetSpectralNoise(location, 5, 4, 2.0, 0.5);
            heightVariance1 = (heightVariance1 + 1);

            var heightVariance2 = _varianceGenerator2.GetSpectralNoise(location, 15, 4, 3.0, 0.5);
            heightVariance2 = (heightVariance2 + 1);

            var heightVariance = DoubleMathHelper.Clamp(heightVariance1 * heightVariance2, 0, 2);

            _sampleCount++;
            if (_sampleCount % 1000 == 0)
            {
                Debug.WriteLine(heightVariance);
            }

            var height = heightVariance * scale;
            return height;
        }
    }
}
