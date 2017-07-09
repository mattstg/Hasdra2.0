using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class SetAlphaSS : StateSlot {

    float setAlpha;

    public override void Initialize(State _parentState)
    {
        base.Initialize(_parentState);
        AddSSTuple("alpha", "1", GV.StateVarType.Float);      
    }

    public override void VariableManualSave()
    {
        setAlpha = ssDict["alpha"].CastValue<float>();
    }

    public override void PerformStateAction(Spell spell)
    {
        spell.spellInfo.alpha = setAlpha;
    }
}
