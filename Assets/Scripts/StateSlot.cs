using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateSlot  {
    public string stateName = "na";
    public string stateDesc = "";
    public GV.States stateType;
    public Dictionary<string, SSTuple> ssDict = new Dictionary<string, SSTuple>();
    protected State parentState;

    public virtual void Initialize(State _parentState)
    {
        parentState = _parentState;
    }

    public void AddSSTuple(string name, string value, GV.StateVarType svt, List<string> _requirements, bool onlyOneRequirementRequired)
    {
        if(!ssDict.ContainsKey(name))
            ssDict.Add(name, new SSTuple(name, value, svt,_requirements, onlyOneRequirementRequired));
    }

    public void AddSSTuple(string name, string value, GV.StateVarType svt)
    {
        if (!ssDict.ContainsKey(name))
            ssDict.Add(name, new SSTuple(name, value, svt));
    }

    public void AddSSTuple(string name, string value, GV.StateVarType svt, string requirement)
    {
        if (!ssDict.ContainsKey(name))
            ssDict.Add(name, new SSTuple(name, value, svt, new List<string>{requirement},true));
    }

    public Dictionary<string, string> ExportForXML()
    {
        Dictionary<string, string> toRet = new Dictionary<string, string>();
        toRet.Add("parentID", parentState.StateID.ToString());
        toRet.Add("stateSlotType", stateType.ToString());
        toRet.Add("SuperType", "StateSlot");
        foreach (KeyValuePair<string, SSTuple> kv in ssDict)
            toRet.Add(kv.Key, kv.Value.svalue);
        return toRet;
    }

    public void ImportFromDictionary(Dictionary<string, string> importDict, TreeTracker treeTracker)
    {
        parentState = treeTracker.GetStateByID(int.Parse(importDict["parentID"]));
        foreach (KeyValuePair<string, string> kv in importDict)
            if (ssDict.ContainsKey(kv.Key))
                ssDict[kv.Key].svalue = kv.Value;

        VariableManualSave();
    }

    public virtual void VariableManualSave(){}

    public void SaveCoreValues(Dictionary<string, string> dict)
    {
        foreach (KeyValuePair<string, string> kv in dict)
            ssDict[kv.Key].svalue = kv.Value;
    }

    public virtual void PerformStateAction(Spell spell) { }


    public static StateSlot CreateStateSlot(GV.States stateType, State parentState, bool initializeOnCreate = true)
    {
        StateSlot toRet;
        switch (stateType)
        {
            case GV.States.Explode:
                toRet =  new ExplodeSS();
                break;
            case GV.States.Fracture:
                toRet = new FractureSS();
                break;
            case GV.States.Create:
                toRet =  new CreateSS();
                break;
            case GV.States.Empty:
                toRet =  new EmptySS();
                break;
            case GV.States.Rotate:
                toRet =  new RotateSS();
                break;
            case GV.States.Variable:
                toRet =  new VariableSS();
                break;
            case GV.States.Velo:
                toRet =  new VeloSS();
                break;
            case GV.States.StartState:
                toRet = new StartStateSS();
                break;
            case GV.States.Follow:
                toRet = new FollowSS();
                break;
            case GV.States.Position:
                toRet = new PositionSS();
                break;
            case GV.States.VeloVector:
                toRet = new VeloVectorSS();
                break;
            case GV.States.Radio:
                toRet = new RadioSS();
                break;
            case GV.States.SkillMod:
                toRet = new SkillModSS();
                break;
            case GV.States.DmgVelo:
                toRet = new DamageVectorSS();
                break;
			case GV.States.FaceVeloVec:
				toRet =  new FaceVeloVecSS();
				break;
            case GV.States.SetAlpha:
                toRet = new SetAlphaSS();
                break;
            case GV.States.SetColor:
                toRet = new ColorSS();
                break;
            case GV.States.ConvertToSolidMaterial:
                toRet = new ConvertToSolidSS();
                break;
            case GV.States.Destroy:
                toRet = new DestroySS();
                break;
            default:
                Debug.LogError("e:default: " + stateType);
                toRet =  new EmptySS();
                toRet.stateType = GV.States.Empty;
                break;
        }
        toRet.stateType = stateType;
        if(initializeOnCreate)
            toRet.Initialize(parentState);
        return toRet;
    }

    public virtual void ExitState(Spell spell)
    {

    }
}
