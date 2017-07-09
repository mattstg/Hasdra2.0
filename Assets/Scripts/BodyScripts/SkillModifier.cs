using UnityEngine;
using System.Collections;

public class SkillModifier {

    public string skillName = "";//affected skill
	public string skillModID = "";
    public float currentEnergy = 0;
    public float energyLimit = 0;
    public float percentChargePerSecond = 0;
    public GV.ConstantOrPercent energyLimitType = GV.ConstantOrPercent.constant;
	public float startTime = 0; 
	public float effectDuration = 0; //set via SetEffect()
	public float effectPower = 0; //set via SetEffect()
    public bool isBuff; //as in whether or not it will add or subtract from skills
	GV.SkillModScalingType timeOrEffect; //either time or effect is independant
	public float control;  // desired value of either Duration or Effect.
	private bool isActive;

	//public float spellWis; //transfer rate
	//public float spellInt; //acts as spell penetration

    public SkillModifier(string skillAffected, float startEnergy, GV.SkillModScalingType _skillModType, float controlValue, float energyTransfered, string spellId, bool isBuffIn, float percentChargeRate, GV.ConstantOrPercent EnergyLimitType)
    {
        //defined without start time, used purely in saving SkillMod to spells, for application on player at later date
        skillName = skillAffected;
        currentEnergy = startEnergy;
        timeOrEffect = _skillModType;
        control = controlValue;
        skillModID = spellId;
        isBuff = isBuffIn;
        percentChargePerSecond = percentChargeRate;
        energyLimitType = EnergyLimitType;
    }

    public SkillModifier(SkillModSS skillmodss, string _skillModID)
    {
        currentEnergy = 0;
        percentChargePerSecond = skillmodss.ssDict["percentCharge"].CastValue<float>();
        skillName = skillmodss.ssDict["skillValue"].CastValue<string>();
        energyLimit = skillmodss.ssDict["energyLimit"].CastValue<float>();
        timeOrEffect = skillmodss.ssDict["skillModType"].CastValue<GV.SkillModScalingType>();
        control = skillmodss.ssDict["controlValue"].CastValue<float>();
        energyLimitType = skillmodss.ssDict["energyLimitType"].CastValue<GV.ConstantOrPercent>();
        isBuff = !skillmodss.ssDict["isDebuff"].CastValue<bool>();
        skillModID = _skillModID;
    }

	/* Never Used...
	public SkillModifier(string affectedSkillName, float startEnergy,  bool _isBuff, GV.SModEffectType typeIn, float controlIn, string ID) // , BodyStats target)
    {
		skillModID = ID;
        startEn = startEnergy;
		startTime = Time.time;
        skillName = affectedSkillName;
        isBuff = _isBuff;
		setEffect (typeIn, controlIn);
		isActive = true;
		type = typeIn;
		control = controlIn;
    } */
		
	public SkillModifier(SkillModifier toCopy){
		skillModID = toCopy.skillModID;
        currentEnergy = toCopy.currentEnergy;
		startTime = Time.time;
		skillName = toCopy.skillName;
		isBuff = toCopy.isBuff;
		isActive = true;
		timeOrEffect = toCopy.timeOrEffect;
		control = toCopy.control;
        //setEffect(toCopy.timeOrEffect, toCopy.control);
		setEffect();
	} 
		
	public float Charge(float energy)
	{
        if (energyLimitType == GV.ConstantOrPercent.constant && (currentEnergy + energy) > energyLimit)
        {
            float toAdd = energyLimit - currentEnergy;
            currentEnergy += toAdd;
            return energy - toAdd;
        }
        else
        {
            currentEnergy += energy;
            return 0;
        }
	}

	/*
	private void setEffect(){
		setEffect (timeOrEffect, control);
	}

	/*
	private void setEffect(GV.SkillModScalingType type, float control){
		switch (type) {
		case GV.SkillModScalingType.forceTime:
			if (control < GV.MIN_EFFECT_DURATION) {
				setEffect (GV.SkillModScalingType.forceTime, GV.MIN_EFFECT_DURATION);
			} else {
				effectDuration = control;
				effectPower = currentEnergy / control;
			}
			break;
		case GV.SkillModScalingType.forceEff:
			if (control < GV.MIN_EFFECT_POWER) {
				setEffect (GV.SkillModScalingType.forceEff, GV.MIN_EFFECT_POWER);
			} else {
				effectPower = control;
				effectDuration = currentEnergy / control;
			}
			break;
		default:
			Debug.Log ("Incorrect SMofEffectType entry. Skill mode critical Failure.");
			break;
		}
	} */

	private void setEffect(){
		switch (timeOrEffect) {
		case GV.SkillModScalingType.forceTime:
				effectDuration = control;
				effectPower = currentEnergy / control;
			break;
		case GV.SkillModScalingType.forceEff:
				effectPower = control;
				effectDuration = currentEnergy / control;
			break;
		default:
			Debug.Log ("Incorrect SMofEffectType entry. Skill mode critical Failure.");
			break;
		}
		//Debug.Log ("SkillMod effect is set: effectDuration: " + effectDuration + "  effectPower: " + effectPower);
		//Debug.Log ("Total Energy in SM: " + currentEnergy);
	}


	public void reset(SkillModifier skillModIn){
		if (skillModIn.skillModID == skillModID && skillModID != "") {
			//so it is the same spell... 
			/*
			if (isActive == false){
				startEn = skillModIn.retRemainingEnergy();
				startTime = Time.time;
				setEffect (type, control);
				isActive = true;
			} else {
				startEn = retRemainingEnergy() + skillModIn.retRemainingEnergy ();
				startTime = Time.time;
				setEffect (type, control);
			} */
			//resetting skillMod, to have greater of two energies
			if (retRemainingEnergy() < skillModIn.currentEnergy) {
				currentEnergy = skillModIn.currentEnergy;
				startTime = Time.time;
				setEffect ();
			}

		} else {
			Debug.Log ("skill modification reset failure.");
		}
	}

	public bool isExpired(){
        //Debug.Log(string.Format("startTime({0}) + effectDuration({1}) > Time.time({2})",startTime,effectDuration,Time.time));
        return startTime + effectDuration < Time.time;// || Time.time == 0;  why if the game just began?

	}

	public float getEff(){
		if (effectIsValid())
			setEffect ();
		if (isExpired () || isActive == false) {
			isActive = false;
			return 0;
		} else {
			if (isBuff) {
				return effectPower;
			}else{
				return effectPower * -1;
			}
		}
	}

	public bool effectIsValid(){
		return effectPower != 0 && effectDuration != 0;
	}

	public bool isTaggedForDestruction(){
        return isExpired();
	}

	public float retRemainingEnergy(){
		if (isExpired ()) {
			return 0;
		} else {
			//float percentCompletion = (Time.time - startTime + effectDuration)/effectDuration;
			return currentEnergy * percentTimeRemaining();
		}
	}

    public float percentTimeRemaining()
    {
        if (isExpired())
            return 0;
        return ((startTime + effectDuration) - Time.time) / effectDuration;
    }

    public void ToString()
    {
       System.Collections.Generic.Dictionary<string,string> toOutDict = new System.Collections.Generic.Dictionary<string,string>();
       toOutDict.Add("skillName",skillName);
        toOutDict.Add("skillModID",skillModID);
        toOutDict.Add("energyLimit",energyLimit.ToString());
        toOutDict.Add("currentEnergy", currentEnergy.ToString());
        toOutDict.Add("percentChargePerSecond", percentChargePerSecond.ToString());
        toOutDict.Add("energyLimitType", energyLimitType.ToString());
        toOutDict.Add("startTime", startTime.ToString());
        toOutDict.Add("effectDuration", effectDuration.ToString());
        toOutDict.Add("effectPower", effectPower.ToString());
        toOutDict.Add("isBuff", isBuff.ToString());
        toOutDict.Add("timeOrEffect", timeOrEffect.ToString());
        toOutDict.Add("control", control.ToString());
        toOutDict.Add("isActive", isActive.ToString());
        Debug.Log(GV.OutputDictToString(("skillMod (" + skillName + ")"),toOutDict));
    }
	/*    OLD CODE
    public void Update()
    {



		float energyExpense = spellWis * Time.deltaTime;
        energyExpense = (energyExpense > remainingEnergy)? remainingEnergy : energyExpense;

        float totalEnergyToSkillModified = (energyExpense > affectedBody.getSkill("debuffResistance").get()*Time.deltaTime) ? energyExpense - affectedBody.getSkill("debuffResistance").get()*Time.deltaTime: 0;
        if (remainingEnergy != 0)
        {
            //so we need to update working stats
            targetSkill.setWorkingSkill(targetSkill.getWorkingSkillValue() - totalEnergyToSkillModified / 15*Time.deltaTime);
        }
        else
        {
            //fizzle and remove from list
			targetSkill.resetWorkingSkill();
            affectedBody.SkillModifiers.Remove(this);
        }
        //needs to update working value of skill once at the biggining of life cycle, and once again at end of lifecycle
        remainingEnergy -= energyExpense;
		
    }
    */
}
