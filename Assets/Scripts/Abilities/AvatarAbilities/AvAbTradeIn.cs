using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AvAbTradeIn : AvatarAbility
{

    float currentTradeInTime = 0;

    public override void ConstructAbility(string triggerKey)
    {
        CreateSkills();
        abilityType = GV.AbilityType.tradeIn;
        activeTriggerKey = triggerKey;
        expToLevel = minExpToPurchase = GV.EXPERIENCE_PER_LEVEL;
        AddTrigger(GV.AbilityTriggerType.Held);
        AddTrigger(GV.AbilityTriggerType.Released);
    }

    public override void TriggerHeld(AbilityResourceRequest resourcesGiven)
    {
        pcs.levelUpManager.TradeInExp(resourcesGiven.expRequested);
        currentTradeInTime += Time.deltaTime;  //whoa how does delta time even work
            /*if (!animationsActive[0])
                ActivateAnimations(0);
            if (!animationsActive[1] && tradeInRate >= GV.ACTIVATE_TIER_1_POWER_ANIMATION_CONSUMPTION_RATE)
                ActivateAnimations(1);
            if (!animationsActive[2] && tradeInRate >= GV.ACTIVATE_TIER_2_POWER_ANIMATION_CONSUMPTION_RATE)
                ActivateAnimations(2);
            if (!animationsActive[3] && tradeInRate >= GV.ACTIVATE_TIER_3_POWER_ANIMATION_CONSUMPTION_RATE)
                ActivateAnimations(3);*/
    }

    public override void TriggerReleased(AbilityResourceRequest resourcesGiven)
    {
        pcs.levelUpManager.StopTradeIn();
            //experienceStored += experienceTradedIn;
            //experienceTradedIn = 0;
            //StopAllAnimations();
    }

    public override AbilityResourceRequest GetResourceRequest() //this can prob be optimized
    {
        return new AbilityResourceRequest(0, 0, 0, (abilitySkills["tradeInBaseRate"] + currentTradeInTime * abilitySkills["tradeInExpRate"]) * Time.deltaTime);
    }

    private void CreateSkills()
    {
        associatedExistingSkill = new List<string>() {};
        expDistributionAsString = new Dictionary<string, float>() { { "tradeInExpRate", .5f }, { "tradeInBaseRate", .5f } };
        associatedSkillModsToCreate = new List<SkillFetus>() 
        { 
			new SkillFetus("tradeInExpRate",  GV.Stats.Int,  1,   8, 50, 50, GV.HorzAsym.MinToMax), 
			new SkillFetus("tradeInBaseRate", GV.Stats.Int,  1,  20, 50, 50, GV.HorzAsym.MinToMax),
        };
    }
}
