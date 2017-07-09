using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class tempScriptAnim1 : MonoBehaviour {
	//INTERNAL LISTS
    List<Rigidbody2D> limbRigidBodies = new List<Rigidbody2D>();
    List<Collider2D> limbColliders = new List<Collider2D>();
    List<SimpleCCD> CCDScript = new List<SimpleCCD>();

	//Connections to Character Object
    public Collider2D HBBox;
    public Collider2D HBCircle;
	public Collider2D GrabBoxLeft;
	public Collider2D GrabBoxRight;
    public Animator anim;
    public GameObject upperBody;
    public GameObject lowerBody;
    public GameObject characterHolder;
    public GameObject animator;
    public GameObject chest;
    public RagdollRecovery getUpScript; //Runs the Get Up Phase
	public desiredRecTransformFiller desiredRecoveryPose;

	//INTERNAL VARIABLES
    private PlayerControlScript pcs;
    private bool inGetUpPhase = false;
    private float timeSpentRagDollToAnim = 0f;
	private float invert = -1f;

	//EXTERNAL TRIGGERS
	public bool startInRagDoll = true; //test variable
	public float maxRagdollToAnimTime = 2f; // <------------ NEED TO REPLACE THIS, MAKE FUNCTION OF STATS
	public bool ragdollToggle = false; // <-------- NEED TO SOMEHOW TRIGGER THIS, MAKES YOU GO FROM RAGDOLL -> ANIMATED, OR RAGDOLL -> GETTING UP -> ANIMATED
	public bool resetDesiredToggle = false;
	public bool allowVeloTransfer = true; //allows velocity to be transfered between char holder rigid body, and limb ribid bodies

	// Use this for initialization
	void Start () {
        pcs = GetComponent<PlayerControlScript>();
        limbRigidBodies.AddRange(animator.GetComponentsInChildren<Rigidbody2D>()); //fill a list with all rigid bodies on character
        limbColliders.AddRange(animator.GetComponentsInChildren<Collider2D>()); //fill a list with all the colliders on character
        CCDScript.AddRange(animator.GetComponentsInChildren<SimpleCCD>()); //get CCD scripts on character 
		//Note: CCD scripts control arms and legs, similarly to inverse kinematics
        
        getUpScript.fillList(this); //initializes getUpScript, which manages the process of recovering from ragdoll
        if (startInRagDoll)
        {
            toggleIsKinematic(true);//set to getup
			getUpScript.jointsEnabled(true);
            toggleHitBoxAndGrabColliders();
            toggleCCD();
            anim.enabled = false;
            //Debug.Log("starting in RagDoll.");
        }
        else
        {
			getUpScript.jointsEnabled (false);
            toggleLimbColliders();
			anim.enabled = true;
			setNewDesiredTransforms();
            //Debug.Log("starting in Animated State.");
        } 
	}
    
	// Update is called once per frame
	void Update () {
		if (resetDesiredToggle) {
			resetDesiredToggle = false;
			unlockDesiredTransforms ();
			//setNewDesiredTransforms (); //if uncommented would immediately reset transforms to current body position
		}

		if (!anim.enabled && !inGetUpPhase) {
			centerOverCenterBody ();
		}

		if (inGetUpPhase)
        {
			//increment the time we have been trying to get up
			timeSpentRagDollToAnim += Time.deltaTime;

				//if we have exeeded time we should have spend in "get up" phase
            if (timeSpentRagDollToAnim >= maxRagdollToAnimTime)
            {
				//at the end of a "get up" phase
                inGetUpPhase = false; //declare end of "get up" phase
                timeSpentRagDollToAnim = 0f;  //reset timer 
				getUpScript.resetLimbAnchors(); //move limb anchors, to make sure no dislocation has happened
				anim.enabled = true; //turn animator back on
                pcs.stats.RagdollRecovered();
                Instantiate(Resources.Load("Prefabs/Spell/GetupExplosion"), upperBody.transform.position, Quaternion.identity);
				//setting desiredRecoveryPose back to idle animation !!!! IS EXTREMELY REDUNDANT!!! CHANGE THIS WHEN YOU CAN!!!!
				setNewDesiredTransforms();
				getUpScript.resetCycle(); //tell the limbs that the cycle has ended. Allows them to find their initial position (for lerping both pos and rot).
            }
            else
				//if we still have time left before we should have recovered
            {
                
                //in "get up" cycle
				//move & rotate limbs over duration of 'max ragdoll to animation' time
				getUpScript.updateLimbTransforms(timeSpentRagDollToAnim/maxRagdollToAnimTime);
				//rotation of a parent fucks with the children limbs position
				//so we lock them to their anchor points
				getUpScript.resetLimbAnchors();
            }
        }
        else
        {
            if (ragdollToggle == true) //we are transitioning from ragdoll to getu up phase, or turning animator off and ragdolling
            {
                pcs.ToggleCamera(false);
                ragdollToggle = false; //reset the toggler
				if (!anim.enabled) //then we are about to enter "get up" phase, and when that is complete (after getUp time of character), animator will be toggled to ON
				{
                    //we want to now move the animator and hitbox over above the chest
					getUpScript.jointsEnabled (false); //turning hinge joints on character limbs OFF
                    centerOverUpperBody();
					if (!characterHolder.GetComponent<PlayerControlScript>().facingRight && characterHolder.transform.localScale.x > 0) {
						flipLimbs ();
						//characterHolder.transform.eulerAngles = new Vector3 (0, 0, 0);
						float temp = characterHolder.transform.localScale.z;
						characterHolder.transform.localScale = new Vector3 (-temp, Mathf.Abs(temp), Mathf.Abs(temp));

					}

                    getUpScript.moveCCDPoints(); //moves CCD points to 
                    toggleIsKinematic(true); //turning kinematics on
                    inGetUpPhase = true; //setting boolean to indicate "get up" phase
                    toggleLimbColliders(); //turning ON limb colliders
                    toggleHitBoxAndGrabColliders(); //turning ON hit box colliders
                    enableCCD(true); //turning on CCD scripts, allowing arms to rise towards CCD target points
					//getUpScript.resetLimbAnchors(); //moving limbs securely to where their anchor connectors are, to stop drifting limbs.

                }
                else //RagDolling, animation toggled to OFF
                {
					toggleHitBoxAndGrabColliders(); //turning OFF hit box colliders
                    pcs.ToggleCamera(true);

					anim.enabled = false; //animator enabled is OFF
					enableCCD(false); //turning CCD points OFF, so limbs fall lifeless
					if (!characterHolder.GetComponent<PlayerControlScript> ().facingRight && characterHolder.transform.localScale.x < 0) {
						flipLimbs ();
						Vector3 temp = characterHolder.transform.localScale;
						//Debug.Log (characterHolder.transform.localScale);
						//characterHolder.transform.eulerAngles = new Vector3(0, 180, 0);
						characterHolder.transform.localScale = new Vector3 (Mathf.Abs(temp.x),temp.y,temp.z);
						//Debug.Log (characterHolder.transform.localScale);

					}
					getUpScript.jointsEnabled (true);
                    toggleIsKinematic(false); //turning kinematics off, so as not to interfere with animator
					 //turning joints on character body ON
					getUpScript.resetLimbAnchors(); //moving limbs securely to where their anchor connectors are, to stop drifting limbs.
                    toggleLimbColliders(); //turning ON limb colliders

                }
            }
        }

        //if we are animating, we want the animation to be centered over the hitBox object
        if (anim.enabled)
            moveAnimatorToHitBox();
		
	}



	//unlocks desiredRecoveryPose, allowing transforms to be reset
	public void setNewDesiredTransforms(){
		desiredRecoveryPose.refillTransforms ();
		getUpScript.reinitializeDesiredTrans ();
		//next updat, desiredRecoveryPose will be reset next time desiredRecoveryPose.resetTransforms() is called
	}

	public void unlockDesiredTransforms (){
		desiredRecoveryPose.unlock ();
	}

	private void flipLimbs(){
		//Debug.Log ("In Flip Limbs.");
		foreach (Rigidbody2D limb in limbRigidBodies) {
			Vector3 tempV3 = limb.gameObject.transform.localRotation.eulerAngles;
			limb.gameObject.transform.localEulerAngles = new Vector3(tempV3.x, tempV3.y, -1 * tempV3.z);
			tempV3 = limb.gameObject.transform.localPosition;
			limb.gameObject.transform.localPosition = new Vector3 (-tempV3.x, tempV3.y, tempV3.z);
			//limb.gameObject.transform.localRotation.eulerAngles.
		}
	}

    private void toggleIsKinematic(bool gettingUp)
    {
		Vector2 tempVelo = new Vector3 ();
		Rigidbody2D charHolderRigidBody = characterHolder.GetComponent<Rigidbody2D> ();
        Rigidbody2D upperBod = upperBody.GetComponent<Rigidbody2D>();
		if (gettingUp) {
            //else we are enabling animator, and are disabling kinematic on limbs and enabling on char holder
            //need to average velocity of limbs and then add that velocity to char holder

            tempVelo = upperBod.velocity + ((pcs.storedForce + pcs.storedForceDt)/ pcs.getMassOfBody()); //fix incase instant concuss
            //Debug.Log(string.Format("temp velo {0} from upperbodVelo {1} + (pcsStored({2}) + pcsStoredDt({3}) / mass({4})) which is {5}", tempVelo, upperBod.velocity, pcs.storedForce, pcs.storedForceDt, pcs.getMassOfBody(), ((pcs.storedForce + pcs.storedForceDt) / pcs.getMassOfBody())));
            int numOfLimbs = pcs.GetNumberOfLimbs();
			foreach (Rigidbody2D limb in limbRigidBodies)
			{
                limb.mass = charHolderRigidBody.mass / numOfLimbs;
				limb.isKinematic = true;
			    limb.velocity = new Vector2(); //remove risidual velo on parts
			}
			charHolderRigidBody.isKinematic = false;
            charHolderRigidBody.velocity = tempVelo;
		} else {
            //then we are going into ragdoll, and want to enable kinematic on limbs and disable on char holder
            //need to add velocity to limbs
            Vector2 velo = charHolderRigidBody.velocity + ((pcs.storedForce + pcs.storedForceDt) / pcs.getMassOfBody());
            foreach (Rigidbody2D limb in limbRigidBodies)
            {
                limb.isKinematic = false;
            }
            foreach(KeyValuePair<string,Transform> kv in pcs.avatarManager.avatarLimbDict)
            {
                //Apply velo to each bodypart, not each part has a rigidbody, like the hands
                Rigidbody2D rb2 = kv.Value.gameObject.GetComponent<Rigidbody2D>();
                if(rb2)
                    rb2.velocity = velo;
            }
            //upperBody.GetComponent<Rigidbody2D>().velocity = charHolderRigidBody.velocity + ((pcs.storedForce + pcs.storedForceDt) / pcs.getMassOfBody());
            //Debug.Log(string.Format("velo set to {0} from charHolderRigidBody {1} + (pcsStored({2}) + pcsStoredDt({3}) / mass({4})) which is {5}", upperBody.GetComponent<Rigidbody2D>().velocity, charHolderRigidBody.velocity, pcs.storedForce, pcs.storedForceDt, pcs.getMassOfBody(), ((pcs.storedForce + pcs.storedForceDt) / pcs.getMassOfBody())));
            charHolderRigidBody.velocity = new Vector2(0, 0);
			charHolderRigidBody.isKinematic = true;            
		}
    }

    private void toggleLimbColliders()
    {
        foreach (Collider2D collider in limbColliders)
        {
            collider.enabled = !collider.enabled;
        }
    }

    private void toggleHitBoxAndGrabColliders()
    {
        HBBox.enabled = !HBBox.enabled;
        HBCircle.enabled = !HBCircle.enabled;
		GrabBoxLeft.enabled = !GrabBoxLeft.enabled;
		GrabBoxRight.enabled = !GrabBoxRight.enabled;
    }

    private void toggleCCD()
    {
        foreach (SimpleCCD script in CCDScript)
        {
            script.enabled = !script.enabled;
        }
    }

    private void enableCCD(bool isOn)
    {
        foreach (SimpleCCD script in CCDScript)
        {
            script.enabled = isOn;
        }
    }

    private void moveAnimatorToHitBox()
    {
            animator.transform.position = characterHolder.transform.position;
    }


	//NOTE: IF YOU MOVE CHEST OR ANIMATOR WHILE CHARACTER IS IN RAGDOLL, THIS CODE WILL CAUSE CHARACTER TO BE PLACED INCORRECTLY AT END OF RAGDOLL
    private void centerOverUpperBody()
    {
        //Purpose: centers the hitBox, and Animating GameObjects to the position of the UpperBody. Useful after ragdolling, because hitBox dosn't follow ragDoll'd characters.
        //determining offset of upperBody before moving hitBox
		Vector3 offset = upperBody.transform.position;
        //move hitBox to x-position of chest, and y-position of estimated point of collision of lowest limb
		characterHolder.transform.position = new Vector3(upperBody.transform.position.x, getDistFromChestToBottomOfFeet() + retRoughDistanceToGround(), 0);
        //now we move the animator to match the hitbox
        moveAnimatorToHitBox();
        //now we move the lower and upper body back to where they were in respect to the hitBox
        lowerBody.transform.position = upperBody.transform.position = offset;
    }

	private void centerOverCenterBody(){
		if (false) {
			Vector3 offset = upperBody.transform.position;
			characterHolder.transform.position = new Vector3 (upperBody.transform.position.x, upperBody.transform.position.y, 0);
			moveAnimatorToHitBox ();
			//now we move the lower and upper body back to where they were in respect to the hitBox
			lowerBody.transform.position = upperBody.transform.position = offset;
		}
	}

    private float retRoughDistanceToGround()
    {
		//old class, but still used. seems to look through for lowest limb transform to estimate location of ground
		//then returns it.
        Transform[] limbs = chest.GetComponentsInChildren<Transform>();
        float roughDistanceToGround = limbs[0].transform.position.y;
        for (int counter = 1; counter < limbs.Length; counter++)
        {
            if (limbs[counter].transform.position.y < roughDistanceToGround)
            {
                roughDistanceToGround = limbs[counter].transform.position.y;
            }
        }
        return roughDistanceToGround;
    }

    private float getDistFromChestToBottomOfFeet()
    {
        //returns the absolute distance from the cennter of the body to the endge of the circle collider at the feet
		//used to place character's colliders above the location where the ragdoll currently is. 
		return (Mathf.Abs(characterHolder.GetComponent<CircleCollider2D>().offset.y) + characterHolder.GetComponent<CircleCollider2D>().radius) * characterHolder.GetComponent<Transform>().localScale.y;
    }

	public bool getIsGetUpPhase(){
		return inGetUpPhase;
	}

}
