using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class IgnoreColiSS : StateSlot {

    GV.ColiMetaType  coliType;
    GV.BasicColiType basicToSet;
    GV.MaterialType  matToSet;
    bool isIgnore;

    public override void Initialize(State _parentState)
    {
        base.Initialize(_parentState);
        string createStartValue = LiveSpellDict.GetAllSpellNames()[0];
        AddSSTuple("ColiType", "Basic", GV.StateVarType.ColiMetaTypes);
        AddSSTuple("Basic_coli_type", "Spell", GV.StateVarType.BasicColiType,"ColiTypeBasic");
        AddSSTuple("Material_coli_type", "Energy", GV.StateVarType.MatType,"ColiTypeMaterial");
        AddSSTuple("Ignore", "True", GV.StateVarType.Bool);
    }

    public override void VariableManualSave()
    {
        coliType = ssDict["ColiType"].CastValue<GV.ColiMetaType>();
        basicToSet = ssDict["Basic_coli_type"].CastValue<GV.BasicColiType>();
        matToSet = ssDict["Material_coli_type"].CastValue<GV.MaterialType>();
        isIgnore = ssDict["Ignore"].CastValue<bool>();
    }

    public override void PerformStateAction(Spell spell)
    {
        if (coliType == GV.ColiMetaType.Basic)
        {
            if (isIgnore && !spell.spellInfo.ignoreMetaColiType.Contains(basicToSet))
                spell.spellInfo.ignoreMetaColiType.Add(basicToSet);
            else if(!isIgnore && spell.spellInfo.ignoreMetaColiType.Contains(basicToSet))
                spell.spellInfo.ignoreMetaColiType.Remove(basicToSet);
        }
        else if (coliType == GV.ColiMetaType.Material)
        {
            if (isIgnore && !spell.spellInfo.ignoreMaterialColiType.Contains(matToSet))
                spell.spellInfo.ignoreMaterialColiType.Add(matToSet);
            else if (!isIgnore && spell.spellInfo.ignoreMaterialColiType.Contains(matToSet))
                spell.spellInfo.ignoreMaterialColiType.Remove(matToSet);
        }
    }
}
