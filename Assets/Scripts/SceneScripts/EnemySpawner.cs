using CombatSystem;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace EnemySpawning
{
    public enum EnemyType
    {
        Patroller,
        Flyer,
        Charger
    }
    public class EnemySpawner : MonoBehaviour
    {
        // Simple pooling system for enemies. not really scalable but it works for this project. If I had more time I would make a more robust pooling system that could handle any type of enemy and would be more efficient.
        [Header("Enemy Prefabs")]
        public GameObject PatrollerPrefab;
        public GameObject FlyerPrefab;
        public GameObject ChargerPrefab;

        [Header("Pools")]
        public List<GameObject> PatrollerPool = new List<GameObject>();
        public List<GameObject> FlyerPool = new List<GameObject>();
        public List<GameObject> ChargerPool = new List<GameObject>();

        [Header("Scene specific")]
        public List<EnemySpawnPoint> SceneSpawnPoints = new List<EnemySpawnPoint>();

        //called by gamemanager
        public void SpawnSceneEnemies()
        {
            foreach (EnemySpawnPoint spawnPoint in SceneSpawnPoints)
            {
                SpawnAndTrack(spawnPoint);
                // if it gets disabled, spawn another enemy after 5 seconds

            }
        }
        private void SpawnAndTrack(EnemySpawnPoint spawnPoint)
        {
            GameObject enemy = SpawnEnemy(spawnPoint.enemyType, spawnPoint.transform.position);

            enemy.OnDisableAsObservable().Take(1).Delay(System.TimeSpan.FromSeconds(5)).Subscribe(_ =>
            {
                SpawnAndTrack(spawnPoint);
            }).AddTo(this);
        }
        public GameObject SpawnEnemy(EnemyType type, Vector2 position)
        {
            GameObject enemyToSpawn = null;
            switch (type)
            {
                case EnemyType.Patroller:
                    enemyToSpawn = GetInactiveEnemyFromList(PatrollerPool);
                    break;
                case EnemyType.Flyer:
                    enemyToSpawn = GetInactiveEnemyFromList(FlyerPool);
                    break;
                case EnemyType.Charger:
                    enemyToSpawn = GetInactiveEnemyFromList(ChargerPool);
                    break;
            }
            if (enemyToSpawn == null)
            {
                enemyToSpawn = InstantiateNewEnemy(type);
            }
            enemyToSpawn.transform.position = position;
            enemyToSpawn.SetActive(true);
            enemyToSpawn.GetComponent<CharacterLogicHandler>().ResetEnemy();
            return enemyToSpawn;
        }
        public GameObject GetInactiveEnemyFromList(List<GameObject> list)
        {
            foreach (GameObject enemy in list)
            {
                if (!enemy.activeInHierarchy)
                {
                    return enemy;
                }
            }
            return null;
        }
        public GameObject InstantiateNewEnemy(EnemyType type)
        {
            GameObject newEnemy = null;
            switch (type)
            {
                case EnemyType.Patroller:
                    newEnemy = Instantiate(PatrollerPrefab);
                    PatrollerPool.Add(newEnemy);
                    break;
                case EnemyType.Flyer:
                    newEnemy = Instantiate(FlyerPrefab);
                    FlyerPool.Add(newEnemy);
                    break;
                case EnemyType.Charger:
                    newEnemy = Instantiate(ChargerPrefab);
                    ChargerPool.Add(newEnemy);
                    break;
            }
            return newEnemy;

        }
    }
}
