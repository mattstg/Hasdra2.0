using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelStatsGUI : MonoBehaviour {

    public GridLayoutGroup abilityGridGroup;
    List<string> textBoxTitles = new List<string>() { "str,agi,const,wis,int,char" };
    Dictionary<string, Text> textBoxes = new Dictionary<string, Text>();
    
    void Start () {
        
        foreach (Transform t in transform)
        {
            if (textBoxTitles.Contains(t.name))
                textBoxes.Add(t.name, t.GetChild(0).GetComponent<Text>());
        }
        foreach (KeyValuePair<string,Text> kv in textBoxes)
        {
            kv.Value.text = "1";
        }
	}
	
	
}
