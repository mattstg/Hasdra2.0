using UnityEngine;
using System.Collections;

public class MenuSceneTraversal : MonoBehaviour {
    

    public void ButtonPressed(string s) //String is set in UnityEditor and should match the scene names
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(s);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
