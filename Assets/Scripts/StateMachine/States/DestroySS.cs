using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class DestroySS : StateSlot {

    public override void Initialize(State _parentState)
    {
        base.Initialize(_parentState);
        stateDesc = "Instantly destroys spell";
    }

    public override void PerformStateAction(Spell spell)
    {
        spell.Fizzle();
    }
}
