using System;

public struct sStageData
{
    public int level;
    public bool isBoss;
    public int enemyHealth;
    public int enemyAttackPower;
    public int goldReward;
    public int diamondReward;

    public void CalculateStageStats(int _stage)
    {
        level = _stage;

        if (_stage <= 50)
        {
            enemyHealth = 100 + _stage * 20;
            enemyAttackPower = 10 + _stage * 2;
            goldReward = _stage * 5;
        }
        else if (_stage <= 100)
        {
            enemyHealth = 1100 + _stage * 30;
            enemyAttackPower = 110 + _stage * 4;
            goldReward = _stage * 6;
        }
        else if (_stage <= 150)
        {
            enemyHealth = 2600 + _stage * 40;
            enemyAttackPower = 310 + _stage * 6;
            goldReward = _stage * 7;
        }
        else
        {
            enemyHealth = 4600 + _stage * 50;
            enemyAttackPower = 610 + _stage * 8;
            goldReward = _stage * 8;
        }

        if (_stage % 5 == 0)
        {
            isBoss = true;
            enemyHealth *= 5;
            enemyAttackPower *= 2;
            diamondReward = _stage * 5;
        }
        else
        {
            isBoss = false;
            diamondReward = 0;
        }
    }
}
public enum SkillType { Normal, Main, Passive }

[Serializable]
public struct sSkillData
{
    public SkillType type;
    public string name;
    public int level;
    public float coolTime;                 // 쿨타임 (패시브는 0)
    public float baseMultiplier;          // 기본 배수 (또는 패시브 효과 수치)
    public float multiplierPerLevel;      // 레벨당 추가 배수
    public int maxLevel;

    public int unlockCost;                // 잠금 해제 비용
    public int initialUnlockCost;         // 0 -> 1레벨 시 소모 비용
    public int levelUpCostMultiplier;     // 현재 레벨 * 계수

    public float GetDamageMultiplier(int _level) =>
        baseMultiplier + multiplierPerLevel * _level;

    public int GetUpgradeCost(int _level) =>
        _level * levelUpCostMultiplier;
}
public enum AbilityType { Attack, Health, Speed, CriticalChance, CriticalPower }

[Serializable]
public struct sAbilityData
{
    public int level;
    public AbilityType type;
    public int maxLevel;
    public float baseValue;
    public float increasePerLevel;
    public int baseCost;
    public int costPerLevel;

    public float GetValue(int _level) => baseValue + increasePerLevel * _level;
    public int GetUpgradeCost(int _level) => baseCost + costPerLevel * _level;
}

public enum ButtonType { Attack, Health, Speed, Critical, CriticalDmg }

