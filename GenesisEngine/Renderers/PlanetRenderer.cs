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
        private GraphicsDevice _graphicsDevice;
        private ContentManager _contentManager;
        private double _radius;
        private Model _model;

        public PlanetRenderer(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _contentManager = contentManager;
        }

        public void Initialize(double radius)
        {
            _radius = radius;
        }

        public void Draw(DoubleVector3 location, DoubleVector3 cameraLocation, Matrix originBasedViewMatrix, Matrix projectionMatrix)
        {
        }
    }
}
