using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public static class DoubleMathHelper
    {
        public static double Clamp(double value, double minimum, double maximum)
        {
            if (value > maximum)
            {
                value = maximum;
            }
            else if (value < minimum)
            {
                value = minimum;
            }

            return value;
        }

        public static double Lerp(double value1, double value2, double amount)
        {
            return (value1 + ((value2 - value1) * amount));
        }

        public static double MetersToFeet(double meters)
        {
            return meters * 3.2808399;
        }
    }
}
