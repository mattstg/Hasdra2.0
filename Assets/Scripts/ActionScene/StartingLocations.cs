using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingLocations : MonoBehaviour {

    public Transform startingLocationParent;
    bool initialized = false;
    Dictionary<string, Transform> startingLocs;

    private void Initialize() //lazy initialization
    {
        initialized = true;
        startingLocs = new Dictionary<string, Transform>();
        foreach(Transform t in startingLocationParent)
            startingLocs.Add(t.name, t);
    }

    public Vector2 GetStartingPosition(string _name)
    {
        if (!initialized)
            Initialize();

        if (startingLocs.ContainsKey(_name))
            return startingLocs[_name].position;

        Debug.Log("starting position: " + _name + " not specified in startingLocations");
        return new Vector2();
    }


}
