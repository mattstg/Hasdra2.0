using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvAbAimDown : AvAbAim{
    public override void ConstructAbility(string triggerKey)
    {
        aimUp = false;
        abilityType = GV.AbilityType.aimDown;
        base.ConstructAbility(triggerKey);
    }
}
