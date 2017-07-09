using UnityEngine;
using System.Collections;

public class recoverToStorage{
    private Vector3 desiredLocalPos;
	private Vector3 initialLocalPos;
	private Quaternion initialLocalRot;
	private Quaternion desiredLocalRot;
    private bool rotates;
	private Transform linkToDesiredTransform;
	private Transform linkToLimbTransform;
    public GameObject centerOfBody;
	private bool firstTime = true; //internal variable, denotes first update in a given "get up" cycle

    public recoverToStorage(Transform desiredInfo, Transform newCurrentTrans, bool newChangeRot, GameObject inBodyRoot){
		linkToDesiredTransform = desiredInfo;
		centerOfBody = inBodyRoot;
		//desiredPos = new Vector3(desiredInfo.position.x, desiredInfo.position.y, desiredInfo.position.z);
		desiredLocalPos = linkToDesiredTransform.localPosition;
		rotates = newChangeRot;
		if(rotates)
			desiredLocalRot = linkToDesiredTransform.localRotation;
        linkToLimbTransform = newCurrentTrans;
    }

	//reset var firstTime, to represent end of a "get up" cycle, and next time update is called it will know its the first update in new cycle
	public void resetCycle(){
		firstTime = true;
	} 

	//moves limb towards desired pos, which was defined w/ constructor
	public void manualUpdate(float percentTimeLeftOfRagdoll){
		if (firstTime) {
			//Debug.Log ("Starting new ragdoll cycle on : " + parentToString());
			firstTime = false;
			if(rotates)
				initialLocalRot = linkToLimbTransform.localRotation;
			initialLocalPos = linkToLimbTransform.localPosition;
			//Debug.Log (parentToString() + " initial position is equal to : " + initialLocalPos);
		}
		//		Debug.Log (parentToString () + " desired position is equal to : " + desiredLocalPos);

		//linkToLimbTransform.position = Vector3.Lerp (initialPos, desiredPos + centerOfBody.transform.position, percentTimeLeftOfRagdoll); (1)
		linkToLimbTransform.localPosition = Vector3.Lerp (initialLocalPos, desiredLocalPos, percentTimeLeftOfRagdoll);
		
		if (rotates)
			linkToLimbTransform.localRotation = Quaternion.Lerp (initialLocalRot, desiredLocalRot, percentTimeLeftOfRagdoll);
	}
		
    public void moveLimb(Vector3 locToMove)
    {
        linkToLimbTransform.position = locToMove;
    }

    public string parentToString()
    {
        return linkToLimbTransform.ToString();
    }

	public void reinitializeDesiredTrans(){
		desiredLocalPos = linkToDesiredTransform.localPosition;
		if(rotates)
			desiredLocalRot = linkToDesiredTransform.localRotation;
	}
}
