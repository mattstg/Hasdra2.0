using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SSTuple {

    public string name;
    public GV.StateVarType varType;
    private string _v;

    public string svalue
    {
        set
        {
            _v = value;
            if (varType == GV.StateVarType.Float)
                fvalue = float.Parse(value);
        }
        get { return _v; }
    }



    public float fvalue;
    public List<string> requirements;
    public bool onlyOneRequirementRequired;

    public SSTuple(SSTuple _sstuple)
    {
        name = _sstuple.name;
        varType = _sstuple.varType;
        svalue = _sstuple.svalue;
    }

    public SSTuple(string _name, string _value, GV.StateVarType _varType) 
    {
        name = _name;
        varType = _varType;
        svalue = _value;
    }

    public SSTuple(string _name, string _value, GV.StateVarType _varType, List<string> _requirements, bool _onlyOneRequirementRequired)
    {
        name = _name;
        varType = _varType;
        svalue = _value;        
        requirements = _requirements;
        onlyOneRequirementRequired = _onlyOneRequirementRequired;
    }

    public float GetFValue()
    {
        return fvalue;
    }

    public T CastValue<T>()
    {
        if(typeof(T).IsEnum)
            return (T)Enum.Parse(typeof(T), svalue);
        return (T)System.Convert.ChangeType(svalue, typeof(T));
    }

   
}

