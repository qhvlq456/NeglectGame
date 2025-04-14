using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISkillItem : MonoBehaviour
{
    [SerializeField]
    private List<UISkillButton> buttonList = new List<UISkillButton>();

    private void Awake()
    {
        foreach (var button in buttonList)
        {
            button.Initialize();
        }
    }
}
