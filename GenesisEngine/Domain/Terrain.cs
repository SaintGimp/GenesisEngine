using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public interface ITerrain
    {
        void Update(DoubleVector3 cameraLocation, DoubleVector3 planetLocation);

        void Draw(DoubleVector3 cameraLocation, BoundingFrustum originBasedViewFrustum, Matrix originBasedViewMatrix, Matrix projectionMatrix);
    }

    public class Terrain : ITerrain
    {
        private IEnumerable<IQuadNode> _faces;

        public Terrain(IEnumerable<IQuadNode> faces)
        {
            _faces = faces;
        }

        public void Update(DoubleVector3 cameraLocation, DoubleVector3 planetLocation)
        {
            foreach (var face in _faces)
            {
                face.Update(cameraLocation, planetLocation);
            }
        }

        public void Draw(DoubleVector3 cameraLocation, BoundingFrustum originBasedViewFrustum, Matrix originBasedViewMatrix, Matrix projectionMatrix)
        {
            foreach (var face in _faces)
            {
                face.Draw(cameraLocation, originBasedViewFrustum, originBasedViewMatrix, projectionMatrix);
            }
        }
    }
}
