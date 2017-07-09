using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class exitButtonMarker : MonoBehaviour {
    public Button exitButton;
	//useful script for knowing which exit buttons are active for pressing esc

    public void PressExitButton()
    {
        exitButton.onClick.Invoke();
    }
}

