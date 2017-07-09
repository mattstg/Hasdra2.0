using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvAbInvis : AvatarAbility {

    float invisAlpha = .2f;

    public override void ConstructAbility(string triggerKey)
    {
        toggableKeyEnabled = true;
        abilityType = GV.AbilityType.invis;
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

    public override void ToggleDisabled()
    {
        Debug.Log("Toggle disabled");
        foreach (KeyValuePair<string, Transform> kv in pcs.avatarManager.avatarLimbDict)
        {
            SpriteRenderer sr = kv.Value.GetComponent<SpriteRenderer>();
            if (sr)
            {
                Color color = sr.color;
                color.a = 1f;
                sr.color = color;
            }
        }
    }

    private void Invis(float energyGiven)
    {
        //Debug.Log("e: " + energyGiven + " requested(nondt): " + abilitySkills["ab_invis_energyPerAlphaSec"].getSkillValue() + " requested(dt): " + abilitySkills["ab_invis_energyPerAlphaSec"] * Time.deltaTime);
        float alphaset = invisAlpha;
        if (energyGiven < abilitySkills["ab_invis_energyPerAlphaSec"] * Time.deltaTime)
        {
            float perc = energyGiven / (abilitySkills["ab_invis_energyPerAlphaSec"] * Time.deltaTime);
            alphaset = 1 - ((1 - alphaset) * perc);
            Debug.Log("alpha set is: " + alphaset);
        }

        foreach(KeyValuePair<string,Transform> kv in pcs.avatarManager.avatarLimbDict)
        {
            SpriteRenderer sr = kv.Value.GetComponent<SpriteRenderer>();
            if (sr)
            {
                Color color = sr.color;
                color.a = alphaset;
                sr.color = color;
            }
        }

    }

    public override void OnUpdate(Ability.AbilityResourceRequest resourcesGiven, float dt)
    {
        //Debug.Log("update called");
        Invis(resourcesGiven.energyRequested);
    }


    public override AbilityResourceRequest GetResourceRequest() //this can prob be optimized
    {
        return new AbilityResourceRequest(abilitySkills["ab_invis_energyPerAlphaSec"] * Time.deltaTime, 0, 0, 0);
    }

    private void CreateSkillMods()
    {
        associatedExistingSkill = new List<string>() { }; //none
        expDistributionAsString = new Dictionary<string, float>() {{ "ab_invis_energyPerAlphaSec", 1f }};
        associatedSkillModsToCreate = new List<SkillFetus>() 
        { 
			new SkillFetus("ab_invis_energyPerAlphaSec", GV.Stats.Int, 1, 3, 30, 80, GV.HorzAsym.MaxToMin),
        };
    }
	 
}
