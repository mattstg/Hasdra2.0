using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class DrawArrow : MonoBehaviour, IDragHandler,IEndDragHandler {

    LineRenderer lineRenderer;

    public void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
    }
    public void SetOrigin(Vector3 originPosition)
    {
        lineRenderer.SetPosition(0, originPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ValidDrag())
        {
            //if drag is valid, connect the two
        }
        transform.position = transform.parent.transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        lineRenderer.SetPosition(1, this.transform.position);
        
    }
    
    private bool ValidDrag()
    {
        return false;
    }


}
