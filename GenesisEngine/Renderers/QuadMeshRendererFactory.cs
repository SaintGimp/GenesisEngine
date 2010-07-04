using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GenesisEngine
{
    public class QuadMeshRendererFactory : IQuadMeshRendererFactory
    {
        readonly GraphicsDevice _graphicsDevice;
        readonly BasicEffect _basicEffect;
        readonly ISettings _settings;

        public QuadMeshRendererFactory(GraphicsDevice graphicsDevice, ISettings settings)
        {
            _graphicsDevice = graphicsDevice;
            _basicEffect = new BasicEffect(_graphicsDevice);
            _settings = settings;
        }

        public IQuadMeshRenderer Create()
        {
            return new QuadMeshRenderer(_graphicsDevice, _basicEffect, _settings);
        }
    }
}
