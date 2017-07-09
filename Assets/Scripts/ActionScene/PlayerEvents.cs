using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerEvents : MonoBehaviour {
    Dictionary<KeyCode, Ability> KeysToAbilities = new Dictionary<KeyCode, Ability>();
    
    /*
    Hold keys and calls ability
     * 
     * */
	// Use this for initialization
	void Start () {
	    //Input.GetKey(
	}
	
	// Update is called once per frame
	void Update () {
        CheckKeysPressed();
	}

    void CheckKeysPressed()
    {


    }
}
