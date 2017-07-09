using UnityEngine;
using System.Collections;

public class FollowTransform : MonoBehaviour {

    public Transform toFollow;

    public void Initialize(Transform t)
    {
        toFollow = t;
    }
	
	// Update is called once per frame
	void Update () {
        if(toFollow)
            transform.localPosition = toFollow.localPosition;
	}
}
