using System;
using System.Collections.Generic;

namespace Akasha.Infrastructure
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void Register<T>(T service) where T : class
        {
            _services[typeof(T)] = service;
        }

        public static T Resolve<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
                return service as T;
            throw new InvalidOperationException($"Service {typeof(T).Name} not registered.");
        }
    }
}
