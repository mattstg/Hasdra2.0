using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StateSlotGUI : MonoBehaviour {

    StateGUI stateGUI;
    public StateSlot stateSlot;
    public Text stateNameText;
    public Text stateDescText;
    public Transform gridGroup;
    Dictionary<string,StateSlotGUIMinor> minorSlots = new Dictionary<string,StateSlotGUIMinor>();
    List<string> activeKeyWords = new List<string>();  //used for selective minor displaying, melee->melee_range...

    public void InitializeUI(StateGUI _stateGUI, StateSlot ss)
    {
        stateSlot = ss;
        stateGUI = _stateGUI;
        stateNameText.text = ss.stateType.ToString();
        stateDescText.text = ss.stateDesc;
        gridGroup.GetComponent<GridLayoutGroup>().cellSize = GV.SSMINOR_SIZE;
        foreach (KeyValuePair<string, SSTuple> kv in stateSlot.ssDict)
            InitializeSlotMinor(kv.Key, kv.Value);
        KeywordUpdated();
    }

    private void InitializeSlotMinor(string minorName, SSTuple _ssTuple)
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/StatePrefabs/SSGMinor")) as GameObject;
        StateSlotGUIMinor minor = go.GetComponent<StateSlotGUIMinor>();
        minor.transform.SetParent(gridGroup);
        minor.Initialize(minorName, _ssTuple, this);
        minorSlots.Add(minorName, minor);
        string minorSlotKeyword = minor.SetupAndGetInitialValue();
        if (!activeKeyWords.Contains(minorSlotKeyword))
        {
            activeKeyWords.Add(minorSlotKeyword);
        }
        ScaleSlotSizeToMatchElements();
    }

    private void ScaleSlotSizeToMatchElements()
    {
        Vector2 minorSize = GV.SSMINOR_SIZE;
        Vector2 gridCellSize = GV.STATESLOT_MIN_SIZE;

        int numOfMinorSlots = minorSlots.Count;
        int cols = (int)(GV.STATESLOT_MIN_SIZE.x / minorSize.x);
        int rows = (int)(GV.STATESLOT_MIN_SIZE.y / minorSize.y);
        int fitTotal = cols * rows;
        int numOfMinorOphans = numOfMinorSlots - fitTotal;
        bool lastGrewX = false;

        while (numOfMinorOphans > 0)
        {
            if (lastGrewX)
                gridCellSize += new Vector2(0, GV.SSMINOR_SIZE.y);
            else
                gridCellSize += new Vector2(GV.SSMINOR_SIZE.x, 0);
            lastGrewX = !lastGrewX;

            cols = (int)(gridCellSize.x / minorSize.x);
            rows = (int)(gridCellSize.y / minorSize.y);
            fitTotal = cols * rows;
            numOfMinorOphans = numOfMinorSlots - fitTotal;
        }
        stateGUI.GrowGridGroupSize(gridCellSize);
    }

 

    public Dictionary<string, string> GetUIDict()
    {
        Dictionary<string, string> toRet = new Dictionary<string, string>();
        foreach (KeyValuePair<string, StateSlotGUIMinor> kv in minorSlots)
        {
            toRet.Add(kv.Key, kv.Value.GetValue());
        }
        return toRet;
    }

    public void DeleteButtonPressed()
    {
        if(stateSlot.stateType != GV.States.StartState)
            stateGUI.RemoveStateSlot(stateSlot);
    }

    public void Save()
    {
        Dictionary<string, string> toSave = new Dictionary<string, string>();
        foreach (KeyValuePair<string, StateSlotGUIMinor> kv in minorSlots)
            toSave.Add(kv.Key, kv.Value.GetValue());
        stateSlot.SaveCoreValues(toSave);
    }

    

    private void KeywordUpdated()
    {
        foreach (KeyValuePair<string, StateSlotGUIMinor> kv in minorSlots)
            kv.Value.gameObject.SetActive(kv.Value.RequirementsMet(activeKeyWords));
    }

    public void ValueChange(string oldValue, string newValue)
    {
        activeKeyWords.Remove(oldValue);
        if(!activeKeyWords.Contains(newValue))
            activeKeyWords.Add(newValue);
        KeywordUpdated();
    }
}
