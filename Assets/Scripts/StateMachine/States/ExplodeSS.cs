using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class ExplodeSS : StateSlot {

    public override void Initialize(State _parentState)
    {
        stateDesc = "Spell go boom";
        AddSSTuple("damageDirectionType", "explosion", GV.StateVarType.damageDirectionType);
        AddSSTuple("use_current_directional", "false", GV.StateVarType.Bool);
        base.Initialize(_parentState);
    }

    public override void PerformStateAction(Spell spell)
    {
        if (!ssDict["use_current_directional"].CastValue<bool>())
        {
            spell.Explode(ssDict["damageDirectionType"].CastValue<GV.DirectionalDamage>());
        }
        else
        {
            spell.Explode();
        }
    }
}
