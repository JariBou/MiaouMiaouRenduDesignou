using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DependentlyInjectYourself.API;
using DependentlyInjectYourself.Attributes;
using DependentlyInjectYourself.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace DependentlyInjectYourself
{
    public class DiyAutoLoader
    {
        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Debug.Log("Initializing DiyAutoLoader");

        #if UNITY_EDITOR
            // HandleNonBehaviourInitialization();
            // MonoBehaviour[] allBehaviours = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            // DoMonoBehaviourInitialization(allBehaviours);
            // DiyContainer.ResolveAll();
        #endif

            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        }

        private static void HandleNonBehaviourInitialization()
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

                // object instance = constructorInfo.Invoke(null);
                Type serviceType = attribute.ServiceType ?? sType;
                if (!DiyContainer.TryAddService(new ServiceEntry(serviceType, () => constructorInfo.Invoke(null), attribute.ServiceLifetime)))
                {
                    Debug.LogWarning($"Duplicate service of type '{sType}' found in services!");
                }
            }
        }
        
        private static void AggregateAllChildrenOfType<T>(GameObject gameObject, ref List<T> container) where T : Object
        {
            container.AddRange(gameObject.GetComponentsInChildren<T>(true));
        }

        private static void DoMonoBehaviourInitialization(MonoBehaviour[] allBehaviours)
        {
            

            Dictionary<Type, MonoBehaviour> servicesBehaviours = allBehaviours.Where(behaviour => behaviour.GetType()
                                                                                                           .GetInterfaces()
                                                                                                           .Any(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDiyService<>)))
                                                                              .ToDictionary(behaviour => behaviour.GetType());

            MonoBehaviour[] diyLoadedBehaviours =
                allBehaviours.Where(behaviour => behaviour.GetType().GetInterfaces().Contains(typeof(IDiyLoaded))).ToArray();

            foreach ((Type instanceType, MonoBehaviour obj) in servicesBehaviours)
            {
                Type[] interfaces = instanceType.GetInterfaces();
                Type genericInterface = interfaces.FirstOrDefault(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IDiyService<>));
                if (genericInterface == null)
                {
                    Debug.LogError($"{typeof(DiyAutoLoader)}::{nameof(DoMonoBehaviourInitialization)}::Invalid type in object '{obj.name}' of type '{instanceType}'");
                    continue;
                }

                Type serviceType = genericInterface.GetGenericArguments()[0];
                if (!DiyContainer.TryAddService(serviceType, obj))
                {
                    Debug.LogError($"{typeof(DiyAutoLoader)}::{nameof(DoMonoBehaviourInitialization)}::Error while trying to inject object of type '{instanceType}' in Container");
                }
            }
            
            foreach (MonoBehaviour diyLoadedBehaviour in diyLoadedBehaviours)
            {
                Type type = diyLoadedBehaviour.GetType();
                DiyContainer.TryAddService(type, diyLoadedBehaviour);
            }
        }

        private static void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            List<MonoBehaviour> allBehaviours = new();

            foreach (GameObject rootGameObject in rootGameObjects)
            {
                // ReSharper disable once RedundantTypeArgumentsOfMethod Justification: I wanna see it pretty please
                AggregateAllChildrenOfType<MonoBehaviour>(rootGameObject, ref allBehaviours);
            }
            
            HandleNonBehaviourInitialization();
            DoMonoBehaviourInitialization(allBehaviours.ToArray());
            DiyContainer.ResolveAll();
        }
    }
}