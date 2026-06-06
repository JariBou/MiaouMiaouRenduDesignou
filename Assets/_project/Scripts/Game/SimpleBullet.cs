using System;
using _project.Scripts.Player;
using UnityEngine;

namespace _project.Scripts.Game
{
    [RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
    public class SimpleBullet : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private int _damageAmount = 1;
        [SerializeField] private float _bulletSpeed = 1;

        public void Setup(Vector3 position, Vector3 direction)
        {
            transform.position = position;
            _rb.linearVelocity = direction * _bulletSpeed;
        }
        
        public void Setup(Vector3 position, Vector3 direction, int damageAmount)
        {
            transform.position = position;
            _rb.linearVelocity = direction * _bulletSpeed;
            _damageAmount = damageAmount;
        }
        
        public void Setup(Vector3 position, Vector3 direction, int damageAmount, float bulletSpeed)
        {
            _bulletSpeed = bulletSpeed;
            Setup(position, direction, damageAmount);
        }
        

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.GetComponent<IDamageable>() is { } damageable)
            {
                damageable.TakeDamage(_damageAmount);
            }
            gameObject.SetActive(false);
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            _rb ??= GetComponent<Rigidbody>();
        }
        #endif
    }
}