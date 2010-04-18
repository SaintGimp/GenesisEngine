using System;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public interface IQuadMesh
    {
        void Initialize(double planetRadius, DoubleVector3 planeNormalVector, DoubleVector3 uVector,
                        DoubleVector3 vVector, QuadNodeExtents extents, int level);

        bool IsVisibleToCamera { get; }

        double WidthToCameraDistanceRatio { get; }

        void Update(TimeSpan elapsedTime, DoubleVector3 cameraLocation, DoubleVector3 planetLocation, ClippingPlanes clippingPlanes);

        void Draw(DoubleVector3 cameraLocation, Matrix originBasedViewMatrix, Matrix projectionMatrix);
    }
}