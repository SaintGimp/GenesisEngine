using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace GenesisEngine.Specs.UISpecs
{
    [Subject(typeof(StatisticsViewModel))]
    public class when_updated : StatisticsViewModelContext
    {
        Establish context = () =>
        {
            _statistics.FrameRate = 10;
        };

        Because of = () =>
            _model.Update();

        It update_the_list_of_statistics = () =>
            _model.StatisticsList.Any(x => x.Contains("10")).ShouldBeTrue();
    }

    public class StatisticsViewModelContext
    {
        public static Statistics _statistics;
        public static StatisticsViewModel _model;

        Establish context = () =>
        {
            _statistics = new Statistics();
            _statistics.NumberOfQuadNodesAtLevel[0] = 6;
            _model = new StatisticsViewModel(_statistics);
        };
    }
}
