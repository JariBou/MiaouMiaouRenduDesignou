using System;
using UnityEngine;

namespace _project.Scripts.Tests
{
    public class TestUn : MonoBehaviour, IDiyLoaded
    {
        public bool Initialized { get; set; }

        [DiyInitializerMethod]
        private void Init(Nounours nounours)
        {
            Debug.Log($"== TestUn ==");
            Debug.Log($"Wahou {nounours.name}");
            Debug.Log($"== ===== ==");
        }

        public event Action ServiceDestroyed;
    }
}