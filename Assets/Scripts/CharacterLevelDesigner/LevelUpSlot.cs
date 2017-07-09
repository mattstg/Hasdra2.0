using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class LevelUpSlot : MonoBehaviour {

    public InputField keyTriggerInputFeild;
    private string _keyTrigger = "";
    public string keyTrigger { get { return _keyTrigger; } set { _keyTrigger = value; keyTriggerInputFeild.text = _keyTrigger; } }

    public GV.LevelPkgType lvlPkgType;

    public Text levelText;
    public int _level = 0;
    public int level{set{_level = value; levelText.text = _level.ToString();}get{return _level;}}

    public Text slotNameText;
    public string _slotName = "";
    public string slotName { set { _slotName = value; slotNameText.text = _slotName.ToString(); } get { return _slotName; } }

    public void Initialize(string _name, string _keycode, GV.LevelPkgType _lvlPkgType)
    {
        slotName = _name;
        keyTrigger = _keycode;
        lvlPkgType = _lvlPkgType;
        if (_lvlPkgType == GV.LevelPkgType.Stat || _lvlPkgType == GV.LevelPkgType.Skill)
            LockNoKey();
        else
            UnlockAndRequireKey();
    }

    public void LevelUpSlotSelected()
    {
        GV.charLvlUI.ms.CreatingDraggingSlot(slotName, lvlPkgType);
        Destroy(this.gameObject);
    }

    private void UnlockAndRequireKey()
    {
        keyTriggerInputFeild.gameObject.SetActive(true);
    }

    private void LockNoKey()
    {
        keyTriggerInputFeild.gameObject.SetActive(false);
    }

    public void KeycodeInputChanged()
    {
        _keyTrigger = keyTriggerInputFeild.text;
    }

}
