using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using NSubstitute;

namespace GenesisEngine.Specs.UISpecs
{
    [Subject(typeof(ScreenCustodian<ITestView, ITestViewModel>))]
    public class when_the_screen_is_activated : ScreenCustodianContext
    {
        Because of = () =>
            _custodian.ShowInactive();

        It should_connect_the_view_to_the_model = () =>
            _view.Model.ShouldEqual(_model);

        It should_show_the_view = () =>
            _view.Received().Show();
    }

    public class ScreenCustodianContext
    {
        public static ITestView _view;
        public static ITestViewModel _model;
        public static ScreenCustodian<ITestView, ITestViewModel> _custodian;

        Establish context = () =>
        {
            _view = Substitute.For<ITestView>();
            _model = Substitute.For<ITestViewModel>();
            _custodian = new ScreenCustodian<ITestView, ITestViewModel>(_view, _model);
        };
    }

    public interface ITestView : IView
    {
    }

    public interface ITestViewModel
    {
        
    }
}
