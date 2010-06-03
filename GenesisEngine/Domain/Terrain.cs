using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public class Terrain : ITerrain
    {
        private IEnumerable<IQuadNode> _faces;

        public Terrain(IEnumerable<IQuadNode> faces)
        {
            _faces = faces;
        }

        public void Draw(DoubleVector3 cameraLocation, Matrix originBasedViewMatrix, Matrix projectionMatrix)
        {
            foreach (var face in _faces)
            {
                face.Draw(cameraLocation, originBasedViewMatrix, projectionMatrix);
            }
        }

        public void Update(DoubleVector3 cameraLocation, DoubleVector3 planetLocation, ClippingPlanes clippingPlanes)
        {
            foreach (var face in _faces)
            {
                face.Update(cameraLocation, planetLocation, clippingPlanes);
            }
        }
    }
}
