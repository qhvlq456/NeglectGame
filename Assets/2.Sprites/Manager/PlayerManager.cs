using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PlayerManager : Singleton<PlayerManager>
{
    [Header("Player GameObject")]
    public Player player; // 실제 캐릭터 프리팹 인스턴스

    public PlayerAbility ability;
    public PlayerSkill skill;

    public bool isLastAttackWasCritical = false;

    public bool isSkillCoolTime { get; private set; }
    public bool isSkill { get; private set; }
    public void Initialize()
    {
        // 유저 데이터 기반 초기화
        var userData = GameManager.Instance.userDataManager.userData;
        ability = new PlayerAbility(userData);
        skill = new PlayerSkill(userData);
        player.Init();
    }

    public float GetAttackDamage()
    {
        float baseAttack = ability.GetAbilityValue(AbilityType.Attack);
        float critChance = ability.GetAbilityValue(AbilityType.CriticalChance);
        float critPower = ability.GetAbilityValue(AbilityType.CriticalPower);

        // 패시브 스킬 적용
        if (skill.HasPassive(SkillType.Passive) && player.IsPessive())
        {
            float passiveBuff = skill.GetDamage(SkillType.Passive);
            baseAttack *= 1f + (passiveBuff / 100f);
        }

        bool isCritical = Random.Range(0f, 100f) < critChance;
        isLastAttackWasCritical = isCritical;

        return isCritical ? baseAttack * (1f + critPower / 100f) : baseAttack;
    }

    public float GetMoveSpeed()
    {
        return ability.GetAbilityValue(AbilityType.Speed);
    }

    public float GetHealth()
    {
        return ability.GetAbilityValue(AbilityType.Health);
    }

    public float GetSkillDamage(SkillType _type)
    {
        Debug.LogError($"skill type : {_type} skill dmg : {skill.GetDamage(_type)}");
        return skill.GetDamage(_type);
    }

    public void UseSkill(SkillType _type)
    {
        if (skill == null || isSkillCoolTime)
            return;

        float damage = GetSkillDamage(_type);

        switch (_type)
        {
            case SkillType.Normal:
                UIManager.Instance.ShowDamageText(player.transform.position, $"기본공격!", Color.white);
                break;

            case SkillType.Main:
                UIManager.Instance.ShowDamageText(player.transform.position, $"처형의 일격!", Color.cyan);
                break;

            case SkillType.Passive:
                // 패시브는 직접 사용하는 방식이 아니라, 체력 조건 체크 등에 의해 발동되므로 별도 처리
                return;
        }

        isSkill = true;
        isSkillCoolTime = true;
        StartCoroutine(CoSkillCoolTime(_type));
        UIManager.Instance.StartSkillCoolTime();
    }
    public void EndSkill()
    {
        isSkill = false;
    }
    IEnumerator CoSkillCoolTime(SkillType _type)
    {
        float coolTime = skill.GetCoolTime(_type);

        // 패시브 타입은 쿨타임 없음
        if (_type == SkillType.Passive || coolTime <= 0f)
        {
            isSkillCoolTime = false;
            yield break;
        }

        yield return new WaitForSeconds(coolTime);
        isSkillCoolTime = false;
    }
}
