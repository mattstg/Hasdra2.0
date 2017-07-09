using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class VariableSS : StateSlot {

    float lastTimeCompared = 0;

    float varCompare;
    GV.statechoice_modVar opType;
    GV.statechoice_modVarTime mode;

    public override void Initialize(State _parentState)
    {
        base.Initialize(_parentState);
        AddSSTuple("varValue", "0", GV.StateVarType.Float);
        AddSSTuple("opType", "mod", GV.StateVarType.ModOPType);
        AddSSTuple("mode", "perSecond", GV.StateVarType.ModVarTime);
    }

    public override void VariableManualSave()
    {
        varCompare = ssDict["varValue"].CastValue<float>();
        opType = ssDict["opType"].CastValue<GV.statechoice_modVar>();
        mode = ssDict["mode"].CastValue<GV.statechoice_modVarTime>();
    }

    public override void PerformStateAction(Spell spell)
    {
        if (opType == GV.statechoice_modVar.mod)
        {
            if (mode == GV.statechoice_modVarTime.perCycle)
            {
                spell.spellInfo.smVariable += varCompare;
            }
            else if (mode == GV.statechoice_modVarTime.perSecond)
            {
                spell.spellInfo.smVariable += varCompare*Time.deltaTime;
            }
        }
        else if (opType == GV.statechoice_modVar.set)
        {
            spell.spellInfo.smVariable = varCompare;
        }
    }
}
