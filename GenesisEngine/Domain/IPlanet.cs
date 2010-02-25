using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public interface IPlanet
    {
        void Initialize(DoubleVector3 location, double radius);
        
        void Update(TimeSpan elapsedTime, DoubleVector3 cameraLocation);
        
        void Draw(ICamera camera);

        double GetGroundHeight(DoubleVector3 observerLocation);
    }
}
