using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class RadioSS : StateSlot {

    float radioFreq;
    GV.RadioStateType radioState;
    GV.ConstantOrPercent energyUseLimitType;
    float radioStrength;
    bool emitOnce;

    public override void Initialize(State _parentState)
    {
        base.Initialize(_parentState);
        stateDesc = "When the spell explodes, anything that hits it will recieve this radio signal freq, which can be used in transition triggers, never use (-1) since it is a default value";
        AddSSTuple("radioFreq", "0", GV.StateVarType.Float); //RadioStateType
        AddSSTuple("radioStateType", "OnExplosion", GV.StateVarType.RadioOption);
        AddSSTuple("energyUseLimit", "percent", GV.StateVarType.constantOrPercent, "radioStateTypeEmit");
        AddSSTuple("radioPulseStrength", "1", GV.StateVarType.Float, "radioStateTypeEmit");
        AddSSTuple("emitOnce", "true", GV.StateVarType.Bool, "radioStateTypeEmit");

    }

    public override void VariableManualSave()
    {
        radioFreq = ssDict["radioFreq"].CastValue<float>();
        radioState = ssDict["radioStateType"].CastValue<GV.RadioStateType>();
        energyUseLimitType = ssDict["energyUseLimit"].CastValue<GV.ConstantOrPercent>();
        radioStrength = ssDict["radioPulseStrength"].CastValue<float>();
        emitOnce = ssDict["emitOnce"].CastValue<bool>();
    }

    public override void PerformStateAction(Spell spell)
    {
        if (radioState == GV.RadioStateType.OnExplosion)
        {
            spell.spellInfo.radioFreq = radioFreq;
        }
        else
        {
            spell.EmitRadioSignal(radioStrength,radioFreq,emitOnce,energyUseLimitType);
        }
        
    }
}
