using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public class QuadMeshFactory : IQuadMeshFactory
    {
        readonly IQuadMeshRendererFactory _rendererFactory;
        readonly IHeightfieldGenerator _generator;
        readonly Settings _settings;

        public QuadMeshFactory(IQuadMeshRendererFactory rendererFactory, IHeightfieldGenerator generator, Settings settings)
        {
            _rendererFactory = rendererFactory;
            _generator = generator;
            _settings = settings;
        }

        public IQuadMesh Create()
        {
            var quadMeshRenderer = _rendererFactory.Create();
            return new QuadMesh(_generator, quadMeshRenderer, _settings);
        }
    }
}
