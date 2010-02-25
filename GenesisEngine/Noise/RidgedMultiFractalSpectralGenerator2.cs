using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public class RidgedMultiFractalSpectralGenerator2 : ISpectralGenerator
    {
        // Inspired by http://britonia-game.com/?p=14

        readonly INoiseGenerator _noiseGenerator;

        public RidgedMultiFractalSpectralGenerator2(INoiseGenerator noiseGenerator)
        {
            _noiseGenerator = noiseGenerator;
        }

        public double GetNoise(DoubleVector3 location, double initialFrequencyMultiplier, int numberOfOctaves, double lacunarity, double gain)
        {
            var sampleLocation = location * initialFrequencyMultiplier;
            return AccumulateNoise(sampleLocation, numberOfOctaves, lacunarity, gain, 1);
        }

        // Ridged multifractal
        // See "Texturing & Modeling, A Procedural Approach", Chapter 12
        public double AccumulateNoise(DoubleVector3 sampleLocation, int numberOfOctaves, double lacunarity, double gain, double offset)
        {
            double sum = 0; // The return result
            double amp = 0.5; // Reduce the amplitude per octave of noise
            double prev = 1.0;

            for (int i = 0; i < numberOfOctaves; i++)
            {
                double n = ridge(_noiseGenerator.GetNoise(sampleLocation), offset);
                sum += n * amp * prev;
                prev = n;
                sampleLocation *= lacunarity;
                amp *= gain;
            }
            
            // Return the result
            return sum * 1.25 * 2 - 1;
        }

        double ridge(double h, double offset)
        {
            h = Math.Abs(h);
            h = offset - h;
            h = h * h;
            return h;
        }
    }
}
