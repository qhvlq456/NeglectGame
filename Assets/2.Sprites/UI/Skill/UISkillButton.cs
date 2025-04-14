using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISkillButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public SkillType type;
    public Button button;
    public TextMeshProUGUI text;

    private bool isHolding = false;
    private float holdTimer = 0f;
    private float holdRepeatDelay = 0.1f;

    public void Initialize()
    {
        button.onClick.AddListener(() => OnSingleClick());
        UpdateText();
    }

    private void Update()
    {
        if (isHolding)
        {
            holdTimer -= Time.deltaTime;
            if (holdTimer <= 0f)
            {
                ButtonsBtnClick();
                holdTimer = holdRepeatDelay;
            }
        }
    }

    public void UpdateText()
    {
        var skill = PlayerManager.Instance.skill;
        int level = skill.GetLevel(type);
        int maxLevel = skill.GetMaxLevel(type);
        int cost = skill.GetUpgradeCost(type);
        bool isMax = level >= maxLevel;

        string title = type switch
        {
            SkillType.Normal => "일반 공격",
            SkillType.Main => "처형의 일격",
            SkillType.Passive => "불굴의 의지",
            _ => "스킬"
        };

        string s = "";

        if (isMax)
        {
            s = $"{title}\n최대 레벨입니다.";
            button.interactable = false;
        }
        else if (!skill.CanUpgrade(type))
        {
            s = $"{title}\n다이아 부족\n필요: {cost}";
            button.interactable = false;
        }
        else
        {
            float now = skill.GetDamage(type);
            float next = skill.GetDamage(type, level + 1);

            s = $"{title}\n레벨 {level} → {level + 1}\n{now:F1} → {next:F1}\n비용: {cost}";
            button.interactable = true;
        }

        text.text = s;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        holdTimer = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        holdTimer = 0f;
        OnButtonReleased();
    }

    private void OnSingleClick()
    {
        ButtonsBtnClick();
    }

    private void OnButtonReleased()
    {
        GameManager.Instance.userDataManager.SaveUserData();
    }

    private void ButtonsBtnClick()
    {
        if (PlayerManager.Instance.skill.TryUpgrade(type))
        {
            UpdateText();
        }
    }
}
