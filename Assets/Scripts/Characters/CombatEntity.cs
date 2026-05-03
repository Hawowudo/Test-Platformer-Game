using GameManagerScripts;
using System.Collections;
using UniRx;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

namespace CombatSystem
{
    public class CombatEntity : MonoBehaviour
    {
        public HitboxHandler hitboxHandler;
        public HurtboxHandler hurtboxHandler;

        public ReactiveProperty<int> currentHealth;
        public ReactiveProperty<int> maxHealth;

        public int damage = 1;
        public Team team;

        public UnityEvent<CombatEntity> OnTakeHit;
        public UnityEvent OnDeath;

        private Blackboard _blackboard;
        #region unity functions
        private void Awake()
        {
            _blackboard = GetComponent<Blackboard>();
        }
        private void OnEnable()
        {
            ResetValues();
            if (hitboxHandler != null)
            {
                hitboxHandler.onHitTarget.AddListener(OnDamageOther);
            }
        }
        private void OnDisable()
        {
            if (hitboxHandler != null)
            {
                hitboxHandler.onHitTarget.RemoveListener(OnDamageOther);
            }

        }

        #endregion
        private void ResetValues()
        {
            currentHealth = new ReactiveProperty<int>(5);
            maxHealth = new ReactiveProperty<int>(5);
        }
        public void OnDamageOther(CombatEntity toDamage)
        {
            if (toDamage.team == this.team)
            {
                return;
            }
            CombatData data = new()
            {
                source = this,
                target = toDamage,
                damage = this.damage,
                isFatal = toDamage.currentHealth.Value - this.damage <= 0
            };
            CombatManager.instance.ExecuteAttack(data);

        }
        public void TakeDamage(int damage, CombatEntity source)
        {
            Debug.Log($"Took damage from {source.name}");
            currentHealth.Value -= damage;
            //OnTakeHit.Invoke(source);
            _blackboard.Set<bool>("hittrigger", true);
            if (currentHealth.Value <= 0)
            {
                Die();
                _blackboard.Set<bool>("isdead", true);
            }
        }
        public void Die()
        {
            OnDeath.Invoke();
        }

        public void Heal(int amount)
        {
            currentHealth.Value += amount;
            if (currentHealth.Value > maxHealth.Value)
            {
                currentHealth.Value = maxHealth.Value;
            }
        }
        public void EnableHitbox(int index = -1)
        {
            if (hitboxHandler == null)
                return;
            hitboxHandler.EnableHitBox(index);
        }
        public void DisableHitbox(int index = -1)
        {
            if (hitboxHandler == null)
                return;
            hitboxHandler.DisableHitBox(index);
        }
        public void EnableHurtbox()
        {
            if (hurtboxHandler == null)
                return;
            //hurtboxHandler.EnableHurtBox();
        }
    }

    public enum Team
    {
        Enemy,
        Player,
        Environment

    }
}
