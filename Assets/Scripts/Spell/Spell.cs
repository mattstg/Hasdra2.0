using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Spell : MonoBehaviour, TemperatureSensitive, DestructibleInterface
{
    public bool DEBUG_GrowHacks = false; //just for debugging the rock
    public bool DEBUG_CauseExplosion = false; //just for debugging the rock
    public float DEBUG_MaxEnergy = 1000;
    public bool DEBUGMODE = false;
   
    public SpellStateMachine spellStateMachine;
    public SpellBridge spellBridge;
    public SpellAnimationManager spellAnimManager;
    public StartState startState;
    public SpellInfo spellInfo;
    public BodyStats bodyStats = null;
    public SpriteRenderer sprite;
    public Destructible2D.D2dRepeatStamp stampRepeater;
    public int casterID;
    public bool stateMachineActive = false;
	public string spellID;
    public SpellCore spellCore;

    public Transform rangedReticle;
    public Transform centerReticle;

    Dictionary<SkillModSS, SkillModifier> storedSkillModifiers = new Dictionary<SkillModSS, SkillModifier>();
    List<SkillModifier> onChargeSkillMods = new List<SkillModifier>(); //placed here once launched or is full of charge (for constant ones)

    public List<PlayerControlScript> hasDealtDmgAsSpell = new List<PlayerControlScript>();
    public List<PlayerControlScript> hasDealtDmgAsExplosion = new List<PlayerControlScript>();

    Rigidbody2D _rigidBodyFix = null;
    Rigidbody2D spellRigidBody2D { set { _rigidBodyFix = value; } get { return (_rigidBodyFix != null) ? _rigidBodyFix : GetComponent<Rigidbody2D>(); } }
	public float facingAng { get { return GV.Vector2ToAngle(gameObject.transform.right); } }
    public bool forcedStampWaiting = false;
	/*
	public float facingAng (){
		Vector3 forward = gameObject.transform.right;
		float angleToRet = Vector2.Angle(new Vector2(1, 0), gameObject.transform.right);
		if (forward.y < 0) {
			return -angleToRet;
		} else
			return angleToRet;
	}*/

    public Vector2 facingDirVector; //this should be a 'get' from facingDir
    string spellCreating;
    SpellDestabilization spellDestabilization;
    protected bool allowedInteractions = true;
    
    public float temperature;

    float _excessEnergyToAdd = 0; //excess left over from, leave at zero plz

    public void Initialize(SpellBridge parentCaster) //called by spell initializer to create things needed before Start()
    {
        spellAnimManager = gameObject.AddComponent<SpellAnimationManager>();
        spellAnimManager.InitializeAnimManager(spellInfo,this);
        if(spellInfo.spellForm == GV.SpellForms.Energy)
            spellAnimManager.AddAnimation(GV.SpellAnimationType.Unstable); //It wont activate, but it'll be loaded and ready
        spellBridge = gameObject.GetComponent<SpellBridge>();
        spellBridge.InitializeSpellBridge(this,parentCaster);
        spellInfo.spellBridge = spellBridge;
        //dounno if i should add the stuff from start below... lets wait till this is stable first
    }

    public void Start() //any subclass that overwrites this needs to call Base.Update()
    {
        spellRigidBody2D = this.gameObject.GetComponent<Rigidbody2D>();
        //spellDestabilization = this.GetComponent<SpellDestabilization>();
        spellDestabilization = gameObject.AddComponent<SpellDestabilization>();
        spellDestabilization.Initialize(spellRigidBody2D, this);
		spellID = Time.deltaTime.ToString () + casterID.ToString () + spellBridge.currentSpell + Random.Range(0,10000).ToString();
		spellInfo.uniqueSpellID = spellID;
        sprite = GetComponent<SpriteRenderer>();
        if (spellInfo.spellForm == GV.SpellForms.Energy)
        {
            stampRepeater = GetComponent<Destructible2D.D2dRepeatStamp>();
            stampRepeater.Delay = .16f; // disables it
        }
    }

    public void Update()
    {
        float dt = Time.deltaTime;
        spellInfo.UpdateInternal();

        if (spellInfo.spellState == GV.SpellState.Launched)
            UpdateAsSpell(dt);
        else if (spellInfo.spellState == GV.SpellState.Charging)
            UpdateAsChargingSpell(dt);
        else if (spellInfo.spellState == GV.SpellState.Exploding)
            UpdateAsExplosion();
        else if (spellInfo.spellState == GV.SpellState.FinishedExplosion)
            UpdateAsFinishedExplosion();

        if (spellInfo.spellForm == GV.SpellForms.Energy)
        {
            stampRepeater.Hardness = GV.EXPLOSION_STAMP*spellInfo.currentEnergy;
            stampRepeater.Size = transform.localScale * .2f; //Cuz of sprite size
        }

        if(forcedStampWaiting && stampRepeater != null)
        {
            forcedStampWaiting = false;
            stampRepeater.ForceStamp();
        }
    }

    private void UpdateAsFinishedExplosion()
    {
        Fizzle();
    }

    public void UpdateAsChargingSpell(float dt)
    {
        if (!spellInfo.initialized)
        {
            Debug.LogError("aww something has to be wrong for this to happen");
            return;
        }

        Debug.Log("Updating, energy current: " + spellInfo.currentEnergy + ", " + spellInfo.energyInSideBank);
        spellInfo.currentEnergy += spellInfo.energyInSideBank;
        spellInfo.spellPos = transform.position;
        spellInfo.currentAngle = facingAng;

        StaticReferences.numericTextManager.CreateNumericDisplay(this, transform, "SpellEnergy", "", spellInfo.currentEnergy, Color.yellow, true);

        SetSizeAndMass();
        if (spellDestabilization.StabilizeUpkeep())
            Explode();

        if (spellInfo.currentEnergy <= 0)
        {
            Debug.Log("Dis fizzle from happend during charging");
            Fizzle();
        }

        spellAnimManager.UpdateAnimations();
    }

    public void UpdateAsSpell(float dt)
    {
        if (!spellInfo.initialized)
        {
            Debug.LogError("Holy fuck something really has to be wrong for this to happen");
            return;
        }

        Debug.Log("Updating, energy current: " + spellInfo.currentEnergy + ", " + spellInfo.energyInSideBank);
        spellInfo.currentEnergy += spellInfo.energyInSideBank;
        spellInfo.spellPos = transform.position;
        spellInfo.currentAngle = facingAng;

        if (spellInfo.spellState == GV.SpellState.Launched)
        {
            DeductUpkeep();
            spellInfo.timeAlive += dt;
        }

        if (DEBUG_CauseExplosion)
        {
            Explode();
            DEBUG_CauseExplosion = false;
        }
        if (DEBUG_GrowHacks)
        {
            spellInfo.currentEnergy += dt * 40;
            spellInfo.currentEnergy = (spellInfo.currentEnergy >= DEBUG_MaxEnergy) ? DEBUG_MaxEnergy : spellInfo.currentEnergy;
        }

        StaticReferences.numericTextManager.CreateNumericDisplay(this, transform, "SpellEnergy", "", spellInfo.currentEnergy, Color.yellow, true);

        spellInfo.velocity = spellRigidBody2D.velocity.magnitude;
        spellInfo.velocityVector = spellRigidBody2D.velocity;
        spellInfo.altitude = gameObject.transform.position.y;

        if (stateMachineActive)
            UpdateStateMachine();

        SetSizeAndMass();
        if (spellDestabilization.StabilizeUpkeep())
            Explode();

        if (spellInfo.currentEnergy <= 0)
        {
            Debug.Log("Dis fizzle");
            Fizzle();
        }

        spellAnimManager.UpdateAnimations();

        if (spellInfo.spellState == GV.SpellState.Launched)
            ApplyStoredForceToVelo();
    }

    public void UpdateAsExplosion()
    {
        spellInfo.currentExpoTime += Time.deltaTime;
        transform.position = spellInfo.spellPos;
        
        StaticReferences.numericTextManager.CreateNumericDisplay(this, transform, "ExplosionDensity", "", spellInfo.density, Color.grey, true);

        if (spellInfo.currentEnergy <= 0 || spellInfo.currentExpoTime >= spellInfo.maxExplosionTime)
        {
            Fizzle();
        }
        else
        {
            //Debug.Log("implode dens: " + spellInfo.implodeDensityMod.ret(spellInfo.currentExpoTime + 1));
            //Debug.Log("explode dens: " + spellInfo.explodeDensityMod.ret(spellInfo.currentExpoTime + 1));

            if (spellInfo.directionalDamageType == GV.DirectionalDamage.implosion)
                spellInfo.density = spellInfo.implodeDensityMod.ret(spellInfo.currentExpoTime + 1); // + 1 at the end because we are using a non zeroes Asymptote
            else
            {
                //Debug.Log("v: " + spellInfo.explodeDensityMod.ret(spellInfo.currentExpoTime + 1));
                spellInfo.density = spellInfo.explodeDensityMod.ret(spellInfo.currentExpoTime + 1); // + 1 at the end because we are using a non zeroes Asymptote
                //Debug.Log("density result: " + spellInfo.density);
            }

            //Debug.Log(" explosion ass storage returns " + spellInfo.explodeDensityMod.ret(spellInfo.currentExpoTime + 1));
            /*if (spellInfo.directionalDamageType != GV.DirectionalDamage.implosion)
            {
                spellInfo.setScale = spellInfo.setScale + new Vector2(2, 2) * Time.deltaTime;
                if (spellInfo.density <= .1)
                    spellInfo.spellState = GV.SpellState.FinishedExplosion;
            }
            else
            {
                spellInfo.setScale = spellInfo.setScale - new Vector2(3, 3) * Time.deltaTime;
                if (spellInfo.setScale.x <= .1 || spellInfo.setScale.y <= .1)
                    spellInfo.spellState = GV.SpellState.FinishedExplosion;
            }
            */
            SetSizeAndMass();
        }
    }

    /*private void SetDensity(float d)
    {
        spellInfo.boom = ((1 / d) - (spellInfo.initialSetScale.x * spellInfo.initialSetScale.y * 2 * .7f)) / (spellInfo.initialSetScale.x * spellInfo.initialSetScale.y * 2 * spellInfo.currentEnergy *.12f);
         //   1 / (spellInfo.initialSetScale.x * spellInfo.initialSetScale.y * 2 * (BalanceFormulaDict.Instance.GetValue("spellScale", spellInfo.currentEnergy)));
    }*/


	public void LaunchThisSpell(BodyStats bs, int playerID)
    {
        spellInfo.timeForCasterImmunity = GV.CASTER_IMMUNITY + Time.time;
        bodyStats = new BodyStats(bs,spellInfo);
        casterID = playerID;
        spellInfo.spellState = GV.SpellState.Launched;
        onChargeSkillMods.AddRange(spellInfo.onChargeSkillMods);
        if (spellInfo.isSelfCast)
        {
            GetComponent<SpellLayerManager>().SpellMatures();
            Explode();
        }
        else
        {
            GetComponent<SpellLayerManager>().FireSpell();
            spellDestabilization.spellFired();
            ToggleStateMachine(true);
            ApplyStoredVelocityForceForLaunch();
            LiveWorldStats.Instance.AddWorldEnergy(_excessEnergyToAdd);
            _excessEnergyToAdd = 0;
            spellInfo.relData.CreateAllRelativeDatas();
        }
    }

    public void LaunchChargingSpell()
    {
        spellBridge.LaunchSpell();
        spellInfo.isCreating = false;
    }

    public void ChargingSpellDied()
    {
        spellInfo.isCreating = false;
    }

    /*public void SetSizeMassSpriteShape()
    {
        SetSizeAndMass();
        this.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(MaterialDict.Instance.GetMaterialInfo(spellInfo.materialType).spriteLocation);
        if (spellInfo.shape == GV.SpellShape.Square)
            this.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/Spells/SquareRock");
    }*/

    public void Fizzle()
    {
        GV.Destroyer(this.gameObject);
        if (spellInfo.spellState == GV.SpellState.Charging) //this spell is currently being charged
            spellInfo.GetNCastersBack(1).SpellHasDied();

        if (spellInfo.isTransforming || spellInfo.isCreating) //the last living refrence to isTransforming, i shlall build a monument
        {
            spellBridge.LaunchSpell();
        }
    }

    public void SetSizeAndMass()
    {
        if (spellInfo.currentEnergy > 0)
        {
            SetSize();
            SetMass();
            SetAlpha();
        }
        else
        {
            this.transform.localScale = new Vector3(.1f, .1f,1);
            spellRigidBody2D.mass = 1;
        }
    }

    private void SetAlpha()
    {
        Color color = spellInfo.spellColor;

        if (spellInfo.spellState == GV.SpellState.Charging && spellInfo.isMelee)  //if a spell is charging and melee it is invis
            color.a = 0;
        else
            color.a = spellInfo.alpha;

        if (spellInfo.spellState == GV.SpellState.Exploding)
            color.a = Mathf.Min(spellInfo.alpha, spellInfo.density);

        if (sprite)
            sprite.color = color;
        else
            GetComponent<Destructible2D.D2dDestructible>().Color = color;
    }

    private void SetSize() //returns radius
    {
        float scaleMult = BalanceFormulaDict.Instance.GetValue("spellScale", spellInfo.currentEnergy); // * MaterialDict.Instance.GetScaleMult(spellInfo.materialType);
        Vector2 scale = spellInfo.setScale * scaleMult; // * spellInfo.boom;
        //Debug.Log("boom: " + spellInfo.boom);
        this.transform.localScale = scale;
    }

    public void Explode(GV.DirectionalDamage dirType)
    {
        spellInfo.directionalDamageType = dirType;
        Explode();
    }

    public virtual void Explode()
    {
        spellInfo.timeForCasterImmunity = 0;
        spellInfo.velocity = 0;
        spellInfo.spellState = GV.SpellState.Exploding;
        spellInfo.stability = 0;
        spellAnimManager.ClearAllAnimations();
        spellInfo.densityAtExplosion = spellInfo.density;
        GetComponent<SpellLayerManager>().SpellMatures();
        float implosionDensity = Mathf.Min(spellInfo.densityAtExplosion * 10, 100);
        float explosionLimit = (1 / (GV.SPELL_MAXMIN_SET_SCALE.y * GV.SPELL_MAXMIN_SET_SCALE.y)) - .01f;
        spellInfo.explodeDensityMod = new AssStorage(1 + 1, explosionLimit,  spellInfo.densityAtExplosion, 80, GV.HorzAsym.MaxToMin); //+1 to zero the asyptote, need to zero the time input by adding 1
        spellInfo.implodeDensityMod = new AssStorage(1 + 1, spellInfo.densityAtExplosion, implosionDensity, 80, GV.HorzAsym.MinToMax); //+1 to zero the asyptote, need to zero the time input by adding 1
        

        if (spellInfo.currentEnergy <= 0)
            Fizzle();
        if (spellInfo.spellState == GV.SpellState.Charging)
            spellInfo.GetNCastersBack(1).SpellHasDied();
        //if phsyical, should explode differently for sure
        //ExplosionFactory.Instance.MakeExplosion(materialType, explosionForce, Explosion.ExplosionType.Explosion, new Vector2(0, 0), transform.position);
        if (spellInfo.spellForm == GV.SpellForms.Physical)
        {
            Fracture(true);
        }
        else
        {
            //List<SkillModifier> _skillMods = new List<SkillModifier>(storedSkillModifiers.Values);
            //_skillMods.AddRange(onChargeSkillMods);
            //ExplosionFactory.Instance.MakeExplosion(spellInfo, transform.position, _skillMods);
            //GV.Destroyer(this.gameObject);
            //allowedInteractions = false; //This thing here is a dirty bug fix, try to legit fix it when u figure out why can have multiple collision per 1 update before getting destroyed
        }
        
    }

    private void SetMass()
    {
        spellRigidBody2D.mass = spellInfo.GetMass();
    }

    public void DeductUpkeep()
    {
        float energyLossToUpkeep = GV.SPELL_UPKEEP[(int)spellInfo.spellForm] * Time.deltaTime  * spellInfo.currentEnergy;
        energyLossToUpkeep = Mathf.Max(energyLossToUpkeep, bodyStats.getSkillValue("flatMinSpellUpkeep")*Time.deltaTime);
		if (spellInfo.isMelee)
			energyLossToUpkeep *= bodyStats.getSkillValue ("meleeDecay");
        energyLossToUpkeep = Mathf.Min(energyLossToUpkeep, spellInfo.currentEnergy);
		LiveWorldStats.Instance.AddWorldEnergy(energyLossToUpkeep);
        spellInfo.currentEnergy -= energyLossToUpkeep;
    }

    public void ApplyStoredForceToVelo() //don't call externally, only exception is spellBridge for solid placement
    {
        spellRigidBody2D.AddForce(spellInfo.forceStoredForVelo, ForceMode2D.Impulse);
        spellInfo.forceStoredForVelo = new Vector2(0, 0);
    }

    public void ApplyVeloState(Vector2 curPos, Vector2 goalPos, float speedLimit, GV.IgnoreXY ignorexy)
    {
        Vector2 diffVector = goalPos - curPos;
        float distanceToGoal = diffVector.magnitude;
        if (distanceToGoal < GV.SS_POSITION_ACCEPTABLE_DISTANCE) //if too short a distance, do nothing
            return;
        float speed = (distanceToGoal > speedLimit) ? speedLimit : distanceToGoal;
        ApplyVeloState(diffVector, speed, ignorexy);
    }

    public void ApplyVeloState(float ang, float speed, GV.IgnoreXY ignorexy)
    {
        ApplyVeloState(GV.DegreeToVector2(ang), speed, ignorexy);
    }

    public void MoveTowardsPlacement(Vector2 goalPos, float speed) //Costs no energy, used for SpellBridge.Placement()
    {
        SetMass(); //Make sure stored mass is set, so accurate force is used
        Vector2 desiredVelo = Vector2.MoveTowards(transform.position,goalPos,speed) - GV.V3toV2(transform.position);
       // Debug.Log(string.Format("cur pos: {0}, goal pos {1}, move towards {2}, desiredVelo {3} ", transform.position, goalPos, Vector2.MoveTowards(transform.position, goalPos, speed), desiredVelo));

        Vector2 forceToCancelGravity = (Physics2D.gravity * spellInfo.gravityScale * spellInfo.mass) * -1 * Time.deltaTime;
        //Debug.Log(string.Format("forceToCancelGravity{0} = grav{1} * gravScale {2} * mass{3}", forceToCancelGravity, Physics2D.gravity, spellInfo.gravityScale, spellInfo.mass));
        Vector2 forceToApply = ((desiredVelo * spellInfo.mass) + forceToCancelGravity);
        //Debug.Log(string.Format("force to apply {0}, from desiredVelo{1}*mass{2} : {3}, forceToCancelGravity: {4}",forceToApply,desiredVelo,spellInfo.mass, desiredVelo*spellInfo.mass,forceToCancelGravity));
        spellRigidBody2D.AddForce(forceToApply, ForceMode2D.Impulse);

        Vector2 speedLimit =  (goalPos - GV.V3toV2(transform.position)) * 4; //Why times 4? cause its a nice balance, means .25 sec to reach at end
        speedLimit = new Vector2(Mathf.Abs(speedLimit.x), Mathf.Abs(speedLimit.y));
        Vector2 currentSpeed = spellRigidBody2D.velocity;
        if (Mathf.Abs(currentSpeed.x) > speedLimit.x)
            currentSpeed.x = speedLimit.x * Mathf.Sign(currentSpeed.x);
        if (Mathf.Abs(currentSpeed.y) > speedLimit.y)
            currentSpeed.y = speedLimit.y * Mathf.Sign(currentSpeed.y);
        spellRigidBody2D.velocity = currentSpeed;
    }

    private void ApplyVeloState(Vector2 v2, float speed, GV.IgnoreXY ignorexy)
    {
        v2 = v2.normalized;
        float energyToForceEff = bodyStats.getSkillValue("energyPerForce");

        VeloCalcHelper vch_initial = new VeloCalcHelper(spellInfo, v2, speed, ignorexy, energyToForceEff);
        float energyAllowedToTransfer = spellInfo.energyInSideBank = spellInfo.CapEnergyAllowedForVelo(vch_initial.energyRequired) *Time.deltaTime;
        spellInfo.currentEnergy -= energyAllowedToTransfer;

        VeloCalcHelper vch_postReduction = new VeloCalcHelper(spellInfo, v2, speed, ignorexy, energyToForceEff);
        energyAllowedToTransfer = spellInfo.CapEnergyAllowedForVelo(vch_postReduction.energyRequired) * Time.deltaTime;
        spellInfo.energyInSideBank -= energyAllowedToTransfer; //always should be less than in side bank, since has less mass

        Vector2 forceApplying = vch_postReduction.veloDiff.normalized * energyToForceEff * energyAllowedToTransfer;
        spellInfo.forceStoredForVelo += forceApplying;
    }

    public void ApplyStoredVelocityForceForLaunch()
    {
        float initialangle,goalAngle;
        initialangle = goalAngle = spellInfo.initialHeadingAngle;

        float casterAngle = 0;

        if (spellBridge.parentCaster == null)
        {
            Debug.Log("caster was null on launch");
        }
        else
        {
            casterAngle = spellBridge.parentCaster.spellBridgeParent.castingAngle;
        }

        switch (spellInfo.initialLaunchAngleRelType)
        {
            case GV.RelativeLaunchType.World:
                break;
            case GV.RelativeLaunchType.Normal:
                goalAngle += casterAngle;
                break;
            default:
                Debug.LogError("weird default");
                break;
        }
			
		if (spellInfo.isFacingLaunchDir) {
			//Debug.Log ("spellInfo.initialHeadingAngle " + spellInfo.initialHeadingAngle.ToString());
			//Debug.Log ("goal angle is " + spellInfo.initialHeadingAngle);
			//Debug.Log ("casterAngle " + casterAngle);
			//transform.rotation.eulerAngles.Set(0, 0, spellInfo.initialHeadingAngle);
			transform.rotation = Quaternion.Euler (0, 0, goalAngle);
				//transform.forward.Set (GV.DegreeToVector2 (goalAngle).x, GV.DegreeToVector2 (goalAngle).y, 0);
				//Debug.Log ("New Facing Dur " + transform.rotation.eulerAngles.ToString ());
		}

        spellRigidBody2D.velocity = GV.DegreeToVector2(goalAngle) * spellInfo.initialLaunchVelo;
        spellInfo.velocity = spellRigidBody2D.velocity.magnitude; //Fuck i need to rename that
        //rigidBody2D.AddForce(GV.DegreeToVector2(goalAngle) * spellInfo.energyStoredForVeloLaunch * bodyStats.getSkillValue("energyPerForce"), ForceMode2D.Impulse);

        //rigidBody2D.AddForce(GV.RadianToVector2(spellInfo.initialHeadingAngle) * spellInfo.energyStoredForVeloLaunch, ForceMode2D.Impulse);
        //spellInfo.energyStoredForVeloLaunch = 0;
    }

    public void CreateSpellState(SpellLaunchParam slp)
    {
        if (slp.spellName != spellInfo.lockedSpellName)
        {
            spellBridge.ChargeSpell(slp.spellName, spellInfo, bodyStats);
            spellCreating = slp.spellName;
            spellInfo.isCreating = true;
            spellBridge.placement();
        }
    }

    private float DistributeEnergyToChargingSkillMods(float energyTransfered, float energyToSkillModEfficency)
    {
        //skillModChargeRate body stat in charge
        //Debug.Log("energy in: " + energyTransfered); 
        float maxAvailable, amountAvailable;
        maxAvailable = amountAvailable = energyTransfered * GV.SPELL_MAX_PERCENT_CONSUMED_FOR_SKILLMOD;
       //Debug.Log(amountAvailable + "/" + energyTransfered + " ,available/transfered, spell -> skillMods");
 //energyTransfered -= amountAvailable;        
        //percent ones first
       //Debug.Log("skill mod count: " + spellInfo.onChargeSkillMods.Count);
       foreach (SkillModifier skillMod in spellInfo.onChargeSkillMods)
       {
           float amtWantToConsume = maxAvailable * skillMod.percentChargePerSecond;
           amtWantToConsume = Mathf.Clamp(amtWantToConsume, 0, amountAvailable);
           //Debug.Log("want to consume: " + amtWantToConsume + " from " + skillMod.percentChargePerSecond + "*" + energyTransfered);
           amountAvailable -= amtWantToConsume;
           float energySentToCharge = amtWantToConsume * energyToSkillModEfficency;
          // Debug.Log("energy sent into skillmod: " + energySentToCharge);
           float excess = skillMod.Charge(energySentToCharge);
           //Debug.Log("excess returned: " + excess);
           LiveWorldStats.Instance.AddWorldEnergy(amtWantToConsume - energySentToCharge);
           amountAvailable += excess;
          // Debug.Log("at end, amt now available: " + amountAvailable);
       }
       //Debug.Log("energy out: " + (energyTransfered - maxAvailable + amountAvailable)); 
       return energyTransfered - maxAvailable + amountAvailable;
    }

    public float ChargeThisSpell(float energyTransfered, BodyStats _bodystatsOfCaster)//float energyToForceEff, float energyToSkillModEfficency)
    {
        //spellBridgeParent.bodyStats.getSkillValue("energyUseEfficiency"), 
        if (spellInfo.currentEnergy >= GV.SPELL_MIN_ENERGY_BEFORE_CHARGE_SKILLMODS) //then can charge skillmods  SPELL_MAX_PERCENT_CONSUMED_FOR_SKILLMOD
            energyTransfered = DistributeEnergyToChargingSkillMods(energyTransfered, _bodystatsOfCaster.getSkillValue("skillModChargeEfficency"));

        float massToForceRatio = (spellInfo.initialLaunchVelo * GV.SPELL_FORM_WEIGHT_BASE[(int)spellInfo.spellForm]) / _bodystatsOfCaster.getSkillValue("energyPerForce");  //for every unit energy to spell, how much loss to maintain velo
        massToForceRatio++; //increase by one cause math
        float energyToSpell = energyTransfered / massToForceRatio;
        float energyToVelo = energyTransfered - energyToSpell;
        //Debug.Log("massToForceRatio: " + massToForceRatio + ", weight per energy: " + MaterialDict.Instance.GetWeightPerEnergy(spellInfo.materialType) + ", initial velo: " + spellInfo.initialLaunchVelo + ", energyToForceEff: " + energyToForceEff);
        //Debug.Log("energy transfer: " + energyTransfered + ", but is split ==> spell: " + energyToSpell + ", Velo: " + energyToVelo);
        //Debug.Log(string.Format("energy to spell {0}, energy to velo {1}", energyToSpell, energyToVelo));
        if (spellInfo.castOnChargeParam == GV.CastOnCharge.CastNoRepeat || spellInfo.castOnChargeParam == GV.CastOnCharge.CastWithRepeat || spellInfo.castOnChargeParam == GV.CastOnCharge.Hold)
        {
            //Debug.Log("energy limit is set to: " + spellInfo.energyLimit);
            float energyMaxAdd = spellInfo.energyLimit - spellInfo.currentEnergy;
            if (energyToSpell > energyMaxAdd)
            {
                //Debug.Log("trying to add: " + energyToSpell + " but that would overchage, so added: " + energyMaxAdd);
                spellInfo.currentEnergy += energyMaxAdd;
                return energyToSpell - energyMaxAdd;
            }
        }
        spellInfo.currentEnergy += energyToSpell;
        //Debug.Log("energy adding to spell: " + energyToSpell);
        //Debug.Log("current energy: " + spellInfo.currentEnergy + " estimated transfer rate: " + (energyToSpell/Time.deltaTime));
        return 0;
        //calcs make it so...  if 20 incoming, 1:4 ratio.  20/(1+4) is 4, so 4 energy is stored

}

    public virtual void FinishTransformingWithExcessEnergy()
    {
        GV.Destroyer(this.gameObject); 
    }

    public void RotateSpell(float degree)
    {
        //spouse to rotate to that value, and consume energy attempting to as well, with a max rotation speed
        transform.Rotate(0, degree, 0);
        //Debug.Log("Rotate Not handled yet: " + degree.ToString());
    }

    public void OnTriggerStay2D(Collider2D coli)
    {
        if (spellInfo.spellState != GV.SpellState.FinishedExplosion)
            ResolveCollision(coli.gameObject);
    }

    public void OnCollisionStay2D(Collision2D coli)
    {
        if (spellInfo.spellState != GV.SpellState.FinishedExplosion)
        {
            spellInfo.lastPointOfPhysContact = coli.contacts[0].point;
            ResolveCollision(coli.gameObject);
        }
    }

    public void ResolveCollision(GameObject otherObj)
    {
        //collisions may happen with "Collider" which is how colliders are handled in d2d, need to grab the parent (the true object) if thats the case
        //things under resolve collision must use spellInfo.velocity, as when the collision occurs, the rigidBody velo is pretty much 0 (post colision)
        if (otherObj.name == "Collider")
            otherObj = otherObj.transform.parent.gameObject;

        if(otherObj.GetComponent<DestructibleInterface>() != null)
            forcedStampWaiting = true;

        bool repelSelf = false;        

        //Debug.Log("collision occured with tag: " + otherObj.tag);  SHOULDNT THERE BE A DELTA TIME IN HERE??? 
        if (otherObj.CompareTag("Spell"))
        {
            //if the other is a spell, but doesnt have a spell component, its because the colision occured at the child destructible layer
            Spell otherSpell = otherObj.GetComponent<Spell>();
            otherSpell = (otherSpell != null) ? otherSpell : otherObj.transform.parent.GetComponent<Spell>();
            SpellInfo otherSpellInfo = otherSpell.spellInfo;
            if (spellInfo.spellForm == GV.SpellForms.Energy && otherSpellInfo.spellForm == GV.SpellForms.Energy)
            {
                Absorb(otherSpell.GetAbsorbed(spellInfo.currentEnergy), otherSpell.GetComponent<Rigidbody2D>().velocity.magnitude, CoresTouching(otherSpell.transform), GV.BasicColiType.Spell);
                Vector2 dmgDir = GV.GetDamageDirection(otherObj.transform.position, otherSpellInfo.directionalDamageType, otherSpellInfo.currentAngle, transform.position);
                otherSpell.TakeKnockBackDamage(dmgDir, DealDamage(otherObj.GetComponent<Rigidbody2D>().velocity));
                repelSelf = true;
            }
            else if(spellInfo.spellForm == GV.SpellForms.Energy && otherSpellInfo.spellForm == GV.SpellForms.Physical)
            {
                Vector2 dmgDir = GV.GetDamageDirection(otherObj.transform.position, otherSpellInfo.directionalDamageType, otherSpellInfo.currentAngle, transform.position);
                otherSpell.TakeKnockBackDamage(dmgDir, DealDamage(otherObj.GetComponent<Rigidbody2D>().velocity));
                Destructible2D.D2dDestructible d2d = otherObj.GetComponent<Destructible2D.D2dDestructible>();
                bool coresAreTouching = CoresTouching(otherSpell.transform);
                spellDestabilization.EnergyVsPhysicalDestablize(stampRepeater.Hardness, d2d.defensivePower, coresAreTouching, spellInfo.velocityVector.magnitude);
                repelSelf = true;
            }
            else if (spellInfo.spellForm == GV.SpellForms.Physical && otherSpellInfo.spellForm == GV.SpellForms.Physical)
            {
                TakeDamage(otherSpell.DealDamage(spellInfo.velocityVector)); //handles delta time internally
            }
            spellInfo.CollisionDetected(otherSpell);

            /*if(stampRepeater && otherSpell.GetComponent<Destructible2D.D2dDestructible>())  stamp optimization is unoptimized until d2d stampAll is optimized to stampSingle
            {
                //Debug.Log("detected destr in spell, hardness " + otherSpell.GetComponent<Destructible2D.D2dDestructible>().defensivePower);
                stampRepeater.ForceStamp();
            }*/
        }
        /*else if (otherObj.CompareTag("Explosion"))  // handle in explosion
        {
            Explosion explosion = otherObj.GetComponent<Explosion>();
            Absorb(explosion.GetAbsorbed(spellInfo.currentEnergy, spellInfo.materialType), explosion.materialType, 0, CoresTouching(explosion.transform), GV.BasicColiType.Explosion);
            spellInfo.lastBasicColiType = GV.BasicColiType.Explosion;
            spellInfo.lastSpellColiType = explosion.materialType;
        }*/
        else if (otherObj.CompareTag("Terrain"))
        {
            SolidMaterial otherMaterial = otherObj.GetComponent<SolidMaterial>();
            otherMaterial = (otherMaterial != null) ? otherMaterial : otherObj.transform.parent.GetComponent<SolidMaterial>();
            if (spellInfo.spellForm == GV.SpellForms.Energy && otherMaterial.spellInfo.spellForm == GV.SpellForms.Energy)
            {
                float energyAbsorbing = otherMaterial.GetAbsorbed(spellInfo.currentEnergy);
                float velocity = (otherMaterial.GetComponent<Rigidbody2D>()) ? otherMaterial.GetComponent<Rigidbody2D>().velocity.magnitude : 0;
                Absorb(energyAbsorbing, velocity, CoresTouching(otherMaterial.transform), GV.BasicColiType.SolidMaterial, true); //  No longer absorbs through that
                repelSelf = true;
            }
            else if (spellInfo.spellForm == GV.SpellForms.Energy && otherMaterial.spellInfo.spellForm == GV.SpellForms.Physical)
            {
                Destructible2D.D2dDestructible d2d = otherObj.GetComponent<Destructible2D.D2dDestructible>();
                bool coresAreTouching = CoresTouching(otherObj.transform);
                spellDestabilization.EnergyVsPhysicalDestablize(stampRepeater.Hardness, d2d.defensivePower, coresAreTouching, spellInfo.velocityVector.magnitude);
                repelSelf = true;
            }
            //else if (spellInfo.spellForm == GV.SpellForms.Physical && otherMaterial.spellInfo.spellForm == GV.SpellForms.Physical)
            //{
            //    if(spellInfo.velocity >= GV.HAIRLINE_SPEED_MIN && spellInfo.hairlineCooldown >= GV.HAIRLINE_COOLDOWN)
            //    {
            //        float hairlineDefense = MaterialDict.Instance.GetHairlineDefense(otherMaterial.spellInfo.materialType, otherMaterial.spellInfo.density);
            //        float dmg = DealDamage(new Vector2(0, 0)); //Assume solidmaterial isn't moving, if it is, doesn't matter really
            //        dmg /= Time.deltaTime; //undelta time it
            //        if (dmg > hairlineDefense)
            //        {
            //            spellInfo.hairlineCooldown = 0;
            //            float incomingAngle = GV.Vector2ToAngle(spellInfo.velocityVector);
            //            HairlineSplitterFactory.Instance.CreateStampingHairline(hairlineDefense, dmg, otherMaterial.gameObject, incomingAngle, spellInfo.lastPointOfPhysContact);
            //        }
            //    }
            //    //
            //    //Debug.Log("ground dealing dmg to eachother eventaully");
            //    //TakeDamage(otherMaterial.DealDamage(spellInfo.velocityVector), otherMaterial.spellInfo.materialType); //handles delta time internally
            //}
            spellInfo.CollisionDetected(otherMaterial);
        }
        else if (otherObj.CompareTag("Player"))
        {
            PlayerControlScript pcs = otherObj.GetComponent<PlayerControlScript>();
            if (!(spellInfo.timeForCasterImmunity >= Time.time && pcs.pid == casterID))
            {
                if (spellInfo.spellForm == GV.SpellForms.Energy)
                {
                    List<PlayerControlScript> pastPcs = (spellInfo.spellState == GV.SpellState.Exploding) ? hasDealtDmgAsExplosion : hasDealtDmgAsSpell;
                    float dealDamage = DealDamage(otherObj.GetComponent<Rigidbody2D>().velocity) / Time.deltaTime; //converts back to non-dt
                    if (!pastPcs.Contains(pcs))
                    {
                        pastPcs.Add(pcs);
                        if (!(spellInfo.interactionParams.Contains(GV.InteractionType.Caster_Damage) && pcs.pid == casterID))
                        {
                            pcs.TakeDamage(dealDamage);
                        }
                        if (!(spellInfo.interactionParams.Contains(GV.InteractionType.Caster_Knockback) && pcs.pid == casterID))
                        {
                            float explosionDirection = (spellInfo.directionalDamageType == GV.DirectionalDamage.specifiedDir) ? spellInfo.dmgDirTrueAng : spellInfo.currentAngle;
                            Vector2 damageDirection = GV.GetDamageDirection(transform.position, spellInfo.directionalDamageType, explosionDirection, otherObj.transform.position);
                            //pcs.KnockBackFromDamage(dealDamage, damageDirection, spellInfo.materialType, true);
                            float energyUsedInKnockback = dealDamage;
                            if (!spellInfo.useDefaultForce)
                                energyUsedInKnockback = Mathf.Min(dealDamage, spellInfo.cappedExplosiveForce / GV.SPELL_KNOCKBACK_PER_ENERGY);
                            pcs.FullKnockbackFromDamage(energyUsedInKnockback, damageDirection);
                        }
                        spellDestabilization.CollideWithEntity();
                    }
                }
                else if (spellInfo.spellForm == GV.SpellForms.Physical)
                {
                    bool headshot = pcs.IsHeadshot(gameObject);
                    float dealDamage = DealDamage(otherObj.GetComponent<Rigidbody2D>().velocity); //wonder if player hitting ground will have same problem as spellInfo.velo
                    dealDamage *= (headshot) ? GV.PLAYER_HEADSHOT_BONUS : 1;
					float dmgdlt = pcs.TakeDamage(dealDamage); //diference in velo*mass
                }
                if (!(spellInfo.interactionParams.Contains(GV.InteractionType.Caster_SkillMod) && pcs.pid == casterID))
                {
                    foreach (KeyValuePair<SkillModSS, SkillModifier> kv in storedSkillModifiers)
                        pcs.AddSkillModifier(kv.Value);
                    foreach (SkillModifier sm in onChargeSkillMods)
                        pcs.AddSkillModifier(sm);
                }
                spellInfo.CollisionDetected(pcs);
            }
        }


        //If energy type, always applies it's own knockback to self as normal force
        if (repelSelf)
        {
            Vector2 dmgDir = GV.GetDamageDirection(transform.position, spellInfo.directionalDamageType, spellInfo.currentAngle, otherObj.transform.position) * -1;
            TakeKnockBackDamage(dmgDir, DealDamage(new Vector2(0, 0)));
        }
    }

    //Not currently called anywhere due to speed clipping through ground issues
    private void CollideWithGround(SolidMaterial solidMaterial)
    {
        Debug.Log("uh oh sphagetti oohhhhs");
        //if (spellInfo.spellForm == GV.SpellForms.Energy)
        //{
        //    //When a spell collides with ground, it should burn through it, as well as absorb. Instead of just exploding
        //    //solidMaterial.HitBySpell(spellInfo.currentEnergy * GV.ENERGYFORM_ABSORBPTION_RATE * Time.deltaTime); //This will be used when thier is absorbption instead of just straight explosion
        //    solidMaterial.HitBySpell(spellInfo.currentEnergy); //explosions should deal the actaul one second dmg to ground instead
        //   //Absorb(
        //    float energyAbsorbing =  MaterialDict.Instance.GetAbsorbtionResitance(spellInfo.materialType,solidMaterial.GetSpellInfo().materialType) * spellInfo.currentEnergy;
        //    //Absorb
        //    //spellDestabilization.Absorb(
        //    Explode();
        //}
        //else if (spellInfo.spellForm == GV.SpellForms.Physical)
        //{
        //    
        //} //for smoke as well in future
    }

    public float ChargeSkillMod(string skillAffected, float startEnergy,GV.SkillModScalingType skillModType, float controlValue, float energyTransfered, SkillModSS ssID, bool isDebuff, float percentChargeRate ,GV.ConstantOrPercent EnergyLimitType )
    {
        if(storedSkillModifiers.ContainsKey(ssID))
        {
            return storedSkillModifiers[ssID].Charge(energyTransfered);
        }
        else
        {
            SkillModifier skillMod = new SkillModifier(skillAffected,startEnergy,skillModType,controlValue,energyTransfered,GetInstanceID().ToString(),isDebuff, percentChargeRate, EnergyLimitType );
            storedSkillModifiers.Add(ssID,skillMod);
            return 0;
        }
    }

    public void Absorb(float energyAbsorbing, float otherSpeed, bool coresTouching, GV.BasicColiType colType, bool justDestabOnly = false)
    {
        spellDestabilization.Absorb(energyAbsorbing, otherSpeed, coresTouching, colType);
        if(!justDestabOnly)
            spellInfo.currentEnergy += energyAbsorbing;
    }

    public void TakeKnockBackDamage(Vector2 dir, float energyAmt)
    { //Energy spells being affected only
        if (spellInfo.spellState == GV.SpellState.Launched)
        {
            Vector2 directionOfDmg = dir.normalized;
            Vector2 forceApplying = directionOfDmg * energyAmt * bodyStats.getResistanceValue("resistKnockback");
            spellInfo.forceStoredForVelo += forceApplying;
        }
    }

    public float GetAbsorbed(float totalEnergyOfConsumer)
    {
        //in the future could have overlap, but would be hard to do that mathmatically with shapes
        
        //GV.ENERGYFORM_ABSORBPTION_RATE
        float energyLosing = totalEnergyOfConsumer * Time.deltaTime * GV.SPELL_ABSORB_OTHER;
        energyLosing = (energyLosing > spellInfo.currentEnergy) ? spellInfo.currentEnergy : energyLosing;
        spellInfo.currentEnergy -= energyLosing;
        //Debug.Log(materialType + "(e:" + spellInfo.currentEnergy + ") is being asborbed by " + materialDevouring + "(e:" + totalEnergyOfConsumer + ")");
        //spellDestabilization.GetAbsorbed( energyLosing, materialDevouring,spellInfo.currentEnergy, materialType);
        //stability -= (energyConsuming / (spellInfo.currentEnergy)) * MaterialDict.Instance.GetDistability(materialType, materialBeingAbsorbedBy);
        return energyLosing;
        
        return 0;
    }

    public void UpdateStateMachine()
    {
        State lastState = spellInfo.currentState;
        spellInfo.currentState = spellStateMachine.GetNextState(spellInfo);
        if (lastState != spellInfo.currentState) //state change
        {
            lastState.StateExiting(this);
            spellInfo.ignoreCurrentState = false; //state changed so it's cleansed
        }
        spellInfo.currentState.PreformStateAction(this);
    }

    public void ToggleStateMachine(bool toggle)
    {
        stateMachineActive = toggle; 
        if (!stateMachineActive) //if toggle state machine off exits states
            spellInfo.currentState.StateExiting(this);
    }

    public float DealDamage(Vector2 damageRecieverVelo)
    {
        //How much damage this spell deals
        
        if (spellInfo.spellForm == GV.SpellForms.Energy)
        {
            //Vector2 relativeVelo = damageRecieverVelo - spellInfo.velocityVector;
            //float speed = relativeVelo.magnitude;
            float Ek = spellInfo.currentEnergy;// * speed;
            return Ek * Time.deltaTime * spellInfo.densityEffect;
        }
        else //spellInfo.spellForm == GV.SpellForms.Physical
        {
            Vector2 relativeVelo = damageRecieverVelo - spellInfo.velocityVector; 
            if (Mathf.Abs(relativeVelo.x) < .1f && Mathf.Abs(relativeVelo.y) < .1f)  //see! no sqrt!
                return 0; //too little dmg

            float speed = relativeVelo.magnitude;
            float Ek = .5f * spellInfo.mass * speed * speed;
            return Ek * Time.deltaTime * spellInfo.densityEffect;
        }
    }

    public void TakeDamage(float amount)
    { //occurs in phys vs phys
        //Debug.Log("physical spell takes damage");
        //spellInfo.currentEnergy -= amount * MaterialDict.Instance.GetDamageResistance(spellInfo.materialType, GV.GetSpellFormByMaterialType(materialDealingDamage));
        //spellInfo.currentEnergy = spellInfo.currentEnergy < 0 ? 0 : spellInfo.currentEnergy;
        //Debug.Log(spellInfo.materialType + " is calling takeDamage, with other dealing dmg being " + materialDealingDamage);
        //float destab = totalDmgAmt / spellInfo.currentEnergy;
        spellDestabilization.TakeDamage(amount); //when you take physical dmg, you "absorb" that energy, causing destabilization
        //stability -= (amount / spellInfo.currentEnergy) * MaterialDict.Instance.GetDistability(materialType, materialDealingDamage);
    }
    
    public float EnergyToHeat()
    {
        float energyTurnedIntoHeat = GV.WORLD_FIRE_CONSUMPTION_RATE * temperature * Time.deltaTime; // * MaterialDict.Instance.GetHeatResistance(spellInfo.materialType);
        energyTurnedIntoHeat = (energyTurnedIntoHeat>spellInfo.currentEnergy)?spellInfo.currentEnergy:energyTurnedIntoHeat;
        spellInfo.currentEnergy -= energyTurnedIntoHeat;
        return energyTurnedIntoHeat;
    }

    public void ChangeTemperature(float amountOfHeat, float maxHeat)
    {
        temperature += amountOfHeat; // * MaterialDict.Instance.GetHeatResistance(spellInfo.materialType);
        temperature = temperature > maxHeat ? maxHeat : temperature;
        PostChangeTemperature();
    }

    public virtual void PostChangeTemperature() //overide if this matters, like ice or nature
    {}

    public virtual void ActivateAnimations()
    {
    }

    public bool pastMinThreshold()
    {
        return spellInfo.currentEnergy > GV.MINTHRESHOLD_FOR_HEAT_PRODUCTION;
    }

    public void EmitRadioSignal(float goalAmount, float radioSignal, bool emitOnce, GV.ConstantOrPercent korperc)
    {
        //How to set the max to charge to
        Debug.Log("Emit radio called");
        //if (!spellInfo.ignoreCurrentState)
        //{
        //    spellInfo.ignoreCurrentState = true;
        //    SpellInfo toSend = new SpellInfo(spellInfo);
        //    toSend.materialType = GV.MaterialType.Radio;
        //    toSend.radioFreq = radioSignal;
        //    toSend.currentEnergy = goalAmount;
        //    ExplosionFactory.Instance.MakeExplosion(toSend, transform.position, new List<SkillModifier>());
        //}
    }

    //DestructibleInterface Inherited functions
    public float GetEnergy()
    {
        return spellInfo.currentEnergy;   
    }

    public virtual void Fracture(bool explosive = false)
    {
        if (spellInfo.spellForm == GV.SpellForms.Energy)
            Explode();

        if (explosive)
        {
            float explosiveEnergy = spellInfo.currentEnergy * GV.EXPLOSIVEFRACTURE_ENERGY_PERCENT;
            spellInfo.currentEnergy -= explosiveEnergy;
            gameObject.AddComponent<FractureSpeedBurst>().Initialize(explosiveEnergy,transform.position,spellInfo);
        }

        Destructible2D.D2dQuadFracturer fract = GetComponent<Destructible2D.D2dQuadFracturer>();
        if (fract == null)
            fract = gameObject.AddComponent<Destructible2D.D2dQuadFracturer>();
        fract.Fracture();
        ConvertSpellIntoSolidMaterial(spellInfo);
    }

    public void AlphaCountTooLow() //when stamped too much, (so must be Form.Phys) it should fracture, or if 0, be removed
    {
        if (spellInfo.numOfPixels > 0)
            Explode(); //If it is physical, should fracture
        else
            DestroySelf();
    }

    public void PixelCountModified(int newPixelAmt) //Part of DestructibleInterface, if it is altered.
    {
        spellInfo.ModifyPixels(newPixelAmt);
    }

    public SpellInfo GetSpellInfo()
    {
        return spellInfo;
    }


    //Conversion

    //Static
    //Spell turns to SolidMaterial
    //A normal conversion from spell has a full stacked and accurate spellinfo and rigidbody
    public void ConvertSpellIntoSolidMaterial(SpellInfo _spellInfo)
    {
        //Debug.Log("so the fracture calls this");
        SolidMaterial addedSM = gameObject.AddComponent<SolidMaterial>();
        addedSM.InitializeMaterial(_spellInfo);
        Destroy(gameObject.GetComponent<SpellDestabilization>());
        Destroy(gameObject.GetComponent<SpellBridge>());

        spellAnimManager.ClearAllAnimations();
        //Removing the things like spot lights and such is more of an issue
        if(spellInfo.interactionParams.Contains(GV.InteractionType.Avatar_Collision))
            GV.SetAllChildTagNLayersRecurisvely(transform, LayerMask.NameToLayer("TerrainNA"), "Terrain");
        else
            GV.SetAllChildTagNLayersRecurisvely(transform, LayerMask.NameToLayer("Terrain"), "Terrain");
        if (addedSM.GetSpellInfo().numOfPixels < GV.GROUND_DESTROY_MIN_PIXEL)
            addedSM.AlphaCountTooLow();
        Destroy(this);
        spellAnimManager = GetComponent<SpellAnimationManager>(); //looses connection in the clone
        spellAnimManager.ClearAllAnimations();
        //return toReturn;
    }

    
    public void ConvertSplitSpellIntoSolidMaterial(SpellInfo _spellInfo, int numOfPixels)
    {
        SpellInfo clonedSI = _spellInfo.Clone();
        SolidMaterial addedSM = gameObject.AddComponent<SolidMaterial>();
        addedSM.InitializeMaterial(clonedSI,numOfPixels);
        addedSM.PixelCountModified(numOfPixels);
        Destroy(gameObject.GetComponent<SpellDestabilization>());
        Destroy(gameObject.GetComponent<SpellBridge>());
        spellAnimManager.ClearAllAnimations();
        //Removing the things like spot lights and such is more of an issue
        GV.SetAllChildTagNLayersRecurisvely(transform, LayerMask.NameToLayer("Terrain"), "Terrain");
        if (numOfPixels < GV.GROUND_DESTROY_MIN_PIXEL)
            addedSM.AlphaCountTooLow();
        Destroy(this);
        spellAnimManager = GetComponent<SpellAnimationManager>();//looses connection in the clone
        spellAnimManager.ClearAllAnimations();
        //return toReturn;
    }

    public bool CoresTouching(Transform otherSpell)
    {
        if (spellCore) //solids have no spell core
        {
            return spellCore.IsTouchingCore(otherSpell);
        }

        return false;
        /*
        RaycastHit2D[] raycasts = Physics2D.RaycastAll(transform.position, new Vector2(0, 1), .1f, otherSpell.gameObject.layer);
        bool coreTouch = false;
        foreach (RaycastHit2D rch in raycasts)
            if (rch.transform == otherSpell)
            {
                coreTouch = true;
                break;
            }
        //Debug.Log("cores touch: " + coreTouch);
        return coreTouch;*/
        //
        //float distance = Vector2.Distance(transform.position,otherSpell.position);
        //if ((distance <= GV.CORE_SIZE * (otherSpell.localScale.x + transform.localScale.x)) || (distance <= GV.CORE_SIZE * (otherSpell.localScale.y + transform.localScale.y)))
        //    return true;
        //return false;
    }

    public void LockSpell(string toLock)
    {
        spellInfo.lockedSpellName = toLock;
    }

    //Debug functions
    public void DestroySelf()
    {
        //may be useful later for spellbridge or something
        Destroy(gameObject);
    }

    public void DEBUG_Destabilize()
    {
        spellInfo.stability = GV.STABILITY_DECAY_THRESHOLD;
    }

    public float GetDensityEffect()
    {
        return spellInfo.densityEffect;
    }

    private class VeloCalcHelper
    {
        public Vector2 curMomentuem;
        public Vector2 desiredMomentuem;
        public Vector2 diffInMomentuem;
        public Vector2 veloDiff;
        public float forceRequired;
        public float energyRequired;

        public VeloCalcHelper(SpellInfo si, Vector2 desiredDir, float desiredSpeed, GV.IgnoreXY ignorexy, float energyToForceEfficency)
        {
            Vector2 curVelo = si.velocityVector + (Physics2D.gravity * si.gravityScale);
            veloDiff = desiredDir - curVelo;
            curMomentuem = curVelo * si.mass;
            diffInMomentuem = veloDiff * si.mass;
            
            if (ignorexy == GV.IgnoreXY.IgnoreX)
                veloDiff.x = diffInMomentuem.x = 0;
            else if (ignorexy == GV.IgnoreXY.IgnoreY)
                veloDiff.y = diffInMomentuem.y = 0;

            forceRequired = diffInMomentuem.magnitude;
            energyRequired = forceRequired / energyToForceEfficency;   //here would be the new bodystats thing, everywhere with that gv
        }

        public override string ToString()
        {
            return ("dif in Mom: " + diffInMomentuem + ", veloVec:" + veloDiff + ", forceReq: " + forceRequired + ", energyReq: " + energyRequired);
        }
    }
}

