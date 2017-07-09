using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellBridge : MonoBehaviour {

    public SpellBridgeParent spellBridgeParent;
    public SpellBridge parentCaster;
    //PlayerControlScript caster;
    int casterID; //player id of original caster
    public GameObject chargingSprite;
    public Spell currentSpell;
    List<Spell> firedSpells = new List<Spell>(); //this list contains nulls, needs to be cleansed on use
    public string chargingSpellName = "";
    float storedEnergy = 0f;
    bool isCharging = false;
    public bool isMelee { get { return currentSpell.spellInfo.isMelee; } } //this is dangerous
    public bool isSelfCast { get { return currentSpell.spellInfo.isSelfCast; } }//this is dangerous
    public GV.MeleeCastType meleeCastType { get { return currentSpell.spellInfo.meleeCastType; } }
    //Vector2 location = new Vector2();     for placementy

    public string retCasterID(){
		return casterID.ToString ();
	}
	// Use this for initialization
    bool isTransforming = false;
    bool isCreating = false;
    bool holdingChargedSpell = false;
    //float melee_rangePerEnergy;

    public void InitializeSpellBridge(PlayerControlScript pcs, SpellBridge _parentCaster)
    {
        parentCaster = _parentCaster;
        spellBridgeParent = new SpellBridgeParent(pcs);
        casterID = pcs.pid;
    }

    public void InitializeSpellBridge(Spell spell, SpellBridge _parentCaster)
    {
        parentCaster = _parentCaster;
        spellBridgeParent = new SpellBridgeParent(spell);
        casterID = spell.casterID;
    }
    
    public bool ChargeSpell(string pressedSpellName, BodyStats stats)  //Avatar charing a spell
    {
        if (pressedSpellName != chargingSpellName && chargingSpellName != "") //not the spell currently charging that player is pressing
            return false;

		CreateSpellIfDoesntExist(pressedSpellName);

        float energyToChargingSpell;
        float stanimaUsed;
        if (isMelee)
        {
            stanimaUsed = stats.getSkillValue ("stanimaTransferToSpellRate") * Time.deltaTime;
            if (stanimaUsed > stats.stamina)
                stanimaUsed = stats.stamina;
            energyToChargingSpell = stanimaUsed * stats.getSkillValue("stanimaToEnergyRatio");
            stats.stamina -= stanimaUsed;
            float excessEnergy = _ChargeSpell(pressedSpellName, energyToChargingSpell);
            stanimaUsed = excessEnergy / stats.getSkillValue("stanimaToEnergyRatio");
            stats.stamina += stanimaUsed;
        }
        else //norm or self cast
        {

            //Debug.Log("skill value for transfer: " + stats.getSkillValue("energyTransferRate"));
            energyToChargingSpell = (stats.getSkillValue ("energyTransferRate")) * Time.deltaTime;
            if (energyToChargingSpell > stats.energy)
                energyToChargingSpell = stats.energy;
            stats.energy -= energyToChargingSpell;
            float excessEnergy = _ChargeSpell(pressedSpellName, energyToChargingSpell);
            stats.energy += excessEnergy;
            //Debug.Log("charging spell: " + energyToChargingSpell);
        }
        return true;
    }

    public void ChargeSpell(string pressedSpellName, SpellInfo si, BodyStats stats)  //Spell charging a spell
    {
        CreateSpellIfDoesntExist(pressedSpellName);
        float energyToChargingSpell;
        float energyUsed;
        if (isMelee)
        {
            energyUsed = stats.getSkillValue("stanimaTransferToSpellRate") * Time.deltaTime;
            if (energyUsed > si.currentEnergy)
                energyUsed = si.currentEnergy;
            energyToChargingSpell = energyUsed * stats.getSkillValue("stanimaToEnergyRatio");
            si.currentEnergy -= energyUsed;
            float excessEnergy = _ChargeSpell(pressedSpellName, energyToChargingSpell);
            si.currentEnergy += energyUsed;
        }
        else  //norm or self cast
        {
            energyToChargingSpell = (stats.getSkillValue("energyTransferRate")) * Time.deltaTime;
            if (energyToChargingSpell > si.currentEnergy)
                energyToChargingSpell = si.currentEnergy;
            si.currentEnergy -= energyToChargingSpell;
            float excessEnergy = _ChargeSpell(pressedSpellName, energyToChargingSpell);
            si.currentEnergy += excessEnergy;
        }
    }

    private void CreateSpellIfDoesntExist(string pressedSpellName)  //this whole layer can be removed at some point, move funcs into _createSpell
    {
        if (chargingSpellName == pressedSpellName && currentSpell == null) 
        {
            SpellHasDied();
            Debug.Log("Spell died while charging");
        }

        if (chargingSpellName == "")
        {
            if (_createSpell(pressedSpellName))
            {
                chargingSpellName = pressedSpellName;
                isCharging = true;
            }
            else
                Debug.LogError("spell creation failure for spell " + pressedSpellName);
        }
    }

    private float _ChargeSpell(string pressedSpellName, float energyToChargingSpell)
    {
        //placement();

        if (holdingChargedSpell)
            return energyToChargingSpell;

        bool willFire = false;
        //transfer energy and return excess, but alter fire mode as well
        float energyLossFromEfficency = energyToChargingSpell * MaterialDict.Instance.GetEnergyConversion(currentSpell.spellInfo.materialType) * spellBridgeParent.bodyStats.getSkillValue("energyUseEfficiency"); //Int Bodystats probably one level up
        float energyLossFromMelee = EnergyLossFromMeleeCharge(energyToChargingSpell - energyLossFromEfficency);
        float energyPassing = energyToChargingSpell - energyLossFromEfficency - energyLossFromMelee;
        //Debug.Log(string.Format("EnergyIn: {0}, EnergyLossFromEfficency(mat & player): {1}, energyLossFromMelee: {2}, energy passing in total to spell charge {3}",energyToChargingSpell,energyLossFromEfficency,energyLossFromMelee,energyPassing));
        LiveWorldStats.Instance.AddWorldEnergy(energyLossFromEfficency + energyLossFromMelee);
        float overchargeEnergy = currentSpell.ChargeThisSpell(energyPassing, spellBridgeParent.bodyStats);

        if ((currentSpell.spellInfo.castOnChargeParam == GV.CastOnCharge.CastNoRepeat || currentSpell.spellInfo.castOnChargeParam == GV.CastOnCharge.CastWithRepeat ) &&  overchargeEnergy > 0)
            willFire = true;

        if(currentSpell.spellInfo.castOnChargeParam == GV.CastOnCharge.Hold && overchargeEnergy > 0)
            holdingChargedSpell = true;

        //if(currentSpell.spellInfo.castOnCharge && overchargeEnergy > 0)
        //    willFire = true;

        if (willFire)
        {
            SpellInfo tempLink = currentSpell.spellInfo; //since fire spell kilsl current spell
            spellBridgeParent.FireSpell(chargingSpellName);
            if (tempLink.castOnChargeParam == GV.CastOnCharge.CastNoRepeat) //only place a spell can repeat after cast is it if was cast on reaching charge
                spellBridgeParent.LockSpell(chargingSpellName);
        }
        return overchargeEnergy;
    }

    private float EnergyLossFromMeleeCharge(float energyIn)
    {
        if (!isMelee)
            return 0;
        float range = Mathf.Max(0,Mathf.Min(currentSpell.spellInfo.melee_maxRange, GV.MELEE_RANGE_MAX));
        float energyLoss = range * (GV.MELEE_RANGE_EFF_LOSS_PER_METER - spellBridgeParent.bodyStats.getSkillValue("meleeRangeEff")) * energyIn;
        Mathf.Min(energyIn * .95f, energyLoss);     //HACK
        return energyLoss;
    }

    public void LaunchSpell()
    {
        //Debug.Log("launch spell: " + chargingSpellName);
        if (currentSpell)
        {
            currentSpell.spellInfo.spellPos = transform.position;
            currentSpell.GetComponent<Rigidbody2D>().velocity = this.gameObject.GetComponent<Rigidbody2D>().velocity;
            currentSpell.LaunchThisSpell(spellBridgeParent.bodyStats, casterID);
            ResetSpellBridgeValues();

        }
        else
        {
            if(chargingSpellName != "")
                SpellHasDied();
        }
    }

    private void ResetSpellBridgeValues()
    {
        storedEnergy = 0;
        isCharging = false;
        chargingSpellName = "";
        currentSpell = null;
        holdingChargedSpell = false;
    }

    public void SpellHasDied()
    {
		spellBridgeParent.LockSpell(chargingSpellName);
        ResetSpellBridgeValues();
        //spellBridgeParent.SpellHasDied();
    }

    public void placement()
    {
        if (!currentSpell)
            return; //sometimes pcs can cull this null
        //every update we are going to place the spell at the some location around player
        //called by player every update if charging

        //if !charging, then this should be invisible?
        //if charging, then we need to place @ reticle
        //chargingSprite.transform = new Vector2(new Vector2(caster.head.x, caster.head.y) + new Vector2(caster.transform.x, caster.transform.y));
        Vector3 displacement = new Vector3(0,0);
        if (!isMelee && !isSelfCast)
        {
            displacement = currentSpell.transform.localScale.x * spellBridgeParent.castingDirection * .1f; //
            float ang = GV.Vector2ToAngle(spellBridgeParent.castingDirection);
            currentSpell.transform.eulerAngles = new Vector3(0, 0, ang);
            displacement.z = 0;
            Vector2 goalLoc = spellBridgeParent.spellChargeLocation.position + displacement;

            if (currentSpell.spellInfo.spellForm == GV.SpellForms.Energy)
            { //charging energy spell
                
                currentSpell.gameObject.transform.position = goalLoc;
                currentSpell.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
            }
            else
            { //physical spells move towards

                float maxSpeed = spellBridgeParent.bodyStats.getSkillValue("spellPlacementSpeed");
                currentSpell.MoveTowardsPlacement(goalLoc, maxSpeed);
                //Vector2 forceToApply = new Vector2((maxSpeed + Physics2D.gravity.x), (maxSpeed + Physics2D.gravity.y)) * -1f * currentSpell.spellInfo.mass; //Basic force to cancel gravity

                //Vector2 
                //Vector2 nextStep = Vector3.MoveTowards(currentSpell.gameObject.)

            }
        }
        else
        { //melee or selfcast always same location, no displacement
            currentSpell.gameObject.transform.position = spellBridgeParent.spellChargeLocation.position + displacement;
            currentSpell.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
        }
    }
    

    public void CreateCrashExplosion(float kineticEnergy)
    {  //This is used for creating the force self cast explosion when colliding with ground really fast
        if (currentSpell)
            LaunchSpell();
        _createSpell("groundCollide");
        currentSpell.spellInfo.currentEnergy = kineticEnergy;

        LaunchSpell();
    }
  

    private Spell _createSpell(string toCreate) {
        SpellStorage spellStorage = StaticReferences.mainScriptsGO.GetComponent<LiveSpellDict>().GetSpell(toCreate);
        if (spellStorage == null)
            return null; //spell was empty

        GameObject newSpell = Instantiate(Resources.Load("Prefabs/Spell/BasicSpell"), new Vector2(spellBridgeParent.spellChargeLocation.position.x, spellBridgeParent.spellChargeLocation.position.y), Quaternion.identity) as GameObject;
        newSpell.GetComponent<SpellInitializer>().InitializeSpell(spellStorage, this);        
        currentSpell = newSpell.GetComponent<Spell>();
        currentSpell.spellInfo.spellState = GV.SpellState.Charging;
        switch (currentSpell.spellInfo.energyLimitType)
        {
            case GV.EnergyLimitType.None:
                currentSpell.spellInfo.energyLimit = GV.SPELL_MAX_ENERGY;
                break;
            case GV.EnergyLimitType.Constant:
                //Values already set so dont need to do anything
                break;
            case GV.EnergyLimitType.PercentOfCasterMax:
                currentSpell.spellInfo.energyLimit = spellBridgeParent.GetMaxEnergy() * currentSpell.spellInfo.energyLimit; //the amount stored is a float (0-1) hopefully, is percentage of parent max energy
                break;
        }
        currentSpell.ActivateAnimations(); //saves the animation layer on the spell
        currentSpell.spellAnimManager.AddAnimation(GV.SpellAnimationType.GatheringDebris);
        spellBridgeParent.SetChargingLocation(currentSpell.spellInfo);
        //SetChargeLocation();
        chargingSpellName = toCreate;
        currentSpell.gameObject.AddComponent<SpellLayerManager>().Initialize(spellBridgeParent.pid,spellBridgeParent.parentIsSpell,currentSpell.spellInfo);
        
        /*if (GetComponent<SpellLayerManager>())//this spell wasnt mature yet, so auto mature
            GetComponent<SpellLayerManager>().SpellMatures();*/

        /*if (currentSpell.spellInfo.isMelee)
        {
            float range = Mathf.Min(currentSpell.spellInfo.melee_maxRange, GV.MELEE_RANGE_MAX);
            melee_rangePerEnergy = Mathf.Max((currentSpell.spellInfo.melee_maxRange / (GV.MELEE_RANGE_CONSTANT * currentSpell.spellInfo.melee_maxRange_energy)),1);
        }*/
        placement();
        return currentSpell;
    }

    private void SetChargeLocation()
    {
        if (isMelee)
        {
            spellBridgeParent.spellChargeLocation = spellBridgeParent.bodyPartChargeTransform;
        }
        else if (isSelfCast)
        {
            spellBridgeParent.spellChargeLocation = spellBridgeParent.selfCastChargeLocation;
        }
        else if(currentSpell.spellInfo.materialType == GV.MaterialType.Charisma)
        {
            spellBridgeParent.spellChargeLocation = spellBridgeParent.headChargeLocation;
        }
        else
        {
            spellBridgeParent.spellChargeLocation = spellBridgeParent.reticleChargeTransform;
        }
    }

    public StartState modifyStartState(StartState ss)
    {

        return ss;
    }

    public bool retIsCharging()
    {
        return isCharging;
    }

	public bool isSameSpell(string spellNameChecking){
		return spellNameChecking == chargingSpellName;
	}
    
}

public class SpellBridgeParent   //THIS REAAAALLLY SHOULD BE AN INTERFACE WITH AN INITIALZIE FOR THE TRANFSORMS
{
    PlayerControlScript pcsParent;
    public Spell spellParent;       //HACKS SET BACK TO PRIVATE WHEN HACK REMOVED
    public bool parentIsSpell;

    public Vector2 castingDirection{get{ return (pcsParent != null ? pcsParent.head : spellParent.facingDirVector);}}
	public float castingAngle { get { return (pcsParent != null ? pcsParent.reticleAngle : spellParent.facingAng); } }
    public BodyStats bodyStats{get{ return ((pcsParent != null) ? pcsParent.stats : spellParent.bodyStats);}}
    public int pid { get { return ((pcsParent != null) ? pcsParent.pid : spellParent.casterID); } }

    public Transform reticleChargeTransform;  //transform of normale cast start
    public Transform bodyPartChargeTransform; //transform of punch location
    public Transform selfCastChargeLocation;
    public Transform headChargeLocation;

    public Transform spellChargeLocation;     //active transform
    //This should be set to 

    //pcs.reticleSprite.transform)
    public SpellBridgeParent(PlayerControlScript pcs) //do i need ref?
    {
        pcsParent = pcs;
        parentIsSpell = false;
        reticleChargeTransform = pcs.reticleChargingLocation;
        bodyPartChargeTransform = pcs.handPunchingLoc;
        spellChargeLocation = reticleChargeTransform;
        selfCastChargeLocation = pcs.selfCastReticle;
        if(pcs.avatarManager != null)  //npcs dont have
            headChargeLocation = pcs.avatarManager.avatarLimbDict["head"];
        else
            headChargeLocation = pcs.reticleChargingLocation;
    }

    public SpellBridgeParent(Spell _spell)  //spell at some point will need reticles
    {
        parentIsSpell = true;
        spellChargeLocation = _spell.centerReticle;
        reticleChargeTransform = _spell.rangedReticle;
        bodyPartChargeTransform = _spell.rangedReticle;
        selfCastChargeLocation = _spell.centerReticle;
        spellParent = _spell;
        headChargeLocation = selfCastChargeLocation;
    }

    public void SetChargingLocation(SpellInfo si)
    {
        if(parentIsSpell)
        {
            if(si.isSelfCast)
                spellChargeLocation = spellParent.centerReticle;
            else
                spellChargeLocation = spellParent.rangedReticle;
        }
        else
        {
            if (pcsParent.isPlayerControlled)
            {
                if (si.isSelfCast)
                {
                    spellChargeLocation = pcsParent.avatarManager.avatarLimbDict["utorso"];
                }
                else if (si.isMelee)
                {
                    switch(si.meleeCastType)
                    {
                        case GV.MeleeCastType.basicPunch:
                            spellChargeLocation = pcsParent.avatarManager.avatarLimbDict["lhand"];
                            break;
                        case GV.MeleeCastType.kick:
                            spellChargeLocation = pcsParent.avatarManager.avatarLimbDict["lfoot"];
                            break;
                        default:
                            spellChargeLocation = pcsParent.avatarManager.avatarLimbDict["utorso"];
                            Debug.Log("Unhandled switch " + si.meleeCastType);
                            break;
                    }
                }
                else
                {
                    spellChargeLocation = pcsParent.reticleChargingLocation;
                }
            }
            else
                spellChargeLocation = pcsParent.transform;
        }


    }

    public void FireSpell(string spellName)
    {
        if (pcsParent)
            pcsParent.BeginSpellLaunch(spellName);
        else if (spellParent)
            spellParent.LaunchChargingSpell();
    }

    public float GetMaxEnergy()
    {
        if (pcsParent)
            return pcsParent.stats.energy;
        else
            return spellParent.spellInfo.currentEnergy;
    }

    public void SpellHasDied()
    {
        if (pcsParent)
            pcsParent.ChargingSpellDied();
        else
            spellParent.ChargingSpellDied();
    }

    public GameObject GetParentGameObject()
    {
        if (pcsParent)
            return pcsParent.gameObject;
        return spellParent.gameObject;
    }

    public void LockSpell(string lockName)
    {
        if (pcsParent)
            pcsParent.LockSpell(lockName);
        else
            spellParent.LockSpell(lockName);
    }
}
