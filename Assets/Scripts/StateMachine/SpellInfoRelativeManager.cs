using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellInfoRelativeManager  {
    SpellInfo si; //parent
    Dictionary<GV.SpellInfoDataType, SpellInfoRelativeData<bool>> relativeBoolData;
    Dictionary<GV.SpellInfoDataType, SpellInfoRelativeData<float>> relativeFloatData;
    Dictionary<GV.SpellInfoDataType, SpellInfoRelativeData<Vector2>> relativeVec2Data;
    Dictionary<GV.SpellInfoDataType, SpellInfoRelativeData<Dictionary<string,float>>> relativeDictionarySFData;  //String Float
    //System.Collections.Generic.Dictionary<GV.SpellInfoDataType, SpellInfoRelativeData<GV.>> relativeBoolData;

    public SpellInfoRelativeManager(SpellInfo _si)
    {
        si = _si;
    }

    //called when spell is launched
    public void CreateAllRelativeDatas()
    {
        relativeBoolData = new Dictionary<GV.SpellInfoDataType, SpellInfoRelativeData<bool>>();
        relativeFloatData = new Dictionary<GV.SpellInfoDataType, SpellInfoRelativeData<float>>();
        relativeVec2Data = new Dictionary<GV.SpellInfoDataType, SpellInfoRelativeData<Vector2>>();

        relativeBoolData.Add(GV.SpellInfoDataType.GoNext, new SpellInfoRelativeData<bool>(GV.SpellInfoDataType.GoNext, true));
        relativeFloatData.Add(GV.SpellInfoDataType.Density, new SpellInfoRelativeData<float>(GV.SpellInfoDataType.Density,si.density));
        relativeFloatData.Add(GV.SpellInfoDataType.Angle, new SpellInfoRelativeData<float>(GV.SpellInfoDataType.Angle, si.currentAngle));
        relativeFloatData.Add(GV.SpellInfoDataType.Energy, new SpellInfoRelativeData<float>(GV.SpellInfoDataType.Energy,si.currentEnergy));
        relativeFloatData.Add(GV.SpellInfoDataType.Intelligence, new SpellInfoRelativeData<float>(GV.SpellInfoDataType.Intelligence,si.intelligence));
        relativeFloatData.Add(GV.SpellInfoDataType.Mass, new SpellInfoRelativeData<float>(GV.SpellInfoDataType.Mass,si.mass));
        relativeFloatData.Add(GV.SpellInfoDataType.Altitude, new SpellInfoRelativeData<float>(GV.SpellInfoDataType.Altitude, si.altitude));
        //relativeFloatData.Add(GV.SpellInfoDataType.Speed, new SpellInfoRelativeData<float>(GV.SpellInfoDataType.Speed));
        relativeFloatData.Add(GV.SpellInfoDataType.Stability, new SpellInfoRelativeData<float>(GV.SpellInfoDataType.Stability,si.stability));
        relativeFloatData.Add(GV.SpellInfoDataType.Time, new SpellInfoRelativeData<float>(GV.SpellInfoDataType.Time,si.timeAlive));
        relativeFloatData.Add(GV.SpellInfoDataType.Velocity, new SpellInfoRelativeData<float>(GV.SpellInfoDataType.Velocity,si.velocity));
        relativeFloatData.Add(GV.SpellInfoDataType.Wisdom, new SpellInfoRelativeData<float>(GV.SpellInfoDataType.Wisdom,si.wisdom));
        relativeFloatData.Add(GV.SpellInfoDataType.Variable, new SpellInfoRelativeData<float>(GV.SpellInfoDataType.Variable, si.smVariable));
        //relativeDictionarySFData.Add(GV.SpellInfoDataType.Variable, new SpellInfoRelativeData<Dictionary<string,float>>(GV.SpellInfoDataType.Variable, si.smVariable));
        relativeVec2Data.Add(GV.SpellInfoDataType.Pos, new SpellInfoRelativeData<Vector2>(GV.SpellInfoDataType.Pos, si.spellPos));
        relativeFloatData.Add(GV.SpellInfoDataType.PosX, new SpellInfoRelativeData<float>(GV.SpellInfoDataType.PosX, si.spellPos.x));
        relativeFloatData.Add(GV.SpellInfoDataType.PosY, new SpellInfoRelativeData<float>(GV.SpellInfoDataType.PosY, si.spellPos.y));
    }

    public void UpdateState() //call on state change
    {
        foreach(KeyValuePair<GV.SpellInfoDataType, SpellInfoRelativeData<float>> kv in relativeFloatData)
        {
            kv.Value.StateUpdated(GetSIRelativeValue<float>(kv.Key, GV.RelativeType.Normal));
        }
        foreach (KeyValuePair<GV.SpellInfoDataType, SpellInfoRelativeData<bool>> kv in relativeBoolData)
        {
            kv.Value.StateUpdated(GetSIRelativeValue<bool>(kv.Key, GV.RelativeType.Normal));
        }
        foreach (KeyValuePair<GV.SpellInfoDataType, SpellInfoRelativeData<Vector2>> kv in relativeVec2Data)
        {
            kv.Value.StateUpdated(GetSIRelativeValue<Vector2>(kv.Key, GV.RelativeType.Normal));
        }
    }

    public T GetSIRelativeValue<T>(GV.SpellInfoDataType sidt, GV.RelativeType relType)
    {
        if (relType == GV.RelativeType.Normal)
        {
            switch (sidt)
            {
                case GV.SpellInfoDataType.GoNext:
                    return (T)System.Convert.ChangeType(true, typeof(T));
                case GV.SpellInfoDataType.BasicColiType:
                    return (T)System.Convert.ChangeType(si.lastBasicColiType, typeof(T));
                case GV.SpellInfoDataType.Density:
                    return (T)System.Convert.ChangeType(si.density, typeof(T));
                case GV.SpellInfoDataType.Angle:
                    return (T)System.Convert.ChangeType(si.currentAngle, typeof(T));
                case GV.SpellInfoDataType.Energy:
                    return (T)System.Convert.ChangeType(si.currentEnergy, typeof(T));
                case GV.SpellInfoDataType.Intelligence:
                    return (T)System.Convert.ChangeType(si.intelligence, typeof(T));
                case GV.SpellInfoDataType.Mass:
                    return (T)System.Convert.ChangeType(si.mass, typeof(T));
                case GV.SpellInfoDataType.Speed:
                    return (T)System.Convert.ChangeType(si.velocity, typeof(T));
                case GV.SpellInfoDataType.Stability:
                    return (T)System.Convert.ChangeType(si.stability, typeof(T));
                case GV.SpellInfoDataType.Time:
                    return (T)System.Convert.ChangeType(si.timeAlive, typeof(T));
                case GV.SpellInfoDataType.Velocity:
                    return (T)System.Convert.ChangeType(si.velocity, typeof(T));
                    //return (T)System.Convert.ChangeType(si.curHeadingDir * si.velocity, typeof(T));
                case GV.SpellInfoDataType.Wisdom:
                    return (T)System.Convert.ChangeType(si.wisdom, typeof(T));
                case GV.SpellInfoDataType.Altitude:
                    return (T)System.Convert.ChangeType(si.altitude, typeof(T));
                case GV.SpellInfoDataType.Variable:
                    return (T)System.Convert.ChangeType(si.smVariable, typeof(T));
                case GV.SpellInfoDataType.Pos:
                    return (T)System.Convert.ChangeType(si.spellPos, typeof(T));
                case GV.SpellInfoDataType.Radio:
                    return (T)System.Convert.ChangeType(si.lastRadioFreqRecieved, typeof(T));
                case GV.SpellInfoDataType.PosX:
                    return (T)System.Convert.ChangeType(si.spellPos.x, typeof(T));
                case GV.SpellInfoDataType.PosY:
                    return (T)System.Convert.ChangeType(si.spellPos.y, typeof(T));
                default:
                    Debug.LogError("Serousily fucked up, passed type " + relType.ToString() + " with type of " + sidt.GetType().Name);
                    return (T)System.Convert.ChangeType(relType, typeof(T));
            }
        }
        else
        {
            GV.VarType varType = TransSlotStructDict.Instance.SIDTtoVarType(sidt);
            
            switch (varType)
            {
                case GV.VarType.Bool:
                    return (T)System.Convert.ChangeType(relativeBoolData[sidt].GetRelData(relType),typeof(T));
                case GV.VarType.Float:
                    return (T)System.Convert.ChangeType(GetRelativeFloatData(relType, sidt), typeof(T));
                case GV.VarType.Vector2:
                    return (T)System.Convert.ChangeType(relativeVec2Data[sidt].GetRelData(relType), typeof(T));
                default:
                    Debug.LogError("attempting to find relative type of: " + sidt.ToString() + ", rel type: " + relType.ToString());
                    return (T)System.Convert.ChangeType(0, typeof(T));
            }
        }
    }

    public float GetRelativeFloatData(GV.RelativeType relType, GV.SpellInfoDataType sidt)
    {
        float v1 = relativeFloatData[sidt].GetRelData(relType);
        float curValue = GetSIRelativeValue<float>(sidt, GV.RelativeType.Normal);
        switch(relType)
        {
            case GV.RelativeType.Normal:
                Debug.LogError("code should not reach here");  //This just means your being inefficent, grabbing the normal value (a normal spellInfo value) but casting its relative data when its norm
                return curValue;
            case GV.RelativeType.SpellLaunched:
            case GV.RelativeType.StateStart:
                return curValue - v1;
            case GV.RelativeType.World:
                Debug.Log("should not use world value, use normal instead");
                return curValue;
            default:
                Debug.LogError("Relativefloatdata retrieve failfure, type: " + relType.ToString() + ", for sidt: " + sidt.ToString());
                return 0;
        }
    }

    /*
    public float GetRelativeDictData<T,U>(GV.RelativeType relType, GV.SpellInfoDataType sidt, string var)
    {
        //GV.SpellInfoDataType.Variable
        float v1 = 0;
        float curValue = GetSpellInfoValue<float>(sidt, GV.RelativeType.normal);
        SpellInfoRelativeData<Dictionary<string,float>> relativeVar = relativeDictionarySFData[sidt];
        switch (relType)
        {
            case GV.RelativeType.normal:
                Dictionary<string, float> dictPtr = relativeVar.aliveValue;
                v1 = dictPtr[var];
                return curValue;
            case GV.RelativeType.spellStart:
            case GV.RelativeType.stateStart:
                v1 = relativeDictionarySFData[var].aliveValue;
                return curValue - v1;
            case GV.RelativeType.world:
                return curValue;
            default:
                Debug.LogError("Relativefloatdata retrieve failfure, type: " + relType.ToString() + ", for sidt: " + sidt.ToString());
                return 0;
        }
    }*/
}

