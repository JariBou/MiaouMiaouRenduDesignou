using System;
using DependentlyInjectYourself.API;
using DependentlyInjectYourself.Attributes;
using UnityEngine;

namespace _project.Scripts.Tests
{
    public class TestUn : MonoBehaviour, IDiyLoaded
    {
        public bool Initialized { get; set; }

        [DiyInitializerMethod]
        private void Init(Nounours nounours, IGraou graou, ISteupleMarche potentialService)
        {
            Debug.Log($"== TestUn ==");
            Debug.Log($"Wahou {nounours.name}");
            Debug.Log($"WahouGrahouGrahou {graou.Test}");
            Debug.Log($"WahouPotentialService {potentialService.GetHashCode()}");
            Debug.Log($"== ===== ==");
        }
    }
}