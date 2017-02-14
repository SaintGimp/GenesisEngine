using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public interface IScreenCustodian<TView, TModel>
    {
        void ShowInactive();
    }

    public class ScreenCustodian<TView, TModel> : IScreenCustodian<TView, TModel> where TView : IView
    {
        TView _view;
        TModel _model;

        public ScreenCustodian(TView view, TModel model)
        {
            _view = view;
            _model = model;

            _view.Model = _model;
        }

        public void ShowInactive()
        {
            _view.Show();
        }
    }
}
