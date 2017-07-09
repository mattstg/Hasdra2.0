using UnityEngine;
using System.Collections;

public class CharacterSelection : MonoBehaviour {

    public string characterName = "";

    public void CharacterSelectButtonPressed()
    {
        GameObject.FindGameObjectWithTag("MainScripts").GetComponent<RoundSetupMainScript>().CharacterSelected(characterName);
    }
}
