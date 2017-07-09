using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ScrollButtons : MonoBehaviour, IPointerDownHandler {

    public Transform scrollingTarget;
    public bool upDir;
    

    public void OnPointerDown(PointerEventData eventData)
    {
        int mod = (upDir) ? 1 : -1 ;
        scrollingTarget.position = new Vector3(0, mod*GV.PLAYERLEVELDESIGN_SCROLL_SPEED, 0) + scrollingTarget.position;
    }

    
}
