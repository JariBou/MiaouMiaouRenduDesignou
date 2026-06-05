using System;
using DependentlyInjectYourself;
using DependentlyInjectYourself.Attributes;
using JetBrains.Annotations;
using UnityEngine;

namespace Modules.DI_Y_.Tests
{
    public interface IMyService
    {
        public int TestInt { get; }
    }
    
    [DiyService(typeof(IMyService)/*, serviceLifetime: ServiceLifetime.Transient*/)]
    public class TestNonBehaviour : IMyService
    {
        [CanBeNull] private TestDiyOne _test;
        public int TestInt { get => _test?.TestValue ?? -1; }

        public int TestIntCustom
        {
            get => _testInt;
            set => _testInt = value;
        }

        [SerializeField] private int _testInt;
        [SerializeField] private int _testInt2;
        [SerializeField] private string _testString;
        
        public TestNonBehaviour()
        {
            
        }

        // [DiyInitializerMethod]
        // private void Init(TestDiyOne test)
        // {
        //     _test = test;
        // }
    }
}