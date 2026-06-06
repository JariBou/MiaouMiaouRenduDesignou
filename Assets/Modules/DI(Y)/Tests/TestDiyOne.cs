using System;
using DependentlyInjectYourself;
using DependentlyInjectYourself.API;
using DependentlyInjectYourself.Attributes;
using UnityEngine;

namespace Modules.DI_Y_.Tests
{
    public class TestDiyOne : MonoBehaviour, IDiyService<TestDiyOne>
    {
        [SerializeField] private int _testValue;
        private TestNonBehaviour _service;
        private float _timer;
        private TestDiyThree _three;

        public int TestValue => _testValue;

        private void Awake()
        {
            // DiyContainer.AddService(this);
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer > 2f)
            {
                _timer -= 2f;
                _three.testInt += 1;
            }
            Debug.Log($"{nameof(TestDiyOne)} : {_three.testInt}");
        }

        [DiyInitializerMethod]
        private void Init(IMyService service, TestDiyThree three)
        {
            _three = three;
            _service = (TestNonBehaviour)service;
        }

        public bool Initialized { get; set; }
        public event Action ServiceDestroyed;
        
        private void OnDestroy()
        {
            ServiceDestroyed?.Invoke();
        }
    }
}