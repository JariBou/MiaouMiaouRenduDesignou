using Pooling;
using UnityEngine;

namespace _project.Scripts.Pooling
{
    [CreateAssetMenu(menuName = "Pooling/Pool Definition")]
    public class PoolDefinition : ScriptableObject
    {
        [SerializeField] private string _poolName;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _defaultSize = Pool.DefaultPoolSize;

        public string PoolName => _poolName;
        public GameObject Prefab => _prefab;
        public int DefaultSize => _defaultSize;
    }
}