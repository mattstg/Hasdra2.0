using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvAbBlock : AvatarAbility {


    public override void ConstructAbility(string triggerKey)
    {
        abilityType = GV.AbilityType.block;
        CreateSkillMods();
        activeTriggerKey = triggerKey;
        expToLevel = minExpToPurchase = GV.EXPERIENCE_PER_LEVEL;
        AddTrigger(GV.AbilityTriggerType.Pressed,0);
        AddTrigger(GV.AbilityTriggerType.Held,1);
        AddTrigger(GV.AbilityTriggerType.Released,0);
    }

    public override void InstallAbility(PlayerControlScript _pcs)
    {
        base.InstallAbility(_pcs);
        _pcs.blockInstalled = true;
    }

    public override void TriggerPressed(AbilityResourceRequest resourcesGiven)
    {
        pcs.isBlocking = true;
        AddTrigger(GV.AbilityTriggerType.Held, 1);
        //do animation
    }
    public override void TriggerHeld(AbilityResourceRequest resourcesGiven)
    {
        //Actual block code is handled in pcs, since it has to be, this just handles the animations, and the cost of the block per second held
        if(!pcs.isBlocking)
            RemoveTrigger(GV.AbilityTriggerType.Held); //if block is breaked, should stop costing
    }
    public override void TriggerReleased(AbilityResourceRequest resourcesGiven)
    {
        if (pcs.isBlocking) //may be ended externally if concuss or breaks sheild
        {
            pcs.isBlocking = false;
            //end animation thing,
        }
    }

    public override AbilityResourceRequest GetResourceRequest()
    {
        return new AbilityResourceRequest(0, abilitySkills["ab_block_consumptionRate"] * Time.deltaTime, 0, 0);
    }

    private void CreateSkillMods()
    {
        associatedExistingSkill = new List<string>() {};
        expDistributionAsString = new Dictionary<string, float>() { { "ab_block_consumptionRate", .25f }, { "ab_block_max", .25f }, { "ab_block_efficency", .25f }, { "ab_block_recharge", .25f } };
        associatedSkillModsToCreate = new List<SkillFetus>() 
        { 
            new SkillFetus("ab_block_consumptionRate", GV.Stats.Agi, -.05f, 1.5f,.5f,1.5f), 
            new SkillFetus("ab_block_max", GV.Stats.Const, 4,10),
            new SkillFetus("ab_block_efficency", GV.Stats.Agi, .5f,1),
            new SkillFetus("ab_block_recharge", GV.Stats.Agi, 1,1) //Rate of recharge for shield
        };
    }
	 
}
