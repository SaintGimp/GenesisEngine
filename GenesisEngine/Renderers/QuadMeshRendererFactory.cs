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
        readonly ISettings _settings;
        readonly Statistics _statistics;
        BasicEffect _basicEffect;

        public QuadMeshRendererFactory(GraphicsDevice graphicsDevice, ISettings settings, Statistics statistics)
        {
            _graphicsDevice = graphicsDevice;
            _settings = settings;
            _statistics = statistics;
        }

        public IQuadMeshRenderer Create()
        {
            if (_basicEffect == null)
            {
                // This is not created in the constructor because it can't be created at unit test time
                // and that messes up our container tests.
                _basicEffect = new BasicEffect(_graphicsDevice);
            }

            return new QuadMeshRenderer(_graphicsDevice, _basicEffect, _settings, _statistics);
        }
    }
}
