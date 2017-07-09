using UnityEngine;
using System.Collections;

public class SkinButton : MonoBehaviour {

    public void ButtonPressed()
    {
        FindObjectOfType<RoundSetupMainScript>().SkinSelected(GetComponent<UnityEngine.UI.Text>().text);
    }
}
