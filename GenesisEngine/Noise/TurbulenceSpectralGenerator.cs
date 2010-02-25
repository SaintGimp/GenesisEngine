using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public class TurbulenceSpectralGenerator
    {
        readonly SimplexNoiseGenerator _simplex;

        public TurbulenceSpectralGenerator()
        {
            _simplex = new SimplexNoiseGenerator();
        }

        public double GetNoise(DoubleVector3 location, int startingOctave, int numberOfOctaves, double lacunarity, double gain)
        {
            var sampleLocation = location * (1 << (startingOctave));
            return AccumulateNoise(sampleLocation, numberOfOctaves, lacunarity, gain);
        }

        double AccumulateNoise(DoubleVector3 location, int numberOfOctaves, double lacunarity, double gain)
        {
            double noiseSum = 0;
            double amplitude = 1;
            double amplitudeSum = 0;

            DoubleVector3 sampleLocation = location;

            for (int x = 0; x < numberOfOctaves; x++)
            {
                noiseSum += amplitude * Math.Abs(_simplex.GetNoise(sampleLocation));
                amplitudeSum += amplitude;

                amplitude *= gain;
                sampleLocation *= lacunarity;
            }

            noiseSum /= amplitudeSum;

            return noiseSum;
        }
    }
}
