using Cinemachine;
using GameManagerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager instance;
        public Material flashMaterial;
        public Material defaultMaterial;
        public Material attackFlashMaterial;
        public float knockbackForce = 10f;
        public float hitStopDuration = 0.2f;
        public float cameraShakeForce = 0.2f;
        public float cameraShakeForceFatal = 0.6f;
        public float cameraShakeDuration = 0.2f;


        CinemachineVirtualCamera mainCamera;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void FindCamera()
        {
            mainCamera = FindObjectOfType<CinemachineVirtualCamera>();
        }
        #region Combat System Logic
        public void ExecuteAttack(CombatData data)
        {
            Debug.Log($"Executing attack from {data.source.name} to {data.target.name} for {data.damage} damage.");
            CombatEntity targetEntity = data.target.GetComponent<CombatEntity>();
            if (targetEntity != null)
            {
                targetEntity.TakeDamage(data.damage, data.source);
                SpriteFlash(data);
                Knockback(data);
                HitStop();
                CameraShake(data);
            }
        }
        #endregion

        #region CombatSystem Juicing
        public void AttackFlash(SpriteRenderer renderer)
        {
            StartCoroutine(SpriteAttackFlashRoutine(renderer));
        }
        IEnumerator SpriteAttackFlashRoutine(SpriteRenderer renderer)
        {
            renderer.material = attackFlashMaterial;
            yield return new WaitForSecondsRealtime(0.1f);
            renderer.material = defaultMaterial;
            yield return new WaitForSecondsRealtime(0.1f);
            renderer.material = attackFlashMaterial;
            yield return new WaitForSecondsRealtime(0.1f);
            renderer.material = defaultMaterial;
        }

        public void SpriteFlash(CombatData data)
        {
            SpriteRenderer renderer = data.target.GetComponent<SpriteRenderer>();
            StartCoroutine(SpriteFlashRoutine(renderer));
        }
        IEnumerator SpriteFlashRoutine(SpriteRenderer renderer)
        {
            renderer.material = flashMaterial;
            yield return new WaitForSecondsRealtime(0.1f);
            renderer.material = defaultMaterial;
        }

        public void Knockback(CombatData data)
        {
            int directionAwayFromSource = data.source.transform.position.x < data.target.transform.position.x ? 1 : -1;
            Rigidbody2D rb = data.target.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 knockbackDirection = new Vector2(directionAwayFromSource, 1).normalized;
                rb.velocity = Vector2.zero;
                rb.velocity = new Vector2((knockbackDirection * knockbackForce).x, 0);
            }
        }
        public void HitStop()
        {
            GameManager.Get().PauseGameDuration(hitStopDuration);
        }

        public void CameraShake(CombatData data)
        {
            if (mainCamera == null)
            {
                FindCamera();
            }
            if (mainCamera != null)
            {
                
                StartCoroutine(ShakeCamera(data.isFatal? cameraShakeForceFatal * 3 : cameraShakeForce));
            }
        }
        IEnumerator ShakeCamera(float shakeForce)
        {
            CinemachineBasicMultiChannelPerlin noise = mainCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            noise.m_AmplitudeGain = shakeForce;
            yield return new WaitForSecondsRealtime(cameraShakeDuration);
            noise.m_AmplitudeGain = 0f;
        }
        #endregion
    }

    // This class is used to pass data about a combat interaction between entities. It can be expanded in the future to include more information such as attack type, hit location, etc.
    public class CombatData
    {
        public CombatEntity source;
        public CombatEntity target;
        public int damage;
        public bool isFatal;
    }
}
