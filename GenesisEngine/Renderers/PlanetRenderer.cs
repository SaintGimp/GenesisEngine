using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace GenesisEngine
{
    public class PlanetRenderer : IPlanetRenderer
    {
        readonly double _radius;
        readonly GraphicsDevice _graphicsDevice;
        readonly ContentManager _contentManager;

        public PlanetRenderer(double radius, ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _radius = radius;
            _graphicsDevice = graphicsDevice;
            _contentManager = contentManager;
        }

        public void Draw(DoubleVector3 location, DoubleVector3 cameraLocation, Matrix originBasedViewMatrix, Matrix projectionMatrix)
        {
        }
    }
}
