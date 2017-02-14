using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public interface IListener
    {
    }

    public interface IListener<T> : IListener
    {
        void Handle(T message);
    }
}
