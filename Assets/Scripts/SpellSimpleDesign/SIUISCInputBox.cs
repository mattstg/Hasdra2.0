using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SIUISCInputBox : SIBasicVarSlotUI
{
    public InputField textBox;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override string GetValue()
    {
        return textBox.text;
    }

}
