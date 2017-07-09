using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
//[RequireComponent(typeof(DragHandler))]
public class State
{
    public int StateID;
    public List<Transition> outwardsTransitions = new List<Transition>();
    public List<StateSlot> stateSlots = new List<StateSlot>();
    public bool isStartState = false;

    public void AddStateSlot(StateSlot ss)
    {
        if (stateSlots.Contains(ss))
            Debug.LogError("trying to add multiple instances");
        if (ss.stateType == GV.States.StartState)
            isStartState = true;
        stateSlots.Add(ss);
    }

    public void RemoveStateSlot(StateSlot ss)
    {
        stateSlots.Remove(ss);
    }

    public void AddTransition(Transition toAdd)
    {
        outwardsTransitions.Add(toAdd);
    }

    public void RemoveTransition(Transition toRemove)
    {
        outwardsTransitions.Remove(toRemove);
    }

    public virtual void RetrieveInfo(){}
    

    public void ImportFromDictionary(Dictionary<string, string> importDict)
    {
        StateID = int.Parse(importDict["ID"]);
    }

    public List<Dictionary<string, string>> ExportForXML(Vector2 guiPosition)
    {
        Dictionary<string, string> statesDict = new Dictionary<string, string>();
        statesDict.Add("SuperType", "State");
        statesDict.Add("PosX", guiPosition.x.ToString());
        statesDict.Add("PosY", guiPosition.y.ToString());
        statesDict.Add("ID", StateID.ToString());

        List<Dictionary<string, string>> toRet = new List<Dictionary<string, string>>();
        toRet.Add(statesDict);
        foreach (StateSlot ss in stateSlots)
            toRet.Add(ss.ExportForXML());
        return toRet;
    }

    public void FillGUIFromInternalVars() { }

    public void PreformStateAction(Spell spell)
    {
        foreach (StateSlot ss in stateSlots)
            ss.PerformStateAction(spell);    
    }

    public StateSlot GetStartStateSlot()
    {
        if (!isStartState)
        {
            Debug.LogError("Is not start state");
            return null;
        }
         return stateSlots.Single(item => item.stateType == GV.States.StartState);
    }

    public void StateExiting(Spell spell)
    {
        foreach (StateSlot ss in stateSlots)
            ss.ExitState(spell);
    }
    
}
    
