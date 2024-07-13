using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace HK
{
    public class TinyServiceLocator
    {
        private static readonly Dictionary<Type, (object service, CancellationTokenSource scope)> services = new();

        private static readonly Dictionary<Type, Dictionary<string, (object service, CancellationTokenSource scope)>> namedServices = new();

        public static async UniTaskVoid RegisterAsync<T>(T service, CancellationToken cancellationToken = default)
        {
            if (services.ContainsKey(typeof(T)))
            {
                Debug.LogError($"Service already registered: {typeof(T)}");
                return;
            }
            var scope = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
#if UNITY_EDITOR
            scope = CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken);
#endif
            services[typeof(T)] = (service, scope);
            await scope.Token.ToUniTask().Item1;
            services[typeof(T)].scope.Dispose();
            services.Remove(typeof(T));
        }

        public static async UniTaskVoid RegisterAsync<T>(string name, T service, CancellationToken cancellationToken = default)
        {
            if (!namedServices.TryGetValue(typeof(T), out var namedService))
            {
                namedService = new Dictionary<string, (object service, CancellationTokenSource scope)>();
                namedServices.Add(typeof(T), namedService);
            }

            if (namedService.ContainsKey(name))
            {
                Debug.LogError($"Service already registered: {typeof(T)}, name: {name}");
                return;
            }

            var scope = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
#if UNITY_EDITOR
            scope = CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken);
#endif
            namedService[name] = (service, scope);
            await scope.Token.ToUniTask().Item1;
            namedService[name].scope.Dispose();
            namedService.Remove(name);
        }

        public static void Register<T>(T service)
        {
            RegisterAsync(service).Forget();
        }

        public static void Register<T>(string name, T service)
        {
            RegisterAsync(name, service).Forget();
        }

        public static T Resolve<T>()
        {
            Assert.IsTrue(services.ContainsKey(typeof(T)), $"Service not found: {typeof(T)}");
            return (T)services[typeof(T)].service;
        }

        public static T Resolve<T>(string name)
        {
            Assert.IsTrue(namedServices.ContainsKey(typeof(T)), $"Service not found: {typeof(T)}");
            Assert.IsTrue(namedServices[typeof(T)].ContainsKey(name), $"Service not found: {typeof(T)}");
            return (T)namedServices[typeof(T)][name].service;
        }

        public static T TryResolve<T>()
        {
            return services.ContainsKey(typeof(T)) ? (T)services[typeof(T)].service : default;
        }

        public static T TryResolve<T>(string name)
        {
            return namedServices.ContainsKey(typeof(T)) && namedServices[typeof(T)].ContainsKey(name) ? (T)namedServices[typeof(T)][name].service : default;
        }

        public static void Remove<T>()
        {
            Assert.IsTrue(services.ContainsKey(typeof(T)), $"Service not found: {typeof(T)}");
            services[typeof(T)].scope.Cancel();
        }

        public static void Remove<T>(string name)
        {
            Assert.IsTrue(namedServices.ContainsKey(typeof(T)), $"Service not found: {typeof(T)}");
            Assert.IsTrue(namedServices[typeof(T)].ContainsKey(name), $"Service not found: {typeof(T)}");
            namedServices[typeof(T)][name].scope.Cancel();
        }

        public static bool Contains<T>()
        {
            return services.ContainsKey(typeof(T));
        }

        public static bool Contains<T>(string name)
        {
            return namedServices.ContainsKey(typeof(T)) && namedServices[typeof(T)].ContainsKey(name);
        }
    }
}