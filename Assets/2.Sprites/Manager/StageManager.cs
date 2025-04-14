using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    public sStageData currentStageData { get; private set; }

    [SerializeField]
    private float enemySpawnDelay = 0.25f;

    [SerializeField]
    private float waitNextStageDelay = 1f;

    [SerializeField] 
    private EnemySpawner spawner;
    [SerializeField] 
    private PlayerManager playerManager;

    IEnumerator StartStageCoroutine = null;

    public void StartStage()
    {
        StopAllCoroutines();
        if (StartStageCoroutine != null)
        {
            StopCoroutine(StartStageCoroutine);
        }

        StartStageCoroutine = StageRoutine();
        StartCoroutine(StageRoutine());
    }
    public void LoadStageData(int _stage)
    {
        if(PlayerManager.Instance.player.IsDead())
        {
            PlayerManager.Instance.player.Init();
        }

        currentStageData = GameManager.Instance.dataManager.stageDic[_stage];
        UIManager.Instance.UpdateStageText(currentStageData.level);
        UIManager.Instance.IsActiveBossBtn();
    }

    private IEnumerator StageRoutine()
    {
        spawner.ClearEnemies();
        int enemyCount = currentStageData.isBoss ? 1 : Random.Range(2,5);
        Queue<Enemy> enemyQueue = new Queue<Enemy>();

        for (int i = 0; i < enemyCount; i++)
        {
            Enemy enemy = spawner.SpawnEnemy(currentStageData);
            enemyQueue.Enqueue(enemy);
            yield return new WaitForSeconds(enemySpawnDelay); // 스폰 텀
        }

        while (enemyQueue.Count > 0)
        {
            if(playerManager.player.IsDead())
            {
                spawner.ClearEnemies();
                enemyQueue.Clear();
                break;
            }

            Enemy enemy = enemyQueue.Dequeue();
            yield return StartCoroutine(playerManager.player.CombatRoutine(enemy)); // 전투 시작 지시
        }

        int nextStage = currentStageData.level + 1;

        if (playerManager.player.IsDead())
        {
            nextStage = GameManager.Instance.userDataManager.userData.lastClearedBossStage + 1;
            Debug.LogError($"nextStage : {nextStage}, GameManager.Instance.userDataManager.userData.lastClearedBossStage : {GameManager.Instance.userDataManager.userData.lastClearedBossStage}");
            if (currentStageData.isBoss)
            {
                GameManager.Instance.userDataManager.userData.isBossRetry = true;
            }
        }
        else
        {
            if (currentStageData.isBoss)
            {
                if(GameManager.Instance.userDataManager.userData.isBossRetry)
                {
                    GameManager.Instance.userDataManager.userData.isBossRetry = false;
                }

                GameManager.Instance.userDataManager.userData.lastClearedBossStage = currentStageData.level; // 여기서만 갱신
                GameManager.Instance.userDataManager.AddDiamond(currentStageData.diamondReward);
                Debug.Log($"[보스 클리어] 다이아 획득: {currentStageData.diamondReward}");
            }
            else
            {
                GameManager.Instance.userDataManager.AddGold(currentStageData.goldReward);
                Debug.Log($"[일반 몬스터 클리어] 골드 획득: {currentStageData.goldReward}");
            }
            // clear 할 경우만 저장
            GameManager.Instance.userDataManager.SetStage(nextStage);
            GameManager.Instance.userDataManager.SaveUserData();
        }

        yield return new WaitForSeconds(waitNextStageDelay);

        LoadStageData(nextStage); // 다음 스테이지로
        StartCoroutine(StageRoutine());
    }
}
