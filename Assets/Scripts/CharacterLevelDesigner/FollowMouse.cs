using UnityEngine;
using System.Collections;

public class FollowMouse : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        transform.position = Input.mousePosition;
	}
}
