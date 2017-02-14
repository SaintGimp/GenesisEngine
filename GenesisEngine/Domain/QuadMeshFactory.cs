using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public interface IQuadMeshFactory
    {
        IQuadMesh Create();
    }

    public class QuadMeshFactory : IQuadMeshFactory
    {
        readonly IHeightmapGenerator _generator;
        readonly ITerrainColorizer _terrainColorizer;
        readonly IQuadMeshRendererFactory _rendererFactory;
        readonly Settings _settings;

        public QuadMeshFactory(IHeightmapGenerator generator, ITerrainColorizer terrainColorizer, IQuadMeshRendererFactory rendererFactory, Settings settings)
        {
            _generator = generator;
            _terrainColorizer = terrainColorizer;
            _rendererFactory = rendererFactory;
            _settings = settings;
        }

        public IQuadMesh Create()
        {
            var quadMeshRenderer = _rendererFactory.Create();
            return new QuadMesh(_generator, _terrainColorizer, quadMeshRenderer, _settings);
        }
    }
}
