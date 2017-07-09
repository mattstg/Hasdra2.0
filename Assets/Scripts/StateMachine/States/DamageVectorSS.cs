using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class DamageVectorSS : StateSlot {



    public override void Initialize(State _parentState)
    {
        stateName = "Explosion Param";
        stateDesc = "parameters for explosion, ideal values, requires enough energy to pull off";
        base.Initialize(_parentState);
        AddSSTuple("damageDirectionType", "explosion", GV.StateVarType.damageDirectionType);
        AddSSTuple("damageDirectionTrueAng", "0", GV.StateVarType.Float, "damageDirectionTypespecifiedDir");
        AddSSTuple("Use_Default_Force", "True", GV.StateVarType.Bool);
        AddSSTuple("Explosive_Force", "0", GV.StateVarType.Float,"Use_Default_ForceFalse");
        //specifiedDir
    }

    public override void VariableManualSave()
    {
     /*   mode = ssDict["mode"].CastValue<GV.statechoice_face>();
        ang = ssDict["ang"].CastValue<float>();
        relType = ssDict["relType"].CastValue<GV.RelativeType>();*/
    }

    public override void PerformStateAction(Spell spell)
    {
        spell.spellInfo.directionalDamageType = ssDict["damageDirectionType"].CastValue<GV.DirectionalDamage>();
        spell.spellInfo.dmgDirTrueAng = ssDict["damageDirectionTrueAng"].CastValue<float>();
        spell.spellInfo.useDefaultForce = ssDict["Use_Default_Force"].CastValue<bool>();
        spell.spellInfo.cappedExplosiveForce = ssDict["Explosive_Force"].CastValue<float>();
    }
}
