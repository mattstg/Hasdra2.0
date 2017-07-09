using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class EmptySS : StateSlot {

    public override void Initialize(State _parentState)
    {
        base.Initialize(_parentState);
    }

    public override void PerformStateAction(Spell spell)
    {
    }
}
