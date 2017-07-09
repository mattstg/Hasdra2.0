using UnityEngine;
using System.Collections;

public class DynamicLine : MonoBehaviour {

    public Transform from;
    public Transform to;
    Vector2 fromOffset;
    Vector2 toOffset;
	
	// Update is called once per frame
	void Update () 
    {
        Vector3 startPt = from.position + new Vector3(fromOffset.x,fromOffset.y,0);
        Vector3 endPt = to.position + new Vector3(toOffset.x, toOffset.y, 0);

        float angle = GV.GetAngleOfLineBetweenTwoPoints(startPt, endPt);
        float distance = Vector3.Distance(startPt, endPt);
        Vector3 midpoint = (startPt + endPt) / 2;
        transform.eulerAngles = new Vector3(0, 0, angle);
        transform.localScale = new Vector3(distance, 1, 1);
        this.transform.position = midpoint;
	}

    public void SetTransforms(Transform _from, Vector2 _fromOffset, Transform _to, Vector2 _toOffset)
    {
        from = _from;
        to = _to;
        fromOffset = _fromOffset;
        toOffset = _toOffset;
    }
}
