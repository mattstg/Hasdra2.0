using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class StateCompactGUI : MonoBehaviour {

    public StateGUI stateGUI; //expanded state gui
    public Dictionary<StateSlot, CompactLisGUISlot> activeSSList = new Dictionary<StateSlot, CompactLisGUISlot>();
    public Transform compactListGridParent;
    public List<TransitionGUI> transitionGUIs = new List<TransitionGUI>();


    public void Start()
    {
        //dragHandler = this.GetComponent<DragHandler>();
    }

    public void InitializeNew()
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/StatePrefabs/StateGUIExpanded")) as GameObject;
        go.transform.SetParent(GV.smUiLayer.expandedStateParent);
        go.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        stateGUI = go.GetComponent<StateGUI>();
        stateGUI.Initialize(new State(), this);
        go.SetActive(false);
    }

    public void LoadExisting(State _state)
    {
        InitializeNew();
        stateGUI.Initialize(_state, this);
    }

    public void ExpandPressed()
    {
        stateGUI.gameObject.SetActive(true);
    }

    public void ContractPressed()
    {
        stateGUI.gameObject.SetActive(false);
        Save();
    }

    private void Save()
    {
        stateGUI.Save();
    }

    public void DeleteButton()
    {
        if (stateGUI.state.isStartState)
            return;
        FindObjectOfType<SaveLoader>().treeTracker.Delete(stateGUI.state);
        for(int i = transitionGUIs.Count - 1; i >= 0; i--)
        {
            FindObjectOfType<SaveLoader>().RemoveTransGUI(transitionGUIs[i]);
            transitionGUIs[i].DeleteTransition();        
        }
        Destroy(this.gameObject);
        FindObjectOfType<SaveLoader>().RemoveStateGUICompact(this);
    }


    public void RemoveFromCompactList(StateSlot ss)
    {
        Destroy(activeSSList[ss].gameObject);
        activeSSList.Remove(ss);
    }

    public List<Dictionary<string, string>> ExportForXML()
    {
        return stateGUI.state.ExportForXML(transform.position);
    }

    public void RemoveTransition(TransitionGUI tgui)
    {
        transitionGUIs.Remove(tgui);
        stateGUI.state.RemoveTransition(tgui.transition);
    }

    public void AddCompactListElement(StateSlot ss)
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/StatePrefabs/StateGUICompactListElem")) as GameObject;
        CompactLisGUISlot clgs = go.GetComponent<CompactLisGUISlot>();
        clgs.Initialize(ss, stateGUI);
        activeSSList.Add(ss,clgs);
        go.transform.SetParent(compactListGridParent);
    }

    public void AddNewStateSlot(GV.States stateType)
    {
        if (stateType == GV.States.StartState)
            stateGUI.state.StateID = 0;
        StateSlot ss = stateGUI.NewStateSlotOfType(stateType);
        AddCompactListElement(ss);
    }

    public void AddTransitionGUI(TransitionGUI tg)
    {
        if (transitionGUIs.Contains(tg))
            Debug.LogError("dats fucked up");
        transitionGUIs.Add(tg);
    }

    public void OnDrag()
    {
        transform.position = Input.mousePosition;
    }
}
