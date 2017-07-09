using UnityEngine;
using System.Collections;

public interface BalanceFormula   {

    float ret(float lvl);
    BalanceFormula CopyThis();
}
