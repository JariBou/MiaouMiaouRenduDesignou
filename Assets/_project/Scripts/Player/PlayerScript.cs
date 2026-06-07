using Sisus.Init;
using UnityEngine;

namespace _project.Scripts.Player
{
    [Service]
    public class PlayerScript : MonoBehaviour, IDamageable
    {
        private static readonly int hit = Animator.StringToHash("Hit");

        [SerializeField] private Animator _playerAnimator;
        [SerializeField] private int _baseMaxHealth;
        [SerializeField] private float _invincibilityTime = 1f;
        private int currHealth;

        private int currMaxHealth;

        private bool hasIFrames;

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
                Debug.Log("Player Death");
            }
        }

        private void PlayDmgAnim()
        {
            _playerAnimator.SetTrigger(hit);
        }
    }
}