using UnityEngine;
using System.Collections;

public class WorldSingletonLinks : MonoBehaviour  {

    public MasterCameraManager masterCameraManager;

    public void Awake()
    {
        GV.worldLinks = this;
    }
	
}
