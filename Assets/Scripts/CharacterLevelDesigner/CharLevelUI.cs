using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharLevelUI : MonoBehaviour {

    public MainLevelDesignerScript ms;
    public Tower tower;
    public Transform storeGrid;
    public Transform canvas;
    public InputField loopAmtInput;
    public InputField characterName;
    public InputField searchBar;
    public Text expCost;

	// Use this for initialization
	void Awake () {
        GV.charLvlUI = this;
	}
}

