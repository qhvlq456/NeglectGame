using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelUpgrade : MonoBehaviour
{
    [SerializeField]
    private Button exitBtn;

    [SerializeField]
    private List<UpgradeItem> upgradeItems = new List<UpgradeItem>();
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
        Close();
    }
}
