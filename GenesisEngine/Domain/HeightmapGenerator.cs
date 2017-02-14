using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public interface IHeightmapGenerator
    {
        HeightmapSample[] GenerateHeightmapSamples(HeightmapDefinition definition);
    }

    public class HeightmapGenerator : IHeightmapGenerator
    {
        readonly IHeightGenerator _heightGenerator;

        public HeightmapGenerator(IHeightGenerator heightGenerator)
        {
            _heightGenerator = heightGenerator;
        }

        public HeightmapSample[] GenerateHeightmapSamples(HeightmapDefinition definition)
        {
            var samples = new HeightmapSample[definition.GridSize * definition.GridSize];

            for (int row = 0; row < definition.GridSize; row++)
            {
                for (int column = 0; column < definition.GridSize; column++)
                {
                    var sample = GetSampleInPlanetSpace(definition, column, row);
                    samples[row * definition.GridSize + column] = sample;
                }
            }

            return samples;
        }

        HeightmapSample GetSampleInPlanetSpace(HeightmapDefinition definition, int column, int row)
        {
            // Check out "Textures and Modelling - A Procedural Approach" by Ken Musgaves 

            // We have several different reference frames to deal with here:
            //   "Quad grid" coordinates which is the column and row index of the vertex in the mesh grid
            //   "Unit plane" coordinates which is the coordinates of the vertex on the unit plane of this quadtree
            //   "Unit sphere" coordinates which is the coordinates of the vertex on the unit sphere arc of this quadtree
            //   "Planet space" coordinates which is the coordinates of the vertex in real units relative to the planet center

            // We start with quad grid coordinates.  We first convert the quad coordinates into a unit plane vector that
            // points to the equivalent point on the quadtree's plane.  Then we project the unit plane to its equivalent unit
            // sphere vector.  We then calculate the terrain height for the vertex and use that information to extend the unit
            // sphere vector to the proper length for the real-space size of our planet.

            var unitPlaneVector = ConvertToUnitPlaneVector(definition, column, row);
            var unitSphereVector = unitPlaneVector.ProjectUnitPlaneToUnitSphere();
            var terrainHeight = _heightGenerator.GetHeight(unitSphereVector, definition.QuadLevel, 8000);
            var planetSpaceVector = ConvertToPlanetSpace(definition, unitSphereVector, terrainHeight);

            return new HeightmapSample()
            {
                Height = terrainHeight,
                Vector = planetSpaceVector
            };
        }

        DoubleVector3 ConvertToUnitPlaneVector(HeightmapDefinition definition, int column, int row)
        {
            var uDelta = definition.UVector * (definition.Extents.North + (row * definition.Stride));
            var vDelta = definition.VVector * (definition.Extents.West + (column * definition.Stride));
            var convertedVector = definition.PlaneNormalVector + uDelta + vDelta;

            return convertedVector;
        }

        DoubleVector3 ConvertToPlanetSpace(HeightmapDefinition definition, DoubleVector3 sphereUnitVector, double terrainHeight)
        {
            return sphereUnitVector * (definition.PlanetRadius + terrainHeight);
        }
    }

    public class HeightmapSample
    {
        public double Height;
        public DoubleVector3 Vector;
    }

    public class HeightmapDefinition
    {
        public int GridSize;
        public double Stride;
        public DoubleVector3 PlaneNormalVector;
        public DoubleVector3 UVector;
        public DoubleVector3 VVector;
        public QuadNodeExtents Extents;
        public int QuadLevel;
        public double PlanetRadius;
    }
}