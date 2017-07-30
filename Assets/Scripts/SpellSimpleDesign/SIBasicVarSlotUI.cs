using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SIBasicVarSlotUI : MonoBehaviour {

    public string varName;
    public Text varNameText;
    protected SSTuple ssTuple;
    public GV.StateVarType stateVarType;

    public void Start()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        
    }

    public virtual string GetValue()
    {
        return "";
    }

    public virtual void FillTuple()
    {
        if (ssTuple == null)
            ssTuple = new SSTuple(varName, GetValue(), stateVarType);
    }

    public SSTuple GetTuple()
    {
        return ssTuple;
    }

}
