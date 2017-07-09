using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public void SpellCreatorButtonPressed()
    {
        Application.LoadLevel("StateMachineScene");
    }

    public void SpellTesterButtonPressed()
    {
        Application.LoadLevel("SpellTesterScene");            
    }

    public void PlayerTesterButtonPressed()
    {
        Application.LoadLevel("PlayerTesterScene");
    }

    public void QuitButtonPressed()
    {
        Application.Quit();
    }
}
