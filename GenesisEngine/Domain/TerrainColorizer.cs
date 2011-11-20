using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public interface ITerrainColorizer
    {
        Color GetColor(double terrainHeight, int column, int row, int gridSize, QuadNodeExtents extents);
    }

    public class TerrainColorizer : ITerrainColorizer
    {
        public Color GetColor(double terrainHeight, int column, int row, int gridSize, QuadNodeExtents extents)
        {
            if (terrainHeight > 0)
            {
                return Color.White;
            }
            else
            {
                return Color.Blue;
            }
        }
    }
}