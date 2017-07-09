using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCore : MonoBehaviour {

    public SpellInfo si;
    public BoxCollider2D coreCollider;
    public List<GameObject> intersectingObjects = new List<GameObject>();
   
    public void Initialize(SpellInfo _si)
    {
        si = _si;
    }

    public bool IsTouchingCore(Transform otherObj)
    {
        //Debug.Log("are cores toucing? only touching N objects " + intersectingObjects.Count);
        //Debug.Log("are you touching " + otherObj.name + " have " + intersectingObjects.Count);
        /*if (intersectingObjects.Contains(otherObj.gameObject))
            Debug.LogError("CORE IS INTERSECTING YAY");
        else
        {
            string toOut = "";
            foreach(GameObject go in intersectingObjects)
                toOut += go.name + ",";
            //Debug.Log("No, only contains " + toOut);
        }*/

        if (si.spellState == GV.SpellState.Charging)
            return false;
        return intersectingObjects.Contains(otherObj.gameObject);
    }

    public void OnTriggerEnter2D(Collider2D coli)
    {
        ResolveCollision(coli.gameObject);
    }
    public void OnCollisionEnter2D(Collision2D coli)
    {
        ResolveCollision(coli.gameObject);
    }

    private void ResolveCollision(GameObject toAdd)
    {
        if (toAdd.name == "Collider")
            toAdd = toAdd.transform.parent.gameObject;

        if (!intersectingObjects.Contains(toAdd))
        {
            //Debug.Log("added " + toAdd.name + " at pos: " + toAdd.transform.position);
            intersectingObjects.Add(toAdd);
        }
    }

    private void ResolveExit(GameObject toRemove)
    {
        if (toRemove.name == "Collider")
            toRemove = toRemove.transform.parent.gameObject;
        intersectingObjects.Remove(toRemove.gameObject);
    }

public void OnTriggerExit2D(Collider2D coli)
    {
        ResolveExit(coli.gameObject);
    }

    public void OnCollisionExit2D(Collision2D coli)
    {
        ResolveExit(coli.gameObject);
    }
}
