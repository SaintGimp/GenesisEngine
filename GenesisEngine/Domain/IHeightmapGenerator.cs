namespace GenesisEngine
{
    public interface IHeightmapGenerator
    {
        HeightmapSample[] GenerateHeightmapSamples(HeightmapDefinition definition);
    }
}