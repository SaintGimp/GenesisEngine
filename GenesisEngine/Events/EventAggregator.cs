using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenesisEngine
{
    public class EventAggregator : IEventAggregator
    {
        object _lockObject = new object();
        List<WeakReference> _listeners = new List<WeakReference>();

        public void SendMessage<T>(T message)
        {
            IEnumerable<IListener<T>> recipients;
            lock (_lockObject)
            {
                recipients = FindEligibleListeners<T>();
            }

            SendMessageToRecipients(message, recipients);
        }

        private void SendMessageToRecipients<T>(T message, IEnumerable<IListener<T>> recipients)
        {
            foreach (var recipient in recipients)
            {
                recipient.Handle(message);
            }
        }

        private IEnumerable<IListener<T>> FindEligibleListeners<T>()
        {
            var eligibleListeners = new List<IListener<T>>();
            foreach (var weakReference in _listeners)
            {
                // We need to create a strong reference before testing aliveness
                // so that the GC doesn't yank it out from under us.  Don't convert
                // this to a LINQ expression because it doesn't guarentee that behavior
                var strongReference = weakReference.Target as IListener<T>;
                if (strongReference != null)
                {
                    eligibleListeners.Add(strongReference);
                }
            }

            return eligibleListeners;
        }

        public void AddListener(object listener)
        {
            lock (_lockObject)
            {
                PruneDeadReferences();

                if (!HasListener(listener))
                {
                    _listeners.Add(new WeakReference((listener)));
                }
            }
        }

        protected bool HasListener(object listener)
        {
            return _listeners.Exists(x => x.Target == listener);
        }

        private void PruneDeadReferences()
        {
            _listeners.RemoveAll(x => x.Target == null);
        }
    }
}
