using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvAbZoom : AvatarAbility {

    //While holding down dash, Stanima is drained and stored in a bank, on releasing, you are launched with the stored force in Newtons.
    //Dash scales with stanimaToForce + dashStanimaToForce
    private float currentZoomMod = 0;

    public override void ConstructAbility(string triggerKey)
    {
        abilityType = GV.AbilityType.zoom;
        CreateSkillMods();
        activeTriggerKey = triggerKey;
        expToLevel = minExpToPurchase = GV.EXPERIENCE_PER_LEVEL;
        AddTrigger(GV.AbilityTriggerType.Pressed);
        AddTrigger(GV.AbilityTriggerType.Held);
        AddTrigger(GV.AbilityTriggerType.Released);
    }

    public override void TriggerPressed(AbilityResourceRequest resourcesGiven)
    {
       RemoveTrigger(GV.AbilityTriggerType.Update);
       ModZoom(resourcesGiven.energyRequested);
    }
    public override void TriggerHeld(AbilityResourceRequest resourcesGiven)
    {
        ModZoom(resourcesGiven.energyRequested);
    }
    public override void TriggerReleased(AbilityResourceRequest resourcesGiven)
    {
        ModZoom(resourcesGiven.energyRequested);
        AddTrigger(GV.AbilityTriggerType.Update,0);
    }
    public override void OnUpdate(Ability.AbilityResourceRequest resourcesGiven, float dt)
    {
        if (currentZoomMod <= 0)
        {
            RemoveTrigger(GV.AbilityTriggerType.Update);
        }
        else
        {
            float zoomRecover = Mathf.Min(currentZoomMod, abilitySkills["ab_zoom_zoomPerMana"] * abilitySkills["ab_zoom_consumptionRate"] * dt);
            pcs.cameraControl.ModZoom(-1 * zoomRecover);
            currentZoomMod -= zoomRecover;
        }
    }

    public override void UnistallAbility()
    {
        pcs.cameraControl.ModZoom(currentZoomMod * -1);
    }

    private void ModZoom(float manaSpent)
    {
       float zoomDelta = manaSpent * abilitySkills["ab_zoom_zoomPerMana"];
       pcs.cameraControl.ModZoom(zoomDelta);
       currentZoomMod += zoomDelta;
    }

    public override AbilityResourceRequest GetResourceRequest() //this can prob be optimized
    {
        AbilityResourceRequest abrr = ResourceRequestHelper(abilitySkills["b_zoom_MaxMod"], currentZoomMod, abilitySkills["ab_zoom_consumptionRate"], abilitySkills["ab_zoom_zoomPerMana"], GV.AbilityResourceRequestType.energy);
        return abrr;
    }

    private void CreateSkillMods()
    {
        associatedExistingSkill = new List<string>() {};
        expDistributionAsString = new Dictionary<string, float>() { { "b_zoom_MaxMod", .4f }, { "ab_zoom_consumptionRate", .3f }, { "ab_zoom_zoomPerMana", .3f } };
        associatedSkillModsToCreate = new List<SkillFetus>() 
        { 
            new SkillFetus("b_zoom_MaxMod", GV.Stats.Int, 4f, 40, 40, 90, GV.HorzAsym.MinToMax), 
            new SkillFetus("ab_zoom_consumptionRate", GV.Stats.Int, 1f, 20, 40, 90, GV.HorzAsym.MinToMax),
            new SkillFetus("ab_zoom_zoomPerMana", GV.Stats.Int, .3f, 4, 40, 90, GV.HorzAsym.MinToMax),
        };
    }
	 
}
