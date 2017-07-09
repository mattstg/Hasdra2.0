using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvAbMove : AvatarAbility {

    protected bool moveRight;

    public override void ConstructAbility(string triggerKey)
    {
        CreateSkills();
        activeTriggerKey = triggerKey;
        expToLevel = minExpToPurchase = GV.EXPERIENCE_PER_LEVEL;
        AddTrigger(GV.AbilityTriggerType.Held);
    }

    public override void TriggerHeld(AbilityResourceRequest resourcesGiven)
    {
        int moveMod = (moveRight)?1:-1;
        if (pcs.facingRight && moveMod == -1 || !pcs.facingRight && moveMod == 1)
            pcs.Flip();

        float stanimaToForce = abilitySkills["runStanimaToForce"];
        pcs.AddForceToAvatar(new Vector2(resourcesGiven.stanimaRequested * stanimaToForce * moveMod, 0),false);
		//pcs.animParaCtrl.setDashDir (GV.V2FromAngle (pcs.reticleAngle).normalized,pcs.facingRight);
        pcs.contrlMove = moveMod;
	}
    
    public override AbilityResourceRequest GetResourceRequest() //this can prob be optimized
    {
        AbilityResourceRequest abrr = ResourceRequestHelper(abilitySkills["maxRunForce"], pcs.currentVelo.x * pcs.getMassOfBody(), abilitySkills["runStanimaConsumption"], abilitySkills["runStanimaToForce"], GV.AbilityResourceRequestType.stanima);
        return abrr;
    }

    private void CreateSkills()
    {
        associatedExistingSkill = new List<string>() { };// { "stanimaToForce" };
        expDistributionAsString = new Dictionary<string, float>() { { "runStanimaConsumption", .3f }, { "maxRunForce", .3f }, { "runStanimaToForce", .4f } };
        associatedSkillModsToCreate = new List<SkillFetus>() 
        { 
			new SkillFetus("runStanimaConsumption", GV.Stats.Agi, 2, 10, 50, 50, GV.HorzAsym.MinToMax), 
			new SkillFetus("maxRunForce", GV.Stats.Agi, 400, 1000, 50, 50, GV.HorzAsym.MinToMax),
			new SkillFetus("runStanimaToForce", GV.Stats.Agi, 125, 500, 50, 50, GV.HorzAsym.MinToMax)   //(200f, 280f, 1000, GV.HorzAsym.MinToMax);
        };
    }
	 
}
