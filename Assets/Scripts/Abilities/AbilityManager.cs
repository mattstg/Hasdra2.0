using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class AbilityManager 
{
    PlayerControlScript pcs;
    //Dictionary<string, Ability> abilities = new Dictionary<string, Ability>();
    Dictionary<KeyCode, Ability> abilityMap = new Dictionary<KeyCode, Ability>();

    public AbilityManager(PlayerControlScript _pcs)
    {
        pcs = _pcs;
    }

    public List<KeyValuePair<KeyCode,Ability>> GetTriggableKeycodes()
    {
        return abilityMap.ToList();
    }
    /* Here once stood a gaint
    public void FinializeAbilityInput()
    {
        if (abilitiesToUpdate.Count == 0)
            return;
        //do all
        //Debug.Log("does all the calculations for spreading resources and calling abilities here, amt to update: " + abilitiesToUpdate.Count);
        Ability.AbilityResourceRequest totalResourceRequest = new Ability.AbilityResourceRequest(0,0,0,0);  //Get the players actual available, then split it depending on limits placed on by bodystats
        Ability.AbilityResourceRequest resourcesAvailable = new Ability.AbilityResourceRequest(pcs.stats.getSkillValue("maxEnergyTransferToAbilities"), pcs.stats.getSkillValue("maxStanimaUse"), pcs.stats.healthPoints, pcs.levelUpManager.experienceStored) * Time.deltaTime;
        bool partialEnergy, partialStanima, partialHp, partialExp;

        foreach (KeyValuePair<Ability, List<GV.AbilityTriggerType>> kv in abilitiesToUpdate) //sum up all ability costs
            foreach (GV.AbilityTriggerType triggerType in kv.Value)
            {
                float rezCostMult = kv.Key.ResourceRequestMultiplier(triggerType);
                if(rezCostMult > 0)
                    totalResourceRequest += (kv.Key.GetResourceRequest() * rezCostMult);
            }
                

        partialEnergy   = (totalResourceRequest.energyRequested  > resourcesAvailable.energyRequested);
        partialStanima  = (totalResourceRequest.stanimaRequested > resourcesAvailable.stanimaRequested);
        partialHp       = (totalResourceRequest.hpRequested      > resourcesAvailable.hpRequested);
        partialExp      = (totalResourceRequest.expRequested     > resourcesAvailable.expRequested);

        foreach (KeyValuePair<Ability, List<GV.AbilityTriggerType>> kv in abilitiesToUpdate)
        {
            foreach (GV.AbilityTriggerType triggerType in kv.Value)
            {
                Ability.AbilityResourceRequest resourceRequest;
                float rezCostMult = kv.Key.ResourceRequestMultiplier(triggerType);
                if (rezCostMult > 0)
                    resourceRequest = kv.Key.GetResourceRequest() * rezCostMult;
                else
                    resourceRequest = new Ability.AbilityResourceRequest();

                if (partialEnergy)
                    resourceRequest.energyRequested = ((resourceRequest.energyRequested / totalResourceRequest.energyRequested) * resourcesAvailable.energyRequested);
                if (partialStanima)
                    resourceRequest.stanimaRequested = ((resourceRequest.stanimaRequested / totalResourceRequest.stanimaRequested) * resourcesAvailable.stanimaRequested);
                if (partialHp)
                    resourceRequest.hpRequested = ((resourceRequest.hpRequested / totalResourceRequest.hpRequested) * resourcesAvailable.hpRequested);
                if (partialExp)
                    resourceRequest.expRequested = ((resourceRequest.expRequested / totalResourceRequest.expRequested) * resourcesAvailable.expRequested);

                switch (triggerType)
                {
                    case GV.AbilityTriggerType.Held:
                        kv.Key.TriggerHeld(resourceRequest);
                        break;
                    case GV.AbilityTriggerType.Pressed:
                        kv.Key.TriggerPressed(resourceRequest);
                        break;
                    case GV.AbilityTriggerType.Released:
                        kv.Key.TriggerReleased(resourceRequest);
                        break;
                    case GV.AbilityTriggerType.Update:
                        kv.Key.OnUpdate(resourceRequest, Time.deltaTime);
                        break;
                    default:
                        Debug.LogError("switch fail: <" + kv.Key + "," + kv.Value + ">");
                        break;
                }
            }
        }
        abilitiesToUpdate.Clear();
    }
    */

    public void UpdateAbility(Ability abilityUpdating, GV.AbilityTriggerType keyState)
    {
        //Ability.AbilityResourceRequest resourcesAvailable = new Ability.AbilityResourceRequest(pcs.stats.getSkillValue("maxEnergyTransferToAbilities"), pcs.stats.getSkillValue("maxStanimaUse"), pcs.stats.healthPoints, pcs.levelUpManager.experienceStored) * Time.deltaTime;
        Ability.AbilityResourceRequest resourcesAvailable= new Ability.AbilityResourceRequest(pcs.stats.energy,pcs.stats.stamina,pcs.stats.healthPoints, pcs.levelUpManager.experienceStored);
        Ability.AbilityResourceRequest resourceRequest;
        float rezCostMult = abilityUpdating.ResourceRequestMultiplier(keyState);
        if (rezCostMult > 0)
            resourceRequest = abilityUpdating.GetResourceRequest() * rezCostMult;
        else
            resourceRequest = new Ability.AbilityResourceRequest();


        Ability.AbilityResourceRequest resourcesGiving;
        if (resourceRequest > resourcesAvailable)
        {
            resourcesGiving = new Ability.AbilityResourceRequest(Mathf.Min(resourceRequest.energyRequested, resourcesAvailable.energyRequested), Mathf.Min(resourceRequest.stanimaRequested, resourcesAvailable.stanimaRequested), Mathf.Min(resourceRequest.hpRequested, resourcesAvailable.hpRequested), Mathf.Min(resourceRequest.expRequested, resourcesAvailable.expRequested));
        }
        else
        {
            resourcesGiving = resourceRequest;
        }

        switch (keyState)
        {
            case GV.AbilityTriggerType.Held:
                abilityUpdating.TriggerHeld(resourcesGiving);
                break;
            case GV.AbilityTriggerType.Pressed:
                abilityUpdating.TriggerPressed(resourcesGiving);
                break;
            case GV.AbilityTriggerType.Released:
                abilityUpdating.TriggerReleased(resourcesGiving);
                break;
            case GV.AbilityTriggerType.Update:
                abilityUpdating.OnUpdate(resourcesGiving, Time.deltaTime);
                break;
        }

        pcs.stats.energy -= resourcesGiving.energyRequested;
        pcs.stats.hp -= resourcesGiving.hpRequested;
        pcs.stats.stamina -= resourcesGiving.stanimaRequested;
        pcs.levelUpManager.experienceStored -= resourcesGiving.expRequested;
    }

     public void LearnOrLevelAbility(string abilityName, string keycode, float exp)
    {
        //Learn
        if (abilityName.Contains("Ab:"))
            abilityName = abilityName.Replace("Ab:", "");
        GV.AbilityType abilityType = GV.ParseEnum<GV.AbilityType>(abilityName);
        LearnOrLevelAbility(abilityType, keycode, exp);
    }

    public void LearnOrLevelAbility(GV.AbilityType abilityTypeLearning, string keycodeAsString, float exp)
    {
        //assume at this point, enough exp to level the ability was given, that will be in statemachine later anyways. account for interupting too?
        KeyCode keycode = GV.ParseEnum<KeyCode>(keycodeAsString.ToUpper());
        if (abilityMap.ContainsKey(keycode))
        {
            if (abilityMap[keycode].abilityType == abilityTypeLearning)
            {
                abilityMap[keycode].LevelAbility(exp); //already exist, straight up leveling
            }
            else
            {  //Another one exists, unistall that one, learn the new one
                abilityMap[keycode].UnistallAbility();
                abilityMap[keycode] = AbilityDict.Instance.GetAbilityByType(abilityTypeLearning);
                abilityMap[keycode].ConstructAbility(keycode.ToString());
                abilityMap[keycode].InstallAbility(pcs);
                exp -= abilityMap[keycode].expToLevel;
                exp = Mathf.Max(0, exp); //no check yet, remove when above is solved
                abilityMap[keycode].LevelAbility(exp);
            }
        }
        else
        {   //First time key is learnt, learn ability
            abilityMap.Add(keycode, AbilityDict.Instance.GetAbilityByType(abilityTypeLearning));
            abilityMap[keycode].ConstructAbility(keycode.ToString());
            abilityMap[keycode].InstallAbility(pcs);
            exp -= abilityMap[keycode].expToLevel;
            exp = Mathf.Max(0, exp); //no check yet, remove when above is solved
            abilityMap[keycode].LevelAbility(exp);
        }
    }
    
}

	
