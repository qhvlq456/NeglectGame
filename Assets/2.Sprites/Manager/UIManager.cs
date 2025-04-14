using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : Singleton<UIManager>
{
    private DamageTextPool damageTextPool;
    [SerializeField]
    private DamageText damangeText;
    [SerializeField]
    private TextMeshProUGUI stageText;
    [SerializeField]
    private TextMeshProUGUI goldText;
    [SerializeField]
    private TextMeshProUGUI diamondText;
    [SerializeField]
    private Button bossBtn;
    [SerializeField]
    private Button skillBtn;
    [SerializeField]
    private Button upgradePanelBtn;
    [SerializeField]
    private Button skillPaenlBtn;

    [SerializeField]
    private UIPanelUpgrade uiPanelUpgrade;
    [SerializeField]
    private UIPanelSkill uipanelSkill;

    public void Initialize()
    {
        damageTextPool = new DamageTextPool(damangeText);

        UpdateGoldText();
        UpdateDiamondText();
        UpdateStageText(GameManager.Instance.userDataManager.userData.stage);

        AllocateBtnEvent();

        IsActiveBossBtn();
        IsActiveMainSkillBtn();
    }
    public void AllocateBtnEvent()
    {
        bossBtn.onClick.AddListener(BossBtnClick);
        skillBtn.onClick.AddListener(SkillBtnClick);
        upgradePanelBtn.onClick.AddListener(UpgradeBtnClick);
        skillPaenlBtn.onClick.AddListener(SkillPanelBtnClick);
    }
    public void UpdateGoldText()
    {
        goldText.text = string.Format("Gold : {0}", GameManager.Instance.userDataManager.userData.gold.ToString());
    }
    public void UpdateDiamondText()
    {
        diamondText.text = string.Format("Diamond : {0}", GameManager.Instance.userDataManager.userData.diamond.ToString());
    }
    public void UpdateStageText(int _stage)
    {
        stageText.text = string.Format("Stage : {0}", _stage.ToString());
    }

    public void ShowDamageText(Vector2 _position, string _value, Color _color)
    {
        DamageText dt = damageTextPool.Get();

        dt.OnClose = damageTextPool.ReturnToPool;
        dt.StartDamage(_position, _value, _color);
    }
    public void IsActiveBossBtn()
    {
        bossBtn.gameObject.SetActive(GameManager.Instance.userDataManager.userData.isBossRetry);

        if(bossBtn.gameObject.activeSelf)
        {
            StartCoroutine(CoBossBtnTime());
        }
    }
    public void IsActiveMainSkillBtn()
    {
        skillBtn.interactable = PlayerManager.Instance.skill.IsUnlocked(SkillType.Main);
    }
    public void BossBtnClick()
    {
        StartCoroutine(CoBossBtnTime());

        int stage = GameManager.Instance.userDataManager.userData.lastClearedBossStage + 5;
        StageManager.Instance.LoadStageData(stage);
        StageManager.Instance.StartStage();
        Debug.LogError($"boss btn click Stage : {stage}");
    }
    public void SkillPanelBtnClick()
    {
        uipanelSkill.Open();
    }
    public void UpgradeBtnClick()
    {
        uiPanelUpgrade.Open();
    }
    public void SkillBtnClick()
    {
        PlayerManager.Instance.UseSkill(SkillType.Main);
    }
    public void StartSkillCoolTime()
    {
        StartCoroutine(CoSkillCoolTime());
    }

    IEnumerator CoSkillCoolTime()
    {
        skillBtn.interactable = false;
        yield return new WaitForSeconds(PlayerManager.Instance.skill.GetCoolTime(SkillType.Main));
        skillBtn.interactable = true;
    }

    IEnumerator CoBossBtnTime()
    {
        bossBtn.interactable= false;
        while(StageManager.Instance.currentStageData.isBoss)
        {
            yield return null;
        }
        bossBtn.interactable = true;
    }
}
