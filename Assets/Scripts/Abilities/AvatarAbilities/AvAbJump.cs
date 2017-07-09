using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvAbJump : AvatarAbility {
	
	private float forceBanked = 0;
    private float timeHeldDown = 0;
    private bool isJumping = false;
    private float maxTimeForJump = .5f; //could be gv or stat

	public override void ConstructAbility(string triggerKey)
	{
		abilityType = GV.AbilityType.jump;
		CreateSkillMods();
		activeTriggerKey = triggerKey;
		expToLevel = minExpToPurchase = GV.EXPERIENCE_PER_LEVEL;
        AddTrigger(GV.AbilityTriggerType.Pressed, 0);
        AddTrigger(GV.AbilityTriggerType.Held);
        AddTrigger(GV.AbilityTriggerType.Released, 0);
	}

	public override void TriggerPressed(AbilityResourceRequest resourcesGiven)
	{
        if (pcs.GroundedTest())
        {
            pcs.animParaCtrl.setJumpTrigger();
            pcs.animParaCtrl.setAbilityTrigger();
            isJumping = true;
        }
	}

	public override void TriggerHeld(AbilityResourceRequest resourcesGiven)
	{
        if (isJumping == true)
        {
            float forceToAdd = (resourcesGiven.stanimaRequested * abilitySkills["jumpStaminaToForce"]) / maxTimeForJump;
            pcs.AddForceToAvatar(new Vector2(0, forceToAdd), false); //abilitySkills["stanimaToForce"]
            timeHeldDown += Time.deltaTime;
            if (timeHeldDown >= maxTimeForJump)
            {
                isJumping = false;
                AddTrigger(GV.AbilityTriggerType.Held,0); //doesnt consume anymore
            }
        }
	}

	public override void TriggerReleased(AbilityResourceRequest resourcesGiven)
	{
        timeHeldDown = 0;
        isJumping = false;
        AddTrigger(GV.AbilityTriggerType.Held,1); //holding would consume again
    }

	public override AbilityResourceRequest GetResourceRequest() //this can prob be optimized
	{

        //AbilityResourceRequest abrr = ResourceRequestHelper(abilitySkills["jumpMaxForceStored"], forceBanked, abilitySkills["jumpStaminaConsumptionRate"], abilitySkills["jumpStaminaToForce"] + abilitySkills["stanimaToForce"], GV.AbilityResourceRequestType.stanima);
        AbilityResourceRequest abrr = new AbilityResourceRequest(0, abilitySkills["jumpStaminaConsumptionRate"] * Time.deltaTime, 0, 0);
        //Debug.Log("Requesting : " + abrr + ", per second is: " + (abrr / Time.deltaTime));
        return abrr;
	}

	private void CreateSkillMods()
	{
		associatedExistingSkill = new List<string>() { }; // { "stanimaToForce" }; two layers hard to balance
		expDistributionAsString = new Dictionary<string, float>() { { "jumpStaminaToForce", .7f }, { "jumpStaminaConsumptionRate", .3f }};
        associatedSkillModsToCreate = new List<SkillFetus>()
        {
            new SkillFetus("jumpStaminaToForce", GV.Stats.Agi, 2, 100), //is total force that would be applied over "maxTimeForJump" seconds
			new SkillFetus("jumpStaminaConsumptionRate", GV.Stats.Agi, 2, 4, 25, 50, GV.HorzAsym.MinToMax)
		};
	}
    
}
