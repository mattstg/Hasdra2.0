using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SIUISCToggle : SIBasicVarSlotUI
{
    public Transform togglesParent;
    List<Toggle> toggles;

    public override void Initialize()
    {
        toggles = new List<Toggle>();
        foreach(Transform t in togglesParent)
            toggles.Add(t.GetComponent<Toggle>());
        base.Initialize();
    }


    public override string GetValue()
    {
        return GetCheckedToggles();
    }

    private string GetCheckedToggles()
    {
        List<string> toOut = new List<string>();
        foreach(Toggle t in toggles)
            if (t.isOn)
            toOut.Add(t.name);
        return string.Join(",", toOut.ToArray());
    }
}
