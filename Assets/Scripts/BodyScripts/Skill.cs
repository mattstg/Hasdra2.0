using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill {
    public string skillName; //name of skill
	private float skillValue = 0; //value of skill
    //public float workingSkillValue = 1; //working value of skill
    private float statValue; //value of the "parent" stat (say strength)
    public GV.Stats statType; //type of the "parenmt" stat ("str" for strength, etc)
    BodyStats body;
	BalanceFormula FuncStorage;
	float lastOutput;
	float lastSumSkillMods;
	bool hasBeenChanged = true;

	List<SkillModifier> skillModifiers = new List<SkillModifier> ();

    public Skill(string inName, GV.Stats _statType, float inStat, BodyStats tempBody, BalanceFormula inFuncStorage)
    {
        skillName = inName;
        statValue = inStat;
        statType = _statType;
        body = tempBody;
        FuncStorage = inFuncStorage.CopyThis();
    }

    public Skill(string inName, GV.Stats _statType, BodyStats tempBody, BalanceFormula inFuncStorage)  //removes instat req
    {
        skillName = inName;
        statType = _statType;
        body = tempBody;
        statValue = 0;
        FuncStorage = inFuncStorage.CopyThis();
        notifiedOfStatChange();
    }

    public Skill(string inName, GV.Stats _statType, float inStat, BodyStats tempBody)
	{
		skillName = inName;
		statValue = inStat;
        statType = _statType;
		body = tempBody;
		FuncStorage = null;
	}

    public Skill(Skill inSkill)
    {
        skillName = inSkill.skillName;
        skillValue = inSkill.skillValue;
        statType = inSkill.statType;
        body = inSkill.body;
		if (inSkill.FuncStorage == null)
			FuncStorage = null;
		else
			FuncStorage = inSkill.FuncStorage;
    }

    //set, get, add, etc.
    #region operatorOverload
    public static float operator *(Skill v1, float v2)
    {
        return v1.get() * v2;
    }
    public static float operator *(Skill v1, Skill v2)
    {
        return v1.get() * v2.get();
    }
    public static float operator +(Skill v1, Skill v2)
    {
        return v1.get() + v2.get();
    }
    public static float operator +(Skill v1, float v2)
    {
        return v1.get() + v2;
    }
    public static implicit operator float(Skill v1)  //casts to a float if circumstance is right
    {
        return v1.get();
    }
    #endregion

    public float get(){
        //if (skillName == "energyTransferRate")
       //     Debug.Log("skill internal value: " + skillValue + " & stat value: " + statValue);
		float skillModEffectSum = 0;
		if (skillModifiers.Count > 0) {
			skillModEffectSum = returnSumOfSkillMods();
			if (lastSumSkillMods != skillModEffectSum) {
				hasBeenChanged = true;
				lastSumSkillMods = skillModEffectSum;
				dumpExpiredSkillMods ();
			}
		}

		if (FuncStorage != null) {
			if (hasBeenChanged) {
				//Debug.Log ("New AssFunc Ret for " + skillName + " returns " + FuncStorage.ret (minValueControl(statValue + skillValue + skillModEffectSum)));
				lastOutput = FuncStorage.ret (minValueControl(statValue + skillValue + skillModEffectSum));
				hasBeenChanged = false;
			}
			return lastOutput;
		} else {
			
			return minValueControl(statValue + skillValue + skillModEffectSum);
			//note dosn't need to be updated, hasBeenChanged is only to recalculate scale with Asymptote
		}

	}
		
	public float minValueControl(float input){
		if (GV.EFFECTS_CAN_DECREASE_STATS || statValue <= 1) {
			if (input >= 1)
				return input;
			else
				return 1;
		} else {
			if (input >= statValue)
				return input;
			else
				return statValue;
		}
	}

    public void modSkillPoint(float toAddToSkill)
    {
        skillValue += toAddToSkill;
        skillValue = (skillValue < 0) ? 0 : skillValue;
		hasBeenChanged = true;
    }
		
	public float getSkillValue(){
		return skillValue;
	}
		
	public void notifiedOfStatChange(){
		statValue = body.getParentStat(statType);
		hasBeenChanged = true;
	}

    public void addSkillModifier(SkillModifier skillModIn)
    {
        foreach (SkillModifier skillMod in skillModifiers)
        {
            if (skillModIn.skillModID.Equals(skillMod.skillModID))
            {
                //Debug.Log("reset");
                skillMod.reset(skillModIn);
                return;
            }
        }
        //Debug.Log("add new");
        skillModifiers.Add(new SkillModifier(skillModIn));
    }

	public void dumpExpiredSkillMods(){
		List<SkillModifier> toRemove = new List<SkillModifier>();
		foreach(SkillModifier skillMod in skillModifiers){
			if (skillMod.isTaggedForDestruction ()) {
				toRemove.Add (skillMod);
			}
		}
		foreach (SkillModifier SMtoRem in toRemove) {
			skillModifiers.Remove (SMtoRem);
		}
		toRemove.Clear ();
	}

	public float returnSumOfSkillMods(){
		float sum = 0f;
		foreach (SkillModifier skillMod in skillModifiers) {
				sum += skillMod.getEff ();
		}
		return sum;
	}

	public List<SkillModifier> retSkillModifiers(){
		dumpExpiredSkillMods ();
		return skillModifiers;
	}
}
