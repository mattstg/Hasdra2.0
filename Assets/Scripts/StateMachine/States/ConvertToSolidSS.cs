using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class ConvertToSolidSS : StateSlot {


    public override void Initialize(State _parentState)
    {
        base.Initialize(_parentState);
        stateDesc = "Converts the spell into a solid material. It no longer uses state machine, but loses no energy";
    }

    public override void VariableManualSave()
    {
    }

    public override void PerformStateAction(Spell spell)
    {
        spell.ConvertSpellIntoSolidMaterial(spell.spellInfo);
    }
}
