// using System;
// using System.Collections.Generic;
// using _project.Scripts.Pooling;
// using Modules.Pooling.Runtime;
// using Sisus.Init;
// using UnityEngine;
// using UnityEngine.Serialization;
//
// namespace _project.Scripts.Game
// {
//     public class PoolInitializer : MonoBehaviour<PoolingService>
//     {
//         [Serializable]
//         private class PoolEnty
//         {
//             [SerializeField] private string _poolName;
//             [SerializeField] private GameObject _prefab;
//             [SerializeField] private int _defaultSize = Pool.DefaultPoolSize;
//
//             public string PoolName => _poolName;
//             public GameObject Prefab => _prefab;
//             public int DefaultSize => _defaultSize;
//         }
//
//         [SerializeField] private List<PoolEnty> _poolEntries;
//         private PoolingService poolingService;
//         
//
//         protected override void Init(PoolingService poolService)
//         {
//             poolingService = poolService;
//         }
//
//         private void Start()
//         {
//             foreach (PoolEnty entry in _poolEntries)
//             {
//                 poolingService.RequestPool(entry.PoolName, entry.Prefab, entry.DefaultSize);
//             }
//         }
//     }
// }