using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public interface IQuadNodeFactory
    {
        IQuadNode Create();
    }

    public class QuadNodeFactory : IQuadNodeFactory
    {
        readonly IQuadMeshFactory _meshFactory;
        readonly IQuadNodeRendererFactory _rendererFactory;
        readonly ISplitMergeStrategy _splitMergeStrategy;
        readonly ITaskSchedulerFactory _taskSchedulerFactory;
        readonly Statistics _statistics;

        public QuadNodeFactory(IQuadMeshFactory meshFactory, ISplitMergeStrategy splitMergeStrategy, ITaskSchedulerFactory taskSchedulerFactory, IQuadNodeRendererFactory rendererFactory, Statistics statistics)
        {
            _meshFactory = meshFactory;
            _rendererFactory = rendererFactory;
            _splitMergeStrategy = splitMergeStrategy;
            _taskSchedulerFactory = taskSchedulerFactory;
            _statistics = statistics;
        }

        public IQuadNode Create()
        {
            var mesh = _meshFactory.Create();
            var quadNodeRenderer = _rendererFactory.Create();
            return new QuadNode(mesh, this, _splitMergeStrategy, _taskSchedulerFactory, quadNodeRenderer, _statistics);
        }
    }
}
