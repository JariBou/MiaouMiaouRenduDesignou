using _project.Scripts.Player;
using UnityEngine;

namespace _project.Scripts.Game
{
    public class HealthComponent : MonoBehaviour, IDamageable
    {
        private static readonly int hit = Animator.StringToHash("Hit");
        
        [SerializeField] private Animator _animator;
        [SerializeField] private int _baseMaxHealth;
        [SerializeField] private float _invincibilityTime = 1f;
        private bool hasIFrames;
        private int currMaxHealth;
        private int currHealth;
        
        private void Awake()
        {
            currMaxHealth = _baseMaxHealth;
            currHealth = _baseMaxHealth;
        }
        
        public void TakeDamage(int damageAmount)
        {
            if (hasIFrames) return;
            hasIFrames = true;
            _ = DoInvincibilityTimer();
            currHealth -= damageAmount;
            PlayDmgAnim();
            CheckDeath();
        }

        private void PlayDmgAnim()
        {
            _animator.SetTrigger(hit);
        }

        private async Awaitable DoInvincibilityTimer()
        {
            await Awaitable.WaitForSecondsAsync(_invincibilityTime);
            hasIFrames = false;
        }
        
        private void CheckDeath()
        {
            if (currHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}