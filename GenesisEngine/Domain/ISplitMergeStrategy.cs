using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public interface ISplitMergeStrategy
    {
        bool ShouldSplit(IQuadMesh mesh, bool hasSubnodes, bool splitOrMergeInProgress, int level);

        bool ShouldMerge(IQuadMesh mesh, bool hasSubnodes, bool splitOrMergeInProgress);
    }
}