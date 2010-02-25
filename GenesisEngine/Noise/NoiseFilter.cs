using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public static class NoiseFilter
    {
        public static double Power(double noise, double power)
        {
            // TODO: short-circuit for square?
            return Math.Pow(noise, power);
        }

        public static double Hermite(double noise)
        {
            // 3t^2 - 2t^3
            return (3 * noise * noise) - (2 * noise * noise * noise);
        }

        public static double SymmetricHermite(double noise)
        {
            if (noise < 0)
            {
                return -(Hermite(-noise));
            }
            else
            {
                return Hermite(noise);
            }
        }

        public static double Quintic(double noise)
        {
            // 6t^5 - 15t^4 + 10t^3
            return (6 * Math.Pow(noise, 5) - 15 * Math.Pow(noise, 4) + 10 * Math.Pow(noise, 3));
        }

        public static double SymmetricQuintic(double noise)
        {
            if (noise < 0)
            {
                return -(Quintic(-noise));
            }
            else
            {
                return Quintic(noise);
            }
        }

        public static double Square(double noise)
        {
            return noise * noise;
        }

        public static double SignPreservingSquare(double noise)
        {
            if (noise > 0)
            {
                return noise * noise;    
            }
            else
            {
                var absNoise = Math.Abs(noise);
                return -(noise * noise);
            }
            
        }
    }
}
