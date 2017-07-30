using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SIUISCDropdown : SIBasicVarSlotUI
{
    public Dropdown dropdown;

    public override void Initialize()
    {
        base.Initialize();
        FillDropdown();
    }
    

    public void FillDropdown()
    {
        GV.SetDropdownByEnum(stateVarType, dropdown);
    }

    public override string GetValue()
    {
        return dropdown.value.ToString();
    }
}
