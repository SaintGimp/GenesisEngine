using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public interface IQuadMeshRenderer : IRenderer
    {
        void Initialize(VertexPositionNormalColor[] vertices, short[] indices, BoundingBox boundingBox);
    }
}