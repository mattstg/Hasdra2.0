using UnityEngine;
using System.Collections;

public class BasicBF : BalanceFormula {

    float a;
    float b;
    float min = -9999;
    float max =  9999;

    float lastLvl = 0; //never have something start at lvl 0 cuz this optimzation
    float lastRetValue;

    public BasicBF(BasicBF bf)
    {
        a = bf.a;
        b = bf.b;
        min = bf.min;
        max = bf.max;
    }

    public BasicBF(float _a, float _b)
    {
        a = _a;
        b = _b;
    }

    public BasicBF(float _a, float _b, float _min, float _max)
    {
        a = _a;
        b = _b;
        min = _min;
        max = _max;
    }

    public float ret(float lvl)
    {
        if (lastLvl == lvl)
            return lastRetValue;

        lastLvl = lvl;
        lastRetValue = Mathf.Min(Mathf.Max((a * lvl + b),min),max);
        return lastRetValue;
    }

    public BalanceFormula CopyThis()
    {
        return new BasicBF(this);
    }
}
