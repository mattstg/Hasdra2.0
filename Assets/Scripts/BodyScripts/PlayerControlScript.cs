using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpellBridge))]
public class PlayerControlScript : MonoBehaviour {

    //Links to Other Classes
    public string pcsName; //useful for creatures or "player0"
    public InputManager inputMap;
    public BodyStats stats;
    public LevelUpManager levelUpManager;
	public tempScriptAnim1 linkToRagdoll;
	public AnimationParameterCntrlr animParaCtrl;
	protected Rigidbody2D playerRigidBody;
	private AnimatorEventManager eventManAnim;
    public CameraControl cameraControl;
    public PlayerGuiController guiControl;
    public AvatarPhysicalBody avatarManager;

	//SpellBridge
	protected SpellBridge spellBridge;
	private string spellLocked = "";

    //VARS FOR INTERNAL PROCESSES
	public bool facingRight = true;
	public int pid = 0;
	//Facing Direction
	public float flightDrag = 1.0f;     //flight drag is applied when in the air, reducing move abilities requested amt
	private float airTime = 0f; //time spent in the air
	//Aiming Reticle
	public Vector2 head; //vector representing "aimer" of player; locationg and direction where spells will be spawed.
	public float headAngle = 0f; //angle of aimer (head) which is altered with vertical input
    public float reticleAngle = 0;
	public GameObject reticleSprite;
    public Transform selfCastReticle;
	public int reticleDirection = 1;
    public float coliExpTimer = 0;
	//Char color
	public Color primaryColor = Color.yellow;
    public Color secondaryColor = Color.white;
	//Grabbing
    private bool canClimb = false;
    private bool isClimbing = false;
	public BoxCollider2D climbZoneLeft;
	public BoxCollider2D climbZoneRight;
	private bool climbFront = false;
	private bool climbBack = false;
	private bool firstClimbTrigger = false;
	private bool hasClimbedThisCycle = false;

	public bool isFiring = false;
	public bool firstCast = true;
	public Transform leftHand;
	public Transform rightHand;
	public floatingTransform betweenHands;

    public Transform reticleChargingLocation;
    public Transform handPunchingLoc;
    public Transform chest;  //eventaully should have a  "get body part" thing
    //private float timeSinceGrab = 0f;
	//Grounded
	private bool isGrounded = false;
    public CircleCollider2D feet;
	public LayerMask whatIsGround;
	//Concussion
	//private bool concussionTracker; //internal bool assisting in determining when to ragdoll and when to recover, when calling concussion() method.
	//Ragdoll to non-ragdoll force trasfer
	public Vector2 storedForce = new Vector2(0,0);  //work around to allow force to be applied to be saved, for transfer to rigidbody, do not alter from outside of this class plz, public for read
    public Vector2 storedForceDt = new Vector2(0, 0); //same as above

    //Animation Vars
    public float contrlMove; //the input controller for moving
	private float animMove; //percent of max horizontal movement
    private bool isCrouched = false;
	private float standHightCollider = 1f;
	private float crouchHightCollider = 1f;
	public BoxCollider2D bodyCollider; // square collider on body, to be shrunk onCrouch and grown !onCrouch

	private float crouchTimer = GV.TIME_BETWEEN_CROUCH;
	private bool fallTrigger;

	//Test Vars (to be removed?)
	public bool flipToggle = false;
	public bool isDummyChar = false; //whether or not character is initialized with stats & level progression or is empty.

    public AbilityManager abilityManager;

    public Vector2 currentVelo { get { return GetComponent<Rigidbody2D>().velocity; } }
    public float currentSpeed = 0; //calculated at start of update
    //public Vector2 currentVelo { get { return GetComponent<Rigidbody2D>().velocity + GetComponent<Rigidbody2D>().mass / forceToBeApplied; } } Missing the mass thing

    public bool isPlayerControlled;
    public bool isBlocking = false;
    public float blockShieldStrength = 0;
    public bool blockInstalled = false;

	Dictionary<string, DamageStruct> damageSources = new Dictionary<string, DamageStruct>();

    // Use this for initialization
    protected virtual void Start () 
    {
        eventManAnim = GetComponentInChildren<AnimatorEventManager> ();
        betweenHands.setPosition(leftHand.transform, rightHand.transform);
		reticleChargingLocation = betweenHands.retTransform ();
		handPunchingLoc = leftHand;
		playerRigidBody = GetComponent<Rigidbody2D> ();
        spellBridge = GetComponent<SpellBridge>();
        spellBridge.InitializeSpellBridge(this,null);
        //avatarManager = GetComponent<AvatarPhysicalBody>();
        //concussionTracker = !linkToRagdoll.startInRagDoll;
        //inputMap = GetComponent<InputManager>().Link(this);
    }

    public virtual void Initialize(LevelTracker lvlTracker, int level, string _name)
    {
        pcsName = _name;
        isPlayerControlled = true;
        stats = new BodyStats(this);
        inputMap = new InputManager(this);
        abilityManager = new AbilityManager(this);
        inputMap.LoadControls(pid); //load ctrls requires abMngr is up, abMangr requires inputmap on cnstr, so order here important
        levelUpManager = new LevelUpManager(lvlTracker, this);
        levelUpManager.ForceTradeInExp(level * GV.EXPERIENCE_PER_LEVEL);
        avatarManager = GetComponent<AvatarPhysicalBody>();
        avatarManager.Initialize();
    }

   

    public void UpdateBodyScale()
    {
        //stats.getSkillValue("constBodyScaleX"); //, new BasicBF(.00267f, 0));
        //stats.getSkillValue("constBodyScaleY"); //, new BasicBF(.0067f, 0));
        //stats.getSkillValue("strBodyScaleX");   //, new BasicBF(.0067f, 0));
        //stats.getSkillValue("strBodyScaleY");   //, new BasicBF(.00267f, 1));
        //stats.getSkillValue("upperBody");       //, new BasicBF(.0067f, 0));  //used for scale and dmg from those limbs
        //stats.getSkillValue("lowerBody");       //, new BasicBF(.0067f, 0));  //used for scale and dmg from those limbs

        int multFace = (facingRight) ? 1 : -1;
        Vector2 baseBodyScale = GV.PLAYER_SCALE_BODY;
        baseBodyScale.x *= multFace;
        transform.localScale = baseBodyScale + new Vector2((stats.getSkillValue("constBodyScaleX") + stats.getSkillValue("strBodyScaleX"))*multFace, stats.getSkillValue("constBodyScaleY") + stats.getSkillValue("strBodyScaleY"));
        avatarManager.avatarLimbDict["ltorso"].localScale = GV.PLAYER_SCALE_LOWERBODY + new Vector2(stats.getSkillValue("constBodyScaleX") + stats.getSkillValue("strBodyScaleX") + stats.getSkillValue("upperBody"), stats.getSkillValue("constBodyScaleY") + stats.getSkillValue("strBodyScaleY") + stats.getSkillValue("upperBody"));
        avatarManager.avatarLimbDict["utorso"].localScale = GV.PLAYER_SCALE_UPPERBODY + new Vector2(stats.getSkillValue("constBodyScaleX") + stats.getSkillValue("strBodyScaleX") + stats.getSkillValue("lowerBody"), stats.getSkillValue("constBodyScaleY") + stats.getSkillValue("strBodyScaleY") + stats.getSkillValue("lowerBody"));
    }
    

    public void LoadSkin(string skinName)
    {
        GameObject.FindObjectOfType<CharacterSkinLoader>().LoadCharacterSkin(skinName, gameObject);
    }

	public void AddSkillModifier(SkillModifier skillModToAdd){
		stats.addSkillModifier (skillModToAdd);
	}

    private void SetMovementAnimations()
    {
        float tempVelo = playerRigidBody.velocity.x;
        	//Debug.Log ("Current Horiz Velo: " + tempVelo);
        if (playerRigidBody.velocity.x != 0)
        {
            float maxRunSpeed = stats.getSkillValue ("maxRunForce") / getMassOfBody();
			animMove = (float)Mathf.Abs(playerRigidBody.velocity.x) / maxRunSpeed;
			if (animMove < 0.015f)
				animMove = 0;
			//Debug.Log ("Anim Move: " + animMove);
			if (((playerRigidBody.velocity.x > 0 && !facingRight) || (playerRigidBody.velocity.x < 0 && facingRight)) && animMove > GV.PRCNT_MOV_SPD_IS_SLIDE_STOP)
            {
                //then we are sliding backwards
                animParaCtrl.setSlideBool(true);
            }
            else
            {
                animParaCtrl.setSlideBool(false);
            }
        }
        else
        {
            animMove = 0;
            animParaCtrl.setSlideBool(false);
        }
        animParaCtrl.setAnimMove(animMove);
    }

	protected virtual void LateUpdate(){
		if (isClimbing)
		{
			// float jumpGrabCooldown = GV.GRAB_TO_JUMP_COOLDOWN_BASE * (1 - GV.GRAB_TO_JUMP_COOLDOWN_SCALE * stats.getSkillValue("acrobatics"));
			//bool canGrab = Physics2D.IsTouchingLayers (grabZoneRight, whatIsGround) || Physics2D.IsTouchingLayers (grabZoneLeft, whatIsGround);
			bool canClimb = GroundedTest(climbZoneRight) || GroundedTest(climbZoneLeft);
			//if (timeSinceGrab > jumpGrabCooldown || !canGrab){
			if(!canClimb || !hasClimbedThisCycle){
				isClimbing = false;
				firstClimbTrigger = false;
				//timeSinceGrab = 0f;
				animParaCtrl.setClimbBack (false);
				animParaCtrl.setClimbFront (false);
			}
			//else
			//timeSinceGrab += dtime;
		}

		hasClimbedThisCycle = false;
	}

    protected virtual void Update()
    {
        float dt = Time.deltaTime;
        currentSpeed = currentVelo.magnitude;
        inputMap.Update(dt);
        stats.Update();
        coliExpTimer += dt;
        crouchTimer += dt;
        betweenHands.setPosition (leftHand.transform,rightHand.transform);
        UpdateSheild(dt);
        //UpdateReticlePosition();

        if (flipToggle) {
			Flip ();
			flipToggle = false;
		}
        SetMovementAnimations();
		isGrounded = GroundedTest ();
		animParaCtrl.setIsGrounded (isGrounded, playerRigidBody);
       	
		if (!isGrounded & Mathf.Abs (playerRigidBody.velocity.y) > GV.VELO_FALL_TRIGG_ANIM) {
			animParaCtrl.setFallTrigger ();
			if (isCrouched) {
				Crouch ();
			}
		}
        UpdateAirDrag(dt);
		//Jump Duration
        /*if (isGrounded || airTime > stats.getSkillValue("acrobatics"))
        {
			animParaCtrl.jumpCycleEnd ();
		}*/
			
        ApplyStoredForce();

        if (stats.healthPoints <= 0)
            Dies();

        if (cameraControl != null)
            cameraControl.Update(dt);
        
        Vector2 setSpeed = playerRigidBody.velocity;
        setSpeed.x = ((Mathf.Abs(setSpeed.x) > GV.PLAYER_MAX_SPEED.x))?GV.PLAYER_MAX_SPEED.x*Mathf.Sign(setSpeed.x):setSpeed.x;
        setSpeed.y = ((Mathf.Abs(setSpeed.y) > GV.PLAYER_MAX_SPEED.y)) ? GV.PLAYER_MAX_SPEED.y * Mathf.Sign(setSpeed.y) : setSpeed.y;
        playerRigidBody.velocity = setSpeed;

        guiControl.UpdateDisplay();
        UpdateBodyScale();
        spellBridge.placement();
    }

    private void UpdateSheild(float dt)
    {
        if(blockInstalled && !isBlocking)
        {
            blockShieldStrength += stats.getSkillValue("ab_block_recharge") * dt;
            blockShieldStrength = Mathf.Min(blockShieldStrength, stats.getSkillValue("ab_block_max"));
        }
    }

    private void UpdateAirDrag(float dt)
    {
        if (!isGrounded && !isClimbing)
        {
            airTime += dt;
            flightDrag = Mathf.Max(0, airTime / stats.getSkillValue("acrobatics"));

        }
        else
        {
            animParaCtrl.jumpCycleEnd();
            airTime = 0;
        }
    }

    public void ApplyStoredForce()
    {
        //add the dt stored force -- needs to be relative, but is in the dt world. So addRelative doesnt work, need to manual it
        Vector2 forceNonDt = storedForceDt / Time.deltaTime;
        Vector2 curForce = GetComponent<Rigidbody2D>().mass * GetComponent<Rigidbody2D>().velocity;
        float forceToApplyx = RelativeKnockbackHelper(curForce.x, forceNonDt.x);
        float forceToApplyy = RelativeKnockbackHelper(curForce.y, forceNonDt.y);
        Vector2 forceToApplyDt = new Vector2(forceToApplyx, forceToApplyy) * Time.deltaTime;
        GetComponent<Rigidbody2D>().AddForce(storedForce, ForceMode2D.Impulse);  //Add the raw force since relative calc already done
        storedForceDt = new Vector2(0,0);

        //Add the non-dt stored force -- Force is large enough that adding relative is fine
        GetComponent<Rigidbody2D>().AddRelativeForce(storedForce, ForceMode2D.Impulse); 
        storedForce = new Vector2(0,0);
    }

    public CameraControl AddCamera()
    {
        if (cameraControl == null)
        {
            cameraControl = new CameraControl(this);
        }
        else
        {
            Debug.Log("attempted to add camera to player : " + pid + " when already existed");
        }
        return cameraControl;
    }

    public void AddForceToAvatar(Vector2 forceToAdd, bool dtForce)
    {
        if (!dtForce)
            storedForce += forceToAdd;
        else
            storedForceDt += forceToAdd;
    }

    public void GainExp(float amt)
    {
        levelUpManager.GainExp(amt);
    }

    public virtual void Concusion(bool isConcussed)
    {
		if (isConcussed) {
				spellBridge.LaunchSpell (); //lauch any charging spell
				inputMap.inputDisabled = true; //disable player input into inputMap
				linkToRagdoll.ragdollToggle = true; //toggle character to begin ragdoll
		} else {
			//then we are not concussed
			if (!linkToRagdoll.getIsGetUpPhase ()) 
					linkToRagdoll.ragdollToggle = true; //then start "Get Up" cycle
		}
    }

    /*public void UpdateReticleAngle(int tempAxis)
    {
		//float tempMaxHeadAngle = GV.RETICLE_ANGLE_MIN + GV.MAX_RETICLE_ANGLE_SCALE * stats.getSkillValue("MaxAimAngle");
		float tempMaxHeadAngle = stats.getSkillValue ("MaxAimAngle");
        if (tempAxis != 0)
        {
			//headAngle += (GV.RETICLE_SPIN_SPD_BASE + GV.RETICLE_SPIN_SPD_SCALE * stats.getSkillValue("aimerSpinSpeed")) * tempAxis * Time.deltaTime;
			headAngle += stats.getSkillValue ("aimerSpinSpeed") * Time.deltaTime * tempAxis;
			//float tempMaxHeadAngle = GV.MAX_RETICLE_ANGLE_BASE + GV.MAX_RETICLE_ANGLE_SCALE * stats.getSkillValue("MaxAimAngle");
            if (headAngle > tempMaxHeadAngle && tempAxis > 0)
            {
                headAngle = tempMaxHeadAngle;
            }
            else if (headAngle < -tempMaxHeadAngle && tempAxis < 0)
            {
                headAngle = -tempMaxHeadAngle;
            }
        } 
		animParaCtrl.setAim (headAngle / GV.RETICLE_ANGLE_MAX);
    }*/

    /*public void UpdateReticlePosition()
    {
        head = new Vector2((float)Mathf.Cos(headAngle * Mathf.PI / 180f) * reticleDirection, (float)Mathf.Sin(headAngle * Mathf.PI / 180f));
        reticleAngle = (!facingRight) ? 180 - headAngle: headAngle;
        //reticleSprite.transform.position = (head + new Vector2(transform.position.x, transform.position.y)) * reticleDistance;
        reticleSprite.transform.localPosition = head * GV.RETICLE_DISTANCE;
		if (!facingRight) {
			reticleSprite.transform.localPosition = new Vector3 (-reticleSprite.transform.localPosition.x, reticleSprite.transform.localPosition.y, reticleSprite.transform.localPosition.z);
		}
       
    }*/

    private void Dies()
    {
        stats.concussion.ModMaxConcussBars(1);
        stats.hp += stats.maxHp * GV.PLAYER_REVIVAL_HP_PERC;
        StaticReferences.numericTextManager.CreateNumericDisplay(this, this.transform, "Revive!", "", 0, Color.green);

        if (GV.DEATH_PERMANENT_ENABLED) {
			//if not in ragdoll, then we want to go ragdoll...
			if (linkToRagdoll.getIsGetUpPhase () || linkToRagdoll.anim.enabled)
				linkToRagdoll.ragdollToggle = true;
			stats.Dies ();
			inputMap.inputDisabled = true;
			spellBridge.LaunchSpell ();
		}
    }

    public void ToggleCamera(bool ragdollMode)
    {
        if (ragdollMode)
        {
            cameraControl.SetFollowTarget(avatarManager.avatarLimbDict["utorso"]);
        }
        else
        {
            cameraControl.SetFollowTarget(this.transform);
        }
    }

    public void Flip()
    {
        facingRight = !facingRight;
        reticleDirection = (facingRight) ? 1 : -1;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
		//linkToRagdoll.getUpScript.jointsEnabled (true);
        transform.localScale = theScale;
		//linkToRagdoll.getUpScript.resetLimbAnchors ();
		//linkToRagdoll.getUpScript.jointsEnabled (false);
    }

    public void SpellPressed(string spellName)
    {
		if (firstCast) {
			animParaCtrl.setSpellTrigger ();
			firstCast = false;
		}
		
		//Debug.Log ("Spell Pressed: " + spellName);
		//Debug.Log ("isFiring? " + isFiring);
		//Debug.Log ("Spell locked? " + spellLocked);
		//Debug.Log("Here1! " + spellLocked);
		//Debug.Log ("Sell Name = " + spellName);
		//Debug.Log ("isFiring? " + isFiring);

        if (spellLocked != spellName && !isFiring)
        {
			//Debug.Log ("Ok! charge spell!");
            spellBridge.ChargeSpell(spellName, stats);            
            //Debug.Log("charging anim: Lock " + spellLocked + " spell name: " + spellName + ", isFiring: " + isFiring);
            if (!isFiring)  //can be set to firing inside of ChargeSpell
            {
				if (spellBridge.isMelee) {
                    switch(spellBridge.meleeCastType)
                    {
                        case GV.MeleeCastType.basicPunch:
                            animParaCtrl.setMeleeCharge(true);
                            animParaCtrl.setPunchSpd(1 / stats.getSkillValue("meleeSpeed"));
                            break;
                        case GV.MeleeCastType.kick:
                            animParaCtrl.setKickPressed(true);
                            animParaCtrl.setKickSpeed(1 / stats.getSkillValue("meleeSpeed"));
                            break;
                        default:
                            animParaCtrl.setMeleeCharge(true);
                            animParaCtrl.setPunchSpd(1 / stats.getSkillValue("meleeSpeed"));
                            break;
                    }					
				} else if (spellBridge.isSelfCast) {
					animParaCtrl.setSelfCast (true);
					animParaCtrl.setSelfCastPower (calculateSelfCastPower ());
				} else {
					animParaCtrl.setCharge (true);
					animParaCtrl.setFireSpd (1 / stats.getSkillValue ("spellFireSpeed"));
				}
            }
        }
    }

    public void LockSpell(string lockName)
    {
        spellLocked = lockName;
    }

	private float calculateSelfCastPower(){
		float castPower = spellBridge.currentSpell.GetEnergy () / 300;
		if (castPower > 1)
			return 1;
		else
			return castPower;
	}

    public void SpellReleased(string spellName)
    {
        if (spellLocked == spellName)
            LockSpell("");

		//Debug.Log ("Here!");
        if (spellBridge.isSameSpell(spellName))
        {
            BeginSpellLaunch(spellName);
        }
    }

    public void BeginSpellLaunch(string spellNameFiring)
    { 
        EndFiringAnimations();        
    }

    private void EndFiringAnimations()
    {
        if (!isFiring)
        {
            if (spellBridge.isMelee)
            {
                animParaCtrl.setKickRelease();
                animParaCtrl.setReleaseMelee();
                animParaCtrl.setMeleeCharge(false);
                animParaCtrl.setKickPressed(false);
            }
			else if(spellBridge.isSelfCast){
				animParaCtrl.setSelfCast (false);
			}else {
				animParaCtrl.setFireSpell();
				animParaCtrl.setCharge(false);
			}
			isFiring = true;
        }
    }

    public void ChargingSpellDied()
    {
        isFiring = false;
        LockSpell("");
        animParaCtrl.setMeleeCharge(false);
        animParaCtrl.setKickPressed(false);
        animParaCtrl.setCharge(false);
		animParaCtrl.setSelfCast (false);
        //eventManAnim.SpellUnlock();
    }

	public void AnimatorFiresSpell(){
        
		if (spellBridge.currentSpell) {
			spellBridge.LaunchSpell ();

			//isFiring = false;
		}//isFiring = false;
	}

	public void unlockFromAnim(){
		isFiring = false;       
		firstCast = true;
	}

	public bool GroundedTest(){
		return GroundedTest (feet);
	}

	public bool GroundedTest(Collider2D against){
		bool result = against.IsTouchingLayers (whatIsGround);
		/*foreach(string layer in GV.spellLayers){     
			if (result == true)  
				return true;
			result = against.IsTouchingLayers (LayerMask.NameToLayer("layer"));  just changed it in prefab "whatisground" to include those, also "layer" means the literal string "layer"
		}*/
		return result; //seems to always return true tho. even default sets it off
	}

    public void OnTriggerEnter2D(Collider2D coli)
    {
        ResolveCollision(coli.gameObject);
    }

    public void OnCollisionEnter2D(Collision2D coli)
    {
        ResolveCollision(coli.gameObject);
    }

    public void ResolveCollision(GameObject otherObj)
    {
        //Only if a player is fast enough will it calculate things to save proc
        if (currentSpeed < GV.PLAYER_SPEED_CREATE_EXPLOSION || coliExpTimer <= GV.PLAYER_SPEED_CREATE_EXPLOSION_REFRESH_RATE)
            return;

        if (otherObj.name == "Collider")  //colliding with d2d requires this since it collides with child instead of parent
            otherObj = otherObj.transform.parent.gameObject;

        if (otherObj.GetComponent<PlayerControlScript>() || otherObj.GetComponent<SolidMaterial>() || otherObj.GetComponent<Spell>())
        {
            coliExpTimer = 0;
            float Ek = .5f * getMassOfBody() * currentSpeed * currentSpeed * GV.PLAYER_SPEED_CREATE_EXPLOSION_EK_MULT;
            spellBridge.CreateCrashExplosion(Ek);
        }

    }


    /*public void Jump()
	{
		if (isCrouched) {
			Crouch ();
		}

		//float tempJumpForce = stats.getSkillValue ("jumpForce") * GV.JUMP_FORCE_SCALE + GV.JUMP_FORCE_MIN;
		float tempJumpForce = stats.getSkillValue ("jumpForce");
			//Debug.Log ("stats.stamina " + stats.stamina);
		if(!isClimbing){
			if (isGrounded) {
				animParaCtrl.jumpInput ();
				tempJumpForce *= SpendStamina (tempJumpForce * Time.deltaTime);
	//			Debug.Log ("Initial Jump Force: " + (tempJumpForce * Time.deltaTime) + " stats.energy " + stats.stamina);

				GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0, tempJumpForce * Time.deltaTime), ForceMode2D.Impulse);
			} else {
				//float tempJumpForceDuration = stats.getSkillValue ("acrobatics") * GV.JUMP_FORCE_DURATION_SCALE + GV.JUMP_FORCE_DURATION_MIN;
				float tempJumpForceDuration = stats.getSkillValue ("jumpDuration");
				if (airTime < tempJumpForceDuration) {
					animParaCtrl.jumpInput ();
					//Debug.Log("jumping b/c airtime < jumpforcedur and !isgrabbing");
					tempJumpForce *= SpendStamina ((1 - airTime / tempJumpForceDuration) * tempJumpForce * Time.deltaTime);
					GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0, (1 - airTime / tempJumpForceDuration) * tempJumpForce * Time.deltaTime), ForceMode2D.Impulse);
				}
					//Debug.Log ("airTime " + airTime + " tempJumpForceDuration " + tempJumpForceDuration);
			//	}
			}       
					//GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, jumpStr * Time.deltaTime +  GetComponent<Rigidbody2D>().velocity.y);
	                // Debug.Log("DeltaTime = " + Time.deltaTime);
    	}
	}*/


    /*public void Move(int direction)
    {
        contrlMove = direction;
        if (facingRight && contrlMove < 0 || !facingRight && contrlMove > 0)
            Flip();

        //float tempMaxHorizontalSpeed = GV.MAX_HORIZ_SPEED_BASE + GV.MAX_HORIZ_SPEED_SCALE * stats.getSkillValue("maxRunSpeed");
        //move = (Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) > tempMaxHorizontalSpeed)? 0 : move;
		if (Mathf.Abs(GetComponent<Rigidbody2D> ().velocity.x) < (GV.MAX_HORIZ_SPEED_BASE + GV.MAX_HORIZ_SPEED_SCALE * stats.getSkillValue ("maxRunSpeed"))) {
			//float tempRunForce = GV.RUN_FORCE_BASE + GV.RUN_FORCE_SCALE * stats.getSkillValue ("runSpeed");
			float tempRunForce = stats.getSkillValue ("runSpeed");
			//Debug.Log (stats.getSkillValue ("runSpeed") + " " + GV.MikesAsymptote (stats.getSkillValue ("runSpeed"), GV.RUN_FORCE_SCALE, GV.RUN_FORCE_MIN, GV.RUN_FORCE_MAX,GV.HorzAsym.MinToMax));
			contrlMove *= SpendStamina (tempRunForce * flightDrag * Time.deltaTime);
			GetComponent<Rigidbody2D> ().AddForce (new Vector2 ((contrlMove * tempRunForce * flightDrag) * Time.deltaTime, 0), ForceMode2D.Impulse);
		} else {
			//hit max horizontal speed

		}
    }*/

    /*public void Grab()
    {
        if(canGrab) //some collision of torso (similar to grounded check)
        {
            float grabStrength = stats.getSkillValue("grabStrength") * GV.GRAB_STRENGTH_SCALE + GV.GRAB_STRENGTH_BASE;
            grabStrength *= SpendStamina(grabStrength * Time.deltaTime);
            NormalizeVelocity(Vector2.up, grabStrength);
            isGrabbing = true;
            timeSinceGrab = 0f;
        }

    }*/

    public void Crouch(){
		if (crouchTimer >= GV.TIME_BETWEEN_CROUCH) {
			if (!isCrouched) {
				animParaCtrl.setCrouch (true);
				animParaCtrl.setCrouchTrigger ();
				shrinkBodyCollider ();
				isCrouched = true;
			} else {
				isCrouched = false;
				animParaCtrl.setCrouch (false);
				growBodyCollider ();
			}
			crouchTimer = 0f;
		}
    }

	private void shrinkBodyCollider(){
	//	Debug.Log ("Shrink");
		bodyCollider.size = new Vector2(bodyCollider.size.x, bodyCollider.size.y - 0.4f);
	//	Debug.Log ("size is " + bodyCollider.size.ToString());
		bodyCollider.offset = new Vector2(bodyCollider.offset.x, bodyCollider.offset.y - 0.2f);
//		Debug.Log ("size is " + bodyCollider.offset.ToString());
	}

	private void growBodyCollider(){
	//	Debug.Log ("Grow");
		bodyCollider.size = new Vector2(bodyCollider.size.x, bodyCollider.size.y + 0.4f);
	//	Debug.Log ("size is " + bodyCollider.size.ToString());
		bodyCollider.offset = new Vector2(bodyCollider.offset.x, bodyCollider.offset.y + 0.2f);
	//	Debug.Log ("size is " + bodyCollider.offset.ToString());

	}

    /*public void Fly()
    {
        NormalizeVelocity(Vector2.up, 75);
        isClimbing = true;
    }*/

    private void NormalizeVelocity(Vector2 along, float maxForce)
    {
        along = along.normalized;
        Vector2 maxApplyableVelocity = along * maxForce / GetComponent<Rigidbody2D>().mass * Time.deltaTime;
        float projection = (maxApplyableVelocity.magnitude != 0) ? Vector2.Dot(new Vector2(GetComponent<Rigidbody2D>().velocity.x, GetComponent<Rigidbody2D>().velocity.y), maxApplyableVelocity) / maxApplyableVelocity.magnitude : 0;
        float forceModifier = (Mathf.Abs(projection) < 1) ? 1 - Mathf.Abs(projection) / 1 : 1;
        GetComponent<Rigidbody2D>().AddForce(forceModifier * (-1) * projection * along * maxForce * Time.deltaTime, ForceMode2D.Impulse);
    }

    public void ToggleInputMap(bool toggleState)
    {
        inputMap.inputDisabled = !toggleState;
    }

    public int GetNumberOfLimbs()
    {
        return 11;
    }

    /*public void WeakpointHit(string partName, float energy, GV.MaterialType matType)
    {
        stats.concussion.RecieveConcussion(energy * GV.PLAYER_HEADSHOT_BONUS);
    }*/

    public virtual bool IsHeadshot(GameObject otherObj)
    {
        return stats.IsHittingLimb(otherObj, "Head");
    }

	/*public void DirectHitBySpell(float energyAmt, GV.MaterialType materialType, Vector2 directionOfDamage, float spellDensity, SpellInfo spellInf)
    {
        energyAmt *= GV.SPELL_DIRECT_HIT_DMG_MODIFIER * Time.deltaTime * spellDensity; //direct hit does increased dmg & effect     
		TakeDamage(energyAmt, materialType, spellInf);
        KnockBackFromDamage(energyAmt, directionOfDamage, materialType);
		//Debug.Log ("heredirecthitbyspell");
    }*/

	public float TakeDamage(float energyAmt, GV.MaterialType materialType)
    {
        if (stats.isInvunerable)
            return 0;
        //Debug.Log("took dmg: " + dmgAmount + " of type " + materialType.ToString() + " in the direction of " + directionOfDamage.ToString());
        if (energyAmt / Time.deltaTime >= .1f) //if is dealing more than .1 dmg, at this point already recieves it after dt, so need to convert to check
        {
            float dmgToAdd = 0;
            dmgToAdd += TakeEarthDamage( energyAmt * MaterialDict.Instance.GetDamage(GV.DamageTypes.Earth)  * MaterialDict.Instance.GetDamageDistribution(materialType, GV.DamageTypes.Earth) * stats.getResistanceValue("resistEarth"));
            dmgToAdd += TakeEnergyDamage(energyAmt * MaterialDict.Instance.GetDamage(GV.DamageTypes.Energy) * MaterialDict.Instance.GetDamageDistribution(materialType, GV.DamageTypes.Energy) * stats.getResistanceValue("resistEnergy"));
            dmgToAdd += TakeAirDamage(   energyAmt * MaterialDict.Instance.GetDamage(GV.DamageTypes.Air)    * MaterialDict.Instance.GetDamageDistribution(materialType, GV.DamageTypes.Air) * stats.getResistanceValue("resistAir"));
            dmgToAdd += TakeWaterDamage( energyAmt * MaterialDict.Instance.GetDamage(GV.DamageTypes.Water)  * MaterialDict.Instance.GetDamageDistribution(materialType, GV.DamageTypes.Water) * stats.getResistanceValue("resistWater"));
            dmgToAdd += TakeFireDamage(  energyAmt * MaterialDict.Instance.GetDamage(GV.DamageTypes.Fire)   * MaterialDict.Instance.GetDamageDistribution(materialType, GV.DamageTypes.Fire) * stats.getResistanceValue("resistFire"));
            dmgToAdd += TakeIceDamage(   energyAmt * MaterialDict.Instance.GetDamage(GV.DamageTypes.Ice)    * MaterialDict.Instance.GetDamageDistribution(materialType, GV.DamageTypes.Ice) * stats.getResistanceValue("resistIce"));
            dmgToAdd += TakeAetherDamage(energyAmt * MaterialDict.Instance.GetDamage(GV.DamageTypes.Aether) * MaterialDict.Instance.GetDamageDistribution(materialType, GV.DamageTypes.Aether) * stats.getResistanceValue("resistAether"));
            dmgToAdd += TakeNatureDamage(energyAmt * MaterialDict.Instance.GetDamage(GV.DamageTypes.Nature) * MaterialDict.Instance.GetDamageDistribution(materialType, GV.DamageTypes.Nature) / stats.getResistanceValue("resistNature"));  //divide to increase effect
            dmgToAdd += TakeManaDamage(  energyAmt * MaterialDict.Instance.GetDamage(GV.DamageTypes.Mana)   * MaterialDict.Instance.GetDamageDistribution(materialType, GV.DamageTypes.Mana) / stats.getResistanceValue("resistMana"));          //divide to increase effect
            StaticReferences.numericTextManager.CreateNumericDisplay(this, this.transform, "dmgTaken", "", dmgToAdd, Color.red);
            //dmgContrl.takeDmg(dmgToAdd);
            return dmgToAdd;
        }
        return 0;
    }

    //static int logs = 0;
    private float BlockIncomingDamage(float incomingDamage)
    {

        float dmgBlocked = Mathf.Min(incomingDamage, blockShieldStrength); //max amt can be blocked
       // if(logs == 0) Debug.Log(string.Format("dmg blocked {0} = min(incomingDmg{1},sheildStr{2})",dmgBlocked,incomingDamage,blockShieldStrength));
        float stanimaConsume = dmgBlocked / stats.getSkillValue("ab_block_efficency");
       // if (logs == 0) Debug.Log(string.Format("stamina consume {0} = dmgBlock{1}/sv{2})",stanimaConsume,dmgBlocked, stats.getSkillValue("ab_block_efficency")));
        stanimaConsume = Mathf.Min(stanimaConsume, stats.stamina);
        //if (logs == 0) Debug.Log("cur stanima: " + stats.stamina);
        dmgBlocked = stanimaConsume * stats.getSkillValue("ab_block_efficency");
        incomingDamage -= dmgBlocked;
        blockShieldStrength -= dmgBlocked;
        stats.stamina -= stanimaConsume;
       // if (logs == 0) Debug.Log("dmg blocked: " + dmgBlocked + " stanima consumed " + stanimaConsume + " remaining shield: " + blockShieldStrength);
       // logs++;
        return incomingDamage;
    }

    //Check this isnt too cpu intensive since recalculates the numbers in take damage
    public void KnockBackFromDamage(float amount, Vector2 _directionOfDamage, GV.MaterialType attackingMaterial, bool relative = false)
    {
        relative = false;
        //Energy spells being affected only
        if (!relative)
        {
            Vector2 directionOfDmg = _directionOfDamage.normalized;
            Vector2 forceApplying = directionOfDmg * amount * MaterialDict.Instance.GetKnockback(attackingMaterial) * stats.getResistanceValue("resistKnockback");
            storedForce += forceApplying;
        }
        else
        {
            Vector2 directionOfDmg = _directionOfDamage.normalized;
            Vector2 forceApplying = directionOfDmg * amount * MaterialDict.Instance.GetKnockback(attackingMaterial) * stats.getResistanceValue("resistKnockback");
            Vector2 forceApplyPerSec = forceApplying / Time.deltaTime;

            Vector2 curForce = currentVelo * getMassOfBody();
            //Debug.Log(string.Format("cur force: {0},{1}", currentVelo, getMassOfBody()));

            Vector2 forceToAdd = new Vector2(RelativeKnockbackHelper(curForce.x, forceApplyPerSec.x),RelativeKnockbackHelper(curForce.y, forceApplyPerSec.y));
            //Debug.Log(string.Format("Current force {0}, Force adding {1}, Force added {2}", curForce, forceApplyPerSec,forceToAdd));
            storedForce += forceToAdd;
        }
    }

    public void FullKnockbackFromDamage(float amount, Vector2 _directionOfDamage, GV.MaterialType attackingMaterial)
    {
        //Energy spells being affected only, Amount should not include dt, for a full second of knockback dealt
        Vector2 directionOfDmg = _directionOfDamage.normalized;
        Vector2 forceApplying = directionOfDmg * amount * MaterialDict.Instance.GetKnockback(attackingMaterial) * stats.getResistanceValue("resistKnockback");
        storedForce += forceApplying;
        gameObject.GetComponent<Rigidbody2D>().AddRelativeForce(forceApplying, ForceMode2D.Impulse);
        //Debug.Log(string.Format("{0} force applying from {1} dmg of {2} type", forceApplying, amount, attackingMaterial));
    }

    private float RelativeKnockbackHelper(float curForce, float forceApplying)
    {
        if (Mathf.Sign(curForce) != Mathf.Sign(forceApplying))
        {   
            return forceApplying; //Then auto add all force
        }
        else if (Mathf.Abs(curForce) > Mathf.Abs(forceApplying))
        {
            return 0;   //Don't add force
        }
        else
        {   
            return (Mathf.Abs(curForce) - Mathf.Abs(forceApplying)) * Mathf.Sign(curForce);  //add partial force
        }
    }

    //at the stage below, they already accounted for time and resistances
    private float TakeEarthDamage(float amount)
    {
        if (isBlocking)
            amount = BlockIncomingDamage(amount);
        stats.healthPoints -= amount;
        stats.concussion.RecieveDamage(amount,GV.DamageTypes.Earth);  
        return amount;
    }
    private float TakeEnergyDamage(float amount)
    {
        if (isBlocking)
            amount = BlockIncomingDamage(amount);
        stats.healthPoints -= amount;
        stats.concussion.RecieveDamage(amount, GV.DamageTypes.Energy);  
        return amount;
    }
    private float TakeAirDamage(float amount)
    {
        if (isBlocking)
            amount = BlockIncomingDamage(amount);
        stats.healthPoints -= amount;
        stats.concussion.RecieveDamage(amount, GV.DamageTypes.Air);  
        return amount;
    }
    private float TakeWaterDamage(float amount)
    {
        if (isBlocking)
            amount = BlockIncomingDamage(amount);
        stats.healthPoints -= amount;
        stats.concussion.RecieveDamage(amount, GV.DamageTypes.Water);  
        return amount;
    }
    private float TakeFireDamage(float amount)
    {
        if (isBlocking)
            amount = BlockIncomingDamage(amount);
        stats.healthPoints -= amount;
        stats.concussion.RecieveDamage(amount, GV.DamageTypes.Fire);  
        return amount;
    }
    private float TakeIceDamage(float amount)
    {
        if (isBlocking)
            amount = BlockIncomingDamage(amount);
        stats.healthPoints -= amount;
        stats.concussion.RecieveDamage(amount, GV.DamageTypes.Ice);  
        return amount;
    }
    private float TakeAetherDamage(float amount)
    {
        if (isBlocking)
            amount = BlockIncomingDamage(amount);
        stats.healthPoints -= amount;
        stats.concussion.RecieveDamage(amount, GV.DamageTypes.Aether);  
        return amount;
    }
    private float TakeNatureDamage(float amount)
    {
        stats.healthPoints += amount;
        stats.concussion.RecieveDamage(amount, GV.DamageTypes.Nature);  
        return amount;
    }
    private float TakeManaDamage(float amount)
    {
        stats.energy += amount;
        stats.concussion.RecieveDamage(amount, GV.DamageTypes.Mana);  
        return amount;
    }

	public float getMassOfBody(){
		float sum = 0;
		Rigidbody2D[] bla = GetComponentsInChildren<Rigidbody2D>();
		foreach (Rigidbody2D ahh in bla) {
			sum += ahh.mass;
		}
		return sum;
	}

    ///Numeric Display Handling
    /////purpose of this is to manage the list of displays belonging to the entity, since certain ND may die unexpected or what not
    /// / This really should be in a seperate script, maybe it will some day

}
