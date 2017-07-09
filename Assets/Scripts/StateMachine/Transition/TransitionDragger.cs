using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TransitionDragger :  MonoBehaviour {
    TransCreateZone creationZone;
    Vector2 fromOffset;
    //public GameObject 

    public void Initialize(TransCreateZone _creationZone, Vector2 _fromOffset)
    {
        creationZone = _creationZone;
        fromOffset = _fromOffset;
    }

    public void Update()
    {
        transform.position = Input.mousePosition;
    }
    
    public void OnPointerUp()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;
        List<RaycastResult> rcr = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, rcr);

        bool validPlacement = false;
        TransCreateZone otherZone = null;

        foreach (RaycastResult rc in rcr)
        {
            if (rc.gameObject.GetComponent<StateCompactGUI>() || rc.gameObject.GetComponent<TransCreateZone>() == creationZone)
            {
                validPlacement = false;
                break;
            }
            if (rc.gameObject.GetComponent<TransCreateZone>() != null)
            {
                otherZone = rc.gameObject.GetComponent<TransCreateZone>();
                validPlacement = true;
            }
        }

        if (validPlacement)
            LinkTwoStates(otherZone,Input.mousePosition);
        Destroy(this.gameObject);
    }

    public void LinkTwoStates(TransCreateZone otherZone, Vector3 currentDropPos)
    {
        Vector2 toOffset = currentDropPos - otherZone.sgc.gameObject.transform.position;
        TransitionGUI newTGui = FindObjectOfType<TransitionCreator>().CreateNewTransition(creationZone.sgc, fromOffset, otherZone.sgc, toOffset);
    }

}
