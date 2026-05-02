using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace CombatSystem
{
    public class HitboxHandler : MonoBehaviour
    {
        //make this into a hurtbox layer
        public LayerMask targetLayerMask;
        public GameObject[] hitBoxGroups;
        public UnityEvent<CombatEntity> onHitTarget;
        public float hitCooldown = 0.5f;
        private List<CombatEntity> _currentHits = new List<CombatEntity>();
        public void EnableHitBox(int groupIndex)
        {
            if (groupIndex < 0 || groupIndex >= hitBoxGroups.Length)
            {
                return;
            }
            DisableAllHitbox();
            hitBoxGroups[groupIndex].SetActive(true);
        }
        public void DisableHitBox(int groupIndex)
        {
            if (groupIndex < 0 || groupIndex >= hitBoxGroups.Length)
            {
                return;
            }
            DisableAllHitbox();
            hitBoxGroups[groupIndex].SetActive(false);
        }
        public void SetDirection(int sign)
        {
            foreach (GameObject hitbox in hitBoxGroups)
            {
                hitbox .transform.localScale = new Vector3(sign, 1, 1);
            }

        }
        public void DisableAllHitbox()
        {
            foreach (GameObject hitbox in hitBoxGroups)
            {
                hitbox.SetActive(false);
            }
            _currentHits.Clear();
            StopAllCoroutines();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & targetLayerMask) != 0)
            {
                HurtboxHandler hurtboxHandler = collision.gameObject.GetComponentInParent<HurtboxHandler>();
                if (hurtboxHandler == null)
                { 
                    Debug.LogWarning("Collided object does not have a HurtboxHandler component.");
                    return;
                }
                if (hurtboxHandler.GetCombatEntity() == null)
                {
                    Debug.LogWarning("HurtboxHandler does not have a valid CombatEntity.");
                    return;
                }
                if (hurtboxHandler.GetCombatEntity() == GetComponentInParent<CombatEntity>())
                {
                    return;
                }

                AddToListOfHits(hurtboxHandler.GetCombatEntity());
            }
        }

        private void AddToListOfHits(CombatEntity newHit)
        {
            if (!_currentHits.Contains(newHit))
            {
                _currentHits.Add(newHit);
                onHitTarget.Invoke(newHit);
                StartCoroutine(RemoveFromHitList(newHit, hitCooldown));
            }
        }
        IEnumerator RemoveFromHitList(CombatEntity entity, float hitCooldown)
        {
            yield return new WaitForSeconds(hitCooldown);
            _currentHits.Remove(entity);
        }
    }
}

