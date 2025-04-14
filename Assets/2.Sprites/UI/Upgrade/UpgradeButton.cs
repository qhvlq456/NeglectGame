using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UpgradeButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public AbilityType type;
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
        var (canUpgrade, isMaxLevel, isGoldEnough) = PlayerManager.Instance.ability.CanUpgrade(type);

        int lv = PlayerManager.Instance.ability.GetLevel(AbilityType.Attack);
        string title = "";
        string s = "";

        switch (type)
        {
            case AbilityType.Attack:
                title = "공격력";
                break;
            case AbilityType.Health:
                title = "체력";
                break;
            case AbilityType.Speed:
                title = "이동속도";
                break;
            case AbilityType.CriticalChance:
                title = "치명타 확률";
                break;
            case AbilityType.CriticalPower:
                title = "치명타 데미지";
                break;
        }

        if (!canUpgrade)
        {
            if (isMaxLevel)
            {
                s = $"레벨 : {lv} \n 최대 레벨입니다.";
                button.interactable = false;
            }
            else if (!isGoldEnough)
            {
                button.interactable = false;
                s = $"레벨 : {lv} \n 골드가 부족합니다." + "\n" + $"{PlayerManager.Instance.ability.GetUpgradeCost(type)}";
            }
        }
        else
        {
            button.interactable = true;

            s = string.Format("{0} \n 레벨 {1} -> {2} \n {3} -> {4} \n 비용 : {5}",
                        title, lv, lv + 1, PlayerManager.Instance.ability.GetAbilityValue(type),
                        PlayerManager.Instance.ability.GetAbilityValue(type, lv + 1),
                        PlayerManager.Instance.ability.GetUpgradeCost(type));
        }

        text.text = s;
    }
    // 버튼 눌렀을 때
    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        holdTimer = 0f; // 즉시 첫 반복 실행
    }

    // 버튼 땠을 때
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
        // 예: 사운드 재생, 이펙트, UI 갱신 등
        GameManager.Instance.userDataManager.SaveUserData();
    }

    private void ButtonsBtnClick()
    {
        PlayerManager.Instance.ability.TryUpgrade(type);
        UpdateText();
    }
}
