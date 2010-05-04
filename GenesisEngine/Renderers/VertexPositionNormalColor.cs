using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public struct VertexPositionNormalColor : IVertexType
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
          new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
          new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
          new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }
}
