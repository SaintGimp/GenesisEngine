using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public interface IQuadNode
    {
        void Initialize(double planetRadius, DoubleVector3 planeNormalVector, DoubleVector3 uVector, DoubleVector3 vVector, QuadNodeExtents extents, int level);

        void Update(DoubleVector3 cameraLocation, DoubleVector3 planetLocation);
        
        void Draw(DoubleVector3 cameraLocation, Matrix originBasedViewMatrix, Matrix projectionMatrix);

        // TODO: does this need to be exposed?
        int Level { get; }
    }
}
