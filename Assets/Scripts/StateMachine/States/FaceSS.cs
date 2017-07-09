using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class RotateSS : StateSlot {

    GV.statechoice_face mode;
    float ang;
    GV.RelativeType relType;

    public override void Initialize(State _parentState)
    {
        base.Initialize(_parentState);
        AddSSTuple("ang", "0", GV.StateVarType.Float);
        AddSSTuple("mode", "faceAngle", GV.StateVarType.Rotate);
        AddSSTuple("relType", "Normal", GV.StateVarType.RelativeType);        
    }

    public override void VariableManualSave()
    {
        mode = ssDict["mode"].CastValue<GV.statechoice_face>();
        ang = ssDict["ang"].CastValue<float>();
        relType = ssDict["relType"].CastValue<GV.RelativeType>();
    }

    public override void PerformStateAction(Spell spell)
    {

        float relativeValue = (relType != GV.RelativeType.Normal) ? spell.spellInfo.relData.GetRelativeFloatData(relType, GV.SpellInfoDataType.Angle) : spell.spellInfo.currentAngle;

        if (mode == GV.statechoice_face.continousRotate)
        {
            float angToAdd = ang * Time.deltaTime;
            spell.transform.Rotate(new Vector3(0, 0, 1), angToAdd);
        }
        else if (mode == GV.statechoice_face.faceAngle)
        {
            Vector3 temp = spell.transform.rotation.eulerAngles;
            temp.z = ang;
            spell.transform.rotation = Quaternion.Euler(temp);
        }
        
        /*
        if (mode == GV.statechoice_face.continousRotate)
        {
            float angToAdd = GV.SPELL_ROTATION_RADS_PER_SEC * Time.deltaTime;
            spell.transform.Rotate(new Vector3(0, 0, 1), angToAdd);
        }
        else if (mode == GV.statechoice_face.faceAngle)
        {
            Vector3 aimPoint = new Vector3(0, Mathf.Sin(Mathf.Deg2Rad * ang), Mathf.Cos(Mathf.Deg2Rad * ang)) + spell.transform.position;
            Vector3.RotateTowards(spell.transform.forward, aimPoint, GV.SPELL_ROTATION_RADS_PER_SEC * Time.deltaTime, 0);
        }*/
    }
}
