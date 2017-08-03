using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SIUISCBool : SIBasicVarSlotUI
{
    public Toggle toggle;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override string GetValue()
    {
        return toggle.isOn.ToString();
    }
}
