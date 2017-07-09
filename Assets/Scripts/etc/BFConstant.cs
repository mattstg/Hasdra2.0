using UnityEngine;
using System.Collections;

public class BFConstant : BalanceFormula {

    float constantValue;

    public BFConstant(float _constantValue)
    {
        constantValue = _constantValue;
    }
    public float ret(float lvl)
    {
        return constantValue;
    }

    public BalanceFormula CopyThis()
    {
        return new BFConstant(constantValue);
    }
}
