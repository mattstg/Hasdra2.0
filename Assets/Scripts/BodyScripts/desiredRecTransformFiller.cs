using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class desiredRecTransformFiller : MonoBehaviour {

	//TWO LISTS WHICH HOLD TRANSFORMS OF DESIRED POSITIONS, AND CORRESPONDING LIMBS
	List<Transform> desiredTransList = new List<Transform>();
	List<Transform> currentTransList = new List<Transform>();
	//CONNECTION TO CHEST ALLOWS FILLING
	public GameObject chest;
	public GameObject targets;
	private bool lockDesiredTrans = false;
	public bool lockMechanism = true;

	// Use this for initialization
	void Start () {
		//Filling desiredtransList with links to Transforms
		desiredTransList.AddRange (GetComponentsInChildren<Transform>());

		//Filling currentTransList with head, upper body, lowerbody, hands and feet Transforms
		List<tempbodyTagScript> tempListBody = new List<tempbodyTagScript>();
		tempListBody.AddRange (chest.GetComponentsInChildren<tempbodyTagScript> ());
		tempListBody.AddRange (targets.GetComponentsInChildren<tempbodyTagScript> ());
		foreach(tempbodyTagScript script in tempListBody){
			currentTransList.Add (script.GetComponent<Transform> ());
			Destroy (script);
		}
			
		desiredTransList.RemoveAt(0); //removing the unnecessary "desiredRecoverTransforms" holder in currentTransList at index 0 
		refillTransforms ();
	}

	public void refillTransforms(){
		if (!lockDesiredTrans) {
			//Debug.Log ("Filling Desired Transforms With Current Position Of Limbs!");
			int counter = 0;
			foreach (Transform desired in desiredTransList) {
				if (counter < currentTransList.Count) {
					Transform tempTransLink = currentTransList.ElementAt (counter);
					desired.localPosition = tempTransLink.localPosition;
					desired.localRotation = tempTransLink.localRotation;
				} else {
					Debug.Log ("refillTransforms method overflowing.");
					Debug.Log ("Counter = " + counter + "; currentTransList max index = " + (currentTransList.Count - 1));
				}
				//desired.localPosition = enumerator.Current.localPosition;
				counter++;
			}
			if (counter < desiredTransList.Count)
				Debug.Log ("refillTransforms method has not completely filling.");
			if (lockMechanism) {
				lockDesiredTrans = true;
				//Debug.Log ("desired recovery pose locked.");
			}
		}
	}

	public void unlock(){
		lockDesiredTrans = false;
		Debug.Log ("desired recovery pose unlocked.");
	}
}
