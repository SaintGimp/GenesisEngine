namespace GenesisEngine
{
    public interface IQuadMeshRenderer : IRenderer
    {
        void Initialize(VertexPositionNormalColored[] vertices, int[] indices);
    }
}