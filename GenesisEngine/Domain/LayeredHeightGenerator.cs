using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public class LayeredHeightGenerator : IHeightGenerator
    {
        ISpectralGenerator _continentalGenerator = new FractalBrownianMotionSpectralGenerator(new SimplexNoiseGenerator());
        ISpectralGenerator _regionalGenerator1 = new RidgedMultiFractalSpectralGenerator2(new SimplexNoiseGenerator());
        ISpectralGenerator _regionalGenerator2 = new RidgedMultiFractalSpectralGenerator2(new SimplexNoiseGenerator());
        ISpectralGenerator _localGenerator = new RidgedMultiFractalSpectralGenerator(new SimplexNoiseGenerator());

        public double GetHeight(DoubleVector3 location, int level, double scale)
        {
            var continentalHeight = GetContinentalHeight(location, level, scale / 2);

            if (continentalHeight < 0)
            {
                return 0;
            }
        
            return continentalHeight + GetTerrainHeight(location, level, scale, continentalHeight);
        }

        double GetContinentalHeight(DoubleVector3 location, int level, double scale)
        {
            var noise = _continentalGenerator.GetSpectralNoise(location, 2, 10, 2.0, 0.5);
            var height = noise * scale;
            return height;
        }

        double GetTerrainHeight(DoubleVector3 location, int level, double scale, double continentalHeight)
        {
            double regionalModifier = GetRegionalModifier(location);
            double localHeight = GetLocalHeight(location, level, scale / 2);
            double coastalModifier = GetCoastalModifier(location, level, scale / 2, continentalHeight);

            return localHeight * regionalModifier * coastalModifier;
        }

        double GetRegionalModifier(DoubleVector3 location)
        {
            var lowFrequencyRegionalModifier = _regionalGenerator1.GetSpectralNoise(location, 21, 4, 2.0, 0.5);
            lowFrequencyRegionalModifier += 1;

            var mediumFrequencyRegionalModifier = _regionalGenerator2.GetSpectralNoise(location, 30, 4, 2.0, 0.5);
            mediumFrequencyRegionalModifier += 1;

            var regionalModifier = DoubleMathHelper.Clamp(lowFrequencyRegionalModifier * mediumFrequencyRegionalModifier, 0, 2);
            return regionalModifier;
        }

        double GetLocalHeight(DoubleVector3 location, int level, double scale)
        {
            var noise = _localGenerator.GetSpectralNoise(location, 100, level + 1, 2.0, 0.5);
            var filteredNoise = (noise + 1) / 2;

            return filteredNoise * scale;
        }

        double GetCoastalModifier(DoubleVector3 location, int level, double scale, double continentalHeight)
        {
            var continentalHeightPercentage = DoubleMathHelper.Lerp(0, 1, continentalHeight / scale / 2);

            var coastalDamping1 = MathHelper.Clamp((float)continentalHeightPercentage * 200, 0, 1);
            var coastalDamping2 = NoiseFilter.Hermite(DoubleMathHelper.Clamp(continentalHeightPercentage * 2000, 0, 1));

            var coastalModifier = coastalDamping1 * coastalDamping2;
            return coastalModifier;
        }
    }
}