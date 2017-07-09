using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvAbMoveRight : AvAbMove{
    public override void ConstructAbility(string triggerKey)
    {
        moveRight = true;
        abilityType = GV.AbilityType.moveRight;
        base.ConstructAbility(triggerKey);
    }
}
