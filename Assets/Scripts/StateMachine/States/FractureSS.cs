using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class FractureSS : StateSlot {

    public override void Initialize(State _parentState)
    {
        stateDesc = "Physical spell fractures";
        AddSSTuple("explosive", "false", GV.StateVarType.Bool);
        base.Initialize(_parentState);
    }

    public override void PerformStateAction(Spell spell)
    {
        spell.Fracture(ssDict["explosive"].CastValue<bool>());
    }
}
