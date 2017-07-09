using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class VeloSS : StateSlot {

    public float ang = 0;
    public float speed = 0;
    public GV.RelativeType relType = GV.RelativeType.Normal;

    public override void Initialize(State _parentState)
    {
        base.Initialize(_parentState);
        AddSSTuple("ang", "0", GV.StateVarType.Float);
        AddSSTuple("speed", "0", GV.StateVarType.Float);
        AddSSTuple("relType", "Normal", GV.StateVarType.RelativeType);
    }

    public override void VariableManualSave()
    {
        ang = ssDict["ang"].CastValue<float>();
        speed = ssDict["speed"].CastValue<float>();
        relType = ssDict["relType"].CastValue<GV.RelativeType>();
    }

    public override void PerformStateAction(Spell spell)
    {
        float goalAngle = ang;
  
        switch (relType)
        {
            case GV.RelativeType.Normal:
            case GV.RelativeType.World:
                break;
            case GV.RelativeType.SpellLaunched:
                goalAngle += spell.spellInfo.relData.GetRelativeFloatData(GV.RelativeType.SpellLaunched, GV.SpellInfoDataType.Angle);
                break;
            case GV.RelativeType.StateStart:
                goalAngle += spell.spellInfo.relData.GetRelativeFloatData(GV.RelativeType.StateStart, GV.SpellInfoDataType.Angle);
                break;
            default:
                Debug.LogError("weird default");
                break;
        }
        Vector2 goalPos = GV.DegreeToVector2(goalAngle) * speed;
        spell.ApplyVeloState(spell.transform.position, goalPos, speed, GV.IgnoreXY.None);
        //spell.ApplyVeloState(ang, speed,GV.IgnoreXY.None);
    }
}
