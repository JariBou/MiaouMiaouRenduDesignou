using System;
using UnityEngine;

namespace _project.Scripts.Player
{
    public class PlayerWeapon : MonoBehaviour
    {
        [SerializeField] private GameObject _ownerHitbox;
        [SerializeField] private int _damageAmount = 10;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == _ownerHitbox) return;
            if (other.gameObject.GetComponent<IDamageable>() is { } damageable)
            {
                damageable.TakeDamage(_damageAmount);
            }
        }
    }
}