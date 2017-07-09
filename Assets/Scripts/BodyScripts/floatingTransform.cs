using UnityEngine;
using System.Collections;

public class floatingTransform : MonoBehaviour {
	public GameObject self;

	public void setPosition(Transform t1, Transform t2){ //your awesome
		self.transform.position = (t1.position + t2.position)/2f;
	}

	public Transform retTransform(){
		return self.transform;
	}

}
