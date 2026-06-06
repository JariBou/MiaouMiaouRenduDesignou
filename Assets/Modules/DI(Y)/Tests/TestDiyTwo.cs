using System;
using DependentlyInjectYourself.API;
using DependentlyInjectYourself.Attributes;
using UnityEngine;

namespace Modules.DI_Y_.Tests
{
    public class TestDiyTwo : MonoBehaviour, IDiyService
    {
        private TestDiyOne _test;
        private TestNonBehaviour _service;
        public bool Initialized { get; set; }

        [SerializeField] private bool _printService;
        private TestDiyThree _three;

        private void Awake()
        {
            // DiyContainer.AddService(this);
        }

        private void Start()
        {
            // DiyContainer.Resolve<TestDiyTwo>();
        }

        [DiyInitializerMethod]
        private void Init(TestDiyOne test, IMyService service, TestDiyThree three)
        {
            _three = three;
            _service = (TestNonBehaviour)service;
            _test = test;
        }

        private void Update()
        {
            Debug.Log($"{nameof(TestDiyTwo)} : {_three.testInt}");
            
            // if (_printService)
            // {
            //     if (_service != null)
            //     {
            //         Debug.Log(_service.TestInt);
            //     }
            // }else
            // {
            //     if (_test != null)
            //     {
            //         Debug.Log(_test.TestValue);
            //     }
            // }
        }

        public event Action ServiceDestroyed;
        
        private void OnDestroy()
        {
            ServiceDestroyed?.Invoke();
        }
    }
}