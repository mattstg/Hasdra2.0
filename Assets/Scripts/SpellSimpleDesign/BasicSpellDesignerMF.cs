using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicSpellDesignerMF : MonoBehaviour {

    public SpellInfo si;
    public Transform siuiscParent;
    List<SIBasicVarSlotUI> sibvsus = new List<SIBasicVarSlotUI>();

    public void Awake()
    {
        si = new SpellInfo();
        foreach (Transform t in siuiscParent)
            sibvsus.Add(t.GetComponent<SIBasicVarSlotUI>());
    }

    public void SavePressed()
    {
        
        Dictionary<string, SSTuple> toSaveDict = new Dictionary<string, SSTuple>();
        foreach (SIBasicVarSlotUI bvs in sibvsus)
        {
            bvs.FillTuple();
            toSaveDict[bvs.varName] = bvs.GetTuple();
        }

        //Some hardcoded ones
        int range = (int)(toSaveDict["Max_Range"].GetFValue());
        switch(range)
        {
            case -2:
                toSaveDict["Cast_type"] = new SSTuple("Cast_type", GV.CastType.SelfCast.ToString(), GV.StateVarType.castType);
                break;
            case -1:
                toSaveDict["Cast_type"] = new SSTuple("Cast_type", GV.CastType.SelfCast.ToString(), GV.StateVarType.castType);
                break;
            default:
                toSaveDict["Cast_type"] = new SSTuple("Cast_type", GV.CastType.Normal.ToString(), GV.StateVarType.castType);
                break;
        }
        toSaveDict["Melee_type"] = new SSTuple("Melee_Type", GV.MeleeCastType.basicPunch.ToString(), GV.StateVarType.MeleeCastType);
        toSaveDict["Min_energy_to_achieve_max_range"] = new SSTuple("Min_energy_to_achieve_max_range", "0", GV.StateVarType.Float);


        State basicSpellState = new State();
        basicSpellState.StateID = 0;
        StartStateSS startState = new StartStateSS(toSaveDict, basicSpellState);
        basicSpellState.AddStateSlot(startState);
        //Right here, you can add the BodyStatMod states! :D

        XMLEncoder.DictionaryListToXML<string>(toSaveDict["name"].svalue, basicSpellState.ExportForXML(new Vector2()), GV.fileLocationType.BasicSpells);
        //ExportForXML
    }

    public void LoadPressed()
    {

    }

    /*
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



    */
}
