﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class StateSlotGUIMinor : MonoBehaviour {

    StateSlotGUI stateSlotGUIParent;
    StateSlot stateSlot;
    GV.StateVarType stateVar;
    public Text slotName;
    public InputField inputFeild;
    public Dropdown dropDown;
    public Toggle toggle;
    string oldValue;
    string _slotName;
    List<string> requirements;
    bool onlyOneRequirementRequired;

    public void Initialize(string _name, SSTuple ssTuple, StateSlotGUI ssGuiParent)
    {
        requirements = ssTuple.requirements;
        onlyOneRequirementRequired = ssTuple.onlyOneRequirementRequired;
        stateSlotGUIParent = ssGuiParent;
        _slotName = _name;
        string[] namesplit = _name.Split(':');
        slotName.text = namesplit[namesplit.Length - 1];
        stateVar = ssTuple.varType;
        switch (stateVar)
        {
            case GV.StateVarType.Bool:
                toggle.gameObject.SetActive(true);
                toggle.isOn = ssTuple.CastValue<bool>();
                break;
            case GV.StateVarType.String:
            case GV.StateVarType.Float:          
                inputFeild.gameObject.SetActive(true);
                inputFeild.text = ssTuple.value;
                break;
            case GV.StateVarType.ExistingSpells: 
            case GV.StateVarType.MatType:
            case GV.StateVarType.RelativeType:            
            case GV.StateVarType.BasicColiType:           
            case GV.StateVarType.Shape:
            case GV.StateVarType.Rotate:
            case GV.StateVarType.ModOPType:
            case GV.StateVarType.ModVarTime:
            case GV.StateVarType.RelativeLaunchType:
            case GV.StateVarType.IgnoreXY:
            case GV.StateVarType.SkillMod:
            case GV.StateVarType.SkillModType:
            case GV.StateVarType.constantOrPercent:
            case GV.StateVarType.damageDirectionType:
            case GV.StateVarType.castType:
            case GV.StateVarType.MeleeCastType:
            case GV.StateVarType.energyLimitType:
            case GV.StateVarType.ColiMetaTypes:
            case GV.StateVarType.RadioOption:
            case GV.StateVarType.InteractionType:
            case GV.StateVarType.CastOnCharge:
                dropDown.gameObject.SetActive(true);
                InitializeEnum(stateVar);
                dropDown.value = UILayer.GetIndexOfValue(dropDown, ssTuple.value);
                dropDown.RefreshShownValue();
                break;
            default:
                Debug.LogError("bad switch: " + stateVar);
                break;
        }
        oldValue = GetValue();
    }

    private void InitializeEnum(GV.StateVarType enumVar)
    {
        switch (enumVar)
        {
            case GV.StateVarType.BasicColiType:
                UILayer.FillDropdown<GV.BasicColiType>(dropDown);
                break;
            case GV.StateVarType.ExistingSpells:
                UILayer.FillDropdown(dropDown, LiveSpellDict.GetAllSpellNames());
                break;
            case GV.StateVarType.SkillMod:
                UILayer.FillDropdown(dropDown, BodyStatFiller.GetAllSkills());
                break;
            case GV.StateVarType.CastOnCharge:
                UILayer.FillDropdown<GV.CastOnCharge>(dropDown);
                break;
            case GV.StateVarType.InteractionType:
                UILayer.FillDropdown<GV.InteractionType>(dropDown);
                break;
            case GV.StateVarType.RadioOption:
                UILayer.FillDropdown<GV.RadioStateType>(dropDown);
                break;
            case GV.StateVarType.ColiMetaTypes:
                UILayer.FillDropdown<GV.ColiMetaType>(dropDown);
                break;
            case GV.StateVarType.energyLimitType:
                UILayer.FillDropdown<GV.EnergyLimitType>(dropDown);
                break;
            case GV.StateVarType.castType:
                UILayer.FillDropdown<GV.CastType>(dropDown);
                break;
            case GV.StateVarType.MeleeCastType:
                UILayer.FillDropdown<GV.MeleeCastType>(dropDown);
                break;
            case GV.StateVarType.damageDirectionType:
                UILayer.FillDropdown<GV.DirectionalDamage>(dropDown);
                break;
            case GV.StateVarType.constantOrPercent:
                UILayer.FillDropdown<GV.ConstantOrPercent>(dropDown);
                break;
            case GV.StateVarType.SkillModType:
                UILayer.FillDropdown<GV.SkillModScalingType>(dropDown);
                break;
            case GV.StateVarType.MatType:
                UILayer.FillDropdown<GV.MaterialType>(dropDown);
                break;
            case GV.StateVarType.ModOPType:
                UILayer.FillDropdown<GV.statechoice_modVar>(dropDown);
                break;
            case GV.StateVarType.ModVarTime:
                UILayer.FillDropdown<GV.statechoice_modVarTime>(dropDown);
                break;
            case GV.StateVarType.RelativeType:
                UILayer.FillDropdown<GV.RelativeType>(dropDown);
                break;
            case GV.StateVarType.Rotate:
                UILayer.FillDropdown<GV.statechoice_face>(dropDown);
                break;
            case GV.StateVarType.Shape:
                UILayer.FillDropdown<GV.SpellShape>(dropDown);
                break;
            case GV.StateVarType.RelativeLaunchType:
                UILayer.FillDropdown<GV.RelativeLaunchType>(dropDown);
                break;
            case GV.StateVarType.IgnoreXY:
                UILayer.FillDropdown<GV.IgnoreXY>(dropDown);
                break;
            default:
                Debug.Log("bad gui load" + enumVar.ToString());
                break;
        }
    }

    public string GetValue()
    {
        switch (stateVar)
        {
            case GV.StateVarType.Bool:
                return toggle.isOn.ToString();

            case GV.StateVarType.String:
            case GV.StateVarType.Float:
                return inputFeild.text;

            case GV.StateVarType.ExistingSpells:
            case GV.StateVarType.MatType:
            case GV.StateVarType.RelativeType:
            case GV.StateVarType.BasicColiType:
            case GV.StateVarType.Shape:
            case GV.StateVarType.Rotate:
            case GV.StateVarType.ModVarTime:
            case GV.StateVarType.ModOPType:
            case GV.StateVarType.RelativeLaunchType:
            case GV.StateVarType.IgnoreXY:
            case GV.StateVarType.SkillMod:
            case GV.StateVarType.SkillModType:
            case GV.StateVarType.constantOrPercent:
            case GV.StateVarType.damageDirectionType:
            case GV.StateVarType.MeleeCastType:
            case GV.StateVarType.castType:
            case GV.StateVarType.energyLimitType:
            case GV.StateVarType.ColiMetaTypes:
            case GV.StateVarType.RadioOption:
            case GV.StateVarType.InteractionType:
            case GV.StateVarType.CastOnCharge:
                return dropDown.options[dropDown.value].text;
            default:
                Debug.LogError("way to fuck up" + stateVar);
                return "";
        }
    }

    public void ValueChanged()
    {
            //if (_slotName == "Cast_type")
            //    Debug.Log("old v: " + _slotName + oldValue + " new name: " + _slotName + GetValue());
            stateSlotGUIParent.ValueChange(_slotName + oldValue, _slotName + GetValue());
            oldValue = GetValue();
    }

    public string SetupAndGetInitialValue()
    {
        oldValue = GetValue();
        return _slotName + oldValue;
    }

    public bool RequirementsMet(List<string> activeKeywords)
    {
        //bool debugOn = _slotName == "Initial_velocity";
        //if(debugOn) Debug.LogError("Checking requirements for: " + _slotName);
        //if (debugOn) Debug.Log("requirements: " + GV.DebugConcatToOneOutputString(requirements) + " , searching inside activekeywords: " + GV.DebugConcatToOneOutputString(activeKeywords) + " true: " + onlyOneRequirementRequired);
        if (requirements == null)
        {
            //if (debugOn) Debug.Log("no req, ret true");
            return true;
        }

        if (onlyOneRequirementRequired)
        {
            for (int i = 0; i < requirements.Count; i++)
                if (activeKeywords.Contains(requirements[i]))
                    return true;
            return false;

        }
        else
        {
            for (int i = 0; i < requirements.Count; i++)
                if (!activeKeywords.Contains(requirements[i]))
                    return false;
        }
        //if (debugOn) Debug.Log("ret true, all reqs met");
        return true;
    }
}

