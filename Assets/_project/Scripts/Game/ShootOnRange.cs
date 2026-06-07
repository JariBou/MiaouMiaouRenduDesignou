using System.Collections.Generic;
using _project.Scripts.Game.Interfaces;
using _project.Scripts.Pooling;
using Sisus.Init;
using UnityEngine;

namespace _project.Scripts.Game
{
    [RequireComponent(typeof(SphereCollider))]
    public class ShootOnRange : MonoBehaviour<PoolingService>
    {
        [SerializeField] private PoolDefinition _poolDefinitionToUse;

        [SerializeField] private float _shootCooldown;
        [SerializeField] private float _shootDirOffsetAmount = 0.5f;
        [SerializeField] private Vector3 _shootOffset;
        private readonly List<IShootTarget> possibleTargets = new();
        private IShootTarget currTarget;
        private PoolingService poolingService;
        private float shootTimer;

        private void FixedUpdate()
        {
            if (currTarget == null) return;

            if (shootTimer > 0)
            {
                shootTimer -= Time.fixedDeltaTime;
                return;
            }

            SimpleBullet simpleBullet = poolingService.RequestPool(_poolDefinitionToUse).Get<SimpleBullet>();
            Vector3 transformPosition = transform.position;
            Vector3 dir = (currTarget.GetAimPosition() - transformPosition).normalized;
            simpleBullet.Setup(transformPosition + dir * _shootDirOffsetAmount + _shootOffset, dir);
            shootTimer = _shootCooldown;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<IShootTarget>() is not { } target) return;

            possibleTargets.Add(target);
            currTarget ??= target;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<IShootTarget>() is not { } target) return;

            possibleTargets.Remove(target);
            if (currTarget == target) return;

            {
                currTarget = possibleTargets.Count > 0 ? possibleTargets[0] : null;
            }
        }

        protected override void Init(PoolingService poolService)
        {
            poolingService = poolService;
        }
    }
}