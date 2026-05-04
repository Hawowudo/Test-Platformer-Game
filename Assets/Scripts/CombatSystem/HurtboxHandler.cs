using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem
{
    public class HurtboxHandler : MonoBehaviour
    {
        // could be used if you want to change the hurtbox size for animation purposes but for now it's just a placeholder

        public CombatEntity combatEntity;
        public Collider2D hurtboxCollider;
        private void Awake()
        {
            if (hurtboxCollider == null)
            {
                hurtboxCollider = GetComponentInChildren<Collider2D>();
            }
        }
        public CombatEntity GetCombatEntity()
        {
            if (combatEntity == null)
            {
                return GetComponentInParent<CombatEntity>();
            }
            return combatEntity;
        }

        public void DisableHurtbox()
        {
            hurtboxCollider.gameObject.SetActive(false);
        }
        public void EnableHurtbox()
        {
            hurtboxCollider.gameObject.SetActive(true);
        }
    }
}
