using UnityEngine;
using System.Collections;

public class StaticReferences : MonoBehaviour {

    //need two of each, one static and one non, since unity editor cannot be used to set static values
    public static GameObject mainScriptsGO;
    public NumericDisplayManager _numericTextManager;
    public static NumericDisplayManager numericTextManager;

    public void Start()
    {
        mainScriptsGO = this.gameObject;
        numericTextManager = _numericTextManager;
        _numericTextManager = null;
    }
}
