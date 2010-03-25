using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public interface IEventAggregator
    {
        void AddListener(object listener);

        void SendMessage<T>(T message);
    }
}
