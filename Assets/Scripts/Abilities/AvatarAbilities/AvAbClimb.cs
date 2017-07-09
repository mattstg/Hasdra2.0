using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvAbClimb : AvatarAbility {
	public override void ConstructAbility(string triggerKey)
	{
		abilityType = GV.AbilityType.climb;
		CreateSkillMods();
		activeTriggerKey = triggerKey;
		expToLevel = minExpToPurchase = GV.EXPERIENCE_PER_LEVEL;
        AddTrigger(GV.AbilityTriggerType.Pressed);
        AddTrigger(GV.AbilityTriggerType.Held);
        AddTrigger(GV.AbilityTriggerType.Released,0);
	}

	private void ApplyForce(float stamina){
		//forceToApply = abilitySkills ["staminaToForce"].get () + abilitySkills ["climbStaminaToForce"].get () * stamina;
		Debug.Log("stamina give " + stamina);
		float forceToApply = (abilitySkills["climbStaminaToForce"].get()) * stamina;
		pcs.AddForceToAvatar (new Vector2(0,forceToApply),false);
		Debug.Log ("Force Added " + forceToApply );
	}

	public override void TriggerPressed(AbilityResourceRequest resourcesGiven)
	{
		if (pcs.GroundedTest (pcs.climbZoneRight)) {
			pcs.animParaCtrl.setAbilityTrigger ();
			pcs.animParaCtrl.setClimbPressed(true);
			pcs.animParaCtrl.setClimbSpeed (getPCSVelo ());
			ApplyForce (resourcesGiven.stanimaRequested);
		}
	}

	public override void TriggerHeld(AbilityResourceRequest resourcesGiven)
	{
		if (pcs.GroundedTest (pcs.climbZoneRight)) {
			pcs.animParaCtrl.setClimbSpeed (getPCSVelo ());
			ApplyForce (resourcesGiven.stanimaRequested);
		} else {
			pcs.animParaCtrl.setClimbPressed(false);
		}
	}

	public override void TriggerReleased(AbilityResourceRequest resourcesGiven)
	{
		pcs.animParaCtrl.setClimbPressed(false);
	}

	public float getPCSVelo(){
		return pcs.GetComponent<Rigidbody2D> ().velocity.y;
	}

	public override AbilityResourceRequest GetResourceRequest() //this can prob be optimized
	{
		float maxForce = abilitySkills ["climbMaxForce"].get ();
		float yVelo = getPCSVelo ();
		//Debug.Log ("y velocity " + yVelo);
		//Debug.Log ("max climb Speed " + maxClimbSpeed);

		float staminaDesired = 0;
		float dt = Time.deltaTime;
		staminaDesired = abilitySkills ["climbStaminaConsumptionRate"].get () * dt;
		//Debug.Log ("staminaToForce = " + abilitySkills ["stanimaToForce"].get () + " climbStaminaToForce = " + abilitySkills ["climbStaminaToForce"].get ());
		float forceWouldGain = staminaDesired * (abilitySkills ["climbStaminaToForce"].get ());
		if (forceWouldGain > maxForce) {
			staminaDesired = maxForce / (abilitySkills ["climbStaminaToForce"].get ());
		} 
		return new AbilityResourceRequest (0, staminaDesired, 0, 0);
	}

	private void CreateSkillMods()
	{
        associatedExistingSkill = new List<string>() { };// { "stanimaToForce" };
		expDistributionAsString = new Dictionary<string, float>() { { "climbStaminaToForce", .3f }, { "climbStaminaConsumptionRate", .3f }, { "climbMaxForce", .4f } };
		associatedSkillModsToCreate = new List<SkillFetus>() 
		{ 
			new SkillFetus("climbStaminaToForce", GV.Stats.Str, 5, 10, 50, 50, GV.HorzAsym.MinToMax), 
			new SkillFetus("climbStaminaConsumptionRate", GV.Stats.Agi, 25, 100, 50, 50, GV.HorzAsym.MinToMax),
			new SkillFetus("climbMaxForce", GV.Stats.Agi, 20, 100, 50, 50, GV.HorzAsym.MinToMax)
		};
	}
}
