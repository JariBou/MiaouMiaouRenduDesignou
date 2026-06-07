using _project.Scripts.Alterable;
using _project.Scripts.Game.Interfaces;
using UnityEngine;

namespace _project.Scripts.Player
{
    public class PlayerWeapon : MonoBehaviour
    {
        [SerializeField] private GameObject _ownerHitbox;
        [SerializeField] private PlayerStatus _ownerStats;
        [SerializeField] private int _damageAmount = 10;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == _ownerHitbox) return;

            if (other.gameObject.GetComponent<IDamageable>() is { } damageable)
            {
                if (!_ownerStats)
                {
                    Debug.Log("No playerStats attached to weapon, default attack value used instead");
                    damageable.TakeDamage(_damageAmount);
                }
                else
                    damageable.TakeDamage(_ownerStats.GetFinalStat(_damageAmount, StatType.Attack));
            }
        }
    }
}