using _project.Scripts.Alterable;
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
        [SerializeField] private PlayerStatus _ownerStats;
        public int BaseDef = 2;
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

            if (_ownerStats) damageAmount -= _ownerStats.GetFinalStat(BaseDef, StatType.Def);
            hasIFrames = true;
            _ = DoInvincibilityTimer();
            if (damageAmount < 0) return;

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
                Debug.Log("You died");
                Destroy(gameObject);
                Application.Quit();
            }
        }
    }
}