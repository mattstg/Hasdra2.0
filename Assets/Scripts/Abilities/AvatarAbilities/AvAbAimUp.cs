using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvAbAimUp : AvAbAim{
    public override void ConstructAbility(string triggerKey)
    {
        aimUp = true;
        abilityType = GV.AbilityType.aimUp;
        base.ConstructAbility(triggerKey);
    }
	 
}
