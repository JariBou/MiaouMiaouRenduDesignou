using System;
using DependentlyInjectYourself.API;
using DependentlyInjectYourself.Attributes;
using UnityEngine;

namespace _project.Scripts.Tests
{
    public class TestEncore : MonoBehaviour, IDiyLoaded
    {
        public bool Initialized { get; set; }

        [DiyInitializerMethod]
        private void Init(Nounours nounours, TestUn autreTest, IGraou graou)
        {
            Debug.Log($"== TestEncore ==");
            Debug.Log($"Wahou {nounours.name}");
            Debug.Log($"Wahou {autreTest.name}");
            Debug.Log($"WahouGrahou: {graou.Test}");
            Debug.Log($"== ===== ==");
        }
    }
}