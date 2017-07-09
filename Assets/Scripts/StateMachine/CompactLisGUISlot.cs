using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CompactLisGUISlot : MonoBehaviour {

    public Text textInput;
    StateGUI parentGUI;
    public StateSlot ss;

    public void Initialize(StateSlot _ss, StateGUI _parentGUI)
    {
        ss = _ss;
        parentGUI = _parentGUI;
        textInput.text = _ss.stateType.ToString();
    }

    public void DeletePressed()
    {
        if(ss.stateType != GV.States.StartState)
            parentGUI.RemoveStateSlot(ss);
    }
}
