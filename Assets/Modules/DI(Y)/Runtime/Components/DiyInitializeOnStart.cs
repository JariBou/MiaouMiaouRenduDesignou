using DependentlyInjectYourself.API;
using UnityEngine;

namespace DependentlyInjectYourself.Components
{
    [DefaultExecutionOrder(-10101)]
    public class DiyInitializeOnStart : MonoBehaviour
    {
        private void Awake()
        {
            MonoBehaviour[] monoBehaviours = gameObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour monoBehaviour in monoBehaviours)
            {
                if (monoBehaviour is IDiyService service)
                {
                    DiyContainer.AddService(monoBehaviour.GetType(), service);
                }
            }
        }

        private void Start()
        {
            // Justin Case there are game objects  creation before, we reget everything for now
            MonoBehaviour[] monoBehaviours = gameObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour monoBehaviour in monoBehaviours)
            {
                if (monoBehaviour is IDiyService)
                {
                    DiyContainer.Resolve(monoBehaviour.GetType());
                }
            }
        }
    }
}