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
        public bool ResetHitboxOnDisable = true;
        private HashSet<CombatEntity> _currentHits = new HashSet<CombatEntity>();
        private bool _isCheckingHitboxes;
        public void EnableHitBox(int groupIndex)
        {
            if (_isCheckingHitboxes)
            {
                DisableAllHitbox();
            }

            Debug.Log($"{name} EnableHitbox called. Frame: {Time.frameCount}");
            Debug.Log(System.Environment.StackTrace);

            if (groupIndex < 0 || groupIndex >= hitBoxGroups.Length)
            {
                return;
            }
            _isCheckingHitboxes = true;

            DisableAllHitbox();
            hitBoxGroups[groupIndex].SetActive(true);

        }
        public void EnableAllHitbox()
        {
            _isCheckingHitboxes = true;
            foreach (GameObject hitbox in hitBoxGroups)
            {
                hitbox.SetActive(true);
            }
        }
        public void DisableHitBox(int groupIndex)
        {
            if (groupIndex < 0 || groupIndex >= hitBoxGroups.Length)
            {
                return;
            }
            DisableAllHitbox();
            _isCheckingHitboxes = false;
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
            if(ResetHitboxOnDisable)
                _currentHits.Clear();
        }
        private void FixedUpdate()
        {
            if (_isCheckingHitboxes)
            {
                CheckActiveHitboxes();
            }

        }
        private void CheckActiveHitboxes()
        {
            CombatEntity self = GetComponentInParent<CombatEntity>();
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
                    if (hit.transform.IsChildOf(self.transform))
                        continue;
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

                if (!_currentHits.Contains(hurtboxHandler.GetCombatEntity()))
                {
                    if(ResetHitboxOnDisable)
                    _currentHits.Add(hurtboxHandler.GetCombatEntity());
                    onHitTarget?.Invoke(hurtboxHandler.GetCombatEntity());
                }
            }
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

