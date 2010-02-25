using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace GenesisEngine.Specs.DomainSpecs
{
    [Subject(typeof(QuadNodeExtents))]
    public class when_an_extent_width_is_queried : QuadNodeExtentsContext
    {
        It should_return_the_correct_width = () =>
            _extents.Width.ShouldEqual(2.0);
    }

    [Subject(typeof(QuadNodeExtents))]
    public class when_an_extent_is_split : QuadNodeExtentsContext
    {
        public static IList<QuadNodeExtents> _subExtents;
        
        Because of = () =>
            _subExtents = _extents.Split().ToList();

        It should_split_the_extent_into_four_subextents = () =>
            _subExtents.Count.ShouldEqual(4);

        It should_split_the_extent_into_subextents_covering_the_original_extent = () =>
        {
            _subExtents[0].West.ShouldEqual(-1.0);
            _subExtents[0].East.ShouldEqual(0.0);
            _subExtents[0].North.ShouldEqual(-1.0);
            _subExtents[0].South.ShouldEqual(0.0);

            _subExtents[1].West.ShouldEqual(0.0);
            _subExtents[1].East.ShouldEqual(1.0);
            _subExtents[1].North.ShouldEqual(-1.0);
            _subExtents[1].South.ShouldEqual(0.0);

            _subExtents[2].West.ShouldEqual(-1.0);
            _subExtents[2].East.ShouldEqual(0.0);
            _subExtents[2].North.ShouldEqual(0.0);
            _subExtents[2].South.ShouldEqual(1.0);

            _subExtents[3].West.ShouldEqual(0.0);
            _subExtents[3].East.ShouldEqual(1.0);
            _subExtents[3].North.ShouldEqual(0.0);
            _subExtents[3].South.ShouldEqual(1.0);
        };
    }

    public class QuadNodeExtentsContext
    {
        public static QuadNodeExtents _extents;

        Establish context = () =>
        {
            _extents = new QuadNodeExtents(-1.0, 1.0, -1.0, 1.0);
        };
    }
}
