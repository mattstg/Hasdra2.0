using UnityEngine;
using System.Collections;

public class SpellInfoRelativeData<T>
{
    public T aliveValue;
    public T stateValue;
    public GV.SpellInfoDataType SIDT;

    public SpellInfoRelativeData(GV.SpellInfoDataType _SIDT, T initialValue)
    {
        aliveValue = initialValue;
        stateValue = initialValue;
        SIDT = _SIDT;
    }

    public void StateUpdated(T curValue)
    {
        stateValue = curValue;   
    }

    public T GetRelData(GV.RelativeType relType)
    {
        switch (relType)
        {
            case GV.RelativeType.Normal:
                Debug.LogError("trying to grab relative type normal from rel data, invalid and inefficent");
                return aliveValue;
            case GV.RelativeType.SpellLaunched:
                return aliveValue;
            case GV.RelativeType.StateStart:
                return stateValue;
            case GV.RelativeType.World:
                return aliveValue;
        }
        return aliveValue;
    }
}

