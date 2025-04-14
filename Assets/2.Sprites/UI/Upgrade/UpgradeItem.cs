using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeItem : MonoBehaviour
{
    [SerializeField]
    private List<UpgradeButton> buttonList = new List<UpgradeButton>();

    private void Awake()
    {
        foreach (var button in buttonList)
        {
            button.Initialize();
        }
    }
}
