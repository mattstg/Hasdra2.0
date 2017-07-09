using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvAbMoveLeft : AvAbMove{
    public override void ConstructAbility(string triggerKey)
    {
        moveRight = false;
        abilityType = GV.AbilityType.moveLeft;
        base.ConstructAbility(triggerKey);
    }
	 
}
