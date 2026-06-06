using System;
using DependentlyInjectYourself.API;
using UnityEngine;

namespace Modules.DI_Y_.Tests
{
    public class TestDiyThree : MonoBehaviour, IDiyService<TestDiyThree>
    {
        public int testInt;
        public event Action ServiceDestroyed;

        private void OnDestroy()
        {
            ServiceDestroyed?.Invoke();
        }
    }
}