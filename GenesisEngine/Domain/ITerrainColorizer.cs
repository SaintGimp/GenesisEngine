using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public interface ITerrainColorizer
    {
        Color GetColor(double terrainHeight, int column, int row, int gridSize, QuadNodeExtents extents);
    }
}