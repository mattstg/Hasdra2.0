using UnityEngine;
using System.Collections;

public class LevelPackage  {

    public GV.LevelPkgType levelPkgType; //ability || spell
    public string pkgName;
    public string activationKey;
    public float expCostToLevel;

    public LevelPackage(GV.LevelPkgType _lvlPkgType, string _pkgName, string _activationKey, float _expCostToLevel)
    {
        levelPkgType = _lvlPkgType;
        pkgName = _pkgName;
        activationKey = _activationKey;
        expCostToLevel = _expCostToLevel;
    }
}
