using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Branch : MonoBehaviour {

    Branch parentBranch;
    public List<treeKnob> treeKnobs;

    public void Start()
    {
        treeKnobs = new List<treeKnob>(){new treeKnob(true,new Vector2(.223f,1)),new treeKnob(true,new Vector2(-.223f,1)),new treeKnob(false,new Vector2(.462f,.722f)),new treeKnob(false,new Vector2(.462f,.353f)),
                    new treeKnob(false,new Vector2(-.462f,.353f)),new treeKnob(false,new Vector2(-.462f,.722f))};
                        
    }

    public bool Split() //return false if unsuccesful
    {
        return true;
    }
}
