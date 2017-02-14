using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public interface ISpectralGenerator
    {
        double GetSpectralNoise(DoubleVector3 location, double initialFrequencyMultiplier, int numberOfOctaves,
                        double lacunarity, double gain); 
    }
}
