using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvAbFly : AvatarAbility {

    public override void ConstructAbility(string triggerKey)
    {
        toggableKeyEnabled = true;
        abilityType = GV.AbilityType.fly;
        CreateSkillMods();
        activeTriggerKey = triggerKey;
        expToLevel = minExpToPurchase = GV.EXPERIENCE_PER_LEVEL;
        AddTrigger(GV.AbilityTriggerType.Pressed);
        AddTrigger(GV.AbilityTriggerType.Held, 0);       //for toggable
        AddTrigger(GV.AbilityTriggerType.Released, 0);   //for toggable
        //AddTrigger(GV.AbilityTriggerType.Update, 1);   //for toggable
    }

    public override void TriggerHeld(AbilityResourceRequest resourcesGiven)
    {
        base.TriggerHeld(resourcesGiven); //for toggable
		
	}
    public override void TriggerReleased(AbilityResourceRequest resourcesGiven)
    {
        base.TriggerReleased(resourcesGiven); //for toggable
    }

    private void Fly(float energyGiven)
    {
        //energyGiven = 10000;
        //all excess energy atm is wasted
        pcs.animParaCtrl.setDashDir(GV.V2FromAngle(pcs.reticleAngle).normalized, pcs.facingRight);
        Vector2 oppForceNeeded  = pcs.currentVelo * -1f * pcs.getMassOfBody();
        Vector2 forceApplying = new Vector2();
        float forceCanProvide = energyGiven * (abilitySkills["energyPerForce"] + abilitySkills["ab_fly_energyPerForce"]);
        //first tries to maintain y, then x
        if(Mathf.Abs(oppForceNeeded.y) <= forceCanProvide)
        {
            forceCanProvide -= Mathf.Abs(oppForceNeeded.y);
            forceApplying = new Vector2(0, oppForceNeeded.y);
            if (Mathf.Abs(oppForceNeeded.x) <= forceCanProvide)
            {
                forceApplying += new Vector2(oppForceNeeded.x, 0);
            }
            else
            {
                forceApplying += new Vector2(forceCanProvide * Mathf.Sign(oppForceNeeded.x), 0);
            }
        }
        else
        {
            forceApplying = new Vector2(0, forceCanProvide * Mathf.Sign(oppForceNeeded.y));
        }

        //Vector2 oppVelo = pcs.currentVelo.normalized * -1 * energyGiven * (abilitySkills["energyPerForce"] + abilitySkills["ab_fly_energyPerForce"]);
        pcs.AddForceToAvatar(forceApplying, false);
        //Debug.Log("flap flap at: " + oppVelo);
    }

    public override void OnUpdate(Ability.AbilityResourceRequest resourcesGiven, float dt)
    {
        //Debug.Log("update called");
        Fly(resourcesGiven.energyRequested);
    }


    public override AbilityResourceRequest GetResourceRequest() //this can prob be optimized
    {
        Vector2 oppVelo = pcs.currentVelo * -1;
        float energyReq = Mathf.Abs(oppVelo.magnitude * pcs.getMassOfBody()) / (abilitySkills["energyPerForce"] + abilitySkills["ab_fly_energyPerForce"]);

        AbilityResourceRequest abrr = ResourceRequestHelper(energyReq, 0, abilitySkills["ab_fly_comsumptionRate"], 1, GV.AbilityResourceRequestType.energy);  //ya no idea if this is correct
        return abrr;
        //return new AbilityResourceRequest(abilitySkills["ab_fly_comsumptionRate"],0,0,0);
    }

    private void CreateSkillMods()
    {
        associatedExistingSkill = new List<string>() { "energyPerForce" };
        expDistributionAsString = new Dictionary<string, float>() { { "ab_fly_energyPerForce", .4f }, { "ab_fly_comsumptionRate", .3f } };
        associatedSkillModsToCreate = new List<SkillFetus>() 
        { 
            new SkillFetus("ab_fly_comsumptionRate", GV.Stats.Wis, 1, 2, 30, 80, GV.HorzAsym.MinToMax),
			new SkillFetus("ab_fly_energyPerForce", GV.Stats.Wis, 10, 50, 50, 90, GV.HorzAsym.MinToMax),
        };
    }
	 
}
