using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public class Statistics
    {
        public float FrameRate;

        public int NumberOfQuadNodes;

        public int[] NumberOfQuadNodesAtLevel = new int[30];

        public double CameraAltitude;

        public int NumberOfQuadMeshesRendered;

        public int PreviousNumberOfQuadMeshesRendered;

        public int NumberOfPendingSplits;

        public int NumberOfPendingMerges;

        public int NumberOfSplitsScheduledPerInterval;

        public int NumberOfSplitsCancelledPerInterval;

        public void Flush()
        {
            PreviousNumberOfQuadMeshesRendered = NumberOfQuadMeshesRendered;
            NumberOfQuadMeshesRendered = 0;
        }
    }
}
