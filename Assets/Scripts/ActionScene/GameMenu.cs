using UnityEngine;
using System.Collections;

public class GameMenu : MonoBehaviour {
    bool menuActive = false;
    Transform menu;
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuActive == false)
            {
                menu = (Instantiate(Resources.Load("Prefabs/ActionScene/GameplayMenu")) as GameObject).transform;
                menuActive = true;
            }
            else
            {
                CloseMenu();
            }
        }
	}

    public void CloseMenu()
    {
        if (menuActive)
        {
            Destroy(menu.gameObject);
            menuActive = false;
        }
    }
}
