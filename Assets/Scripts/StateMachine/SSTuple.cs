using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SSTuple {

    public string name;
    public GV.StateVarType varType;
    public string value;
    public List<string> requirements;
    public bool onlyOneRequirementRequired;


    public SSTuple(string _name, string _value, GV.StateVarType _varType) 
    {
        name = _name;
        value = _value;
        varType = _varType;
    }

    public SSTuple(string _name, string _value, GV.StateVarType _varType, List<string> _requirements, bool _onlyOneRequirementRequired)
    {
        name = _name;
        value = _value;
        varType = _varType;
        requirements = _requirements;
        onlyOneRequirementRequired = _onlyOneRequirementRequired;
    }


    public T CastValue<T>()
    {
        if(typeof(T).IsEnum)
            return (T)Enum.Parse(typeof(T), value);
        return (T)System.Convert.ChangeType(value, typeof(T));
    }

   
}

