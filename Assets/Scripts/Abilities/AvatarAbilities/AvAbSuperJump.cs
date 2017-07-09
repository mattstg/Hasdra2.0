using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvAbSuperJump : AvatarAbility {
	
	private float forceBanked = 0;

	public override void ConstructAbility(string triggerKey)
	{
		abilityType = GV.AbilityType.superJump;
		CreateSkillMods();
		activeTriggerKey = triggerKey;
		expToLevel = minExpToPurchase = GV.EXPERIENCE_PER_LEVEL;
        AddTrigger(GV.AbilityTriggerType.Pressed);
        AddTrigger(GV.AbilityTriggerType.Held);
        AddTrigger(GV.AbilityTriggerType.Released);
	}

	public override void TriggerPressed(AbilityResourceRequest resourcesGiven)
	{
		pcs.animParaCtrl.setJumpPressed(true);
		pcs.animParaCtrl.setAbilityTrigger ();
		StoreForce(resourcesGiven.stanimaRequested);
	}

	public override void TriggerHeld(AbilityResourceRequest resourcesGiven)
	{
		StoreForce(resourcesGiven.stanimaRequested);
	}

	public override void TriggerReleased(AbilityResourceRequest resourcesGiven)
	{
		pcs.animParaCtrl.setJumpPressed (false);
		StoreForce (resourcesGiven.stanimaRequested);
		//Debug.Log ("isGrounded? " + pcs.GroundedTest());
		if (pcs.GroundedTest()) {
			//pcs.AddForceToAvatar (new Vector2 (0, forceBanked));
            pcs.AddForceToAvatar(new Vector2(0, forceBanked),false);
			//Debug.Log ("Force Bank at fire: " + forceBanked);
			forceBanked = 0;
		} else {
			forceBanked = 0;
            //should kick someone, summon explosion if strong enough, very dense one, (higher hairline raise?)
			//Debug.Log ("Not grounded: " + forceBanked);
		}
	}

	private void StoreForce(float stanima)
	{
        //Debug.Log("stanima consuming " + stanima + " STF " + (abilitySkills["stanimaToForce"].get() + abilitySkills["jumpStaminaToForce"].get()));
		forceBanked += (abilitySkills["jumpStaminaToForce"].get()) * stanima;
		forceBanked = GV.Limit(forceBanked, abilitySkills["jumpMaxForceStored"].get());
        //Debug.Log(string.Format("Force stored {0}/{1}", forceBanked, abilitySkills["jumpMaxForceStored"].get()));

	}

	public override AbilityResourceRequest GetResourceRequest() //this can prob be optimized
	{

        AbilityResourceRequest abrr = ResourceRequestHelper(abilitySkills["jumpMaxForceStored"], forceBanked, abilitySkills["jumpStaminaConsumptionRate"], abilitySkills["jumpStaminaToForce"], GV.AbilityResourceRequestType.stanima);
        //Debug.Log("Requesting : " + abrr + ", per second is: " + (abrr / Time.deltaTime));
        return abrr;
	}

	private void CreateSkillMods()
	{
        associatedExistingSkill = new List<string>() { }; // { "stanimaToForce" };
		expDistributionAsString = new Dictionary<string, float>() {{ "jumpStaminaToForce", .3f }, { "jumpStaminaConsumptionRate", .3f }, { "jumpMaxForceStored", .4f } };
		associatedSkillModsToCreate = new List<SkillFetus>() 
		{ 
			new SkillFetus("jumpStaminaToForce", GV.Stats.Agi, 50f, 300, 25, 50, GV.HorzAsym.MinToMax), 
			new SkillFetus("jumpStaminaConsumptionRate", GV.Stats.Agi, 1, 10, 25, 50, GV.HorzAsym.MinToMax),
			new SkillFetus("jumpMaxForceStored", GV.Stats.Agi, 1000, 3000, 50, 25, GV.HorzAsym.MinToMax)
		};
	}

}
