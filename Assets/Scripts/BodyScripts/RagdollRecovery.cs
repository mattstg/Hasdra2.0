using UnityEngine;
using System.Collections;
using  System.Collections.Generic;


public class RagdollRecovery : MonoBehaviour {
    public GameObject recoveryTransforms;
    public GameObject bodyRoot;
    private List<recoverToStorage> limbData = new List<recoverToStorage>();
    private List<Transform> handFootTrans = new List<Transform>();
	private List<HingeJoint2D> joints = new List<HingeJoint2D>();
	private int numberOfRotatingBodyParts = 3; //used to determine which objects in limbData are (head, upper body, or lowerbody) vs (hand & foot transforms)

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void jointsEnabled(bool toSet){
		foreach(HingeJoint2D hinge in joints){
			hinge.enabled = toSet;
		}
	}

	public void resetLimbAnchors(){
		Transform tempTrans;
		int counter = 0;
		foreach (HingeJoint2D hinge in joints) {
			if (counter != 5) { //we dont want to love the lower body hinge, but we still want to be able to disable/enable it
				tempTrans = hinge.GetComponentInParent<Transform> ();
				tempTrans.localPosition = hinge.connectedAnchor;
			}
			counter++;
		}
	}

	public void updateLimbTransforms(float percentLeftOfGetUp){
		foreach (recoverToStorage limb in limbData)
		{
				limb.manualUpdate(percentLeftOfGetUp);
		}
	}

	public void reinitializeDesiredTrans(){
		foreach(recoverToStorage limb in limbData){
			limb.reinitializeDesiredTrans ();
		}
	}

	public void resetCycle(){
		foreach (recoverToStorage limb in limbData) {
			limb.resetCycle ();
		}
	}

    public void moveCCDPoints()
    {
		//need two counters, one to go through the hands and feet, one to check of is a hand and foot
		int limbCounter = 0;
		int handFootCounter = 0;
        foreach (recoverToStorage limb in limbData)
        {
			if (limbCounter > 2) //so only hand and ccd points don t need rotation. Therefore, these will only be those transforms.
            {
				limb.moveLimb(handFootTrans[handFootCounter].position);
				handFootCounter++;
            }
			limbCounter++;
        }
    }

    public void fillList(tempScriptAnim1 caller){
        

		//finding all hinge joints in character body, so as to allow them to be enabled / disabled for animation/ragdoll
		joints.AddRange (bodyRoot.GetComponentsInChildren<HingeJoint2D> ());

		//Finding the hands and feet, so as to be able to move CCD targets to them when we want to recover from ragdoll
		List<handFootIdentifyingScript> handFootScriptList = new List<handFootIdentifyingScript> ();
		handFootScriptList.AddRange (bodyRoot.GetComponentsInChildren<handFootIdentifyingScript> ());
		foreach (handFootIdentifyingScript script in handFootScriptList) {
			handFootTrans.Add (script.GetComponent<Transform> ());
			//Destroy (script);
		}

		//A temp script is used to indicate the character limbs which will be loaded into Limb Data List
		//then script is destroyed
		List<Transform> curTransformList = new List<Transform> ();
		List<tempbodyTagScript> bodyScriptList = new List<tempbodyTagScript> ();
		bodyScriptList.AddRange (bodyRoot.GetComponentsInChildren<tempbodyTagScript> ());
		foreach (tempbodyTagScript script in bodyScriptList) {
			curTransformList.Add (script.GetComponent<Transform> ());
			//Destroy (script);
		}
			
		//Now we are going to create a List of recoverToStorage, which is necessary for the Get Up Phase (after ragdoll).
		List<Transform> desTransformList = new List<Transform> (); //list of transforms where we desire to end up.
		desTransformList.AddRange (recoveryTransforms.GetComponentsInChildren<Transform> ()); 
		desTransformList.Remove (recoveryTransforms.transform);
		bool changeRot = true; //first few limbs rotate, then the rest dont rotate; changeRot indicated which and is dependent of counter
		int counter = 0; //counter used to track current # of limb
		foreach (Transform trans in desTransformList) {
			//if we are counter > numberOfRotatingBodyParts - 1, then are are at the index of curTransformList that indicate hand and foot positions (which do not rotate).
			//else leave changeRot = true for recoverToStorage constructor
			if (counter > numberOfRotatingBodyParts-1)
				changeRot = false; //changeRot is given in recoverToStorage constructor tells the object whether it rotates or not
            limbData.Add(new recoverToStorage(trans, curTransformList[counter], changeRot, bodyRoot));
			//recoveryToStorage, stores a limb object and the location where it wants to recover to (currently the position at the start of the idle animation).
			counter++;
        }
        
    }
}
