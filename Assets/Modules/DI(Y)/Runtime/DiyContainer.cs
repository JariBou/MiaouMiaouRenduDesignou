using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using DependentlyInjectYourself.API;
using DependentlyInjectYourself.Exceptions;

[assembly: InternalsVisibleTo("DIY.Editor")]
namespace DependentlyInjectYourself
{
    public struct ServiceEntry
    {
        public readonly ServiceLifetime lifetime;
        public readonly Type serviceType;
        [Obsolete] public readonly Type objectType;
        public IDiyService ServiceInstance { get; private set; }
        public readonly Func<IDiyService> serviceGetter; // TODO
        public bool Resolved { get; private set; }

        public ServiceEntry(Type serviceType, IDiyService serviceInstance)
        {
            this.serviceType = serviceType;
            ServiceInstance = serviceInstance;
            lifetime = ServiceLifetime.Singleton;
            objectType = serviceInstance.GetType();
            Resolved = false;
            serviceGetter = () => serviceInstance;
        }
        
        //TODO:  change object to IDiyService
        public ServiceEntry(Type serviceType, Func<IDiyService> serviceGetter, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            this.serviceType = serviceType;
            
            this.serviceGetter = serviceGetter;
            
            this.lifetime = lifetime;
            objectType = null;
            ServiceInstance = null;
            
            Resolved = false;
        }

        public object GetService()
        {
            if (lifetime == ServiceLifetime.Singleton)
            {
                return ServiceInstance ??= serviceGetter();
            }
            return serviceGetter();
        }

        public object Resolve()
        {
            if (Resolved) return GetService();
            object service = GetService();
            MethodInfo initMethod = DiyResolver.GetDiyInitializerMethodInfoOfType(service.GetType());

            if (initMethod == null)
            {
                Resolved = lifetime == ServiceLifetime.Singleton; // If transient it's never "resolved"
                return service;
            }
            
            ParameterInfo[] parameterInfos = initMethod.GetParameters();
            object[] parameters = new object[parameterInfos.Length];

            foreach (ParameterInfo parameterInfo in parameterInfos)
            {
                if (DiyContainer.Instance._services.TryGetValue(parameterInfo.ParameterType, out ServiceEntry parameterEntry))
                {
                    parameters[parameterInfo.Position] = parameterEntry.Resolve();
                }
                else
                    throw new MissingServiceException(
                        $"Missing service of type '{parameterInfo.ParameterType.Name}' while resolving dependencies for object of type '{serviceType.Name}'");
            }

            initMethod.Invoke(service, parameters);
                
            Resolved = lifetime == ServiceLifetime.Singleton;
            return service;
        }
    }

    public enum ServiceScope // TODO: scope
    {
        Project,
        Scene,
        // GameObject,
    }
    
    public enum ServiceLifetime // TODO: lifetime
    {
        Singleton,
        Transient,
    }
    
    public class DiyContainer
    {
        private static DiyContainer _instance;
        public static DiyContainer Instance => _instance ??= new DiyContainer();
        
        internal Dictionary<Type, ServiceEntry> _services = new (); // TODO: support arrays of injections
        
        public static void AddService<TObject>() where TObject : class, IDiyService, new()
        {
            AddService<TObject, TObject>();
        }
        
        public static void AddService<TService, TObject>() where TObject : TService, IDiyService, new() where TService : class
        {
            // TODO: Remove new() constraint and add constructor lookup
            Instance._services.Add(typeof(TService), new ServiceEntry(typeof(TService), new TObject()));
        }

        public static void AddService<TObject>(TObject service) where TObject : class, IDiyService
        {
            AddService<TObject, TObject>(service);
        }

        public static void AddService<TService, TObject>(TObject service) where TObject : TService where TService : class, IDiyService
        {
            Instance._services.Add(typeof(TService), new ServiceEntry(typeof(TService), service));
        }
        
        public static void AddService(Type serviceType, IDiyService service)
        {
            Instance._services.Add(serviceType, new ServiceEntry(serviceType, service));
        }
        
        public static bool TryAddService(Type serviceType, IDiyService service)
        {
            return Instance._services.TryAdd(serviceType, new ServiceEntry(serviceType, service));
        }
        
        public static bool TryAddService(ServiceEntry service)
        {
            return Instance._services.TryAdd(service.serviceType, service);
        }
        
        public static bool TryAddService(Type serviceType, Func<IDiyService> service)
        {
            return Instance._services.TryAdd(serviceType, new ServiceEntry(serviceType, service));
        }

        public static TService Resolve<TService>() where TService : class
        {
            return Resolve(typeof(TService)) as TService;
        }

        public static object Resolve(Type serviceType)
        {
            if (!Instance._services.TryGetValue(serviceType, out ServiceEntry entry))
            {
                return null;
            }

            return entry.Resolve();
        }
        
        public static void ResolveAll()
        {
            foreach ((Type _, ServiceEntry entry) in Instance._services)
            {
                if (entry.Resolved) continue;

                entry.Resolve();
                // ResolveEntry(entry);
            }
        }

        // TODO: should this stay here or go to entry?
        // prob stay here, I mean entry should only be data
        // or not... easier to support transient.. or not, it's the same actually
        // Will see when I get to scoping
        private static object ResolveEntry(ServiceEntry entry)
        {
            if (entry.Resolved) return entry.GetService();

            MethodInfo initMethod = DiyResolver.GetDiyInitializerMethodInfoOfType(entry.objectType);

            if (initMethod == null)
            {
                entry.Resolve();
                return entry.ServiceInstance;
            }
            
            ParameterInfo[] parameterInfos = initMethod.GetParameters();
            object[] parameters = new object[parameterInfos.Length];

            foreach (ParameterInfo parameterInfo in parameterInfos)
            {
                if (Instance._services.TryGetValue(parameterInfo.ParameterType, out ServiceEntry parameterEntry))
                {
                    parameters[parameterInfo.Position] = ResolveEntry(parameterEntry);
                }
                else
                    throw new MissingServiceException(
                        $"Missing service of type '{parameterInfo.ParameterType.Name}' while resolving dependencies for object of type '{entry.serviceType.Name}'");
            }

            initMethod.Invoke(entry.GetService(), parameters);
                
            // TODO
            entry.Resolve();

            return entry.GetService();
        }

        internal static void Reset()
        {
            _instance._services.Clear();
        }

        internal static Dictionary<Type, ServiceEntry> GetAllServices()
        {
            return _instance._services;
        }
    }
}