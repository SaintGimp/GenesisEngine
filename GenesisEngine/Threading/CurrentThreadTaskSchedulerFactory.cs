using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;

namespace GenesisEngine
{
    public class CurrentThreadTaskSchedulerFactory : ITaskSchedulerFactory
    {
        readonly CurrentThreadTaskScheduler _scheduler;

        public CurrentThreadTaskSchedulerFactory()
        {
            _scheduler = new CurrentThreadTaskScheduler();
        }

        public TaskScheduler Create()
        {
            return _scheduler;
        }

        public TaskScheduler CreateForLevel(int level)
        {
            return _scheduler;
        }
    }
}