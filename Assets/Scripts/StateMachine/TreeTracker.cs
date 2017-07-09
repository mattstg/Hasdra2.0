using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TreeTracker : MonoBehaviour {

    public List<State> allStates = new List<State>();
    public List<Transition> allTransitions = new List<Transition>();

    public void AddState(State toAdd)
    {
        if (!allStates.Contains(toAdd))
            allStates.Add(toAdd);
    }

    public void AddTransition(Transition toAdd)
    {
        if (!allTransitions.Contains(toAdd))
        {
            allTransitions.Add(toAdd);
        }
    }

    public void AddTransitionByGUI(TransitionGUI toAdd)
    {

    }

    public void DeleteDraggable(GameObject go)
    {

    }

    public void Delete(State toDelete)
    {
        //add so it takes away from the lists as well
        foreach(Transition t in toDelete.outwardsTransitions)
            if(allTransitions.Contains(t))
                allTransitions.Remove(t);
        allStates.Remove(toDelete);
    }

    //only used in SM
    public void Delete(TransitionGUI toDelete)
    {
        if (allTransitions.Contains(toDelete.transition))
            allTransitions.Remove(toDelete.transition);
        GameObject.FindObjectOfType<SaveLoader>().RemoveTransGUI(toDelete);
        //GameObject.Destroy(toDelete.GetComponent<FollowTransform>().toFollow.gameObject); //deletes the button (dynamic line)
        GameObject.Destroy(toDelete.gameObject);
    }

    public void ClearTree()
    {
        allStates.Clear();
        allTransitions.Clear();
        List<Transform> toDelete = new List<Transform>();
        foreach (Transform t in GV.smUiLayer.stateParent)
        {
            if(t != GV.smUiLayer.stateParent)
                toDelete.Add(t);
        }
        foreach (Transform t in GV.smUiLayer.expandedStateParent)
        {
            if (t != GV.smUiLayer.expandedStateParent)
                toDelete.Add(t);
        }
        foreach (Transform t in GV.smUiLayer.transitionParent)
        {
            if (t != GV.smUiLayer.transitionParent)
                toDelete.Add(t);
        }
        foreach (Transform t in GV.smUiLayer.transitionGUIParent)
        {
            if (t != GV.smUiLayer.transitionGUIParent)
                toDelete.Add(t);
        }
        for (int i = toDelete.Count - 1; i >= 0; i--)
            GameObject.Destroy(toDelete[i].gameObject);
    }

    public State GetStateByID(int id)
    {
        return allStates.Single(item => item.StateID == id);
    }

    public Transition GetTransitionByID(int id)
    {
        try
        {
            return allTransitions.Single(item => item.transitionID == id);
        }
        catch
        {
            Debug.Log("getTransitionByID failure: " + id);
            return null;
        }
    }

    public State GetStartState()
    {
        return allStates.Single(item => item.isStartState == true);
    }

    public StateSlot GetStartStateSlot()
    {
        return allStates.Single(item => item.isStartState == true).GetStartStateSlot();
    }
}
