using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ability  {

   protected PlayerControlScript pcs;
   protected Dictionary<string, float> expDistributionAsString; //will replace with above? unsure yet
   public Dictionary<GV.AbilityTriggerType, float> abilityTriggers = new Dictionary<GV.AbilityTriggerType, float>();
   
   
   public string abilityName { get { return abilityType.ToString(); } }
   public GV.AbilityType abilityType;
   protected List<string> associatedExistingSkill = new List<string>();
   protected List<SkillFetus> associatedSkillModsToCreate = new List<SkillFetus>();  //total list of names, in case some are... not existing yet due to creation
   protected Dictionary<string, Skill> abilitySkills = new Dictionary<string, Skill>(); //used for shortcut, should also exist in pcs.stats.getSkillValue("tradeInBaseRate")
   protected List<KeyCode> keycodes = new List<KeyCode>();

   protected float minExpToPurchase; //replace with needs to be lvl 1
   public float expToLevel;

   public bool toggableKeyEnabled = false;  //if on don't manually mess with OnUpdate trigger, also require TriggerHeld,Released active, must also call Base() for these
   private float toggableKeyHeldDown = 0;
   public float toggableResourcePrice = 1; //for onupdate
   protected string activeTriggerKey;

   protected AbilityResourceRequest resourceRequest;


    public Ability()
    {
       
    }

    public void AddKeyCode(KeyCode keyTrigger)
    {
        if (!keycodes.Contains(keyTrigger))
            keycodes.Add(keyTrigger);
    }

    public bool RemoveKeyCode(KeyCode keyTrigger)
    {
        if (!keycodes.Contains(keyTrigger))
            keycodes.Remove(keyTrigger);
        if (keycodes.Count <= 0)
        {
            UnistallAbility();
            return true;
        }
        return false;
    }

    public virtual void ConstructAbility(string triggerKey)
    {

    }

    public virtual void TriggerPressed(AbilityResourceRequest resourcesGiven)
    {

    }
    public virtual void TriggerHeld(AbilityResourceRequest resourcesGiven)
    {
        if (toggableKeyEnabled)
            toggableKeyHeldDown += Time.deltaTime;
    }
    public virtual void TriggerReleased(AbilityResourceRequest resourcesGiven)
    {
        if (toggableKeyEnabled)
        {
            if (toggableKeyHeldDown < GV.ABILITY_TIME_HELD_TILL_KEY_LOCK)
            {
                if (abilityTriggers.ContainsKey(GV.AbilityTriggerType.Update))
                {
                    RemoveTrigger(GV.AbilityTriggerType.Update);
                    ToggleDisabled();
                }
                else
                    AddTrigger(GV.AbilityTriggerType.Update, toggableResourcePrice);
            }
            else
            {
                RemoveTrigger(GV.AbilityTriggerType.Update);
                ToggleDisabled();
            }

            toggableKeyHeldDown = 0;
        }

    }

    public virtual void ToggleDisabled()
    {
    }

    public virtual void OnUpdate(AbilityResourceRequest resourcesGiven, float dt)
    {
    
    }

    public virtual void UnistallAbility()
    {
    }

    public virtual void InstallAbility(PlayerControlScript _pcs)
    {
        //Install the abilities bodystat stuff here
        pcs = _pcs;
        BodyStats bodyStats = pcs.stats;
        foreach (string reqSkillMod in associatedExistingSkill)
        {
            if (bodyStats.ContainsSkill(reqSkillMod))
            {
                abilitySkills.Add(reqSkillMod, bodyStats.Skills[reqSkillMod]);
            }
            else
            {
                Debug.LogError("Skill: " + reqSkillMod + " was labeled as required skill mod, but did not already exist in BodyStat");
            }
        }

        foreach (SkillFetus skillFetus in associatedSkillModsToCreate)
        {
            if (!bodyStats.ContainsSkill(skillFetus.skillName))
                bodyStats.addSkill(skillFetus.skillName,skillFetus.statType,skillFetus.assStorage);
            try
            {
                abilitySkills.Add(skillFetus.skillName, bodyStats.Skills[skillFetus.skillName]);
            }
            catch
            {
                Debug.LogError("failure for adding: " + skillFetus.skillName);
            }
        }

        associatedExistingSkill.Clear();
        associatedSkillModsToCreate.Clear();
    }

    public virtual AbilityResourceRequest GetResourceRequest()
    {
        return null;
    }

    public float ResourceRequestMultiplier(GV.AbilityTriggerType triggerType)
    {
        if (abilityTriggers.ContainsKey(triggerType))
            return abilityTriggers[triggerType];
        Debug.LogError("should never reach here");
        return 0;
    }

    public bool IsTriggable(GV.AbilityTriggerType toCheck)
    {
        return abilityTriggers.ContainsKey(toCheck);
    }

    public void AddTrigger(GV.AbilityTriggerType toAdd, float rezMultiplier = 1)
    {
        if (abilityTriggers.ContainsKey(toAdd))
            abilityTriggers[toAdd] = rezMultiplier;
        else
            abilityTriggers.Add(toAdd, rezMultiplier);
    }

    public void RemoveTrigger(GV.AbilityTriggerType toRemove)
    {
        if (abilityTriggers.ContainsKey(toRemove))
            abilityTriggers.Remove(toRemove);
    }

    public void LevelAbility(float expGiven)
    {
        //uses expToLevel & expDistribution to level it
        float levelsGained = expGiven / expToLevel;
        foreach (KeyValuePair<string, float> kv in expDistributionAsString) //where value si percent of level it gets when leveling the entire ability
            pcs.stats.modSkillValue(kv.Key, kv.Value * levelsGained);
    }

    protected AbilityResourceRequest ResourceRequestHelper(float maxBank, float currentAmt, float consumptionRate, float conversionRate, GV.AbilityResourceRequestType resourceType) //if need could be list of abrr
    {
        if (maxBank > currentAmt)
        {
            float consumption = Mathf.Min(maxBank - currentAmt, consumptionRate * conversionRate * Time.deltaTime); //in the unit of Force or w/e, not yet AbilityResource
            consumption /= conversionRate; //now converted
            switch (resourceType)
            {
                case GV.AbilityResourceRequestType.energy:
                    return new AbilityResourceRequest(consumption, 0, 0, 0);
                case GV.AbilityResourceRequestType.stanima:
                    return new AbilityResourceRequest(0, consumption, 0, 0);
                case GV.AbilityResourceRequestType.hp:
                    return new AbilityResourceRequest(0, 0, consumption, 0);
                case GV.AbilityResourceRequestType.exp:
                    return new AbilityResourceRequest(0, 0, 0, consumption);
                default:
                    Debug.LogError("switch type note handled: " + resourceType.ToString());
                    return new AbilityResourceRequest();
            }
        }
        return new AbilityResourceRequest();
    }

    public class AbilityResourceRequest
    {
        public float energyRequested;
        public float stanimaRequested;
        public float hpRequested;
        public float expRequested;

        public AbilityResourceRequest()
        {
            energyRequested = 0;
            stanimaRequested = 0;
            hpRequested = 0;
            expRequested = 0;
        }

        public AbilityResourceRequest(float _energyRequested, float _stanimaRequested, float _hpRequested, float _expRequested)
        {
            energyRequested = _energyRequested;
            stanimaRequested = _stanimaRequested;
            hpRequested = _hpRequested;
            expRequested = _expRequested;
        }

        public override string ToString()
        {
 	        return string.Format("Requested: Energy({0}),Stanima({1}), Hp({2}, Exp{3})",energyRequested,stanimaRequested,hpRequested,expRequested);
        }

        public static AbilityResourceRequest operator +(AbilityResourceRequest v1, AbilityResourceRequest v2)
        {
            return new AbilityResourceRequest(v1.energyRequested + v2.energyRequested, v1.stanimaRequested + v2.stanimaRequested, v1.hpRequested + v2.hpRequested, v1.expRequested + v2.expRequested);
        }

        public static AbilityResourceRequest operator *(AbilityResourceRequest v1, float v2)
        {
            return new AbilityResourceRequest(v1.energyRequested * v2, v1.stanimaRequested * v2, v1.hpRequested * v2, v1.expRequested * v2);
        }

        public static AbilityResourceRequest operator /(AbilityResourceRequest v1, AbilityResourceRequest v2)
        {
            return new AbilityResourceRequest(v1.energyRequested / v2.energyRequested, v1.stanimaRequested / v2.stanimaRequested, v1.hpRequested / v2.hpRequested, v1.expRequested / v2.expRequested);
        }

        public static AbilityResourceRequest operator /(AbilityResourceRequest v1, float v2)
        {
            return new AbilityResourceRequest(v1.energyRequested / v2, v1.stanimaRequested / v2, v1.hpRequested / v2, v1.expRequested / v2);
        }

        public static bool operator >(AbilityResourceRequest v1, AbilityResourceRequest v2)
        {
            return (v1.energyRequested > v2.energyRequested || v1.stanimaRequested > v2.stanimaRequested || v1.hpRequested > v2.hpRequested || v1.expRequested > v2.expRequested);
        }

        public static bool operator >=(AbilityResourceRequest v1, AbilityResourceRequest v2)
        {
            return (v1.energyRequested >= v2.energyRequested || v1.stanimaRequested >= v2.stanimaRequested || v1.hpRequested >= v2.hpRequested || v1.expRequested >= v2.expRequested);
        }

        public static bool operator <(AbilityResourceRequest v1, AbilityResourceRequest v2)
        {
            return (v1.energyRequested < v2.energyRequested || v1.stanimaRequested < v2.stanimaRequested || v1.hpRequested < v2.hpRequested || v1.expRequested < v2.expRequested);
        }

        public static bool operator <=(AbilityResourceRequest v1, AbilityResourceRequest v2)
        {
            return (v1.energyRequested <= v2.energyRequested && v1.stanimaRequested <= v2.stanimaRequested && v1.hpRequested <= v2.hpRequested && v1.expRequested <= v2.expRequested);
        }
    }

    protected class SkillFetus
    {
        public string skillName;
        public GV.Stats statType;
        public BalanceFormula assStorage;

        public SkillFetus(string _skillName, GV.Stats _statType, float min, float max, float ctrl, float percent, GV.HorzAsym assType)
        {
            skillName = _skillName;
            statType = _statType;
            assStorage = new AssStorage(ctrl,min,max,percent,assType);
        }

        public SkillFetus(string _skillName, GV.Stats _statType, float a, float b)
        {
            skillName = _skillName;
            statType = _statType;
            assStorage = new BasicBF(a, b);
        }

        public SkillFetus(string _skillName, GV.Stats _statType, float a, float b, float min, float max)
        {
            skillName = _skillName;
            statType = _statType;
            assStorage = new BasicBF(a, b, min, max);
        }
    }

}
