using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public struct VertexPositionNormalColored
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        public static int SizeInBytes = 7 * 4;
        public static VertexElement[] VertexElements = new VertexElement[]
        {
          new VertexElement( 0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0 ),
          new VertexElement( 0, sizeof(float) * 3, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, 0 ),
          new VertexElement( 0, sizeof(float) * 4, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0 ),
        };
    }
}
