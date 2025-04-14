using System.Collections.Generic;

public class PlayerSkill
{
    private Dictionary<SkillType, int> currentLevels;           // 유저 데이터에 저장된 레벨
    private Dictionary<SkillType, sSkillData> baseData;         // DataManager에서 불러온 스킬 기획 데이터

    public PlayerSkill(UserData data)
    {
        currentLevels = data.skillLevels;
        baseData = GameManager.Instance.dataManager.skillDic;
    }

    public float GetDamage(SkillType _type)
    {
        int level = currentLevels.ContainsKey(_type) ? currentLevels[_type] : 0;
        var data = baseData[_type];

        if (data.type == SkillType.Passive) return 0;
        return data.baseMultiplier + data.multiplierPerLevel * level;
    }
    public float GetDamage(SkillType _type, int _level)
    {
        var data = baseData[_type];
        if (data.type == SkillType.Passive) return 0;
        return data.baseMultiplier + data.multiplierPerLevel * _level;
    }

    public float GetCoolTime(SkillType _type)
    {
        return baseData[_type].coolTime;
    }

    public int GetUpgradeCost(SkillType _type)
    {
        int level = currentLevels.ContainsKey(_type) ? currentLevels[_type] : 0;

        if(level == 0)
        {
            return baseData[_type].initialUnlockCost;
        }
        else
        {
            return baseData[_type].levelUpCostMultiplier * level;
        }
    }

    public bool IsUnlocked(SkillType _type)
    {
        return currentLevels.ContainsKey(_type) && currentLevels[_type] > 0;
    }

    public bool CanUpgrade(SkillType _type, int _currentDia)
    {
        if (!baseData.ContainsKey(_type)) return false;

        int level = currentLevels.ContainsKey(_type) ? currentLevels[_type] : 0;
        var data = baseData[_type];

        if (level >= data.maxLevel) return false;
        int cost = data.levelUpCostMultiplier * level;
        return _currentDia >= cost;
    }

    public bool TryUpgrade(SkillType _type, ref int _dia)
    {
        if (!baseData.ContainsKey(_type)) return false;

        int level = currentLevels.ContainsKey(_type) ? currentLevels[_type] : 0;
        var data = baseData[_type];

        if (level >= data.maxLevel) return false;

        int cost = data.levelUpCostMultiplier * level;
        if (_dia < cost) return false;

        _dia -= cost;
        currentLevels[_type] = level + 1;
        UIManager.Instance.UpdateDiamondText(); // 골드 UI 갱신
        return true;
    }

    public bool TryUnlock(SkillType _type, ref int _dia)
    {
        if (!baseData.ContainsKey(_type)) return false;
        if (IsUnlocked(_type)) return false;

        var data = baseData[_type];
        int totalUnlockCost = data.unlockCost + data.initialUnlockCost;

        if (_dia < totalUnlockCost) return false;

        _dia -= totalUnlockCost;
        currentLevels[_type] = 1;
        return true;
    }

    public float GetPassiveBonusPercent()
    {
        if (!baseData.ContainsKey(SkillType.Passive)) return 0;
        int level = currentLevels.ContainsKey(SkillType.Passive) ? currentLevels[SkillType.Passive] : 0;
        var data = baseData[SkillType.Passive];
        return data.baseMultiplier + data.multiplierPerLevel * level;
    }

    public bool HasPassive(SkillType _type)
    {
        return currentLevels.ContainsKey(_type) && currentLevels[_type] > 0;
    }
    public int GetLevel(SkillType _type) => currentLevels.TryGetValue(_type, out var lv) ? lv : 0;

    public int GetMaxLevel(SkillType _type) => baseData[_type].maxLevel;

    public bool CanUpgrade(SkillType _type)
    {
        int level = GetLevel(_type);
        int cost = GetUpgradeCost(_type);
        return level < GetMaxLevel(_type) && GameManager.Instance.userDataManager.userData.diamond >= cost;
    }

    public bool TryUpgrade(SkillType _type)
    {
        if (!CanUpgrade(_type)) return false;

        int cost = GetUpgradeCost(_type);
        GameManager.Instance.userDataManager.userData.diamond -= cost;
        currentLevels[_type]++;
        return true;
    }

}
