using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelUpOption : MonoBehaviour {

    private string _optionName = "";
    public string optionName{set{_optionName = value; gameObject.GetComponentInChildren<Text>().text = _optionName;}get{return _optionName;}}
    public GV.LevelPkgType lvlPkgType;

    public void Initialize(string optName, GV.LevelPkgType _lvlPkgType)
    {
        lvlPkgType = _lvlPkgType;
        optionName = optName;
    }

    public void LevelUpOptionSelected()
    {
        GV.charLvlUI.ms.CreatingDraggingSlot(optionName, lvlPkgType);
    }

    public void LevelUpOptionDropped()
    {
        GV.charLvlUI.ms.DropDraggingSlot(this);
    }
}

