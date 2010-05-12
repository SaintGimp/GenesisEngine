using System;
using Microsoft.Xna.Framework;

namespace GenesisEngine
{
    public class EdgeColorizer : ITerrainColorizer
    {
        readonly ITerrainColorizer _baseColorizer;
        readonly ISettings _settings;

        public EdgeColorizer(ITerrainColorizer baseColorizer, ISettings settings)
        {
            _baseColorizer = baseColorizer;
            _settings = settings;
        }

        public Color GetColor(double terrainHeight, int column, int row, int gridSize, QuadNodeExtents extents)
        {
            var color = _baseColorizer.GetColor(terrainHeight, column, row, gridSize, extents);

            if (!_settings.ShowQuadBoundaries)
            {
                return color;
            }

            if (row == 0)
            {
                color = extents.North == -1 ? Color.Green : Color.Red;
            }
            else if (row == gridSize - 1)
            {
                color = extents.South == 1 ? Color.Green : Color.Red;
            }
            else if (column == 0)
            {
                color = extents.West == -1 ? Color.Green : Color.Red;
            }
            else if (column == gridSize - 1)
            {
                color = extents.East == 1 ? Color.Green : Color.Red;
            }

            return color;
        }
    }
}