using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem
{
    public class HurtboxHandler : MonoBehaviour
    {
        // could be used if you want to change the hurtbox size for animation purposes but for now it's just a placeholder

        public CombatEntity combatEntity;

        public CombatEntity GetCombatEntity()
        {
            if (combatEntity == null)
            {
                return GetComponentInParent<CombatEntity>();
            }
            return combatEntity;
        }
    }
}
