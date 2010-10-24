using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public class Statistics
    {
        public Statistics()
        {
            NumberOfQuadNodesAtLevel = new int[30];
        }

        public float FrameRate { get; set; }

        public int NumberOfQuadNodes { get; set; }

        public int[] NumberOfQuadNodesAtLevel { get; private set; }

        public double CameraAltitude { get; set; }

        public int NumberOfQuadMeshesRendered { get; set; }

        public int PreviousNumberOfQuadMeshesRendered { get; set; }

        public void Flush()
        {
            PreviousNumberOfQuadMeshesRendered = NumberOfQuadMeshesRendered;
            NumberOfQuadMeshesRendered = 0;
        }
    }
}
