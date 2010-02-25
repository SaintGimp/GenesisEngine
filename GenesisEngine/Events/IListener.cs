using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public interface IListener<T>
    {
        void Handle(T message);
    }
}
