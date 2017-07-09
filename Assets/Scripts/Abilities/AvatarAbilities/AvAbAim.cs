using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvAbAim : AvatarAbility {

    protected bool aimUp;

    public override void ConstructAbility(string triggerKey)
    {
        CreateSkills();
        activeTriggerKey = triggerKey;
        expToLevel = minExpToPurchase = GV.EXPERIENCE_PER_LEVEL;
        AddTrigger(GV.AbilityTriggerType.Update,0);
        AddTrigger(GV.AbilityTriggerType.Held);
    }

    public override void TriggerHeld(AbilityResourceRequest resourcesGiven)
    {
        int moveMod = (aimUp)?1:-1;
        pcs.headAngle += (moveMod * abilitySkills["ab_aim_speed"] * Time.deltaTime);
        float maxAngle = abilitySkills["ab_aim_MaxAngle"];
        pcs.headAngle = Mathf.Min(maxAngle, Mathf.Abs(pcs.headAngle)) * Mathf.Sign(pcs.headAngle);
	}

    public override void OnUpdate(Ability.AbilityResourceRequest resourcesGiven, float dt)  //copy pasted from player
    {
        pcs.head = new Vector2((float)Mathf.Cos(pcs.headAngle * Mathf.PI / 180f) * pcs.reticleDirection, (float)Mathf.Sin(pcs.headAngle * Mathf.PI / 180f));
        pcs.reticleAngle = (!pcs.facingRight) ? 180 - pcs.headAngle : pcs.headAngle;
        //reticleSprite.transform.position = (head + new Vector2(transform.position.x, transform.position.y)) * reticleDistance;
        pcs.reticleSprite.transform.localPosition = pcs.head * GV.RETICLE_DISTANCE;
        if (!pcs.facingRight)
        {
            pcs.reticleSprite.transform.localPosition = new Vector3(-pcs.reticleSprite.transform.localPosition.x, pcs.reticleSprite.transform.localPosition.y, pcs.reticleSprite.transform.localPosition.z);
        }
        if (pcs.facingRight)
            pcs.animParaCtrl.setAim(pcs.reticleAngle / 90);
        else
            pcs.animParaCtrl.setAim(-(pcs.reticleAngle - 180)/ 90);
    } 
    
    public override AbilityResourceRequest GetResourceRequest() //Its free
    {
        return new AbilityResourceRequest();
    }

    private void CreateSkills()
    {
        associatedExistingSkill = new List<string>() { };
        expDistributionAsString = new Dictionary<string, float>() { { "ab_aim_MaxAngle", .3f }, {"ab_aim_speed", .7f} };
        associatedSkillModsToCreate = new List<SkillFetus>() 
        { 
			new SkillFetus("ab_aim_MaxAngle", GV.Stats.Agi, 70, 90, 20, 95, GV.HorzAsym.MinToMax), 
			new SkillFetus("ab_aim_speed", GV.Stats.Agi, 37, 90, 20, 95, GV.HorzAsym.MinToMax),
        };
    }
}
