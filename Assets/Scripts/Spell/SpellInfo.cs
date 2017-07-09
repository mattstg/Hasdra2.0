using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellInfo{

    public SpellInfoRelativeManager relData;
    public GV.SpellState spellState = GV.SpellState.Charging;
    public GV.SpellShape spellShape = GV.SpellShape.Circle;
    
    public GV.CastType castType;
    public GV.MeleeCastType meleeCastType;
    public float intelligence = 0;
    public float wisdom = 0;
    public List<SkillModifier> onChargeSkillMods = new List<SkillModifier>();
    public SpellBridge spellBridge; //used for parent traversal
    public Vector2 lastPointOfPhysContact = new Vector2();

	public string uniqueSpellID = "";

    public State currentState;
    //public System.Collections.Generic.Dictionary<string, float> smVariable = new System.Collections.Generic.Dictionary<string, float>();
    public List<GV.InteractionType> interactionParams = new List<GV.InteractionType>();
    public float smVariable = 0;
    private float _curEnergy = 0;
    public float currentEnergy{set{ChangeEnergy(value);} get{return _curEnergy;}}
    public float timeInState = 0f;
    public float timeAlive = 0f;
    public float timeForCasterImmunity;
    public float initialHeadingAngle;
    public GV.RelativeLaunchType initialLaunchAngleRelType;
    public float altitude;
    public float currentAngle = 0;
    public float velocity = 0;  //updated every cycle, I need to rename to speed when I can, but its tied ina  few places
    public Vector2 velocityVector;
    public float initialLaunchVelo = 0;
    public float percentEnergy { get { return currentEnergy / relData.GetRelativeFloatData(GV.RelativeType.SpellLaunched, GV.SpellInfoDataType.Energy); }}
    public bool isStable{get{return (stability > GV.STABILITY_DECAY_THRESHOLD);}}
    public float stability;
    public Vector2 spellPos;
    public float gravityScale;
    public Color spellColor;
    public bool colorAltered = false;
    public float alpha = 1;
    public float boom = 1;
    public float hairlineCooldown = GV.HAIRLINE_COOLDOWN;
    public Transform followTarget = null; //Follow and position states can use this

    //explosion
    public GV.DirectionalDamage directionalDamageType = GV.DirectionalDamage.explosion;  //applys to energyForm spell coli with player, and explosions
    public bool useDefaultForce = true;
    public float cappedExplosiveForce = 0;
    public float dmgDirTrueAng = 0;
    public float densityAtExplosion;
    public float maxExplosionTime = 1.5f;
    public float currentExpoTime = 0;
    public AssStorage explodeDensityMod;
    public AssStorage implodeDensityMod;

    public bool ignoreCurrentState = false; //used if a state has a "use once" functionality, is turned off when state changes
    public bool stateFirstPass = true; //useful for checking if first time that state is called
    //self cast (sc)
    public bool isSelfCast {get {return castType == GV.CastType.SelfCast;}}
    public bool sc_selfApplySkillMod;
    public bool sc_selfApplyKnockback;
    public bool sc_selfApplySpell;

    //melee
    public bool isMelee {get {return castType == GV.CastType.Melee;}}
    public float melee_maxRange;
    public float melee_maxRange_energy;


    public string lockedSpellName = "";
	public bool isFacingLaunchDir;

    public float radioFreq = 0;
    public float lastRadioFreqRecieved = -1;

    //////Launch info
    public float energyLimit = 10;
    public GV.EnergyLimitType energyLimitType;
    public Vector2 initialSetScale;
    public GV.CastOnCharge castOnChargeParam = GV.CastOnCharge.None;
    
    public GV.SpellForms spellForm;
    private Vector2 _setScale = new Vector2(1, 1);
    public Vector2 setScale { set { _setScale = new Vector2(Mathf.Clamp(value.x, GV.SPELL_MAXMIN_SET_SCALE.x, GV.SPELL_MAXMIN_SET_SCALE.y),Mathf.Clamp(value.y, GV.SPELL_MAXMIN_SET_SCALE.x, GV.SPELL_MAXMIN_SET_SCALE.y)); } get { return _setScale; } }
    //When density 
    public float density { get { return 1 / (_setScale.x * _setScale.y); } set { SetScaleByDensityInput(value); } }
    public float densityEffect { get { return density * (1 - GV.DENSITY_EFFECT_MIN) + GV.DENSITY_EFFECT_MIN; }}

    public float energyInSideBank = 0; //energy in side bank gets re-added to spell next turn
    ////// Not yet initialized    
    public GV.BasicColiType lastBasicColiType = GV.BasicColiType.None;
    public Vector2 curHeadingDir;
    public Vector2 forceStoredForVelo = new Vector2(0,0); 
    /// ///////
    public float energyStoredForVeloLaunch = 0;

    public bool isTransforming = false;
    public bool isCreating = false;
    public List<GV.BasicColiType> ignoreMetaColiType = new List<GV.BasicColiType>();

    //Energy and Mass per pixel need to be updated if Pixel,Mass or Energy changes
    public int pixelOptimizationLevel = 0; //amount of times was called "halve" or "optimize" by d2d  (not accurate or much used yet)
    private int _numOfPixels = 1;
    public int numOfPixels{set{ChangeNumOfPixels(value);} get{return _numOfPixels;}} //Should be updated
    private float _energyPerPixel = 0;
    public float energyPerPixel{get{return _energyPerPixel;}}
    public float _massPerPixel;
    public float massPerPixel{get{return _massPerPixel;}}
    public float fakeEnergyForScale = 0;

    private float _mass = 0;
    private bool massDirty = true;
    public float mass { set { ChangeMass(value); } get { return GetMass(); } }  //does this need to be a function? guess well find out

    private float _energyTransferRate = 1;
    public float energyTransferRate { set { _energyTransferRate = value; } get { return intelligence + _energyTransferRate;} }

    private float _velocityEnergyTransfer = 1;
    public float velocityEnergyTransfer { set { _velocityEnergyTransfer = value; } get { return (_velocityEnergyTransfer + intelligence) * GV.ENERGY_TO_SPELLVELO; } }

    public SpellInfo()
    {
        relData = new SpellInfoRelativeManager(this);
    }

    public SpellInfo(SpellInfo toClone)  //I know this is important, but honestly forget when i use it
    {
        intelligence = toClone.intelligence;
        wisdom = toClone.wisdom;
        currentState = toClone.currentState;
        currentEnergy  = toClone.currentEnergy;
        timeAlive = toClone.timeAlive;
        //smVariable = new System.Collections.Generic.Dictionary<string,float>(toClone.smVariable);
        smVariable = toClone.smVariable;
        initialHeadingAngle = toClone.initialHeadingAngle;
        initialLaunchAngleRelType = toClone.initialLaunchAngleRelType;
        currentAngle = toClone.currentAngle;
        velocity = toClone.velocity;
        initialLaunchVelo = toClone.initialLaunchVelo;
		isFacingLaunchDir = toClone.isFacingLaunchDir;
    
        velocityEnergyTransfer  = toClone.velocityEnergyTransfer - toClone.wisdom;
        castOnChargeParam  = toClone.castOnChargeParam;
        energyLimitType = toClone.energyLimitType;
        castType = toClone.castType;
        meleeCastType = toClone.meleeCastType;
        sc_selfApplySkillMod  = toClone.sc_selfApplySkillMod ;
        sc_selfApplyKnockback = toClone.sc_selfApplyKnockback;
        sc_selfApplySpell     = toClone.sc_selfApplySpell;
        energyLimit = toClone.energyLimit;
        spellShape  = toClone.spellShape;
        //materialType = toClone.materialType;
        initialSetScale = setScale = new Vector2(toClone.setScale.x, toClone.setScale.y);
        melee_maxRange = toClone.melee_maxRange;
        melee_maxRange_energy = toClone.melee_maxRange_energy;
        relData = new SpellInfoRelativeManager(this);
		uniqueSpellID = toClone.uniqueSpellID;

        directionalDamageType = toClone.directionalDamageType;
        useDefaultForce = toClone.useDefaultForce;
        cappedExplosiveForce = toClone.cappedExplosiveForce;
        dmgDirTrueAng = toClone.dmgDirTrueAng;
        alpha = toClone.alpha;
        spellColor = toClone.spellColor;

        foreach (GV.InteractionType it in toClone.interactionParams)
            interactionParams.Add(it);
}

    public void InitializeSpellInfo(StateSlot startState)
    {
        //Start state => spell info
        SpellInfo newSpellInfo = new SpellInfo();
        AddExtractedSpellInfo(((StartStateSS)startState).extractAsSpellInfo());
    }

    public void AddExtractedSpellInfo(SpellInfo toAdd) //I know this is important, but honestly forget when i use it
    {
        energyLimitType = toAdd.energyLimitType;
        castType = toAdd.castType;
        meleeCastType = toAdd.meleeCastType;
        energyLimit = toAdd.energyLimit;
        castOnChargeParam = toAdd.castOnChargeParam;
        initialHeadingAngle = toAdd.initialHeadingAngle;
        velocity = initialLaunchVelo = toAdd.initialLaunchVelo;
        initialLaunchAngleRelType = toAdd.initialLaunchAngleRelType;
        spellShape = toAdd.spellShape;
        initialSetScale = setScale = new Vector2(toAdd.setScale.x, toAdd.setScale.y);
        spellForm = toAdd.spellForm;
        melee_maxRange = toAdd.melee_maxRange;
        melee_maxRange_energy = toAdd.melee_maxRange_energy;
		isFacingLaunchDir = toAdd.isFacingLaunchDir;
        sc_selfApplySkillMod = toAdd.sc_selfApplySkillMod;
        sc_selfApplyKnockback = toAdd.sc_selfApplyKnockback;
        sc_selfApplySpell = toAdd.sc_selfApplySpell;
        directionalDamageType = toAdd.directionalDamageType;
        useDefaultForce = toAdd.useDefaultForce;
        cappedExplosiveForce = toAdd.cappedExplosiveForce;
        dmgDirTrueAng = toAdd.dmgDirTrueAng;
        currentAngle = toAdd.currentAngle;
        alpha = toAdd.alpha;
        spellColor = toAdd.spellColor;

        foreach (GV.InteractionType it in toAdd.interactionParams)
            interactionParams.Add(it);
    }

    public void SetScaleByDensityInput(float dIn)
    {
        Vector2 ss = setScale;
        float xRatio = ss.y / ss.x ;
        //Debug.Log(string.Format("Current scale {0} and density {1} has an xratio {2}", ss, density, xRatio));
        float newSSx = Mathf.Sqrt(1 / (dIn * xRatio));
        //Debug.Log("din*xratio " + dIn * xRatio + " next step: " + (1 / (dIn * xRatio)) + " final step: " + Mathf.Sqrt(1 / (dIn * xRatio)));
        float newSSy = newSSx * xRatio;
        //Debug.Log(string.Format("for density {0}, new ss calculated is ({1},{2})", dIn, newSSx, newSSy));
        setScale = new Vector2(newSSx, newSSy);
        //Debug.Log(string.Format("din {0} vs dout {1}", dIn, density));

        /*
      boom = ((1 / dIn) - (initialSetScale.x * initialSetScale.y * 2 * .7f)) / (initialSetScale.x * initialSetScale.y * 2 * currentEnergy * .12f);
      Debug.Log("Setting Density, New boom = " + boom);
        Debug.Log("dIn" + dIn + " 2nd part " + (initialSetScale.x * initialSetScale.y * 2 * .7f) + " 3rd devider " + (initialSetScale.x * initialSetScale.y * 2 * currentEnergy * .12f));
        Debug.Log("initial set scale " + initialSetScale.x + " " + initialSetScale.y);
        //   1 / (spellInfo.initialSetScale.x * spellInfo.initialSetScale.y * 2 * (BalanceFormulaDict.Instance.GetValue("spellScale", spellInfo.currentEnergy)));*/
    }

    public float GetMass() //be sure to recalculate if any have been changed (have a dirty flag)
    {
        return GV.SPELL_FORM_WEIGHT_BASE[(int)spellForm] * _curEnergy;
        //_mass = massPerPixel*currentEnergy*density;
        //return _mass;
    }
    

    public void ChangeMass(float newAmt)
    {
        _mass = newAmt;
        _massPerPixel = _mass / _numOfPixels;
    }

    public void ChangeEnergy(float newAmt)
    {
        _curEnergy = newAmt;
        _energyPerPixel = _curEnergy / _numOfPixels;
    }

    private void ChangeNumOfPixels(int newAmt)
    {
      _numOfPixels = (newAmt >= 1)?newAmt:1;
      _energyPerPixel = currentEnergy / _numOfPixels;
      _massPerPixel = mass / _numOfPixels;
    }

    /// <summary>
    /// This function simulates adding/removing pixels, Removing or adding mass/energy from the difference
    /// </summary>
    /// <param name="newAmount"></param>
    public void ModifyPixels(int newAmount)
    {
        float pixelDifference = newAmount - _numOfPixels;
        _mass += massPerPixel * pixelDifference;
        float energyDifference = _energyPerPixel * pixelDifference;
        _curEnergy += energyDifference;
        _numOfPixels = newAmount;
        if (energyDifference < 0) //material stamped out
            fakeEnergyForScale += energyDifference * -1;
    }

    public SpellInfo Clone()
    {
        SpellInfo toRet = (SpellInfo)this.MemberwiseClone();
        //Cant clone the Identifiers, so do that manually
        toRet.spellForm = spellForm;
        toRet._numOfPixels = _numOfPixels;
        toRet.currentEnergy = _curEnergy;
        toRet.mass = _mass;
        return toRet;
    }

    public override string ToString()
    {
        string toRet = "";
        toRet = "Px: " + numOfPixels + ",Energy: " + currentEnergy + ",engrPx: " + energyPerPixel + ",mass: " + mass + ",massPerPx: " + massPerPixel + ",int: " + intelligence + ",Wis: " + wisdom;
        return toRet;
    }

    public float CapEnergyAllowedForVelo(float desiredEnergy)
    {
        float toRet = (desiredEnergy > velocityEnergyTransfer) ? velocityEnergyTransfer : desiredEnergy ;
        toRet = (desiredEnergy > currentEnergy) ? currentEnergy : desiredEnergy;
        return toRet;
    }

    public SpellBridge GetNCastersBack(int numParentsBack)
    {
        float numOfParent = numParentsBack;
        if (numOfParent <= 0 || spellBridge.parentCaster == null)
            return spellBridge;
        else
        {
            SpellBridge prevouisCaster = spellBridge.parentCaster;
            for (int i = 1; i <= numOfParent; i++)
            {
                if (prevouisCaster.parentCaster != null)
                    prevouisCaster = prevouisCaster.parentCaster;
                else
                    break; //found top parent in chain
            }
            return prevouisCaster;
        }
    }


    public void CollisionDetected(Spell otherObj)
    {
        if(otherObj.spellInfo.spellState == GV.SpellState.Exploding)
            CollisionDetected(GV.BasicColiType.Explosion);
        else
            CollisionDetected(GV.BasicColiType.Spell);

        if (otherObj.spellInfo.radioFreq != -1)
            otherObj.spellInfo.lastRadioFreqRecieved = otherObj.spellInfo.radioFreq;
        
    }

    public void CollisionDetected(SolidMaterial otherObj)
    {
        CollisionDetected(GV.BasicColiType.SolidMaterial);
    }

    public void CollisionDetected(Explosion otherObj)
    {
        CollisionDetected(GV.BasicColiType.Explosion);
    }

    public void CollisionDetected(PlayerControlScript otherObj)
    {
        if (!ignoreMetaColiType.Contains(GV.BasicColiType.Player))
        {
            lastBasicColiType = GV.BasicColiType.Player;
        }
    }

    private void CollisionDetected(GV.BasicColiType coliType)
    {
        if (!ignoreMetaColiType.Contains(coliType))
        {
            lastBasicColiType = coliType;
        }
    }

    //Update any vars that need to be
    public void UpdateInternal()
    {
        hairlineCooldown += Time.deltaTime;
    }
}
