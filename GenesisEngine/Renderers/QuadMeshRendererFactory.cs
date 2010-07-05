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
        GraphicsDevice _graphicsDevice;
        ISettings _settings;

        public QuadMeshRendererFactory(GraphicsDevice graphicsDevice, ISettings settings)
        {
            _graphicsDevice = graphicsDevice;
            _settings = settings;
        }

        public IQuadMeshRenderer Create()
        {
            return new QuadMeshRenderer(_graphicsDevice, _settings);
        }
    }
}
