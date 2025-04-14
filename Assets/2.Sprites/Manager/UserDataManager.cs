using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public struct AbilityLevelInfo
{
    public AbilityType type;
    public int level;
}

[Serializable]
public struct SkillLevelInfo
{
    public SkillType type;
    public int level;
}

[Serializable]
public class UserData
{
    public int gold;
    public int diamond;
    public int stage;
    public int lastClearedBossStage;
    public bool isBossRetry;

    public List<AbilityLevelInfo> abilityLevelsList = new();
    public List<SkillLevelInfo> skillLevelsList = new();

    // 런타임에서 쓰기 위한 캐시
    [NonSerialized]
    public Dictionary<AbilityType, int> abilityLevels = new();
    [NonSerialized]
    public Dictionary<SkillType, int> skillLevels = new();

    public UserData()
    {
        gold = 0;
        diamond = 0;
        stage = 1;
        lastClearedBossStage = 0;
        isBossRetry = false;

        foreach (AbilityType type in Enum.GetValues(typeof(AbilityType)))
        {
            abilityLevels[type] = 0;
            abilityLevelsList.Add(new AbilityLevelInfo { type = type, level = 0 });
        }

        foreach (SkillType type in Enum.GetValues(typeof(SkillType)))
        {
            int level = (type == SkillType.Normal ? 1 : 0);
            skillLevels[type] = level;
            skillLevelsList.Add(new SkillLevelInfo { type = type, level = level });
        }
    }

    public void SyncFromLists()
    {
        abilityLevels.Clear();
        skillLevels.Clear();

        foreach (var item in abilityLevelsList)
        {
            abilityLevels[item.type] = item.level;
        }

        foreach (var item in skillLevelsList)
        {
            skillLevels[item.type] = item.level;
        }
    }

    public void SyncToLists()
    {
        abilityLevelsList.Clear();
        foreach (var kvp in abilityLevels)
        {
            abilityLevelsList.Add(new AbilityLevelInfo { type = kvp.Key, level = kvp.Value });
        }

        skillLevelsList.Clear();
        foreach (var kvp in skillLevels)
        {
            skillLevelsList.Add(new SkillLevelInfo { type = kvp.Key, level = kvp.Value });
        }
    }
}
public class UserDataManager
{
    private const string fileName = "UserData.json";
    private static string filePath => Path.Combine(Application.persistentDataPath, fileName);

    public UserData userData { get; private set; }

    public void AddGold(int _gold)
    {
        userData.gold += _gold;
        UIManager.Instance.UpdateGoldText();
    }
    public void AddDiamond(int _diamond)
    {
        userData.diamond += _diamond;
        UIManager.Instance.UpdateDiamondText();
    }
    public void SetStage(int _stage)
    {
        userData.stage = _stage;
    }
    public void LoadUserData()
    {
        if (File.Exists(filePath))  // persistentDataPath에서 파일이 존재하면
        {
            string json = File.ReadAllText(filePath);
            userData = JsonUtility.FromJson<UserData>(json);
            userData.SyncFromLists();  // 딕셔너리 복구
        }
        else
        {
            // persistentDataPath에 기본 데이터를 로드해야 할 경우
            InitializeDefaultData();
        }
    }

    // persistentDataPath에 기본 데이터를 저장
    private void InitializeDefaultData()
    {
        userData = new UserData();
        SaveUserData();
    }


    public void SaveUserData()
    {
        userData.SyncToLists(); // 딕셔너리 -> 리스트 변환
        string json = JsonUtility.ToJson(userData, true);
        File.WriteAllText(filePath, json);
    }

    public void ResetUserData()
    {
        TextAsset defaultData = Resources.Load<TextAsset>("UserData");
        if (defaultData != null)
        {
            userData = JsonUtility.FromJson<UserData>(defaultData.text);
            SaveUserData();
        }
    }
}
