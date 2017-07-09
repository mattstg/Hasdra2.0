using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvAbDash : AvatarAbility {

    //While holding down dash, Stanima is drained and stored in a bank, on releasing, you are launched with the stored force in Newtons.
    //Dash scales with stanimaToForce + dashStanimaToForce
    private float forceBanked = 0;

    public override void ConstructAbility(string triggerKey)
    {
        abilityType = GV.AbilityType.dash;
        CreateSkillMods();
        activeTriggerKey = triggerKey;
        expToLevel = minExpToPurchase = GV.EXPERIENCE_PER_LEVEL;
        AddTrigger(GV.AbilityTriggerType.Pressed);
        AddTrigger(GV.AbilityTriggerType.Held);
        AddTrigger(GV.AbilityTriggerType.Released);

    }

    protected virtual Vector2 GetDashAngle()
    {
        return GV.V2FromAngle(pcs.reticleAngle).normalized;
    }

    public override void TriggerPressed(AbilityResourceRequest resourcesGiven)
    {
        //Debug.Log("Here1");
		pcs.animParaCtrl.setDashPressed(true);
		pcs.animParaCtrl.setDashDir (GetDashAngle(), pcs.facingRight);
		pcs.animParaCtrl.setAbilityTrigger();
        StoreForce(resourcesGiven.stanimaRequested);
		//Debug.Log ("resource given " + resourcesGiven.stanimaRequested);
    }

    public override void TriggerHeld(AbilityResourceRequest resourcesGiven)
    {
       // Debug.Log("Here2");
        
        StoreForce(resourcesGiven.stanimaRequested);
		pcs.animParaCtrl.setDashDir (GetDashAngle(), pcs.facingRight);
		//Debug.Log ("resource given " + resourcesGiven.stanimaRequested);
	}
    public override void TriggerReleased(AbilityResourceRequest resourcesGiven)
    {
		pcs.animParaCtrl.setDashPressed(false);
        StoreForce(resourcesGiven.stanimaRequested);
        //Debug.Log("Trigger Released for dash, releasing " + forceBanked + " force");
		pcs.AddForceToAvatar(GetDashAngle() * forceBanked,false);
		//Debug.Log ("Force Bank at fire: " + forceBanked);
		forceBanked = 0;
    }

    private void StoreForce(float stanima)
    {
		forceBanked += (abilitySkills["dashStanimaToForce"].get()) * stanima;
        forceBanked = GV.Limit(forceBanked, abilitySkills["dashMaxForceStored"].get());
    }

    public override AbilityResourceRequest GetResourceRequest() //this can prob be optimized
    {
		if (abilitySkills.ContainsKey ("dashMaxForceStored")) {
			float maxForce = abilitySkills ["dashMaxForceStored"].get ();
			float stanimaDesired = 0;
			float dt = Time.deltaTime;
			if (forceBanked < maxForce) {
				stanimaDesired = abilitySkills ["dashStanimaConsumptionRate"].get () * dt;
				float forceWouldGain = stanimaDesired * (abilitySkills ["dashStanimaToForce"].get ());

				if (forceWouldGain + forceBanked > maxForce) {
					float forceDesired = maxForce - forceBanked;
					stanimaDesired = forceDesired / (abilitySkills ["dashStanimaToForce"].get ());
				}
			} else {
				forceBanked = maxForce;
			}
        
			return new AbilityResourceRequest (0, stanimaDesired, 0, 0);
		}
		Debug.Log ("out ability skills does not contain key " + "dashMaxForceStored");
		return new AbilityResourceRequest (0, 0, 0, 0);
    }

    private void CreateSkillMods()
    {
        associatedExistingSkill = new List<string>() { };// { "stanimaToForce" };
        expDistributionAsString = new Dictionary<string, float>() {{ "dashStanimaToForce", .3f }, { "dashStanimaConsumptionRate", .3f }, { "dashMaxForceStored", .4f } };
        associatedSkillModsToCreate = new List<SkillFetus>() 
        { 
			new SkillFetus("dashStanimaToForce", GV.Stats.Agi, 7f ,75),
			new SkillFetus("dashStanimaConsumptionRate", GV.Stats.Agi, .2f , 2),
            new SkillFetus("dashMaxForceStored", GV.Stats.Agi, 8f , 300)
        };
    }
	 
}
