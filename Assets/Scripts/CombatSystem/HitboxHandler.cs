using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Playables;
using static UnityEngine.UI.GridLayoutGroup;


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

        private bool _checkingHitboxes;
        public void EnableHitBox(int groupIndex)
        {
            Debug.Log($"Enabling hitbox group {groupIndex}");
            if (groupIndex < 0 || groupIndex >= hitBoxGroups.Length)
            {
                return;
            }
            DisableAllHitbox();
            hitBoxGroups[groupIndex].SetActive(true);
            _checkingHitboxes = true;
        }
        public void DisableHitBox(int groupIndex)
        {
            if (groupIndex < 0 || groupIndex >= hitBoxGroups.Length)
            {
                return;
            }
            DisableAllHitbox();
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
            _checkingHitboxes = false;
            _currentHits.Clear();
            StopAllCoroutines();
        }
        private void Update() 
        {
            if(_checkingHitboxes)
                CheckActiveHitboxes();
        }
        private void CheckActiveHitboxes()
        {
            foreach (GameObject hitbox in hitBoxGroups)
            {
                if (!hitbox.activeInHierarchy)
                    continue;

                BoxCollider2D box = hitbox.GetComponent<BoxCollider2D>();

                if (box == null)
                    continue;

                Vector2 center = box.bounds.center;
                Vector2 size = box.bounds.size;

                Collider2D[] hits = Physics2D.OverlapBoxAll( center, size,  0f,targetLayerMask);
                DrawDebugBox(center, size);

                foreach (Collider2D hit in hits)
                {
                    HitCheck(hit);
                }
            }

        }
        private void HitCheck(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & targetLayerMask.value) != 0)
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

        private void DrawDebugBox(
            Vector2 origin,
            Vector2 size)
        {
            Vector2 half = size * .5f;

            Vector2 tl =
                origin +
                new Vector2(-half.x, half.y);

            Vector2 tr =
                origin +
                new Vector2(half.x, half.y);

            Vector2 bl =
                origin +
                new Vector2(-half.x, -half.y);

            Vector2 br =
                origin +
                new Vector2(half.x, -half.y);

            Debug.DrawLine(tl, tr, Color.green);
            Debug.DrawLine(tr, br, Color.green);
            Debug.DrawLine(br, bl, Color.green);
            Debug.DrawLine(bl, tl, Color.green);
        }
    }

}

