using System.Collections;
using System.Collections.Generic;
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

        private void Start()
        {
            SpawnSceneEnemies();
        }
        public void SpawnSceneEnemies()
        {
            foreach (EnemySpawnPoint spawnPoint in SceneSpawnPoints)
            {
                SpawnEnemy(spawnPoint.enemyType, spawnPoint.transform.position);
            }
        }
        public void SpawnEnemy(EnemyType type, Vector2 position)
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
