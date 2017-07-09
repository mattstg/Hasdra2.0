using UnityEngine;
using System.Collections;

public class ObjectDestroyer : MonoBehaviour {

    public void CustomDestroyObject(GameObject toDestroy)
    {
        //eventaully there will be object, so pass everything through here
        //Debug.Log("Destroyed: " + toDestroy.name);

        GameObject.Destroy(toDestroy);
    }
    
}
