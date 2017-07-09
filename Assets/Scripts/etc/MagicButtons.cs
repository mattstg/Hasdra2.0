using UnityEngine;
using System.Collections;

public class MagicButtons : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            PlayerControlScript[] pcs = FindObjectsOfType<PlayerControlScript>();
            foreach (PlayerControlScript pc in pcs)
            {
                Debug.Log("player gained 500 exp");
                pc.GainExp(500);
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GV.worldUI.menuManager.OpenMenu(GV.MenuType.statusEffects);
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            GV.worldUI.menuManager.OpenMenu(GV.MenuType.debugControls);
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            GV.worldUI.menuManager.OpenMenu(GV.MenuType.spells);
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            GV.worldUI.menuManager.OpenMenu(GV.MenuType.stats);
        }
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.D))
        {
            GV.worldUI.debugMenu.gameObject.SetActive(!GV.worldUI.debugMenu.gameObject.activeInHierarchy);
        }

        /*if (Input.GetKeyDown(KeyCode.Home))
        {
            Debug.Log("Increasing base run force");
            PlayerControlScript[] pcss = FindObjectsOfType<PlayerControlScript>();
			foreach (PlayerControlScript pcs in pcss) {
				pcs.stats.agility += 10;
			}
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.C))
        {
            PlayerControlScript[] pcss = FindObjectsOfType<PlayerControlScript>();
            foreach (PlayerControlScript pcs in pcss)
            {
                pcs.stats.concusionScore = 1000f; //will auto correct, or should
            }
        }   */
        if (Input.GetKey(KeyCode.End))
        {
            Spell[] allSpells = GameObject.FindObjectsOfType<Spell>();
            foreach(Spell s in allSpells)
            {
                s.DEBUG_GrowHacks = true;
                s.DEBUG_MaxEnergy = 200;
                s.GetComponent<Rigidbody2D>().isKinematic = true;
            }
        }
        /*if(Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus))
            FindObjectOfType<PlayerCameraScript>().cameraDistance -= .5f;
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            FindObjectOfType<PlayerCameraScript>().cameraDistance += .5f;*/
        if (Input.GetKeyDown(KeyCode.Alpha0))
            FindObjectOfType<OnClickInstantiate>().Toggle();

        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.M))
        {
            GV.ND_ON = !GV.ND_ON;
        }
	}

    public void ToggleStamper()
    {
        FindObjectOfType<OnClickInstantiate>().Toggle();
    }
}
