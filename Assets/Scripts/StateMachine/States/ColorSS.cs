using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class ColorSS : StateSlot {

    Vector4 newColor;

    public override void Initialize(State _parentState)
    {
        base.Initialize(_parentState);
        stateDesc = "0->1, to convert from 255, value/255";
        AddSSTuple("r", "1", GV.StateVarType.Float);
        AddSSTuple("g", "1", GV.StateVarType.Float);
        AddSSTuple("b", "1", GV.StateVarType.Float);
    }

    public override void VariableManualSave()
    {
        newColor = new Vector4(ssDict["r"].CastValue<float>(), ssDict["g"].CastValue<float>(), ssDict["b"].CastValue<float>(), 1);
    }

    public override void PerformStateAction(Spell spell)
    {
        spell.spellInfo.spellColor = new Color(newColor.x, newColor.y, newColor.z, 1);
        spell.spellInfo.colorAltered = true;
    }
}
