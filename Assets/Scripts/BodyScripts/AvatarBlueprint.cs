using UnityEngine;
using System.Collections;

public class AvatarBlueprint  {

    public string characterSelected;
    public string skin;
    public int startingLevel;
    public int pid;
    public string startingPosName;

    public AvatarBlueprint(int _pid, string _charSelected, string _skin, int _startingLevel, string _startPosName)
    {
        pid = _pid;
        characterSelected = _charSelected;
        skin = _skin;
        startingLevel = _startingLevel;
        startingPosName = _startPosName;
    }

}
