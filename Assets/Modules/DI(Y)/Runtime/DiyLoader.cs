using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DependentlyInjectYourself.API;
using DependentlyInjectYourself.Attributes;
using DependentlyInjectYourself.Exceptions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;


namespace DependentlyInjectYourself
{
    public class DiyLoader
    {
        private static DiyLoader _instance;

        private static Dictionary<Type, object> _services;

        private readonly Dictionary<Type, IDiyService> _servicesMap = new();
        private static DiyLoader Instance => _instance ??= new DiyLoader();


        private void RegisterService(Type serviceType, IDiyService serviceInstance)
        {
            serviceInstance.ServiceDestroyed += () => { _servicesMap.Remove(serviceType); };
            if (!_servicesMap.TryAdd(serviceType, serviceInstance)) Debug.LogError($"Service of type '{serviceType.Name}' already registered!");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Debug.Log("Initializing DiyLoader");

        #if UNITY_EDITOR
            DoMonoBehaviourInitialization();
        #endif

            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }

        // For now let's just support MonoBehaviour services and same-scene services
        private static void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            List<MonoBehaviour> allBehaviours = new();

            foreach (GameObject rootGameObject in rootGameObjects)
            {
                // ReSharper disable once RedundantTypeArgumentsOfMethod Justification: I wanna see it pretty please
                AggregateAllChildrenOfType<MonoBehaviour>(rootGameObject, ref allBehaviours);
            }

            Dictionary<object, MethodInfo> methodInfosMap = new();
            ServiceContainer services = new();

            HandleNonMonoBehaviourServices(ref services, ref methodInfosMap);
            DoMonoBehaviourInitialization(allBehaviours.ToArray(), ref services, ref methodInfosMap);

            foreach (object obj in methodInfosMap.Keys)
            {
                ResolveDependenciesFor(obj, ref services, ref methodInfosMap);
            }
        }

        // For now let's just support same-scene services
        private static void DoMonoBehaviourInitialization(MonoBehaviour[] allBehaviours, ref ServiceContainer services,
                                                          ref Dictionary<object, MethodInfo> methodInfosMap)
        {
            MonoBehaviour[] diyLoadedBehaviours =
                allBehaviours.Where(behaviour => behaviour.GetType().GetInterfaces().Contains(typeof(IDiyLoaded))).ToArray();

            foreach (MonoBehaviour behaviour in diyLoadedBehaviours)
            {
                // behaviour.gameObject.scene.name == "DontDestroyOnLoad";
                Type type = behaviour.GetType();

                MethodInfo methodInfo = GetDiyInitializerMethodInfoOfType(type);
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
                    object potentialService = services.TryGetValue(parameterInfo.ParameterType, out object service)
                        ? service
                        : allBehaviours.FirstOrDefault(monoBehaviour =>
                                                           parameterInfo.ParameterType == monoBehaviour.GetType() || parameterInfo.ParameterType ==
                                                           monoBehaviour.GetType().GetCustomAttribute<DiyServiceAttribute>()?.ServiceType);
                    // MonoBehaviour potentialService = allBehaviours.FirstOrDefault(monoBehaviour => parameterInfo.ParameterType == monoBehaviour.GetType());
                    if (potentialService != null)
                        services.TryAdd(parameterInfo.ParameterType, potentialService);
                    else
                    {
                        // For now
                        throw new MissingServiceException(
                            $"Missing service of type '{parameterInfo.ParameterType.Name}' while resolving dependencies for Object '{behaviour.name}'");
                    }
                    // servicesNeeded.Add(parameterInfo.ParameterType);
                }
            }
        }

        private static void HandleNonMonoBehaviourServices(ref ServiceContainer services, ref Dictionary<object, MethodInfo> methodInfosMap)
        {
            IEnumerable<Type> nonBehaviourServices = AssemblyCacheUtils.NonUnityAssemblies.SelectMany(a => a.GetTypes()
                .Where(type => type.IsDefined(
                                   typeof(DiyServiceAttribute)) &&
                               !type.IsAbstract &&
                               !type.IsSubclassOf(
                                   typeof(MonoBehaviour))));

            foreach (Type sType in nonBehaviourServices)
            {
                DiyServiceAttribute attribute = sType.GetCustomAttribute<DiyServiceAttribute>();
                ConstructorInfo constructorInfo = sType.GetConstructor(new Type[] { });
                if (constructorInfo == null)
                {
                    Debug.LogWarning($"Service of type '{sType}'has no default ctor!");
                    continue;
                }

                object instance = constructorInfo.Invoke(null);
                Type serviceType = attribute.ServiceType ?? sType;
                if (!services.TryAdd(serviceType, instance))
                {
                    Debug.LogWarning($"Duplicate service of type '{sType}' found in services!");
                    continue;
                }

                MethodInfo diyInitializerMethodInfoOfType = GetDiyInitializerMethodInfoOfType(sType);
                if (diyInitializerMethodInfoOfType == null) continue;

                ParameterInfo[] parameterInfos = diyInitializerMethodInfoOfType.GetParameters();
                if (parameterInfos.Length == 0)
                {
                    diyInitializerMethodInfoOfType.Invoke(instance, null); // Let's call it but still raise a warning still
                    Debug.LogWarning($"Object of type {sType.Name} implements a DiyInitializer Method but takes no arguments!");
                    continue;
                }

                if (!methodInfosMap.TryAdd(instance, diyInitializerMethodInfoOfType))
                    Debug.LogError($"Duplicate service of type '{sType}' found in init methods map!");
            }
        }

        private static MethodInfo GetDiyInitializerMethodInfoOfType(Type type)
        {
            return type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                       .FirstOrDefault(info => info.GetCustomAttributes(typeof(DiyInitializerMethodAttribute), false).Any());
        }

        private static void AggregateAllChildrenGameObjects(GameObject gameObject, ref List<GameObject> container)
        {
            container.Add(gameObject);
            foreach (Transform t in gameObject.transform)
            {
                AggregateAllChildrenGameObjects(t.gameObject, ref container);
            }
        }

        private static void AggregateAllChildrenOfType<T>(GameObject gameObject, ref List<T> container) where T : Object
        {
            container.AddRange(gameObject.GetComponentsInChildren<T>(true));
        }

        private static void AggregateAllChildrenOfType<TSearch, TConstraint>(GameObject gameObject, ref List<TConstraint> container)
            where TSearch : Object
        {
            TSearch[] componentsInChildren = gameObject.GetComponentsInChildren<TSearch>(true);
            foreach (TSearch child in componentsInChildren)
            {
                if (child is TConstraint constraint) container.Add(constraint);
            }
            // container.AddRange(componentsInChildren);
        }

        private static void AggregateAllChildrenOfType<TSearch, TConstraint>(GameObject gameObject, ref List<TSearch> container) where TSearch : Object
        {
            TSearch[] componentsInChildren = gameObject.GetComponentsInChildren<TSearch>(true);
            foreach (TSearch child in componentsInChildren)
            {
                if (child is TConstraint) container.Add(child);
            }
            // container.AddRange(componentsInChildren);
        }

        private static void DoMonoBehaviourInitialization()
        {
            MonoBehaviour[] allBehaviours = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Dictionary<object, MethodInfo> methodInfosMap = new();
            ServiceContainer services = new();

            HandleNonMonoBehaviourServices(ref services, ref methodInfosMap);
            DoMonoBehaviourInitialization(allBehaviours, ref services, ref methodInfosMap);

            foreach (object obj in methodInfosMap.Keys)
            {
                ResolveDependenciesFor(obj, ref services, ref methodInfosMap);
            }
        }

        private static void ResolveDependenciesFor(object obj, ref ServiceContainer services,
                                                   ref Dictionary<object, MethodInfo> methodInfosMap)
        {
            if (!methodInfosMap.TryGetValue(obj, out MethodInfo method))
            {
                Debug.LogError($"Missing method for object of type '{obj.GetType().Name}'");
                return;
            }

            ParameterInfo[] parameterInfos = method.GetParameters();
            object[] parameters = new object[parameterInfos.Length];

            foreach (ParameterInfo parameterInfo in parameterInfos)
            {
                if (services.TryGetValue(parameterInfo.ParameterType, out object service))
                {
                    if (service is IDiyLoaded { Initialized: false }) ResolveDependenciesFor(service, ref services, ref methodInfosMap);

                    parameters[parameterInfo.Position] = service;
                }
                else
                    throw new MissingServiceException(
                        $"Missing service of type '{parameterInfo.ParameterType.Name}' while resolving dependencies for object '{obj.GetHashCode()}'");
            }

            method.Invoke(obj, parameters);
            if (obj is IDiyLoaded diyLoaded) diyLoaded.Initialized = true;
        }

        private class ServiceContainer : Dictionary<Type, object>
        {
        }
    }
}