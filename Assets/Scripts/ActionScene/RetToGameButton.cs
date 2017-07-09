using UnityEngine;
using System.Collections;

public class RetToGameButton : MonoBehaviour {

    //ugly patch i know, menu traversal will need a lil clean later
    public void RetToGamePressed()
    {
        StaticReferences.mainScriptsGO.GetComponent<GameMenu>().CloseMenu();
    }
}
