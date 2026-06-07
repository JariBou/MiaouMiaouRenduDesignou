using System;
using _project.Scripts.Game;
using UnityEngine;

namespace _project.Scripts.Player
{
    public class PlayerProxy : HealthComponentProxy, IShootTarget
    {

        private void Start()
        {
            if (healthComponent == null)
            {
                throw new NullReferenceException("HealthComponent is missing");
            }
        }

        public Vector3 GetAimPosition()
        {
            return healthComponent.transform.position;
        }
    }
}