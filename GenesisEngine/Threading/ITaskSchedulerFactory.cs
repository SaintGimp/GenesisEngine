using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenesisEngine
{
    public interface ITaskSchedulerFactory
    {
        TaskScheduler CreateFor(int level);
    }
}
