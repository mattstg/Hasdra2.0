using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Transition 
{
    public List<TransSlotStruct> slotStructs = new List<TransSlotStruct>();
    public State connectedFrom;
    public State connectedTo;
    public int transitionID = 0;

    public Transition(State _connectedFrom, State _connectedTo)
    {
        connectedFrom = _connectedFrom;
        connectedTo = _connectedTo;
    }
    
    //Created for game
    public Transition(Dictionary<string, string> dictInput,TreeTracker treeTracker)
    {
        ImportFromDictionary(dictInput, treeTracker);
    }

    /// <summary>
    /// TransitionXML extraction MUST occur only AFTER state's recieve the proper ids
    /// </summary>
    /// <returns></returns>
    public List<Dictionary<string, string>> ExportForXML(Vector2 fromOffset, Vector2 toOffset)
    {
        List<Dictionary<string, string>> toReturn = new List<Dictionary<string, string>>();

        Dictionary<string, string> mainTransXML = new Dictionary<string, string>();
        mainTransXML.Add("SuperType", "Transition");
        mainTransXML.Add("LinkFrom", connectedFrom.StateID.ToString());
        mainTransXML.Add("LinkTo", connectedTo.StateID.ToString());
        mainTransXML.Add("TransID", transitionID.ToString());
        mainTransXML.Add("fromOffsetX", fromOffset.x.ToString());
        mainTransXML.Add("fromOffsetY", fromOffset.y.ToString());
        mainTransXML.Add("toOffsetX", toOffset.x.ToString());
        mainTransXML.Add("toOffsetY", toOffset.y.ToString());
        toReturn.Add(mainTransXML);

        foreach (TransSlotStruct ss in slotStructs)
        {
            toReturn.Add(ss.ExportForXML());
        }

        return toReturn;
    }

    public virtual void ImportFromDictionary(Dictionary<string, string> dictInput, TreeTracker treeTracker)
    {
        connectedFrom = treeTracker.GetStateByID(int.Parse(dictInput["LinkFrom"]));
        connectedFrom.AddTransition(this);
        connectedTo = treeTracker.GetStateByID(int.Parse(dictInput["LinkTo"]));
        transitionID = int.Parse(dictInput["TransID"]);
    }

    public State CheckTransition(SpellInfo spellInfo)
    {
        foreach (TransSlotStruct ss in slotStructs)
        {
            if(!ss.CheckValid(spellInfo))
                return connectedFrom;
        }
        return connectedTo;
    }

    public List<TransSlotStruct> GetSlotStructs()
    {
        return slotStructs;
    }

    public void AddSlotStruct(TransSlotStruct _slotStruct)
    {
        _slotStruct.parentTransID = transitionID;
        if(!slotStructs.Contains(_slotStruct))
            slotStructs.Add(_slotStruct);
    }

    public void RemoveSlotStruct(TransSlotStruct _slotStruct)
    {
        slotStructs.Remove(_slotStruct);
    }
}
