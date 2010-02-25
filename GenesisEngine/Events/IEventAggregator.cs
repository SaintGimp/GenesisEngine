using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public interface IEventAggregator
    {
        void SendMessage<T>(T message);

        void AddListener(object listener);
    }
}
