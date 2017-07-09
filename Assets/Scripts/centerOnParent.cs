using UnityEngine;
using System.Collections;

public class centerOnParent : MonoBehaviour {

    Transform _parent;
    public Transform parent { set { if (value != null) { this.enabled = true; _parent = value; } } get { return _parent; } }
    public bool scaleWithParent = false;

    public void Update()
    {
        if (parent)
        {
            transform.position = parent.position;
            if (scaleWithParent)
                transform.localScale = parent.transform.localScale;
        }
        else
            this.enabled = false;
    }
}
