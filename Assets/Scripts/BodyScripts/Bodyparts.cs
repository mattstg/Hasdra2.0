using UnityEngine;
using System.Collections;

public class Bodyparts {
    //Contains locations and sizes of bodyparts for tracking
    // this class could have a dictionary and store all parts, but for now just want the head
    //so since everything is a circle, and has a scale, we can use that to do the correct form of tri-ray cast :D then use LINQ to optimize the raycasthit code
    //at the moment, this just works for the head, since it raycast upwards from the point, instead of just a circle
    System.Collections.Generic.Dictionary<string,Transform> bodyParts = new System.Collections.Generic.Dictionary<string,Transform>();

    public Bodyparts(GameObject charShell)
    {
        Transform[] allt = charShell.GetComponentsInChildren<Transform>();
        foreach (Transform t in allt)
            bodyParts.Add(t.name, t);
    }

    private Transform GetPart(string name)
    {
        if (bodyParts.ContainsKey(name))
            return bodyParts[name];
        Debug.Log("Body part not found: " + name + " returning null");
        return null;
    }

    public bool IsHittingLimb(GameObject go,string limbName)
    {
        Transform bp = GetPart(limbName);
        if (bp == null)
            return false;
        else
        {
            int layerMaskValue = 1 << go.layer;
            RaycastHit2D[] hits = Physics2D.RaycastAll(bp.position, -Vector2.down, .2f, layerMaskValue);
            foreach (RaycastHit2D rc in hits)
            {
                if (rc.collider.name == "Collider")
                {
                    GameObject hitobj = rc.collider.transform.parent.gameObject;
                    if (hitobj == go)
                        return true;
                }
                else
                    if (rc.collider.gameObject == go)
                        return true;
            }
        }
        return false;

    }
	
}
