namespace GenesisEngine
{
    public interface IQuadMeshRenderer : IRenderer
    {
        void Initialize(VertexPositionNormalColor[] vertices, short[] indices);
    }
}