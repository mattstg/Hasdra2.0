using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class StartStateSS : StateSlot {

    public override void Initialize(State _parentState)
    {
        base.Initialize(_parentState);
        AddSSTuple("Cast_type", "Normal", GV.StateVarType.castType);
        AddSSTuple("Melee_type", "basicPunch", GV.StateVarType.MeleeCastType, "Cast_typeMelee");
        AddSSTuple("Exclude_Interaction0", "None", GV.StateVarType.InteractionType);
        AddSSTuple("Exclude_Interaction1", "None", GV.StateVarType.InteractionType, new List<string>() { "Exclude_Interaction0Caster_Knockback", "Exclude_Interaction0Caster_Damage", "Exclude_Interaction0Caster_SkillMod", "Exclude_Interaction0Avatar_Collision" }, true);
        AddSSTuple("Exclude_Interaction2", "None", GV.StateVarType.InteractionType, new List<string>() { "Exclude_Interaction1Caster_Knockback", "Exclude_Interaction1Caster_Damage", "Exclude_Interaction1Caster_SkillMod", "Exclude_Interaction1Avatar_Collision" }, true);
        AddSSTuple("Exclude_Interaction3", "None", GV.StateVarType.InteractionType, new List<string>() { "Exclude_Interaction2Caster_Knockback", "Exclude_Interaction2Caster_Damage", "Exclude_Interaction2Caster_SkillMod", "Exclude_Interaction2Avatar_Collision" }, true);
        AddSSTuple("Exclude_Interaction4", "None", GV.StateVarType.InteractionType, new List<string>() { "Exclude_Interaction3Caster_Knockback", "Exclude_Interaction3Caster_Damage", "Exclude_Interaction3Caster_SkillMod", "Exclude_Interaction3Avatar_Collision" }, true);

        AddSSTuple("Max_range", "2", GV.StateVarType.Float,"Cast_typeMelee");
        AddSSTuple("Min_energy_to_achieve_max_range", "5", GV.StateVarType.Float, "Cast_typeMelee");
        AddSSTuple("SpellForm_Type", "Energy", GV.StateVarType.SpellForm);
        AddSSTuple("Cast_on_charge_param", "None", GV.StateVarType.CastOnCharge, new List<string> { "Energy_limit_typeConstant", "Energy_limit_typePercentOfCasterMax" }, true);
        AddSSTuple("Initial_velocity", "0", GV.StateVarType.Float, new List<string> {"Cast_typeNormal", "Cast_typeMelee"}, true);        
        AddSSTuple("Energy_limit_type", "None", GV.StateVarType.energyLimitType);
        AddSSTuple("Energy_limit", "0", GV.StateVarType.Float, new List<string>{"Energy_limit_typePercentOfCasterMax","Energy_limit_typeConstant"},true);
        AddSSTuple("Cast_dir_relative_to_cast", "Normal", GV.StateVarType.RelativeLaunchType);
        AddSSTuple("initialHeadingDirection","0",GV.StateVarType.Float);
        AddSSTuple("Spell_faces_launch_dir", "true", GV.StateVarType.Bool);        
        AddSSTuple("SetScaleX", "1", GV.StateVarType.Float);
        AddSSTuple("SetScaleY", "1", GV.StateVarType.Float);
        AddSSTuple("shape", "Circle", GV.StateVarType.Shape);
        AddSSTuple("StartAlpha", "1", GV.StateVarType.Float);
    }

    public override void PerformStateAction(Spell spell)
    {
		
    }

    public SpellInfo extractAsSpellInfo()
    {
        SpellInfo toRet = new SpellInfo();
        toRet.castType = ssDict["Cast_type"].CastValue<GV.CastType>();
        toRet.meleeCastType = ssDict["Melee_type"].CastValue<GV.MeleeCastType>();
        toRet.melee_maxRange = ssDict["Max_range"].CastValue<float>();
        toRet.melee_maxRange_energy = ssDict["Min_energy_to_achieve_max_range"].CastValue<float>();
        toRet.spellForm = ssDict["SpellForm_Type"].CastValue<GV.SpellForms>();
        toRet.castOnChargeParam = ssDict["Cast_on_charge_param"].CastValue<GV.CastOnCharge>();
        toRet.initialLaunchVelo = ssDict["Initial_velocity"].CastValue<float>();
        toRet.energyLimitType = ssDict["Energy_limit_type"].CastValue<GV.EnergyLimitType>();
        toRet.energyLimit = ssDict["Energy_limit"].CastValue<float>();
        toRet.initialLaunchAngleRelType = ssDict["Cast_dir_relative_to_cast"].CastValue<GV.RelativeLaunchType>();
        toRet.isFacingLaunchDir = ssDict["Spell_faces_launch_dir"].CastValue<bool>();
        float scaleX = (InDict("SetScaleX")) ? ssDict["SetScaleX"].CastValue<float>() : 1;
        float scaleY = (InDict("SetScaleY")) ? ssDict["SetScaleY"].CastValue<float>() : 1;
        toRet.setScale = toRet.initialSetScale = new Vector2(scaleX, scaleY);
        toRet.spellShape = ssDict["shape"].CastValue<GV.SpellShape>();
        toRet.alpha = ssDict["StartAlpha"].CastValue<float>();

        if (InDict("Exclude_Interaction0")) //cuz older versions 
        for (int i = 0; i < 5; i++)
            if (ssDict["Exclude_Interaction" + i].CastValue<GV.InteractionType>() != GV.InteractionType.None)
                toRet.interactionParams.Add(ssDict["Exclude_Interaction" + i].CastValue<GV.InteractionType>());
        return toRet;
    }

    private bool InDict(string _name)
    { //there is a chance of older start state versions
        return ssDict.ContainsKey(_name);
    }

    //Used by basic spell creator
    public StartStateSS(Dictionary<string,SSTuple> _ssDict, State _parentState) 
    {
        ssDict = _ssDict;
        parentState = _parentState;
    }

    public StartStateSS()
    {
    }
}
