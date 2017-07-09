using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvAbBackDash : AvAbDash {

	//While holding down dash, Stanima is drained and stored in a bank, on releasing, you are launched with the stored force in Newtons.
	//Dash scales with stanimaToForce + dashStanimaToForce
	private float forceBanked = 0;

	public override void ConstructAbility(string triggerKey)
	{
		abilityType = GV.AbilityType.backDash;
        base.ConstructAbility(triggerKey);
	}

    protected override Vector2 GetDashAngle()
    {
        return GV.V2FromAngle(pcs.reticleAngle + 180).normalized;
    }
}

