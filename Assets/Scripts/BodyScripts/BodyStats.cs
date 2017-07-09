using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BodyStats {
    public PlayerControlScript pcs;
    public Concussion concussion;
    Bodyparts bodyParts;
    SpellInfo spellInfo; //Optional, but if is linked to it, will update energy
    //need Two Libraries
    //one for Skills
    //one for Debuffs

    public Dictionary<string, Skill> Skills = new Dictionary<string, Skill>();
    //need to fill Skills
    

    //public Dictionary<string, SkillModifier> Debuffs = new Dictionary<string, SkillModifier>();
    //public List<SkillModifier> SkillModifiers = new List<SkillModifier>();
    //starts empty needs capabilities 

    
    float currentLevel = 0;

    private float _energy = 1f;
    public float energy { set { _energy = Mathf.Clamp(value, 0, maxEnergy); } get { energy = _energy;  return _energy; } } //since max can vary, this limits it
	public float maxEnergy {get {return getSkillValue("maximumEnergy");}}

    private float _hp = 1f;
    public float healthPoints { set { _hp = Mathf.Clamp(value, 0, maxHp); } get { healthPoints = _hp; return _hp; } }
    public float hp { set { _hp = Mathf.Clamp(value, 0, maxHp); } get { healthPoints = _hp; return _hp; } }
	public float maxHp { get { return getSkillValue("maximumHP"); } }

    private float _stam = 1f;
    public float stamina { set { _stam = Mathf.Clamp(value, 0, maxStamina); } get { stamina = _stam; return _stam; } }
	public float maxStamina { get { return getSkillValue("maximumStamina"); } }
	
	
    public float weight;

	bool isDead = false;

    public float concusionScore { get { return concussion.curConcuss; } }
    public bool isConcussed { get { return concussion.isConcussed; } }
    public bool isInvunerable { get { return concussion.isConcussed; } }

	private float Strength = 1f; 
	private float Intelligence = 1f; 
	private float Charisma = 1f;  
	private float Constitution = 1f; 
	private float Wisdom = 1f; 
	private float Agility = 1f; 
	private float Dexterity = 1f; 

	public float strength {
		get{ return this.Strength; } 
		set{ this.Strength = value; 
			notifyStatChange (GV.Stats.Str);
		}
	}
	public float intelligence {
		get{ return this.Intelligence; } 
		set{ this.Intelligence = value;
        notifyStatChange(GV.Stats.Int);
		}
	}
	public float charisma {
		get{ return this.Charisma; } 
		set{ this.Charisma = value;
        notifyStatChange(GV.Stats.Char);
		}
	}
	public float constitution {
		get{ return this.Constitution; } 
		set{ this.Constitution = value;
        notifyStatChange(GV.Stats.Const);
		}
	}
	public float wisdom {
		get{ return this.Wisdom; } 
		set{ this.Wisdom = value;
        notifyStatChange(GV.Stats.Wis);
		}
	}
	public float agility {
		get{ return this.Agility; } 
		set{ this.Agility = value;
        notifyStatChange(GV.Stats.Agi);
		}
	}
	public float dexterity {
		get{ return this.Dexterity; } 
		set{ this.Dexterity = value;
        notifyStatChange(GV.Stats.Dex);
		}
	}  

    //live stats
    
    public float temperature = GV.PLAYER_TEMPERATURE_START;

    //int
    private int   _energyUseEfficiency = 1;  //not sure if was redundant same or not
    private float _energyTransferRate = 0; //energy per second transfered into charging a spell

    public BodyStats(bool arbitraryArguement) //only used to create dummy, use for nothing else
    {

    }

    public BodyStats(BodyStats toClone, SpellInfo toLink)
    {
        Clone(toClone);
        spellInfo = toLink;
    }

    public BodyStats(BodyStats toClone) 
    {
        Clone(toClone);
    }

    public BodyStats(PlayerControlScript _pcs)  //created normally
	{
        pcs = _pcs;
        BodyStatFiller temp = new BodyStatFiller(this);
        energy = maxEnergy;
        healthPoints = maxHp;
        stamina = maxStamina;
        weight = GV.PLAYER_START_WEIGHT;
        concussion = new Concussion(this);
        bodyParts = new Bodyparts(pcs.gameObject);
    }

    public BodyStats(SpellInfo si)  //created by a spell
    {
        intelligence = si.intelligence;
        wisdom = si.wisdom;
    }
    
    public void Update() 
    {
		if (!isDead) {
			UpdateEnergy ();
			UpdateHp ();
			UpdateStamina ();
			UpdateHeatScore ();
			concussion.UpdateConcusionScore (); //getSkillValue ("concusionRecoverRate"), Time.deltaTime);
		}
    }

    public Skill getSkill(string SkillName)
    {
		if (Skills.ContainsKey (SkillName))
			return Skills [SkillName];
		else {
			Debug.LogError ("no Skill found by the name:" + SkillName);
			return null;
		}
	}

	public void addSkillModifier(SkillModifier sMToAdd){
		if (Skills.ContainsKey (sMToAdd.skillName))
			getSkill (sMToAdd.skillName).addSkillModifier (sMToAdd);
		else
			Debug.LogError("no Skill found by the name:" + sMToAdd.skillName);
		//Debug.Log ("Invalid skillName in a skillModifier which is trying to be added.");
	}

    public void addSkill(string newSkillName, GV.Stats statType, BalanceFormula asformula)
    {
        if (!Skills.ContainsKey(newSkillName))
        {
            Skills.Add(newSkillName, new Skill(newSkillName, statType, this, asformula));
        }
        else
        {
            Debug.LogError("Attempted to add skill: " + newSkillName + " to bodystats when it already exists");
        }
    }

	public List<SkillModifier> getAllSkillModifiers(){
		List<SkillModifier> SMList = new List<SkillModifier> ();
		foreach (KeyValuePair<string,Skill> skill in Skills) {
            /*if(skill.Key == "stanimaToForce")
            {
                Debug.Log("stf count: " + skill.Value.retSkillModifiers().Count);
            }*/
			SMList.AddRange (skill.Value.retSkillModifiers ());
		}
		return SMList;
	}

    /// <summary>
    /// Given a required amt of stamina, returns a % efficency of the task and consumes stamina
    /// </summary>
    /// <param name="amt"></param>
    /// <returns></returns>
    public float expendStamina(float amt)
    { //in the future, we can have max stamina expenditure and such
        if (amt <= stamina)
        {
            stamina -= amt;
            return 1;
        }
        else
        {
            float efficency = amt / stamina;
            Debug.Log("expended partial stamina: " + efficency + " because stanima: " + stamina + " and amt required: " + amt);
            stamina = 0;
            return efficency;
        }
    }

    public float getSkillValue(string SkillName)
    {
        if (Skills.ContainsKey(SkillName))
            return getSkill(SkillName).get();
        else
            Debug.LogError("no Skill found by the name:" + SkillName);
        return 0;
    }

    public bool ContainsSkill(string skillName)
    {
        return (Skills.ContainsKey(skillName));
    }

    public float getResistanceValue(string SkillName)
    {
        if (Skills.ContainsKey(SkillName))
			return 1 - (getSkill(SkillName).get() /(getSkill(SkillName).get()+GV.RESISTANCE_CONSTANT));
        else
            Debug.LogError("no resistance found by the name:" + SkillName);
        return 0;
    }

	public void notifyStatChange(GV.Stats stat){
		//Debug.Log ("stat " + stat + " was notified of stat change...");
		foreach(KeyValuePair<string,Skill> skill in Skills){
			//Debug.Log ("statName of skill " + skill.Value.statName + " and testing for " + stat);
			if (skill.Value.statType == stat) {
				skill.Value.notifiedOfStatChange ();
				//Debug.Log ("skill name notified " + skill.Value.skillName);
			}
		} 
	}

    public void modSkillValue(string _skillName, float _modBy)
    {
        try
        {
            Skills[_skillName].modSkillPoint(_modBy);
        }
        catch
        {
            Debug.LogError("failure to mod skill value: " + _skillName);
        }
    }

    public float getParentStat(GV.Stats _statType)
    {
        switch (_statType)
        {
            case GV.Stats.Str:
                return strength;
            case GV.Stats.Int:
                return intelligence;
            case GV.Stats.Char:
                return charisma;
            case GV.Stats.Const:
                return constitution;
            case GV.Stats.Wis:
                return wisdom;
            case GV.Stats.Agi:
                return agility;
            case GV.Stats.Dex:
                return dexterity;
            default:
                Debug.LogError("invalid stat type " + _statType);
                return 0;
        }
    }

    public void setParentStat(GV.Stats _statType, float newValue)
    {
        switch (_statType)
        {
            case GV.Stats.Str:
                strength = newValue;
                break;
            case GV.Stats.Int:
                intelligence = newValue;
                break;
            case GV.Stats.Char:
                charisma = newValue;
                break;
            case GV.Stats.Const:
                constitution = newValue;
                break;
            case GV.Stats.Wis:
                wisdom = newValue;
                break;
            case GV.Stats.Agi:
                agility = newValue;
                break;
            case GV.Stats.Dex:
                dexterity = newValue;
                break;
            default:
                Debug.LogError("invalid stat type " + _statType);
                break;
        }
    }

    public bool IsHittingLimb(GameObject otherColi, string limbName)
    {
        return bodyParts.IsHittingLimb(otherColi, limbName);
    }

    #region bodyStat updates
   
    private void UpdateHeatScore()
    {
        float heatRecovered = getSkillValue("heatRecoverRate") * GV.CONCUSION_RECOVER_RATE * Time.deltaTime;
        if (temperature < GV.PLAYER_TEMPERATURE_START)
        {
            temperature += heatRecovered;
            temperature = (temperature > GV.PLAYER_TEMPERATURE_START) ? GV.PLAYER_TEMPERATURE_START : temperature;
        }
        else if (temperature > GV.PLAYER_TEMPERATURE_START)
        {
            temperature -= heatRecovered;
            temperature = (temperature < GV.PLAYER_TEMPERATURE_START) ? GV.PLAYER_TEMPERATURE_START : temperature;
        }
    }

	private void UpdateEnergy()
	{
        energy += getSkillValue("energyRegenRate") * Time.deltaTime;
	}


    private void UpdateHp()
    {
        healthPoints += (getSkillValue("hpRegenRate") * Time.deltaTime);
	}

    private void UpdateStamina()
    {
        stamina += (getSkillValue("staminaRegenRate") * Time.deltaTime);
    }

    #endregion



    public void RagdollRecovered() //call when ragdoll is recovered back into normal mode
    {
        concussion.RecoverAnimationComplete();
    }

	public void Dies(){
		isDead = true;
	}

    private void Clone(BodyStats toClone)
    {
        Skills = new Dictionary<string, Skill>();

        foreach (KeyValuePair<string, Skill> kv in toClone.Skills)
            Skills.Add(kv.Key, new Skill(kv.Value));
        

        currentLevel = toClone.currentLevel;
        _energy = toClone._energy;
        energy = toClone.energy;
        healthPoints = toClone.healthPoints;
        stamina = toClone.stamina;
        weight = toClone.weight;
        healthPoints = toClone.healthPoints;
        strength = toClone.strength;
        intelligence = toClone.intelligence;
        charisma = toClone.charisma;
        constitution = toClone.constitution;
        wisdom = toClone.wisdom;
        agility = toClone.agility;
        dexterity = toClone.dexterity;
    }
}


