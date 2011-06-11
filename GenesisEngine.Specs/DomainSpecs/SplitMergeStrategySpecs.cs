using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using NSubstitute;

namespace GenesisEngine.Specs.DomainSpecs
{
    [Subject(typeof(DefaultSplitMergeStrategy))]
    public class when_the_quad_mesh_is_above_the_horizon_and_close_to_the_camera_and_node_is_not_at_maximum_level : SplitMergeStrategyContext
    {
        Establish context = () =>
        {
            _mesh.IsAboveHorizonToCamera.Returns(true);
            _mesh.CameraDistanceToWidthRatio.Returns(0.1);
        };

        It should_recommend_a_split = () =>
            _strategy.ShouldSplit(_mesh, 5).ShouldBeTrue();
    }

    [Subject(typeof(DefaultSplitMergeStrategy))]
    public class when_the_quad_is_otherwise_splittable_but_the_mesh_is_below_the_horizon : SplitMergeStrategyContext
    {
        Establish context = () =>
        {
            _mesh.IsAboveHorizonToCamera.Returns(false);
            _mesh.CameraDistanceToWidthRatio.Returns(0.1);
        };

        It should_not_recommend_a_split = () =>
            _strategy.ShouldSplit(_mesh, 5).ShouldBeFalse();
    }

    [Subject(typeof(DefaultSplitMergeStrategy))]
    public class when_the_quad_is_otherwise_splittable_but_the_mesh_is_far_from_the_camera : SplitMergeStrategyContext
    {
        Establish context = () =>
        {
            _mesh.IsAboveHorizonToCamera.Returns(true);
            _mesh.CameraDistanceToWidthRatio.Returns(10);
        };

        It should_not_recommend_a_split = () =>
            _strategy.ShouldSplit(_mesh, 5).ShouldBeFalse();
    }

    [Subject(typeof(DefaultSplitMergeStrategy))]
    public class when_the_quad_is_otherwise_splittable_but_it_is_at_the_maximum_level : SplitMergeStrategyContext
    {
        Establish context = () =>
        {
            _mesh.IsAboveHorizonToCamera.Returns(true);
            _mesh.CameraDistanceToWidthRatio.Returns(0.1);
        };

        It should_not_recommend_a_split = () =>
            _strategy.ShouldSplit(_mesh, 10).ShouldBeFalse();
    }

    [Subject(typeof(DefaultSplitMergeStrategy))]
    public class when_the_quad_mesh_is_below_the_horizon_and_close_to_the_camera : SplitMergeStrategyContext
    {
        Establish context = () =>
        {
            _mesh.IsAboveHorizonToCamera.Returns(false);
            _mesh.CameraDistanceToWidthRatio.Returns(0.1);
        };

        It should_recommend_a_merge = () =>
            _strategy.ShouldMerge(_mesh).ShouldBeTrue();
    }

    [Subject(typeof(DefaultSplitMergeStrategy))]
    public class when_the_quad_mesh_is_above_the_horizon_and_far_from_the_camera : SplitMergeStrategyContext
    {
        Establish context = () =>
        {
            _mesh.IsAboveHorizonToCamera.Returns(true);
            _mesh.CameraDistanceToWidthRatio.Returns(10);
        };

        It should_recommend_a_merge = () =>
            _strategy.ShouldMerge(_mesh).ShouldBeTrue();
    }

    [Subject(typeof(DefaultSplitMergeStrategy))]
    public class when_the_quad_is_otherwise_mergable_but_the_quad_mesh_is_above_the_horizon_and_close_to_the_camera : SplitMergeStrategyContext
    {
        Establish context = () =>
        {
            _mesh.IsAboveHorizonToCamera.Returns(true);
            _mesh.CameraDistanceToWidthRatio.Returns(0.1);
        };

        It should_not_recommend_a_merge = () =>
            _strategy.ShouldMerge(_mesh).ShouldBeFalse();
    }

    public class SplitMergeStrategyContext
    {
        public static IQuadMesh _mesh;
        public static ISettings _settings;
        public static DefaultSplitMergeStrategy _strategy;

        Establish context = () =>
        {
            _mesh = Substitute.For<IQuadMesh>();
            _settings = Substitute.For<ISettings>();
            _strategy = new DefaultSplitMergeStrategy(_settings);

            _settings.MaximumQuadNodeLevel = 10;
        };
    }
}
