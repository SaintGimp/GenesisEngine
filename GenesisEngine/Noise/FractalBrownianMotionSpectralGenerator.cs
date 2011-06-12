using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public class FractalBrownianMotionSpectralGenerator : ISpectralGenerator
    {
        readonly INoiseGenerator _noiseGenerator;

        public FractalBrownianMotionSpectralGenerator(INoiseGenerator noiseGenerator)
        {
            _noiseGenerator = noiseGenerator;
        }

        public double GetSpectralNoise(DoubleVector3 location, double initialFrequencyMultiplier, int numberOfOctaves, double lacunarity, double gain)
        {
            var sampleLocation = location * initialFrequencyMultiplier;
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
                noiseSum += amplitude * _noiseGenerator.GetNoise(sampleLocation);
                amplitudeSum += amplitude;

                amplitude *= gain;
                sampleLocation *= lacunarity;
            }

            noiseSum /= amplitudeSum;

            return noiseSum * 1.35;
        }
    }
}


