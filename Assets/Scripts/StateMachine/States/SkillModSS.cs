using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class SkillModSS : StateSlot {

    string skillAffected;
    float percentCharge;
    float energyLimit;
    GV.SkillModScalingType skillModType;
    float controlValue;
    bool isDebuff;

    public GV.ConstantOrPercent kOrPerc; //specail case for public

    public override void Initialize(State _parentState)
    {
        stateDesc = "Begins charging a skill mod that is attached to the spell and applys on-hit. Will consume [percentCharge]% of incoming charging energy, to limit. if limit type is percent, it will charge to a percent of parent's energy";
        base.Initialize(_parentState);
        AddSSTuple("skillValue", "climbStrength", GV.StateVarType.SkillMod);
        AddSSTuple("percentCharge", ".1", GV.StateVarType.Float);
        AddSSTuple("energyLimitType", "constant", GV.StateVarType.constantOrPercent);
        AddSSTuple("energyLimit", "1", GV.StateVarType.Float);
        AddSSTuple("skillModType", "forceEff", GV.StateVarType.SkillModType);
        AddSSTuple("controlValue", "1", GV.StateVarType.Float);
        AddSSTuple("isDebuff", "false", GV.StateVarType.Bool);
       // AddSSTuple("isBuff","True",GV.StateVarType.Bool);

    }

    public override void VariableManualSave()
    {
        percentCharge = ssDict["percentCharge"].CastValue<float>();
        skillAffected = ssDict["skillValue"].svalue;
        energyLimit = ssDict["energyLimit"].CastValue<float>();
        skillModType = ssDict["skillModType"].CastValue<GV.SkillModScalingType>();
        controlValue = ssDict["controlValue"].CastValue<float>();
        kOrPerc = ssDict["energyLimitType"].CastValue<GV.ConstantOrPercent>();
        isDebuff = ssDict["isDebuff"].CastValue<bool>();
    }

    public override void PerformStateAction(Spell spell)
    {
        float energyToTransfer = 0;
        if(kOrPerc == GV.ConstantOrPercent.constant)
        {
            energyToTransfer = spell.spellInfo.energyTransferRate * Time.deltaTime;
        } 
        else if(kOrPerc == GV.ConstantOrPercent.percent)
        {
            energyToTransfer = spell.spellInfo.currentEnergy * energyLimit * Time.deltaTime;
        }
        energyToTransfer = (spell.spellInfo.currentEnergy < energyToTransfer) ? spell.spellInfo.currentEnergy : energyToTransfer;
        energyToTransfer = (energyToTransfer >= energyLimit) ? energyLimit : energyToTransfer ;  
        spell.ChargeSkillMod(skillAffected, energyLimit, skillModType, controlValue, energyToTransfer, this, isDebuff, percentCharge, kOrPerc);





    }
}

