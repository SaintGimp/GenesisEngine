using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public class SurfaceGenerator : ISurfaceGenerator
    {
        public DoubleVector3[] GenerateSurfaceVectors(int gridSize)
        {
            var surfaceVectors = new DoubleVector3[gridSize * gridSize];

            for (int row = 0; row < gridSize; row++)
            {
                for (int column = 0; column < gridSize; column++)
                {
                    var vector = GetVectorInPlanetSpace(column, row);
                    surfaceVectors[row * gridSize + column] = vector;
                }
            }

            return surfaceVectors;
        }

        DoubleVector3 GetVectorInPlanetSpace(int column, int row)
        {
            throw new NotImplementedException();
        }
    }
}