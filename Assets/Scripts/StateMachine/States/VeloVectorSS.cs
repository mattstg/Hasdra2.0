using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class VeloVectorSS : StateSlot {

    public Vector2 xy;
    public float speed;
    public GV.IgnoreXY xyIgnore = GV.IgnoreXY.None;
    public GV.RelativeType relType = GV.RelativeType.Normal;

    public override void Initialize(State _parentState)
    {
        base.Initialize(_parentState);
        stateDesc = "Move xy relative to an angle. Norm(current angle), world(xy angle) ... speed 0 does no action, this ss is fincky ";
        AddSSTuple("veloX", "0", GV.StateVarType.Float);
        AddSSTuple("veloY", "0", GV.StateVarType.Float);
        AddSSTuple("speed", "1", GV.StateVarType.Float);
        AddSSTuple("xyIgnore", "None", GV.StateVarType.IgnoreXY);
        AddSSTuple("relType", "Normal", GV.StateVarType.RelativeType);
    }

    public override void VariableManualSave()
    {
        xy = new Vector2(ssDict["veloX"].CastValue<float>(),ssDict["veloY"].CastValue<float>());
        speed = ssDict["speed"].CastValue<float>();
        xyIgnore = ssDict["xyIgnore"].CastValue<GV.IgnoreXY>();
        relType = ssDict["relType"].CastValue<GV.RelativeType>();
    }

    public override void PerformStateAction(Spell spell)
    {
        float goalAngle = 0;
        float ang = GV.Vector2ToAngle(xy);
        //float ang = Vector2.Angle(new Vector2(1, 0), xy);
		float curSpellAng = spell.facingAng;

        switch (relType)
        {
            case GV.RelativeType.Normal:
                goalAngle = ang + curSpellAng;
                break;
            case GV.RelativeType.World:
                goalAngle = ang;
                break;
            case GV.RelativeType.SpellLaunched:
                goalAngle = ang + spell.spellInfo.relData.GetRelativeFloatData(GV.RelativeType.SpellLaunched, GV.SpellInfoDataType.Angle);
                break;
            case GV.RelativeType.StateStart:
                goalAngle = ang + spell.spellInfo.relData.GetRelativeFloatData(GV.RelativeType.StateStart, GV.SpellInfoDataType.Angle);
                break;
            default:
                Debug.LogError("weird default");
                break;
        }
        Vector2 goalPos = GV.DegreeToVector2(goalAngle);
        //spell.ApplyVeloState(spell.transform.position, goalPos, speed, xyIgnore);
        spell.ApplyVeloState(goalAngle, speed, xyIgnore);
    }
}

