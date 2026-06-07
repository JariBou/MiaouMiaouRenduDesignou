using _project.Scripts.Alterable;
using _project.Scripts.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace _project.Scripts.Game
{
    [RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
    public class SimpleBullet : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private int _damageAmount = 1;
        [SerializeField] private float _bulletSpeed = 1;

        [SerializeField, Tooltip("Deactivate bullet after s")]
        private float _maxFlightTime = 15;

        [FormerlySerializedAs("InflictAlteration"),SerializeField] private bool _inflictAlteration;
        [FormerlySerializedAs("InflictedAlteration"),SerializeField] private Alteration _inflictedAlteration;

        private float flightTime;

        private void FixedUpdate()
        {
            if (!gameObject.activeSelf) return; // Justin Case

            flightTime += Time.fixedDeltaTime;
            if (flightTime > _maxFlightTime) gameObject.SetActive(false);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.GetComponent<IDamageable>() is { } damageable) damageable.TakeDamage(_damageAmount);
            if (_inflictAlteration && other.gameObject.GetComponent<IAlterable>() is { } alterable) alterable.ReceiveAlteration(_inflictedAlteration);
            gameObject.SetActive(false);
        }

    #if UNITY_EDITOR
        private void OnValidate()
        {
            _rb ??= GetComponent<Rigidbody>();
        }
    #endif

        public void Setup(Vector3 position, Vector3 direction)
        {
            transform.position = position;
            _rb.linearVelocity = direction * _bulletSpeed;
            flightTime = 0;
        }

        public void Setup(Vector3 position, Vector3 direction, int damageAmount)
        {
            _damageAmount = damageAmount;
            Setup(position, direction);
        }

        public void Setup(Vector3 position, Vector3 direction, int damageAmount, float bulletSpeed)
        {
            _bulletSpeed = bulletSpeed;
            Setup(position, direction, damageAmount);
        }
    }
}