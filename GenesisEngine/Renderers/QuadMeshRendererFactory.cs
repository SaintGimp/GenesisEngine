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
        ContentManager _contentManager;
        GraphicsDevice _graphicsDevice;
        ISettings _settings;

        public QuadMeshRendererFactory(ContentManager contentManager, GraphicsDevice graphicsDevice, ISettings settings)
        {
            _contentManager = contentManager;
            _graphicsDevice = graphicsDevice;
            _settings = settings;
        }

        public IQuadMeshRenderer Create()
        {
            return new QuadMeshRenderer(_contentManager, _graphicsDevice, _settings);
        }
    }
}
