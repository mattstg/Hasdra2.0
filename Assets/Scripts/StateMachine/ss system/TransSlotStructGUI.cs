using UnityEngine;
using UnityEngine.UI;
using System.Collections;
//GetSIDTRestrictions
public class TransSlotStructGUI : MonoBehaviour {
    TransitionGUI transGUIParent;
    
    public TransSlotStruct slotstruct;
    public Dropdown SIDTChoice;
    public Dropdown OPChoice;
    public Dropdown RelChoice;
    
    //3 mutually exclusive ones
    public Dropdown EnumInput;
    public InputField FeildCInput;
    public Toggle ToggleInput;

    public void InitializeNew(TransSlotStruct ss,TransitionGUI tg)
    {
        slotstruct = ss;
        transGUIParent = tg;
        FillDropdowns(GV.SpellInfoDataType.GoNext);
    }

    public void InitializeExisting(TransSlotStruct ss, TransitionGUI tg)
    {
        slotstruct = ss;
        transGUIParent = tg;
        FillSlotStructGUI();
    }

    public void DeleteSlot()
    {
        transGUIParent.DeleteSlot(this);
    }

    public void FillSlotStructGUI()
    {
        FillDropdowns(slotstruct.SIDT);
        OPChoice.value = (int)slotstruct.opValue;
        RelChoice.value = (int)slotstruct.relValue;
        SIDTChoice.value = (int)slotstruct.SIDT;
        ToggleInput.isOn = slotstruct.bValue;
        FeildCInput.text = slotstruct.fValue.ToString();
        FeildCInput.text = slotstruct.strValue;
        
        SIDTChanged();

        //since enum values need to be set after enum bars are filled
        if (slotstruct.SIDT == GV.SpellInfoDataType.BasicColiType)
        {
            EnumInput.value = (int)slotstruct.basicColiType;
            EnumInput.RefreshShownValue();
        }
       // else if (slotstruct.SIDT == GV.SpellInfoDataType.SpellColiType)
       // {
       //     EnumInput.value = (int)slotstruct.spellColiType;
       //     EnumInput.RefreshShownValue();
       // }
    }

    private void LoadInputBox()
    {
        GV.VarType vartype = TransSlotStructDict.Instance.SIDTtoVarType(slotstruct.SIDT);
        ToggleInput.gameObject.SetActive(false);
        EnumInput.gameObject.SetActive(false);
        FeildCInput.gameObject.SetActive(false);

        switch (vartype)
        {
            case GV.VarType.Bool:
                ToggleInput.gameObject.SetActive(true);
                break;
            case GV.VarType.Float:
                FeildCInput.gameObject.SetActive(true);
                FeildCInput.contentType = InputField.ContentType.DecimalNumber;
                break;
            //case GV.VarType.SpellType:
            //    EnumInput.gameObject.SetActive(true);
            //    FillSpellColiTypeEnumDropdown();
            //    break;
            case GV.VarType.BasicColiType:
                EnumInput.gameObject.SetActive(true);
                FillBasicColiTypeEnumDropdown();
                break;
            case GV.VarType.String:
                FeildCInput.gameObject.SetActive(true);
                FeildCInput.contentType = InputField.ContentType.Standard;
                break;
            case GV.VarType.Vector2:
                Debug.Log("Squirell-man-jee unhandled (v2)");
                break;
            default:
                Debug.LogError(string.Format("Unhandled type {1}",vartype));
                break;
        }
    }

    public TransSlotStruct GetSlotStruct()
    {
        return slotstruct;
    }

    public void SaveAllValues()
    {
        slotstruct.opValue = (GV.OperatorType)OPChoice.value;
        slotstruct.relValue = (GV.RelativeType)RelChoice.value;
        slotstruct.SIDT = (GV.SpellInfoDataType)SIDTChoice.value;
        slotstruct.bValue = ToggleInput.isOn;
        slotstruct.fValue = (FeildCInput.text == "")?0:float.Parse(FeildCInput.text);
        slotstruct.strValue = FeildCInput.text;
        if ((GV.SpellInfoDataType)SIDTChoice.value == GV.SpellInfoDataType.BasicColiType)
            slotstruct.basicColiType = (GV.BasicColiType)EnumInput.value;
        //if ((GV.SpellInfoDataType)SIDTChoice.value == GV.SpellInfoDataType.SpellColiType)
        //    slotstruct.spellColiType = (GV.MaterialType)EnumInput.value;

    }

    public void SIDTChanged()
    {
        RelChoice.gameObject.SetActive(true);
        OPChoice.gameObject.SetActive(true);
        slotstruct.SIDT = (GV.SpellInfoDataType)SIDTChoice.value;
        LoadInputBox();
        FillRelativeAndOPDropdown(TransSlotStructDict.Instance.GetSidtOPRestrictions((GV.SpellInfoDataType)SIDTChoice.value));
        //which one of the exclusives turn on
    }

    private void FillBasicColiTypeEnumDropdown()
    {
        EnumInput.ClearOptions();
        foreach (GV.BasicColiType SIDT in System.Enum.GetValues(typeof(GV.BasicColiType)))
        {
            EnumInput.options.Add(new Dropdown.OptionData(SIDT.ToString()));
        }
    }

    //private void FillSpellColiTypeEnumDropdown()
    //{
    //    EnumInput.ClearOptions();
    //    foreach (GV.MaterialType SIDT in System.Enum.GetValues(typeof(GV.MaterialType)))
    //    {
    //        EnumInput.options.Add(new Dropdown.OptionData(SIDT.ToString()));
    //    }
    //}

    private void FillDropdowns(GV.SpellInfoDataType loadDropdownsForType)
    {
        foreach (GV.SpellInfoDataType SIDT in System.Enum.GetValues(typeof(GV.SpellInfoDataType)))
        {
            SIDTChoice.options.Add(new Dropdown.OptionData(SIDT.ToString()));  //SIDTChoice.options.Add(new Dropdown.OptionData() { text = SIDT.ToString()});  whoa do you see what that hack implies???!?
        }
        SIDTChoice.value = (int)loadDropdownsForType;
        SIDTChoice.RefreshShownValue();
        FillRelativeAndOPDropdown(TransSlotStructDict.Instance.GetSidtOPRestrictions(loadDropdownsForType));
    }

    private void FillRelativeAndOPDropdown(GV.SlotStructRestrictions restriction)
    {
        RelChoice.ClearOptions();
        OPChoice.ClearOptions();
        switch (restriction)
        {
            case GV.SlotStructRestrictions.none:
                foreach (GV.RelativeType RelType in System.Enum.GetValues(typeof(GV.RelativeType)))
                {
                    RelChoice.options.Add(new Dropdown.OptionData(RelType.ToString()));  //SIDTChoice.options.Add(new Dropdown.OptionData() { text = SIDT.ToString()});  whoa do you see what that hack implies???!?
                }
                OPChoice.options.Add(new Dropdown.OptionData("=="));
                OPChoice.options.Add(new Dropdown.OptionData("<"));
                OPChoice.options.Add(new Dropdown.OptionData(">"));
                break;
            case GV.SlotStructRestrictions.noRelative:
                RelChoice.gameObject.SetActive(false);
                OPChoice.options.Add(new Dropdown.OptionData("=="));
                OPChoice.options.Add(new Dropdown.OptionData("<"));
                OPChoice.options.Add(new Dropdown.OptionData(">"));
                break;
            case GV.SlotStructRestrictions.onlyEquals:
                foreach (GV.RelativeType RelType in System.Enum.GetValues(typeof(GV.RelativeType)))
                {
                    RelChoice.options.Add(new Dropdown.OptionData(RelType.ToString()));  //SIDTChoice.options.Add(new Dropdown.OptionData() { text = SIDT.ToString()});  whoa do you see what that hack implies???!?
                }
                OPChoice.options.Add(new Dropdown.OptionData("=="));
                break;
            case GV.SlotStructRestrictions.all:
                RelChoice.gameObject.SetActive(false);
                OPChoice.options.Add(new Dropdown.OptionData("=="));
                break;
        }
        OPChoice.RefreshShownValue();
        RelChoice.RefreshShownValue();

    }
	
}

