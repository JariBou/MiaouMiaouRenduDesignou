using System;
using Sisus.Init;
using UnityEngine;

namespace _project.Scripts.Player
{
    [Service]
    public class PlayerScript : MonoBehaviour, IDamageable
    {
        [SerializeField] private int _baseMaxHealth;
        private int currMaxHealth;
        private int currHealth;

        private void Awake()
        {
            currMaxHealth = _baseMaxHealth;
            currHealth = _baseMaxHealth;
        }

        public void TakeDamage(int damageAmount)
        {
            currHealth -= damageAmount;
            CheckDeath();
        }

        private void CheckDeath()
        {
            if (currHealth <= 0)
            {
                Destroy(gameObject);
                Debug.Log("Player Death");
            }
        }
    }
}