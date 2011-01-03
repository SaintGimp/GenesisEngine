using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;

namespace GenesisEngine
{
    public class QueuedTaskSchedulerFactory : ITaskSchedulerFactory
    {
        readonly QueuedTaskScheduler _scheduler;
        readonly Dictionary<int, TaskScheduler> _queues = new Dictionary<int, TaskScheduler>();

        public QueuedTaskSchedulerFactory()
        {
            // TODO: hook up the number of threads to settings and figure out the optimal value.
            // We expect node splitting to have a lot of blocking when the work is moved to the GPU
            // so more than the number of cores is probably desired.

            // TODO: we might be able to build our own prioritized task scheduler that takes more things
            // into account than just the level of the node.  For instance, we could prioritize
            // currently-visible nodes over non-visible nodes.  To do this we'd probably drop
            // QueuedTaskScheduler in favor of a custom scheduler that uses Task.AsyncState to get
            // information about each queued node and then sorts them accordingly.
            _scheduler = new QueuedTaskScheduler(0, "", false, ThreadPriority.BelowNormal);
        }

        public TaskScheduler Create()
        {
            return CreateForLevel(0);
        }

        public TaskScheduler CreateForLevel(int level)
        {
            TaskScheduler queue;
            if (!_queues.TryGetValue(level, out queue))
            {
                queue = _scheduler.ActivateNewQueue(level);
                _queues.Add(level, queue);
            }

            return queue;
        }
    }
}