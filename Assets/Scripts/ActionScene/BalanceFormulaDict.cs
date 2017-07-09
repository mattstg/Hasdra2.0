using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BalanceFormulaDict
{

    #region Singleton
    private static BalanceFormulaDict instance;

    private BalanceFormulaDict() { FillBook(); }

    public static BalanceFormulaDict Instance
    {
        get{
            if(instance == null)
            {
                instance = new BalanceFormulaDict();
            }
            return instance;
        }
    }
    #endregion

    public Dictionary<string, BalanceFormula> balanceBook = new Dictionary<string, BalanceFormula>();

    ///Ass Storages
    /////stats.Skills.Add("MeleeRangeEff", new Skill("MeleeRangeEff", "str", stats.strength, stats, new AssStorage(GV.as_meleeRangeEff)));
    void FillBook()
    {
        balanceBook.Add("meleeRangeEff", new BasicBF(.005f, 0, 0, GV.MELEE_RANGE_EFF_LOSS_PER_METER));
        balanceBook.Add("concussMax", new BasicBF(1f, 1));
        balanceBook.Add("concusionRecoverRate", new BasicBF(.02f, 0, 0, 1)); // percent recover of 1 full bar per second when not taking dmg
        balanceBook.Add("maximumHP", new BasicBF(4f, 6));
        balanceBook.Add("maximumStamina", new BasicBF(4f, 6));
        balanceBook.Add("maximumEnergy", new BasicBF(4f, 6));
        balanceBook.Add("spellChargeRadio", new BasicBF(.33f, 0)); //How fast you can spend energy to charge a spell radio

        balanceBook.Add("resistance", new AssStorage(30, .2f, .95f, 30, GV.HorzAsym.MaxToMin)); //all resistances follow from this

        balanceBook.Add("hpRegenRate", new BasicBF(.1f, .5f));
        balanceBook.Add("staminaRegenRate", new BasicBF(.4f, .5f));
        balanceBook.Add("energyRegenRate", new BasicBF(.2f, .5f));

        balanceBook.Add("spellScale", new BasicBF(.12f, .7f));

        balanceBook.Add("constBodyScaleX", new BasicBF(.00267f, 0));
        balanceBook.Add("constBodyScaleY", new BasicBF(.0067f, 0));
        balanceBook.Add("strBodyScaleX", new BasicBF(.0067f, 0));
        balanceBook.Add("strBodyScaleY", new BasicBF(.00267f, 0));
        balanceBook.Add("upperBody", new BasicBF(.0067f, 0));  //used for scale and dmg from those limbs
        balanceBook.Add("lowerBody", new BasicBF(.00267f, 0));  //used for scale and dmg from those limbs

        //
        balanceBook.Add("spellPlacementSpeed", new BasicBF(.2f,3)); //Max speed for hover, placement, etc
        balanceBook.Add("maxSpellLaunchSpeed", new BasicBF(.25f,2));
        //Abilities


    }

    public BalanceFormula GetFormula(string balanceName)
    {
        if (!balanceBook.ContainsKey(balanceName))
        {
            Debug.LogError("Balance " + balanceName + " not found");
            return new BasicBF(0, 0);
        }
        return balanceBook[balanceName].CopyThis();
    }

    public float GetValue(string balanceName, float _value)
    {
        return GetFormula(balanceName).ret(_value);
    }
}
