using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Rhino.Mocks;

namespace GenesisEngine.Specs.DomainSpecs
{
    [Subject(typeof(DefaultSplitMergeStrategy))]
    public class when_the_quad_mesh_is_above_the_horizon_and_close_to_the_camera_and_there_are_no_subnodes_and_no_splits_or_merges_are_in_progress_and_node_is_not_at_maximum_level : SplitMergeStrategyContext
    {
        Establish context = () =>
        {
            _mesh.Stub(x => x.IsAboveHorizonToCamera).Return(true);
            _mesh.Stub(x => x.CameraDistanceToWidthRatio).Return(0.1);
        };

        It should_recommend_a_split = () =>
            _strategy.ShouldSplit(_mesh, false, false, 5).ShouldBeTrue();
    }

    [Subject(typeof(DefaultSplitMergeStrategy))]
    public class when_the_quad_is_otherwise_splittable_but_the_mesh_is_below_the_horizon : SplitMergeStrategyContext
    {
        Establish context = () =>
        {
            _mesh.Stub(x => x.IsAboveHorizonToCamera).Return(false);
            _mesh.Stub(x => x.CameraDistanceToWidthRatio).Return(0.1);
        };

        It should_not_recommend_a_split = () =>
            _strategy.ShouldSplit(_mesh, false, false, 5).ShouldBeFalse();
    }

    [Subject(typeof(DefaultSplitMergeStrategy))]
    public class when_the_quad_is_otherwise_splittable_but_the_mesh_is_far_from_the_camera : SplitMergeStrategyContext
    {
        Establish context = () =>
        {
            _mesh.Stub(x => x.IsAboveHorizonToCamera).Return(true);
            _mesh.Stub(x => x.CameraDistanceToWidthRatio).Return(10);
        };

        It should_not_recommend_a_split = () =>
            _strategy.ShouldSplit(_mesh, false, false, 5).ShouldBeFalse();
    }

    [Subject(typeof(DefaultSplitMergeStrategy))]
    public class when_the_quad_is_otherwise_splittable_but_it_already_has_subnodes : SplitMergeStrategyContext
    {
        Establish context = () =>
        {
            _mesh.Stub(x => x.IsAboveHorizonToCamera).Return(true);
            _mesh.Stub(x => x.CameraDistanceToWidthRatio).Return(0.1);
        };

        It should_not_recommend_a_split = () =>
            _strategy.ShouldSplit(_mesh, true, false, 5).ShouldBeFalse();
    }

    [Subject(typeof(DefaultSplitMergeStrategy))]
    public class when_the_quad_is_otherwise_splittable_but_a_split_or_merge_is_already_in_progress : SplitMergeStrategyContext
    {
        Establish context = () =>
        {
            _mesh.Stub(x => x.IsAboveHorizonToCamera).Return(true);
            _mesh.Stub(x => x.CameraDistanceToWidthRatio).Return(0.1);
        };

        It should_not_recommend_a_split = () =>
            _strategy.ShouldSplit(_mesh, false, true, 5).ShouldBeFalse();
    }

    [Subject(typeof(DefaultSplitMergeStrategy))]
    public class when_the_quad_is_otherwise_splittable_but_it_is_at_the_maximum_level : SplitMergeStrategyContext
    {
        Establish context = () =>
        {
            _mesh.Stub(x => x.IsAboveHorizonToCamera).Return(true);
            _mesh.Stub(x => x.CameraDistanceToWidthRatio).Return(0.1);
        };

        It should_not_recommend_a_split = () =>
            _strategy.ShouldSplit(_mesh, false, false, 10).ShouldBeFalse();
    }

    [Subject(typeof(DefaultSplitMergeStrategy))]
    public class when_the_quad_mesh_is_below_the_horizon_and_close_to_the_camera_and_there_are_subnodes_and_no_splits_or_merges_are_in_progress : SplitMergeStrategyContext
    {
        Establish context = () =>
        {
            _mesh.Stub(x => x.IsAboveHorizonToCamera).Return(false);
            _mesh.Stub(x => x.CameraDistanceToWidthRatio).Return(0.1);
        };

        It should_recommend_a_merge = () =>
            _strategy.ShouldMerge(_mesh, true, false).ShouldBeTrue();
    }

    [Subject(typeof(DefaultSplitMergeStrategy))]
    public class when_the_quad_mesh_is_above_the_horizon_and_far_from_the_camera_and_there_are_subnodes_and_no_splits_or_merges_are_in_progress : SplitMergeStrategyContext
    {
        Establish context = () =>
        {
            _mesh.Stub(x => x.IsAboveHorizonToCamera).Return(true);
            _mesh.Stub(x => x.CameraDistanceToWidthRatio).Return(10);
        };

        It should_recommend_a_merge = () =>
            _strategy.ShouldMerge(_mesh, true, false).ShouldBeTrue();
    }

    [Subject(typeof(DefaultSplitMergeStrategy))]
    public class when_the_quad_is_otherwise_mergable_but_the_quad_mesh_is_above_the_horizon_and_close_to_the_camera : SplitMergeStrategyContext
    {
        Establish context = () =>
        {
            _mesh.Stub(x => x.IsAboveHorizonToCamera).Return(true);
            _mesh.Stub(x => x.CameraDistanceToWidthRatio).Return(0.1);
        };

        It should_not_recommend_a_merge = () =>
            _strategy.ShouldMerge(_mesh, true, false).ShouldBeFalse();
    }

    [Subject(typeof(DefaultSplitMergeStrategy))]
    public class when_the_quad_is_otherwise_mergable_but_it_does_not_have_subnodes : SplitMergeStrategyContext
    {
        Establish context = () =>
        {
            _mesh.Stub(x => x.IsAboveHorizonToCamera).Return(false);
            _mesh.Stub(x => x.CameraDistanceToWidthRatio).Return(10);
        };

        It should_not_recommend_a_merge = () =>
            _strategy.ShouldMerge(_mesh, false, false).ShouldBeFalse();
    }

    [Subject(typeof(DefaultSplitMergeStrategy))]
    public class when_the_quad_is_otherwise_mergable_but_a_split_or_merge_is_already_in_progress : SplitMergeStrategyContext
    {
        Establish context = () =>
        {
            _mesh.Stub(x => x.IsAboveHorizonToCamera).Return(false);
            _mesh.Stub(x => x.CameraDistanceToWidthRatio).Return(10);
        };

        It should_not_recommend_a_merge = () =>
            _strategy.ShouldMerge(_mesh, true, true).ShouldBeFalse();
    }

    public class SplitMergeStrategyContext
    {
        public static IQuadMesh _mesh;
        public static ISettings _settings;
        public static DefaultSplitMergeStrategy _strategy;

        Establish context = () =>
        {
            _mesh = MockRepository.GenerateStub<IQuadMesh>();
            _settings = MockRepository.GenerateStub<ISettings>();
            _strategy = new DefaultSplitMergeStrategy(_settings);

            _settings.MaximumQuadNodeLevel = 10;
        };
    }
}
