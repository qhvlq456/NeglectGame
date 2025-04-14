using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelSkill : MonoBehaviour
{
    [SerializeField]
    private Button exitBtn;

    [SerializeField]
    private List<UISkillItem> skillItems = new List<UISkillItem>();
    private void Awake()
    {
        exitBtn.onClick.AddListener(ExitBtnClick);
    }
    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void ExitBtnClick()
    {
        UIManager.Instance.IsActiveMainSkillBtn();
        Close();
    }
}
