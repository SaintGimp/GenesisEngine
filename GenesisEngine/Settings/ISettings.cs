using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    // TODO: it's not clear this is needed
	public interface ISettings
	{
		bool ShouldUpdate { get; set; }
	
        bool ShouldSingleStep { get; set; }

        bool ShouldDrawWireframe { get; set; }

        DoubleVector3 CameraStartingLocation { get; set; }

        DoubleVector3 CameraStartingLookAt { get; set; }

        double CameraMoveSpeedPerSecond { get; set; }

        float CameraMouseLookDamping { get; set; }

        int MaximumQuadNodeLevel { get; set;  }

        bool ShowQuadBoundaries { get; set; }

        double FarClippingPlaneDistance { get; set; }
    }
}
