using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public interface IView
    {
        object Model { get; set; }

        void Show();
    }
}
