using System;
using System.Collections.Generic;
using _project.Scripts.ExtensionClasses;
using Pooling;
using UnityEngine;

namespace _project.Scripts.Pooling
{
    public class PoolingService : MonoBehaviour
    {
        [SerializeField] private List<PoolDefinition> _poolEntries;
        private readonly Dictionary<string, Pool> _pools = new();

        private void Start()
        {
            foreach (PoolDefinition entry in _poolEntries)
            {
                Pool requestPool = RequestPool(entry.PoolName, entry.Prefab, entry.DefaultSize);
                requestPool.ScheduleCleanEverySeconds(5, 2);
                requestPool.PoolCleaned += (pool, b) => Debug.Log($"Pool Cleaned with: {b}");
            }
        }

        public Pool RequestPool(string id)
        {
            return _pools.GetValueOrDefault(id);
        }

        /// <summary>
        /// Requests a pool and creates it if it doesn't already exist, useful when the order of creation is not guaranteed.
        /// </summary>
        /// <param name="id">The id of the pool</param>
        /// <param name="prefab">the prefab to use in the pool</param>
        /// <param name="poolSize">the pool size</param>
        /// <returns></returns>
        public Pool RequestPool(string id, GameObject prefab, int poolSize = -1)
        {
            return _pools.TryAddAndGet(id, () => new Pool(prefab, poolSize != -1 ? poolSize : Pool.DefaultPoolSize));
        }

        /// <summary>
        /// Requests a pool and creates it if it doesn't already exist
        /// </summary>
        /// <param name="poolDefinition">The pool definition to use</param>
        /// <returns>The Pool</returns>
        public Pool RequestPool(PoolDefinition poolDefinition)
        {
            return RequestPool(poolDefinition.PoolName, poolDefinition.Prefab, poolDefinition.DefaultSize);
        }

        /// <summary>
        /// <inheritdoc cref="RequestPool(PoolDefinition)"/>
        /// </summary>
        /// <param name="poolDefinition"><inheritdoc cref="RequestPool(PoolDefinition)"/></param>
        /// <param name="poolSize">The pool size override for pool creation if needed</param>
        /// <returns>
        /// <inheritdoc cref="RequestPool(PoolDefinition)"/>
        /// </returns>
        public Pool RequestPool(PoolDefinition poolDefinition, int poolSize)
        {
            return RequestPool(poolDefinition.PoolName, poolDefinition.Prefab, poolSize);
        }

        [Serializable]
        private class PoolEnty
        {
            [SerializeField] private string _poolName;
            [SerializeField] private GameObject _prefab;
            [SerializeField] private int _defaultSize = Pool.DefaultPoolSize;

            public string PoolName => _poolName;
            public GameObject Prefab => _prefab;
            public int DefaultSize => _defaultSize;
        }
    }
}