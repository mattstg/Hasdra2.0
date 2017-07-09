using UnityEngine;
using System.Collections;

public class testAmmo : MonoBehaviour {

    public bool updateInfo = true;
	void Update ()
    {
	    if(updateInfo)Debug.Log("Velocity: " + this.GetComponent<Rigidbody2D>().velocity.x);            
	}
}
