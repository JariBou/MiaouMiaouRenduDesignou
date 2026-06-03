using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public interface IDiyService
{
    public event Action ServiceDestroyed;
}

public interface IDiyLoaded : IDiyService
{
    public bool Initialized { get; set; }
}

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class DiyInitializerMethodAttribute : Attribute
{
    
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DiyServiceAttribute : Attribute
{
    public Type ServiceType { get; }

    public DiyServiceAttribute(Type serviceType)
    {
        ServiceType = serviceType;
    }
}


public class DiyLoader
{
    private static DiyLoader _instance;
    private static DiyLoader Instance => _instance ??= new DiyLoader();
    
    private Dictionary<Type, IDiyService> _servicesMap = new();

    public DiyLoader()
    {
    }


    private void RegisterService(Type serviceType, IDiyService serviceInstance)
    {
        serviceInstance.ServiceDestroyed += () =>
        {
            _servicesMap.Remove(serviceType);
        };
        if (!_servicesMap.TryAdd(serviceType, serviceInstance))
        {
            Debug.LogError($"Service of type '{serviceType.Name}' already registered!");
        }
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void Initialize()
    {
        Debug.Log("Initializing DiyLoader");
        Instance.DoInitialization();
        SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
    }

    // For now let's just support MonoBehaviour services
    private static void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Instance.DoInitialization();
        // TODO OOOOOO
    }

    private void DoInitialization()
    {
        MonoBehaviour[] allBehaviours = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        MonoBehaviour[] diyLoadedBehaviours = allBehaviours.Where(behaviour => behaviour.GetType().GetInterfaces().Contains(typeof(IDiyLoaded))).ToArray();

        Dictionary<MonoBehaviour, MethodInfo> methodInfosMap = new();
        Dictionary<Type, MonoBehaviour> services = new();
        // HashSet<Type> servicesNeeded =  new();
        
        foreach (MonoBehaviour behaviour in diyLoadedBehaviours)
        {
            Type type = behaviour.GetType();
            MethodInfo methodInfo = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(info => info.GetCustomAttributes(typeof(DiyInitializerMethodAttribute), false).Any());
            if (methodInfo == null)
            {
                Debug.LogWarning($"Object of type'{type.Name}' implements {nameof(IDiyLoaded)} but doesn't have a DiyInitializer Method!");
                continue;
            }

            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                
            if (parameterInfos.Length == 0)
            {
                methodInfo.Invoke(behaviour, null); // Let's call it but still raise a warning still
                Debug.LogWarning($"Object of type {type.Name} implements a DiyInitializer Method but takes no arguments!");
                continue;
            }
            
            methodInfosMap.Add(behaviour, methodInfo);

            foreach (ParameterInfo parameterInfo in parameterInfos)
            {
                MonoBehaviour potentialService = allBehaviours.FirstOrDefault(monoBehaviour => parameterInfo.ParameterType == monoBehaviour.GetType());
                if (potentialService != null)
                {
                    services.TryAdd(parameterInfo.ParameterType, potentialService);
                } else
                {
                    // For now
                    throw new MissingServiceException($"Missing service of type '{parameterInfo.ParameterType.Name}' while resolving dependencies for Object '{behaviour.name}'");
                }
                // servicesNeeded.Add(parameterInfo.ParameterType);
            }
        }

        foreach ((MonoBehaviour obj, MethodInfo method) in methodInfosMap)
        {
            ResolveDependenciesFor(obj, method, ref services, ref methodInfosMap);
        }
    }

    private void ResolveDependenciesFor(MonoBehaviour obj, MethodInfo method, /*Justin Case*/ ref Dictionary<Type, MonoBehaviour> services,
                                               ref Dictionary<MonoBehaviour, MethodInfo> methodInfosMap )
    {
        ParameterInfo[] parameterInfos = method.GetParameters();
        object[] parameters = new object[parameterInfos.Length];
            
        foreach (ParameterInfo parameterInfo in parameterInfos)
        {
            if (services.TryGetValue(parameterInfo.ParameterType, out MonoBehaviour service))
            {
                if (service is IDiyLoaded{Initialized: false})
                {
                    ResolveDependenciesFor(service, methodInfosMap[service], ref services, ref methodInfosMap);
                }
                
                parameters[parameterInfo.Position] = service;
            }
            else
            {
                throw new MissingServiceException($"Missing service of type '{parameterInfo.ParameterType.Name}' while resolving dependencies for MonoBehaviour '{obj.name}'");
            }
        }
            
        method.Invoke(obj, parameters);
        if (obj is IDiyLoaded diyLoaded)
        {
            diyLoaded.Initialized = true;
        }
    }
}

public sealed class MissingServiceException : Exception
{
    public MissingServiceException(string message) : base(message)
    {
    }
}