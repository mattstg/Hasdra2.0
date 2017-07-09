using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class PositionSS : StateSlot {

    Vector2 xy = new Vector2();
    float speedLimit = 0;
    GV.IgnoreXY ignorexy = GV.IgnoreXY.None;
    GV.RelativeType relType = GV.RelativeType.Normal;

    public override void Initialize(State _parentState)
    {
        base.Initialize(_parentState);
        stateDesc = "Move to position offset by PosXY, relative to Normal(currentSelf), StateState, world pos, Spell Launched";
        AddSSTuple("PosX", "0", GV.StateVarType.Float);
        AddSSTuple("PosY", "0", GV.StateVarType.Float);
        AddSSTuple("Speed", "0", GV.StateVarType.Float);
        AddSSTuple("IgnoreXY", "None", GV.StateVarType.IgnoreXY);
        AddSSTuple("RelativeType", "Normal", GV.StateVarType.RelativeType);

    }

    public override void VariableManualSave()
    {
        xy = new Vector2(ssDict["PosX"].CastValue<float>(), ssDict["PosY"].CastValue<float>());
        speedLimit = ssDict["Speed"].CastValue<float>();
        ignorexy = ssDict["IgnoreXY"].CastValue<GV.IgnoreXY>();
        relType = ssDict["RelativeType"].CastValue<GV.RelativeType>();
    }

    public override void PerformStateAction(Spell spell)
    {
        Vector2 goalVec = xy;
        switch (relType)
        {
            case GV.RelativeType.World:
                break;
            case GV.RelativeType.SpellLaunched:
                goalVec += spell.spellInfo.relData.GetSIRelativeValue<Vector2>(GV.SpellInfoDataType.Pos, GV.RelativeType.SpellLaunched);
                break;
            case GV.RelativeType.Normal:
                goalVec += spell.spellInfo.spellPos;
                break;
            case GV.RelativeType.StateStart:
                goalVec += spell.spellInfo.relData.GetSIRelativeValue<Vector2>(GV.SpellInfoDataType.Pos, GV.RelativeType.StateStart);
                break;
            default:
                Debug.LogError("weird default");
                break;
        }
        spell.ApplyVeloState(spell.transform.position, goalVec, speedLimit, ignorexy);
    }
}
