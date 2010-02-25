using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public class ClippingPlanes
    {
        public ClippingPlanes()
        {
            Near = double.MaxValue;
            Far = double.MinValue;
        }

        public double Near { get; set; }
        public double Far { get; set; }
    }
}
