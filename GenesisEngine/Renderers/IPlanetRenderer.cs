using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public interface IPlanetRenderer: IRenderer
    {
        void Initialize(double radius);
    }
}
