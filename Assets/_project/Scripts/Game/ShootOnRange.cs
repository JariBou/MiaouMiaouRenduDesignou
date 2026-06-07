using System;
using System.Collections.Generic;
using _project.Scripts.Player;
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
        private IShootTarget currTarget;
        private List<IShootTarget> _possibleTargets = new();
        private float shootTimer = 0;
        private PoolingService poolingService;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<IShootTarget>() is not {} target) return;

            _possibleTargets.Add(target);
            currTarget ??= target;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<IShootTarget>() is not {} target) return;

            _possibleTargets.Remove(target);
            if (currTarget == target) return;
            {
                currTarget = _possibleTargets.Count > 0 ? _possibleTargets[0] : null;                    
            }
        }

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
            simpleBullet.Setup(transformPosition + dir*_shootDirOffsetAmount + _shootOffset, dir);
            shootTimer = _shootCooldown;
        }

        protected override void Init(PoolingService poolService)
        {
            poolingService = poolService;
        }
    }
}