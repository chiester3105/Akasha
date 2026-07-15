using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akasha.Events
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();

        public void Publish<T>(T @event) where T : class
        {
            if (_handlers.TryGetValue(typeof(T), out var delegates))
            {
                foreach (Action<T> handler in delegates)
                    handler?.Invoke(@event);
            }
        }

        public void Subscribe<T>(Action<T> handler) where T : class
        {
            var type = typeof(T);
            if (!_handlers.ContainsKey(type))
                _handlers[type] = new List<Delegate>();
            _handlers[type].Add(handler);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : class
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var delegates))
                delegates.Remove(handler);
        }
    }
}
