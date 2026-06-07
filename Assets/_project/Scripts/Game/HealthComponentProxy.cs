using _project.Scripts.Player;
using UnityEngine;

namespace _project.Scripts.Game
{
    public class HealthComponentProxy : MonoBehaviour, IDamageable
    {
        [SerializeField] protected HealthComponent healthComponent;
        public virtual void TakeDamage(int damage)
        {
            healthComponent.TakeDamage(damage);
        }
    }
}