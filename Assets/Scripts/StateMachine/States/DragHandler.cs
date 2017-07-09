using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector3 startPosition;
    public List<GameObject> collidingObjects = new List<GameObject>(); //can acccess this to get list of objects colliding with (not children)
    public bool dragEnabled = true;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (dragEnabled)
        {
            startPosition = transform.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragEnabled)
        {
            transform.position = Input.mousePosition;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (dragEnabled)
        {
            foreach (GameObject go in collidingObjects)
            {
                if (go.CompareTag("DeleteZone"))
                    GameObject.FindObjectOfType<TreeTracker>().DeleteDraggable(this.gameObject);
            }
        }
    }

    public void ValidPlacement(bool isValid)  //call this during OnEndDrag
    {
        if (isValid)
        {
            startPosition = transform.position;
        }
        else
        {
            transform.position = startPosition;
        }
    }

    public void OnTriggerEnter2D(Collider2D otherObject)
    {
        if (dragEnabled)
        {
            //first find topmost parent
            List<Transform> allParents = GV.GetAllParents(transform);
            List<Transform> ignoredTransforms = new List<Transform>();
            foreach (Transform t in transform) //ignore one layer down children (should be all layers) TOADD
                ignoredTransforms.Add(t);
            ignoredTransforms.AddRange(allParents);

            if (!ignoredTransforms.Contains(otherObject.transform)) //not a child of this object
            {
                if (!collidingObjects.Contains(otherObject.gameObject))
                    collidingObjects.Add(otherObject.gameObject);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D otherObject)
    {
        if (dragEnabled)
        {
            if (collidingObjects.Contains(otherObject.gameObject))
                collidingObjects.Remove(otherObject.gameObject);
        }
    }



   
}
