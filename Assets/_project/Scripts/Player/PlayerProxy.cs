using System;
using UnityEngine;

namespace _project.Scripts.Player
{
    public class PlayerProxy : MonoBehaviour, IShootTarget, IDamageable
    {
        [SerializeField] private PlayerScript _playerScript;

        private void Start()
        {
            if (_playerScript == null)
            {
                throw new NullReferenceException("PlayerScript is missing");
            }
        }

        public void TakeDamage(int damage)
        {
            _playerScript.TakeDamage(damage);
        }

        public Vector3 GetAimPosition()
        {
            return _playerScript.transform.position;
        }
    }
}