using System.Collections.Generic;

public class PlayerAbility
{
    private Dictionary<AbilityType, int> currentLevels;
    private Dictionary<AbilityType, sAbilityData> baseData;
    private UserData userData;

    public PlayerAbility(UserData data)
    {
        userData = data;
        currentLevels = data.abilityLevels;
        baseData = GameManager.Instance.dataManager.abilityDic;
    }

    public float GetAbilityValue(AbilityType _type)
    {
        var level = currentLevels[_type];
        var data = baseData[_type];
        return data.baseValue + data.increasePerLevel * level;
    }

    public int GetUpgradeCost(AbilityType _type)
    {
        var level = currentLevels[_type] + 1;
        var data = baseData[_type];
        return data.baseCost + data.costPerLevel * level;
    }

    // 다음 레벨 값 (미리보기용)
    public float GetAbilityValue(AbilityType _type, int _level)
    {
        var data = baseData[_type];
        return data.baseValue + data.increasePerLevel * _level;
    }
    public bool TryUpgrade(AbilityType _type)
    {
        int cost = GetUpgradeCost(_type);
        if (userData.gold < cost)
        {
            UnityEngine.Debug.LogWarning("골드 부족!");
            return false;
        }

        userData.gold -= cost;
        currentLevels[_type]++;
        // GameManager.Instance.userDataManager.SaveUserData(); // 저장
        UIManager.Instance.UpdateGoldText(); // 골드 UI 갱신
        return true;
    }
    public (bool canUpgrade, bool isMaxLevel, bool isGoldEnough) CanUpgrade(AbilityType _type)
    {
        var currentLevel = currentLevels[_type];
        var data = baseData[_type];

        bool isMax = currentLevel >= data.maxLevel;
        bool hasGold = userData.gold >= data.GetUpgradeCost(currentLevel + 1);

        return (!isMax && hasGold, isMax, hasGold);
    }
    public int GetLevel(AbilityType _type)
    {
        return currentLevels[_type];
    }
}
