using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SpawnDirection
{
    East, West, South, North
}
public class EnemySpawner : MonoBehaviour
{
    public Transform eastSpawn;
    public Transform westSpawn;
    public Transform southSpawn;
    public Transform northSpawn;

    private Dictionary<SpawnDirection, Transform> spawnPoints;

    public List<GameObject> enemyObjList = new List<GameObject>();

    private List<Enemy> activeEnemies = new List<Enemy>();
    private Queue<Enemy> pooledEnemies = new Queue<Enemy>();

    [Header("Spawn Y Position")]
    [Range(0, 100)]
    [SerializeField]
    private float maxPosition;
    private void Awake()
    {
        spawnPoints = new Dictionary<SpawnDirection, Transform>
        {
            { SpawnDirection.East, eastSpawn },
            { SpawnDirection.West, westSpawn },
            { SpawnDirection.South, southSpawn },
            { SpawnDirection.North, northSpawn }
        };
    }
    public Enemy SpawnEnemy(sStageData _stage)
    {
        GameObject enemyPrefab = enemyObjList[Random.Range(0, enemyObjList.Count)];
        SpawnDirection dir = (SpawnDirection)Random.Range(maxPosition, spawnPoints.Count);
        Transform spawnPoint = spawnPoints[dir];
        Vector3 spawnPos = spawnPoint.position;

        switch (dir)
        {
            case SpawnDirection.East:
                spawnPos += new Vector3(Random.Range(0f, 2f), Random.Range(-1f, 1f));
                break;
            case SpawnDirection.West:
                spawnPos += new Vector3(Random.Range(-2f, 0f), Random.Range(-1f, 1f));
                break;
            case SpawnDirection.South:
                spawnPos += new Vector3(Random.Range(-1f, 1f), -1f);
                break;
            case SpawnDirection.North:
                spawnPos += new Vector3(Random.Range(-1f, 1f), 1.5f);
                break;
        }

        if (_stage.enemyHealth <= 0 || _stage.enemyAttackPower <= 0)
        {
            Debug.LogError($"[EnemySpawner] StageData 비정상! HP: {_stage.enemyHealth}, ATK: {_stage.enemyAttackPower}");
        }

        Enemy enemy = GetEnemyFromPool(enemyPrefab, spawnPos);
        enemy.Init(_stage);
        enemy.gameObject.SetActive(true);
        activeEnemies.Add(enemy);
        return enemy;
    }

    private Enemy GetEnemyFromPool(GameObject prefab, Vector3 position)
    {
        Enemy enemy;

        if (pooledEnemies.Count > 0)
        {
            enemy = pooledEnemies.Dequeue();
            enemy.transform.position = position;
            enemy.gameObject.SetActive(true);
        }
        else
        {
            GameObject obj = Instantiate(prefab, position, Quaternion.identity);
            enemy = obj.GetComponent<Enemy>();
            enemy.OnDeath += ReturnEnemyToPool; // 적이 죽으면 풀로 돌아오게 설정
        }

        return enemy;
    }

    private void ReturnEnemyToPool(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            enemy.gameObject.SetActive(false);
            pooledEnemies.Enqueue(enemy);
        }
    }

    public void ClearEnemies()
    {
        foreach (Enemy enemy in activeEnemies)
        {
            if (enemy != null)
            {
                enemy.gameObject.SetActive(false);
                pooledEnemies.Enqueue(enemy);
            }
        }
        activeEnemies.Clear();
    }
}
