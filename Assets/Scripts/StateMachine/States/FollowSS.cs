using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class FollowSS : StateSlot {

    Vector3 xy = new Vector2();
    float speedLimit = 0;
    Transform parentToFollow;
    GV.IgnoreXY ignorexy = GV.IgnoreXY.None;
    int nCastersBack;
    int cleanupMax = 1000; //every N iterations cleans it up
    int cleanupCounter = 1000;


    public override void Initialize(State _parentState)
    {
        base.Initialize(_parentState);
        stateDesc = "Follow up to N parents back capping at last available parent, <= 0 is self, if parent dies, resets to self";
        AddSSTuple("NumOfParentsBack", "0", GV.StateVarType.Float);
        AddSSTuple("PosX", "0", GV.StateVarType.Float);
        AddSSTuple("PosY", "0", GV.StateVarType.Float);
        AddSSTuple("Speed", "0", GV.StateVarType.Float);
        AddSSTuple("IgnoreXY", "None", GV.StateVarType.IgnoreXY);
    }

    public override void VariableManualSave()
    {
        xy = new Vector3(ssDict["PosX"].CastValue<float>(), ssDict["PosY"].CastValue<float>(),0);
        speedLimit = ssDict["Speed"].CastValue<float>();
        ignorexy = ssDict["IgnoreXY"].CastValue<GV.IgnoreXY>();
        nCastersBack = Mathf.Clamp((int)(ssDict["NumOfParentsBack"].CastValue<float>()),0,999);
    }

    public override void PerformStateAction(Spell spell)
    {
        if (spell.spellInfo.followTarget == null)
        {
            spell.spellInfo.followTarget = spell.spellInfo.GetNCastersBack(nCastersBack).transform;
        }

        //Debug.Log("following " + spell.spellInfo.followTarget.name + " at pos: " + spell.spellInfo.followTarget.position);
        Vector2 goalPos = spell.spellInfo.followTarget.position + xy;
        spell.ApplyVeloState(spell.transform.position, goalPos, speedLimit, ignorexy);
    }

    public override void ExitState(Spell spell)
    {
        spell.spellInfo.followTarget = null;
    }
}
