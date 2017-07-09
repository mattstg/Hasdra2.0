using UnityEngine;
using System.Collections;

public class TransSlotStruct {
    //remove all publics if find out how to make SlotStructGUI a freind class
    public int parentTransID;
    public bool bValue             = false;
    public float fValue            = 0;
    public string strValue         = "";
    public GV.SlotDataType slotType   = GV.SlotDataType.boolType;
    public GV.OperatorType opValue = GV.OperatorType.equalTo;
    public GV.SpellInfoDataType SIDT = GV.SpellInfoDataType.GoNext;
    public GV.RelativeType relValue = GV.RelativeType.Normal;
    public GV.BasicColiType basicColiType = GV.BasicColiType.None;
    public GV.MaterialType spellColiType = GV.MaterialType.None;

    /*
    T GetData<T>(GV.SlotDataType slotType)
    {
        switch (slotType)
        {
            case GV.SlotDataType.boolType:
                return (T)System.Convert.ChangeType(bValue, typeof(T));
            case GV.SlotDataType.floatType:
                return (T)System.Convert.ChangeType(fValue, typeof(T));
            case GV.SlotDataType.stringType:
                return (T)System.Convert.ChangeType(strValue, typeof(T));
            case GV.SlotDataType.operatorType:
                return (T)System.Convert.ChangeType(opValue, typeof(T));
            default:
                Debug.LogError("you done serouis fucked up");
                return (T)System.Convert.ChangeType(bValue, typeof(T));
        }
    }

    void FillData<T>(T value, GV.SlotDataType slotType)
    {
        switch (slotType)
        {
            case GV.SlotDataType.boolType:
                bValue = (bool)System.Convert.ChangeType(value, typeof(bool));
                break;
            case GV.SlotDataType.floatType:
                fValue = (float)System.Convert.ChangeType(value, typeof(float));
                break;
            case GV.SlotDataType.stringType:
                strValue = (string)System.Convert.ChangeType(value, typeof(string));
                break;
            case GV.SlotDataType.operatorType:
                opValue = (GV.OperatorType)System.Convert.ChangeType(value, typeof(GV.OperatorType));
                break;
            default:
                Debug.LogError("you done serouis fucked up 2");
                bValue = (bool)System.Convert.ChangeType(value, typeof(bool));
                break;
        }
    }*/

    public bool CheckValid(SpellInfo si)
    {
        //First get the correct value
        GV.VarType vartype = TransSlotStructDict.Instance.SIDTtoVarType(SIDT);
        switch (vartype)
        {
            case GV.VarType.Bool:
                return CheckValid<bool>(si.relData.GetSIRelativeValue<bool>(SIDT,relValue), bValue);
            case GV.VarType.Float:
                return CheckValid<float>(si.relData.GetSIRelativeValue<float>(SIDT, relValue), fValue);
            case GV.VarType.SpellType:
                return CheckValid<GV.MaterialType>(si.relData.GetSIRelativeValue<GV.MaterialType>(SIDT, relValue), spellColiType);
            case GV.VarType.BasicColiType:
                return CheckValid<GV.BasicColiType>(si.relData.GetSIRelativeValue<GV.BasicColiType>(SIDT, relValue), basicColiType);
            case GV.VarType.String:
                return CheckValid<string>(si.relData.GetSIRelativeValue<string>(SIDT, relValue), strValue);
            default:
                Debug.LogError("invalid vartype : " + vartype.ToString());
                return false;
        }
    }

    private bool CheckValid<T>(T siValue, T compareValue) where T : System.IComparable
    {
        switch (opValue)
        {
            case GV.OperatorType.equalTo:
                return siValue.CompareTo(compareValue) == 0;
            case GV.OperatorType.greaterThan:
                return siValue.CompareTo(compareValue) >= 0;
            case GV.OperatorType.lessThan:
                return siValue.CompareTo(compareValue) <= 0;
        }
        return true;
    }

    public System.Collections.Generic.Dictionary<string, string> ExportForXML()
    {
        System.Collections.Generic.Dictionary<string, string> toRet = new System.Collections.Generic.Dictionary<string, string>();
        toRet.Add("SuperType", "TransSlotStruct");
        toRet.Add("parentID", parentTransID.ToString());
        toRet.Add("bValue", bValue.ToString());
        toRet.Add("fValue", fValue.ToString());
        toRet.Add("sValue", strValue.ToString());
        toRet.Add("slotValue", slotType.ToString());
        toRet.Add("OPValue", opValue.ToString());
        toRet.Add("relValue", relValue.ToString());
        toRet.Add("SIDT", SIDT.ToString());
        toRet.Add("basicColiValue", basicColiType.ToString());
        toRet.Add("spellColiValue", spellColiType.ToString());
        return toRet;
    }

    public void ImportFromXML(System.Collections.Generic.Dictionary<string, string> xmlImportDict)
    {
        System.Collections.Generic.Dictionary<string, string> toRet = new System.Collections.Generic.Dictionary<string, string>();
        parentTransID     = int.Parse(xmlImportDict["parentID"]); 
        bValue            = bool.Parse(xmlImportDict["bValue"]);
        fValue            = float.Parse(xmlImportDict["fValue"]);
        strValue          = xmlImportDict["sValue"];
        slotType          = (GV.SlotDataType)System.Enum.Parse(typeof(GV.SlotDataType), xmlImportDict["slotValue"]);
        opValue           = (GV.OperatorType)System.Enum.Parse(typeof(GV.OperatorType), xmlImportDict["OPValue"]);
        SIDT              = (GV.SpellInfoDataType)System.Enum.Parse(typeof(GV.SpellInfoDataType),xmlImportDict["SIDT"]);
        relValue          = (GV.RelativeType)System.Enum.Parse(typeof(GV.RelativeType), xmlImportDict["relValue"]);
        basicColiType     = (GV.BasicColiType)System.Enum.Parse(typeof(GV.BasicColiType), xmlImportDict["basicColiValue"]);
        spellColiType     = (GV.MaterialType)System.Enum.Parse(typeof(GV.MaterialType), xmlImportDict["spellColiValue"]);
    }
}
