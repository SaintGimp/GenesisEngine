using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Microsoft.Xna.Framework;

namespace GenesisEngine.Specs.DomainSpecs
{
    [Subject(typeof(TerrainColorizer))]
    public class when_a_terrain_vertex_above_sea_level_is_colorized : TerrainColorizerContext
    {
        Because of = () =>
            _color = _colorizer.GetColor(100, _column, _row, _gridSize, _extents);

        It should_color_the_vertex_white = () =>
            _color.ShouldEqual(Color.White);
    }

    [Subject(typeof(TerrainColorizer))]
    public class when_a_terrain_vertex_below_sea_level_is_colorized : TerrainColorizerContext
    {
        Because of = () =>
            _color = _colorizer.GetColor(-100, _column, _row, _gridSize, _extents);

        It should_color_the_vertex_blue = () =>
            _color.ShouldEqual(Color.Blue);
    }

    public class TerrainColorizerContext
    {
        public static Color _color;
        public static int _column;
        public static int _row;
        public static int _gridSize;
        public static QuadNodeExtents _extents;
        public static ITerrainColorizer _colorizer;

        Establish context = () =>
        {
            _column = 15;
            _row = 15;
            _gridSize = 65;
            _extents = new QuadNodeExtents(-1, 1, -1, 1);

            _colorizer = new TerrainColorizer();
        };
    }
}
