using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StateGUI : MonoBehaviour {

    public State state;
    public StateCompactGUI stateGUICompact;
    public Transform gridGroup;
    public List<StateSlotGUI> SSGs = new List<StateSlotGUI>();

    public void Initialize(State ss,StateCompactGUI sgc)
    {
        state = ss;
        stateGUICompact = sgc;
    }

    public void NewStateSlotButton()
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/StatePrefabs/CreateStateList")) as GameObject;
        go.transform.SetParent(GV.smUiLayer.inputPopups);
        go.transform.localPosition = new Vector3(0, 0, 0);
        go.GetComponent<NewStatePopup>().Initialize(stateGUICompact);
    }

    public StateSlot NewStateSlotOfType(GV.States stateType)
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/StatePrefabs/StateSlotGUI")) as GameObject;
        go.transform.SetParent(gridGroup);
        StateSlotGUI ssg = go.GetComponent<StateSlotGUI>();
        StateSlot ss = StateSlot.CreateStateSlot(stateType, state);
        ssg.stateSlot = ss;
        ssg.InitializeUI(this,ss);
        SSGs.Add(ssg);
        state.AddStateSlot(ss);
        //if (stateType == GV.States.StartState)
        //    TransformIntoStartStateGUI();
        return ss;
    }

    public void AddExistingStateSlot(StateSlot ss)
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/StatePrefabs/StateSlotGUI")) as GameObject;
        go.transform.SetParent(gridGroup);
        StateSlotGUI ssg = go.GetComponent<StateSlotGUI>();
        ssg.stateSlot = ss;
        ssg.InitializeUI(this, ss);
        SSGs.Add(ssg);
        state.AddStateSlot(ss);
        if (ss.stateType == GV.States.StartState)
            TransformIntoStartStateGUI();
    }

    public void TransformIntoStartStateGUI()
    {
        DestroyImmediate(gridGroup.GetComponent<GridLayoutGroup>());
        gridGroup.gameObject.AddComponent<VerticalLayoutGroup>();
    }

    public void ExitButton()
    {
        stateGUICompact.ContractPressed();  //this will run around and call save
    }

    public void RemoveStateSlot(StateSlot ss)
    {
        StateSlotGUI toRemove = SSGs.Single(item => item.stateSlot == ss);
        SSGs.Remove(toRemove);
        state.RemoveStateSlot(ss);
        Destroy(toRemove.gameObject);
        stateGUICompact.RemoveFromCompactList(ss);
    }

    public void GrowGridGroupSize(Vector2 newSize)
    {
        if (gridGroup.GetComponent<GridLayoutGroup>() == null)
            return;  //start state uses something different, so if not there, do ntohing

        Vector2 curSize = gridGroup.GetComponent<GridLayoutGroup>().cellSize;
        if (newSize.x > curSize.x && newSize.y > curSize.y)
            gridGroup.GetComponent<GridLayoutGroup>().cellSize = newSize;
    }

    public void Save()
    {
        foreach (StateSlotGUI ssg in SSGs)
            ssg.Save();
    }

	
}
