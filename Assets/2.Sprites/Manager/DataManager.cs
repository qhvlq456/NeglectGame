using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    public Dictionary<int, sStageData> stageDic = new Dictionary<int, sStageData>();
    public Dictionary<AbilityType, sAbilityData> abilityDic = new Dictionary<AbilityType, sAbilityData>();
    public Dictionary<SkillType, sSkillData> skillDic = new Dictionary<SkillType, sSkillData>();

    public void LoadData()
    {
        LoadStageData();
        LoadAbillityData();
        LoadSkillData();
    }

    private void LoadStageData()
    {
        stageDic.Clear();

        for (int i = 1; i <= 200; i++)
        {
            sStageData data = new sStageData();
            data.CalculateStageStats(i);
            stageDic.Add(i, data);
        }

        Debug.Log("StageData Loaded: " + stageDic.Count);
    }

    private void LoadAbillityData()
    {
        var list = GameUtil.JsonLoader<sAbilityData>.LoadJson("Data/ability_data");
        abilityDic.Clear();

        foreach (var item in list)
        {
            abilityDic[item.type] = item;
        }

        Debug.Log("AbilityData Loaded: " + abilityDic.Count);
    }

    private void LoadSkillData()
    {
        var list = GameUtil.JsonLoader<sSkillData>.LoadJson("Data/skill_data");
        skillDic.Clear();

        foreach (var item in list)
        {
            skillDic[item.type] = item;
            Debug.LogError($"Skill Loaded: {item.type}");
        }

        Debug.Log("SkillData Loaded: " + skillDic.Count);
    }
}
