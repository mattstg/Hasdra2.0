using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateMachineHotKeys : MonoBehaviour {

    public void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.K))
                GameObject.FindObjectOfType<SaveLoader>().SaveButtonPressed();
            if (Input.GetKeyDown(KeyCode.L))
                GameObject.FindObjectOfType<SaveLoader>().LoadButtonPressed();
            if (Input.GetKeyDown(KeyCode.X))
                GameObject.FindObjectOfType<SaveLoader>().ClearCurrentStateMachine(true);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            exitButtonMarker[] exitButtons = GameObject.FindObjectsOfType<exitButtonMarker>();
            foreach (exitButtonMarker ebtn in exitButtons)
            {
                if (ebtn.gameObject.activeInHierarchy)
                {
                    ebtn.PressExitButton();
                    break;
                }
            }
        }
    }

    
	
}

