using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GenesisEngine
{
    public class QuadNodeRendererFactory
    {
        ContentManager _contentManager;
        GraphicsDevice _graphicsDevice;
        ISettings _settings;

        public QuadNodeRendererFactory(ContentManager contentManager, GraphicsDevice graphicsDevice, ISettings settings)
        {
            _contentManager = contentManager;
            _graphicsDevice = graphicsDevice;
            _settings = settings;
        }

        public IQuadNodeRenderer Create()
        {
            return new QuadNodeRenderer(_contentManager, _graphicsDevice, _settings);
        }
    }
}
