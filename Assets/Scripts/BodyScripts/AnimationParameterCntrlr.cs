using UnityEngine;
using System.Collections;

public class AnimationParameterCntrlr : MonoBehaviour {
	public Animator animator;
	private bool inJumpCycle = false;
	private bool inSlideCycle = false;
	private float timeInJumpCycle = 0f;
	private float timeSinceLastLand = 0f;
	private bool hasLanded = false;
	private bool isFalling = false;
	private bool currentIsGrounded;

	// Use this for initialization
	void Start () {
		currentIsGrounded = animator.GetBool ("isGrounded");
		timeSinceLastLand = GV.MIN_TIME_BETWEEN_LAND_ANIM;
	}

	void FixedUpdate(){
		
	}

	// Update is called once per frame
	void Update () {
		if (inJumpCycle) {
			timeInJumpCycle += Time.deltaTime;
		}
		else if (hasLanded && timeSinceLastLand < GV.MIN_TIME_BETWEEN_LAND_ANIM)
			timeSinceLastLand += Time.deltaTime;
		//Debug.Log ("timeInJumpCycle " + timeInJumpCycle);
	}

	public void jumpInput(){
		if (!inJumpCycle) {
//			Debug.Log ("jumpInput");
			inJumpCycle = true;
			setIsJumping (true);
			setJumpTrigger ();
		}
	}

	public void jumpCycleEnd(){
		if (inJumpCycle && timeInJumpCycle > GV.MIN_TIME_BETWEEN_JUMP_ANIM && !animator.GetBool("jumpTrigger")) {
//			Debug.Log ("jumpCycleEnd");
			inJumpCycle = false;
			setIsJumping (false);
			timeInJumpCycle = 0f;
		}
	}


    public void setFloat(string _name, float _value)
    {
        animator.SetFloat(_name, _value);
    }


    public void setBool(string _name, bool _value)
    {
        animator.SetBool(_name, _value);
    }

    public void setTrigger(string _name)
    {
        animator.SetTrigger(_name);
    }


    public void setClimbPressed(bool toSet){
		animator.SetBool ("climbPressed", toSet);
	}

	public void setClimbSpeed(float toSet){
		animator.SetFloat ("climbSpeed", toSet);
	}

	public void setAbilityTrigger(){
		animator.SetTrigger ("abilityTrigger");
	}

	public void setDashPressed(bool toSet){
		animator.SetBool ("dashPressed", toSet);
	}

	public void setBlockPressed(bool toSet){
		animator.SetBool ("blockPressed", toSet);
	}

    public void setKickRelease()
    {
        animator.SetTrigger("kickRelease");
    }

	public void setKickPressed(bool toSet){
		animator.SetBool ("kickPressed", toSet);
	}

	public void setKickSpeed(float toSet){
		animator.SetFloat ("kickSpeed", toSet);
	}


	public void setBasicPunchCharge(bool toSet){
		animator.SetBool ("basicPunchPressed", toSet);
	}

	public void setBasicPunchRelease(bool toSet){
		animator.SetTrigger ("basicPunchReleased");
	}

	public void setBasicPunchSpeed(float toSet){
		animator.SetFloat ("basicPunchSpeed", toSet);
	}

	public void setDashDir(Vector2 toSet, bool facingRight){
		toSet = toSet.normalized;
		if (!facingRight)
			toSet.x *= -1;
		//Debug.Log ("Dash Vector " + toSet);
		animator.SetFloat ("dashDirX", toSet.x);
		animator.SetFloat ("dashDirY", toSet.y);
	}

	public void setDashDir(Vector2 toSet){
		setDashDir (toSet, true);
	}

	public void setJumpPressed(bool toSet){
		animator.SetBool ("jumpPressed", toSet);
	}

	public void setCharge(bool toSet){
		animator.SetBool ("Charge", toSet);
	}

	public void setFireSpell(){
		animator.SetTrigger ("fireSpell");
		//setIsFiring (true);
	}

	public void setMeleeCharge(bool toSet){
		animator.SetBool ("Melee", toSet);
	}

	public void setReleaseMelee(){
		animator.SetTrigger ("releaseMelee");
		//setIsFiring (true);
	}

	public void setAnimMove(float toSet){
		animator.SetFloat ("animMove",toSet);
//		Debug.Log ("Anim Move Set To: " + toSet);
	}

	private void setSlideTrigger(){
		animator.SetTrigger ("slideTrigger");
	}

	public void setSlideBool(bool toSet){
		if (!inSlideCycle && toSet) {
			setSlideTrigger ();
			inSlideCycle = true;
		} else if (inSlideCycle && !toSet) {
			inSlideCycle = false;
		}
		animator.SetBool ("slideBool", toSet);
	}

	public void setAim(float toSet){
		animator.SetFloat ("Aim",toSet);
	}

	public void setSelfCastPower(float toSet){
		animator.SetFloat("selfCastPower", toSet);
	}

	public void setSelfCast(bool toSet){
		animator.SetBool("selfCast", toSet);
	}

	public void setSpellTrigger(){
		animator.SetTrigger ("genSpellTrigger");
	}
	/*
	public void setIsGrounded(bool toSet){
		if (currentIsGrounded != toSet) {
			currentIsGrounded = toSet;
			animator.SetBool ("isGrounded", toSet);
			if (toSet) {
				setLandTrigger ();
			}
		}
	} */

	public void setIsGrounded(bool toSet, Rigidbody2D playerRigid){
		if (currentIsGrounded != toSet) {
			//Debug.Log ("Current Is Grounded changed to " + toSet);
			currentIsGrounded = toSet;
			if (toSet) {
				setLandTrigger (playerRigid.velocity.y * playerRigid.mass);
				isFalling = false;
//				Debug.Log ("landForce " + landForce);
			}
			animator.SetBool ("isGrounded", toSet);
		}
	}


	public void setFallTrigger(){
		if (!isFalling) {
			animator.SetTrigger ("fallTrigger");
			isFalling = true;
		}
	} 

	public void setCrouch(bool toSet){
		animator.SetBool ("Crouch", toSet);
	}

	public void setCrouchTrigger(){
		animator.SetTrigger ("crouchTrigger");
	}

	public void setLandTrigger(){
		animator.SetTrigger ("landTrigger");
	}

	public void setLandTrigger(float landForce){
		//Debug.Log ("land Force " + landForce + " timeSinceLastLand " + timeSinceLastLand);
		if ((landForce * -1f) >= GV.LAND_FORCE_TO_FALL_ANIM && timeSinceLastLand >= GV.MIN_TIME_BETWEEN_LAND_ANIM){
//			Debug.Log ("Ding!");
			//animator.SetTrigger ("landTrigger");
			hasLanded = true;
			timeSinceLastLand = 0f;
		}
	}

	public void setIsJumping(bool toSet){
		animator.SetBool ("isJumping", toSet);
	}
		
	public void setJumpPressed(){
		animator.SetTrigger ("jumpPressed");
	}

	public void setJumpTrigger(){
		animator.SetTrigger ("jumpTrigger");
	}

	public void setClimbTrigger(){
		animator.SetTrigger ("climbTrigger");
	}

	public void setClimbBack(bool toSet){
		animator.SetBool ("climbBack", toSet);
	}

	public void setClimbFront(bool toSet){
		animator.SetBool ("climbFront", toSet);
	}

	public void setIsFiring(bool toSet){
		animator.SetBool ("canFallorJumpHands", toSet);
	}

	public void setPunchSpd(float toSet){
		animator.SetFloat ("meleeReleaseSpeed", toSet);
	}

	public void setFireSpd(float toSet){
		animator.SetFloat ("spellFireSpeed", toSet);
	}

}

/*
		if (!isMoving && Mathf.Abs(move) > 0.2f && move != 0) {
			//move -= move * (1 - GV.MOVE_DECAY) * dtime;
			move -= move * GV.MOVE_DECAY * Time.deltaTime;
			Debug.Log (move);
		} else if (Mathf.Abs(move) <= 0.2f) {
			move = 0f;
		} */

/*
* if (facingRight && move > 0 || !facingRight && move < 0 && isGrounded) {
	if (animator.isActiveAndEnabled) {
		animator.SetTrigger("slideTrigger");
		Debug.Log ("Sliding has been triggered.");
	}
} */


/*
if (animator.isActiveAndEnabled) {
	//need the falltrigger
	if (animator.isActiveAndEnabled) {
		//Movement
		animator.SetFloat ("animMove", animMove);

		//Debug.Log ("Move set to " + Mathf.Abs(move));
		if (isCrouched != animator.GetBool("isCrouched")) {
			animator.SetBool ("isCrouched",isCrouched);
		}

		if (isGrounded != animator.GetBool ("isGrounded"))
			animator.SetBool ("isGrounded", isGrounded);

		//jump and fall
		if (isJumping != animator.GetBool("isJumping")) {
			if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Land")) {

			} else {
				animator.SetBool ("isJumping", isJumping);
			}
		}
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Land") && isJumping) {
			animator.SetBool ("isJumping", false);
		}

		if (!isJumping && !isGrounded && !isGrabbing) {
			if (animator.GetBool ("isFalling") == false) {
				//animator.SetTrigger ("fallTrigger");
			}
			animator.SetBool ("isFalling", true);
		} else {
			animator.SetBool ("isFalling", false);
		}

		//Grabbing
		if (grabBack != animator.GetBool ("grabBack")) {
			animator.SetBool ("grabBack", grabBack);
		}
		if (grabFront != animator.GetBool ("grabFront")) {
			animator.SetBool ("grabFront", grabFront);
		}
	}
}
*/