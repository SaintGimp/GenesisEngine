using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Microsoft.Xna.Framework;
using Rhino.Mocks;

namespace GenesisEngine.Specs.DomainSpecs
{
    [Subject(typeof(EdgeColorizer))]
    public class when_the_edges_of_a_root_node_mesh_are_colorized : EdgeColorizerContext
    {
        Establish context = () =>
        {
            _settings.ShowQuadBoundaries = true;
            _extents = new QuadNodeExtents(-1, 1, -1, 1);
        };

        // TODO: I'm not happy about departing from the canonical MSpec spec pattern here but
        // I suppose what I'm really looking for here is a data-driven system which we don't have.
        
        It should_color_all_edges_green = () =>
        {
            _colorizer.GetColor(100, 0, 10, _gridSize, _extents).ShouldEqual(Color.Green);
            _colorizer.GetColor(100, _gridSize - 1, 10, _gridSize, _extents).ShouldEqual(Color.Green);
            _colorizer.GetColor(100, 10, 0, _gridSize, _extents).ShouldEqual(Color.Green);
            _colorizer.GetColor(100, 10, _gridSize - 1, _gridSize, _extents).ShouldEqual(Color.Green);
        };

        It should_not_color_the_interior = () =>
            _colorizer.GetColor(100, 10, 10, _gridSize, _extents).ShouldEqual(Color.White);
    }

    [Subject(typeof(EdgeColorizer))]
    public class when_the_edges_of_an_interior_mesh_are_colorized : EdgeColorizerContext
    {
        Establish context = () =>
        {
            _settings.ShowQuadBoundaries = true;
            _extents = new QuadNodeExtents(-0.5, 0.5, -0.5, 0.5);
        };

        It should_color_all_edges_red = () =>
        {
            _colorizer.GetColor(100, 0, 10, _gridSize, _extents).ShouldEqual(Color.Red);
            _colorizer.GetColor(100, _gridSize - 1, 10, _gridSize, _extents).ShouldEqual(Color.Red);
            _colorizer.GetColor(100, 10, 0, _gridSize, _extents).ShouldEqual(Color.Red);
            _colorizer.GetColor(100, 10, _gridSize - 1, _gridSize, _extents).ShouldEqual(Color.Red);
        };

        It should_not_color_the_interior = () =>
            _colorizer.GetColor(100, 10, 10, _gridSize, _extents).ShouldEqual(Color.White);
    }

    [Subject(typeof(EdgeColorizer))]
    public class when_the_edges_of_an_outside_mesh_are_colorized : EdgeColorizerContext
    {
        Establish context = () =>
        {
            _settings.ShowQuadBoundaries = true;
            _extents = new QuadNodeExtents(-1, 0, -1, 0);
        };

        It should_color_the_outside_edges_green = () =>
        {
            _colorizer.GetColor(100, 0, 10, _gridSize, _extents).ShouldEqual(Color.Green);
            _colorizer.GetColor(100, 10, 0, _gridSize, _extents).ShouldEqual(Color.Green);
        };

        It should_color_the_inside_edges_red = () =>
        {
            _colorizer.GetColor(100, _gridSize - 1, 10, _gridSize, _extents).ShouldEqual(Color.Red);
            _colorizer.GetColor(100, 10, _gridSize - 1, _gridSize, _extents).ShouldEqual(Color.Red);
        };

        It should_not_color_the_interior = () =>
            _colorizer.GetColor(100, 10, 10, _gridSize, _extents).ShouldEqual(Color.White);
    }

    [Subject(typeof(EdgeColorizer))]
    public class when_the_show_boundaries_setting_is_off : EdgeColorizerContext
    {
        Establish context = () =>
        {
            _settings.ShowQuadBoundaries = false;
            _extents = new QuadNodeExtents(-1, 0, -1, 0);
        };

        It should_not_color_any_edges = () =>
        {
            _colorizer.GetColor(100, 0, 10, _gridSize, _extents).ShouldEqual(Color.White);
            _colorizer.GetColor(100, _gridSize - 1, 10, _gridSize, _extents).ShouldEqual(Color.White);
            _colorizer.GetColor(100, 10, 0, _gridSize, _extents).ShouldEqual(Color.White);
            _colorizer.GetColor(100, 10, _gridSize - 1, _gridSize, _extents).ShouldEqual(Color.White);
        };
    }

    public class EdgeColorizerContext
    {
        public static int _gridSize;
        public static QuadNodeExtents _extents;
        public static Color _color;

        public static ISettings _settings;
        public static ITerrainColorizer _baseColorizer;
        public static ITerrainColorizer _colorizer;

        Establish context = () =>
        {
            _gridSize = 65;

            _settings = MockRepository.GenerateStub<ISettings>();
            _baseColorizer = MockRepository.GenerateStub<ITerrainColorizer>();
            _baseColorizer.Stub(x => x.GetColor(Arg<double>.Is.Anything, Arg<int>.Is.Anything, Arg<int>.Is.Anything, Arg<int>.Is.Anything, Arg<QuadNodeExtents>.Is.Anything))
                .Return(Color.White);

            _colorizer = new EdgeColorizer(_baseColorizer, _settings);
        };
    }
}
