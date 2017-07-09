using UnityEngine;
using System.Collections;

public class ScrollTracker : MonoBehaviour {

    public bool clampY = false;
    public Vector2 yClamp = new Vector2(0,0);
	// Update is called once per frame
	void Update () {
        if (clampY)
        {
            Vector2 newPos = transform.position;
            if (newPos.y < yClamp.x)
                newPos.y = yClamp.x;
            if (newPos.y > yClamp.y)
                newPos.y = yClamp.y;
            transform.position = newPos;
        }
	}

    
}
