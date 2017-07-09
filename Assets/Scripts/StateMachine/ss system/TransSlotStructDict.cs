using UnityEngine;
using System.Collections;

public class TransSlotStructDict
{
    #region Singleton
    private static TransSlotStructDict instance;

    private TransSlotStructDict() { }

    public static TransSlotStructDict Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new TransSlotStructDict();
            }
            return instance;
        }
    }
    #endregion

    public GV.VarType SIDTtoVarType(GV.SpellInfoDataType SIDT)
    {
        switch (SIDT)
        {
            case GV.SpellInfoDataType.Density: 
            case GV.SpellInfoDataType.Angle:
            case GV.SpellInfoDataType.Energy:
            case GV.SpellInfoDataType.Intelligence:
            case GV.SpellInfoDataType.Mass:
            case GV.SpellInfoDataType.Speed:
            case GV.SpellInfoDataType.Stability:
            case GV.SpellInfoDataType.Time:
            case GV.SpellInfoDataType.Velocity:
            case GV.SpellInfoDataType.Wisdom:
            case GV.SpellInfoDataType.Altitude:
            case GV.SpellInfoDataType.Variable:
            case GV.SpellInfoDataType.Radio:
            case GV.SpellInfoDataType.PosX:
            case GV.SpellInfoDataType.PosY:
                return GV.VarType.Float;

            case GV.SpellInfoDataType.BasicColiType:
                return GV.VarType.BasicColiType;

            case GV.SpellInfoDataType.SpellColiType:
                return GV.VarType.SpellType;

            case GV.SpellInfoDataType.GoNext:
                return GV.VarType.Bool;

            case GV.SpellInfoDataType.Pos:
                return GV.VarType.Vector2;

            default:
                Debug.LogError("SIDT not accounted for: " + SIDT.ToString());
                return GV.VarType.Float;
        }
    }

    public GV.SlotStructRestrictions GetSidtOPRestrictions(GV.SpellInfoDataType SIDT)
    {
        switch (SIDT)
        {
            case GV.SpellInfoDataType.Density:
            case GV.SpellInfoDataType.Angle:
            case GV.SpellInfoDataType.Energy:
            case GV.SpellInfoDataType.Intelligence:
            case GV.SpellInfoDataType.Mass:
            case GV.SpellInfoDataType.Speed:
            case GV.SpellInfoDataType.Stability:
            case GV.SpellInfoDataType.Time:
            case GV.SpellInfoDataType.Velocity:
            case GV.SpellInfoDataType.Wisdom:
            case GV.SpellInfoDataType.Altitude:
            case GV.SpellInfoDataType.Variable:
            case GV.SpellInfoDataType.Pos:
            case GV.SpellInfoDataType.PosX:
            case GV.SpellInfoDataType.PosY:
                return GV.SlotStructRestrictions.none;

            case GV.SpellInfoDataType.BasicColiType:
            case GV.SpellInfoDataType.SpellColiType:
            case GV.SpellInfoDataType.GoNext:
            
                return GV.SlotStructRestrictions.all;

            case GV.SpellInfoDataType.Radio:
                return GV.SlotStructRestrictions.noRelative;

            default:
                Debug.LogError("SIDT not accounted for: " + SIDT.ToString());
                return GV.SlotStructRestrictions.none;
        }
    }
}
