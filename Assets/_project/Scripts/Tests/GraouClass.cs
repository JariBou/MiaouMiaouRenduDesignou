using DependentlyInjectYourself.Attributes;
using UnityEngine;

namespace _project.Scripts.Tests
{
    public interface IGraou
    {
        public int Test { get; }
    }
    
    [DiyService(typeof(IGraou))]
    public class GraouClass : IGraou
    {
        public int Test { get; set; } = 56;
        
        [DiyInitializerMethod]
        private void GraouGraouInitializerMethod()
        {
            Test = 89;
            Debug.Log($"{GetType()}::Miaouuuuuuuu!");
        }
    }
}