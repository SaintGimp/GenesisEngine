//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
////using Machine.Specifications;

//namespace GenesisEngine.Specs.DomainSpecs
//{
//    [Subject(typeof(SplitMergeStrategy))]
//    public class when_the_quad_is_above_the_horizon_and_the_camera_is_close : SplitMergeStrategyContext
//    {
//        It should_split = () =>
//            _strategy.ShouldSplit(0, _aboveHorizonBoundingBox, _nearCameraLocation, _planetLocation, _planetRadius).
//                ShouldBeTrue();

//        It should_split = () =>
//            _strategy.ShouldSplit(0, _aboveHorizonBoundingBox, _nearCameraLocation, _planetLocation, _planetRadius).
//                ShouldBeTrue();
//    }

//    public class SplitMergeStrategyContext
//    {
//        public static DoubleVector3 _planetLocation;
//        public static double _planetRadius;
//        public static DoubleVector3 _nearCameraLocation;
//        public static DoubleBoundingBox _aboveHorizonBoundingBox;
//        public static bool _answer;
//        public static SplitMergeStrategy _strategy;

//        Establish context = () =>
//        {
//            _planetLocation = DoubleVector3.Zero;
//            _planetRadius = 10.0;
//            _nearCameraLocation = DoubleVector3.Up * (_planetRadius + 1);
//            _aboveHorizonBoundingBox = new DoubleBoundingBox(new DoubleVector3(-_planetRadius, 0, _planetRadius), new DoubleVector3(_planetRadius, _planetRadius, -_planetRadius));

//            _strategy = new SplitMergeStrategy();
//        };
//    }
//}
