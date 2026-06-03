using System;
using UnityEngine;

namespace _project.Scripts.Tests
{
    public class TestEncore : MonoBehaviour, IDiyLoaded
    {
        public bool Initialized { get; set; }

        [DiyInitializerMethod]
        private void Init(Nounours nounours, TestUn autreTest)
        {
            Debug.Log($"== TestEncore ==");
            Debug.Log($"Wahou {nounours.name}");
            Debug.Log($"Wahou {autreTest.name}");
            Debug.Log($"== ===== ==");
        }

        public event Action ServiceDestroyed;
    }
}