using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private bool isGameStart = false;

    public UserDataManager userDataManager;
    public DataManager dataManager;

    private IEnumerator GameFlowCoroutine = null;
    private void Start()
    {
        LoadGame();
    }
    private void LoadGame()
    {
        if (userDataManager == null)
        {
            userDataManager = new UserDataManager();
            userDataManager.LoadUserData();
            //Debug.LogError($"gold : {userDataManager.userData.gold}, dia : {userDataManager.userData.diamond}" +
            //    $" , stage : {userDataManager.userData.stage}");
        }

        if (dataManager == null) 
        { 
            dataManager = new DataManager();
            dataManager.LoadData();
        }

        if (GameFlowCoroutine != null) 
        {
            StopCoroutine(GameFlowCoroutine);
        }

        GameFlowCoroutine = CoGameFlow();
        StartCoroutine(GameFlowCoroutine);
    }

    private void Update()
    {
        if (isGameStart) 
        {
            LoadGame();
            isGameStart = false;
        }
    }
    private IEnumerator CoGameFlow()
    {
        PlayerManager.Instance.Initialize();
        UIManager.Instance.Initialize();

        // StageData를 세팅하고
        StageManager.Instance.LoadStageData(userDataManager.userData.stage);

        // 그다음에 Stage 시작
        yield return null; 
        StageManager.Instance.StartStage();
    }

    private void OnApplicationQuit()
    {
        // userDataManager.SaveUserData();
    }
}
