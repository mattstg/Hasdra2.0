using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class WorldUILinks : MonoBehaviour {
    
    public GenericPopup genericMenu;
    public MenuManager menuManager; //use this instead of generic menu
    public Transform mainCanvas;
    public Transform splitScreenBar;
    public Dictionary<int, PlayerControlScript> players = new Dictionary<int, PlayerControlScript>();
    public Transform habitatParent;
    public Transform debugMenu;

    void Awake()
    {
        GV.worldUI = this;
    }
}
