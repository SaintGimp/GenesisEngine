using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GenesisEngine
{
    public interface IQuadNodeRenderer : IRenderer
    {
    }

    public class QuadNodeRenderer : IQuadNodeRenderer, IDisposable
    {
        private ContentManager _contentManager;
        private GraphicsDevice _graphicsDevice;
        private ISettings _settings;

        public QuadNodeRenderer(ContentManager contentManager, GraphicsDevice graphicsDevice, ISettings settings)
        {
            _contentManager = contentManager;
            _graphicsDevice = graphicsDevice;
            _settings = settings;
        }

        public void Draw(DoubleVector3 location, DoubleVector3 cameraLocation, Matrix originBasedViewMatrix, Matrix projectionMatrix)
        {
        }

        public void Dispose()
        {
            // Don't dispose _effect here because the ContentManager gives us a shared instance
        }
    }
}
