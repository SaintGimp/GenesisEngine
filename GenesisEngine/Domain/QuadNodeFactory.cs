using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public class QuadNodeFactory : IQuadNodeFactory
    {
        readonly QuadNodeRendererFactory _rendererFactory;
        readonly IHeightfieldGenerator _generator;
        readonly Settings _settings;
        readonly Statistics _statistics;

        public QuadNodeFactory(QuadNodeRendererFactory rendererFactory, IHeightfieldGenerator generator, Settings settings, Statistics statistics)
        {
            _rendererFactory = rendererFactory;
            _generator = generator;
            _settings = settings;
            _statistics = statistics;
        }

        public IQuadNode Create()
        {
            var quadNodeRenderer = _rendererFactory.Create();
            return new QuadNode(this, _generator, quadNodeRenderer, _settings, _statistics);
        }
    }
}
