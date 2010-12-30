using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    // TODO: I'm not entirely convinced this is the right design yet.  It does move a combinatorial explosion of states
    // out of the quadnode and simplifies the quadnode tests, which is good.  But it feels like we've moved too much
    // of the responsibility into the strategy object.  For instance, testing whether a split or merge is already in
    // progress - shouldn't that be intrinsic to the quadnode?  It's not like a different strategy would ever make
    // a different decision about that, unlike the part about where the mesh is in relation to the camera

    public class DefaultSplitMergeStrategy : ISplitMergeStrategy
    {
        readonly ISettings _settings;

        public DefaultSplitMergeStrategy(ISettings settings)
        {
            _settings = settings;
        }

        public bool ShouldSplit(IQuadMesh mesh, int level)
        {
            return (IsMeshCloseAndVisible(mesh) && !IsAtMaximumLevelOfDetail(level));
        }

        bool IsMeshCloseAndVisible(IQuadMesh mesh)
        {
            return mesh.IsAboveHorizonToCamera && mesh.CameraDistanceToWidthRatio < 1;
        }

        bool IsAtMaximumLevelOfDetail(int level)
        {
            return level >= _settings.MaximumQuadNodeLevel;
        }

        public bool ShouldMerge(IQuadMesh mesh)
        {
            return (!IsMeshCloseAndVisible(mesh));
        }
    }
}