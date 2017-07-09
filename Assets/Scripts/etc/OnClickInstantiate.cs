using UnityEngine;
using System.Collections;

public class OnClickInstantiate : MonoBehaviour {

    public bool isActive = true;
    public GameObject toMake;
    float cnt = 0;
	// Update is called once per frame
	void Update () {
        if (isActive)
        {
            cnt += Time.deltaTime;
            if (Input.GetMouseButton(0) && cnt >= .1f && !Input.GetKey(KeyCode.LeftShift))
            {
                cnt = 0f;
                int playerCamToGet = 0;                 
                
                if (GV.NumOfPlayers == 2)
                    if (GV.GetPositionScreenPercentage(Input.mousePosition).y > .5f)
                        playerCamToGet = 1;

                //Instantiate(toMake, GV.worldLinks.masterCameraManager.GetPlayerCamera(playerCamToGet).ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
                GameObject go = Instantiate(toMake) as GameObject;
                go.transform.position = GV.worldLinks.masterCameraManager.GetPlayerCamera(playerCamToGet).ScreenToWorldPoint(Input.mousePosition);
            }
        }
	}

    public void Toggle()
    {
        isActive = false;
    }
}
