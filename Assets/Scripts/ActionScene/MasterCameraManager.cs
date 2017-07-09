using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MasterCameraManager : MonoBehaviour {
    CameraControl[] cameras = new CameraControl[2];

    public void AddCameraToTrack(CameraControl playerCam, int pid)
    {
        cameras[pid] = playerCam;
    }

    public Camera GetPlayerCamera(int pid)
    {
        return cameras[pid].cam;
    }

    public CameraControl GetCameraControl(int pid)
    {
        return cameras[pid];
    }

}
