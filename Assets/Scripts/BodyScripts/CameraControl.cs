using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraControl {

    PlayerControlScript pcs;

    bool isLocked = false;

    Transform followTarget;

	public Camera cam;
    //public Camera particleCam;
    public float cameraDistance = 3.5f;
	// Use this for initialization
    public CameraControl(PlayerControlScript avatar)
    {
        pcs = avatar;
        followTarget = pcs.transform;
        GameObject newCam = new GameObject();
        newCam.name = "camera";
        cam = newCam.AddComponent<Camera>();
        if (pcs.pid == 0)
            avatar.gameObject.AddComponent<AudioListener>();
        cam.orthographic = true;
        SetupPlayerCamera(pcs.pid);
	}
	
	// Update is called once per frame
	public void Update (float dt) {
        if (isLocked)
            return;

        Vector3 loc = new Vector3(followTarget.position.x, followTarget.position.y + GV.CAMERA_OFFSET_Y, -10);

        cameraDistance = Mathf.Clamp(cameraDistance, 0, 50);

        cam.orthographicSize = cameraDistance;
        cam.transform.position = loc;
	}

    public void SetNormalizedViewport(Rect size)
    {
        cam.rect = size;
        //particleCam.rect = size;
    }

    void SetupPlayerCamera(int pid)
    {
        cam.cullingMask |= 1 << LayerMask.NameToLayer("Player" + pid + "GUI");
    }

    public void SetZoom(float zoom)
    {
        cam.orthographicSize = cameraDistance = zoom;
    }

    public void ModZoom(float modZoomAmt)
    {
        cameraDistance += modZoomAmt;
    }

    public void SetAsSplitScreen(bool setAsSplitScreen, int index)
    {
        if (setAsSplitScreen)
        {
            float numOfPlayers = 2;
            Rect cameraRect = new Rect(0, 0 + (index * .5f), 1, 1 - ((numOfPlayers - 1) * .5f));
            //Rect cameraRect = new Rect(0, (.5f * (numOfPlayers - 1)), 1, 1 - (.5f * (numOfPlayers - 1)));
            cam.rect = cameraRect;
        }
        else
        {
            Debug.Log("no logic for splitscreen -> normal supported yet");
        }

    }

    public void SetFollowTarget(Transform t)
    {
        followTarget = t;
    }
    
}
