using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ducBodyStats : DebugUIComponent {

    public override void Initialize(string _title, DucType _ducType)
    {
        base.Initialize(_title, _ducType);
        AddStatSlots(10);        
    }

    public override void Update()
    {
        foreach (StatSlotUI ssui in statSlots)
        {
            if (ssui.titleText.text != "")
            {
                PlayerControlScript pcs = GV.worldUI.players[0];
                if (pcs.stats.ContainsSkill(ssui.titleText.text))
                {
                    ssui.valueText.text = pcs.stats.getSkillValue(ssui.titleText.text).ToString();
                }
                else
                {
                    Debug.Log("skill " + ssui.titleText.text + " DNE");
                }
            }
        }
    }
}
