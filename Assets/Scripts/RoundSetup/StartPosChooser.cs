using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPosChooser : MonoBehaviour {
    public Transform gridParent;
    string buttonPrefabLocation = "Prefabs/Menus/GenericButton";
    
    public void Start()
    {
        foreach(string s in GV.STARTING_LOCATIONS)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>(buttonPrefabLocation));
            Text text = go.GetComponentInChildren<Text>();
            text.text = s;
            go.transform.SetParent(gridParent);
            go.GetComponent<Button>().onClick.AddListener(delegate { ButtonSelected(s);});
            //Gotta set up the buttons delegate here
        }

    }

    private void ButtonSelected(string startPosSelected)
    {
        GameObject.FindObjectOfType<RoundSetupMainScript>().startPosName = startPosSelected;
    }
}
