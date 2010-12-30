using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public class QuadNodeFactory : IQuadNodeFactory
    {
        readonly IQuadMeshFactory _meshFactory;
        readonly IQuadNodeRendererFactory _rendererFactory;
        readonly ISplitMergeStrategy _splitMergeStrategy;
        readonly Settings _settings;
        readonly Statistics _statistics;

        public QuadNodeFactory(IQuadMeshFactory meshFactory, IQuadNodeRendererFactory rendererFactory, ISplitMergeStrategy splitMergeStrategy, Settings settings, Statistics statistics)
        {
            _meshFactory = meshFactory;
            _rendererFactory = rendererFactory;
            _splitMergeStrategy = splitMergeStrategy;
            _settings = settings;
            _statistics = statistics;
        }

        public IQuadNode Create()
        {
            var mesh = _meshFactory.Create();
            var quadNodeRenderer = _rendererFactory.Create();
            return new QuadNode(mesh, this, _splitMergeStrategy, quadNodeRenderer, _settings, _statistics);
        }
    }
}
