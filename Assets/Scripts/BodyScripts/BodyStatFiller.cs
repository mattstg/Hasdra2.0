using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class BodyStatFiller
{

    public BodyStatFiller()
    {
        //do nothing HA!
        //ghost of  CHA!
    }

    public BodyStatFiller(BodyStats stats)
    {
        FillAvatarBodyStats(stats);
    }

    public static void FillAvatarBodyStats(BodyStats stats)
    {
        

        //Str
		stats.Skills.Add("climbStrength", new Skill("grabStrength", GV.Stats.Str ,stats.strength, stats, new AssStorage(GV.as_ClimbStrength)));
        stats.Skills.Add("stanimaToEnergyRatio", new Skill("stanimaToEnergyRatio", GV.Stats.Str, stats.strength, stats, new AssStorage(GV.as_stanimaToEnergyRatio)));
        stats.Skills.Add("stanimaTransferToSpellRate", new Skill("stanimaTransferToSpellRate", GV.Stats.Str, stats.strength, stats, new AssStorage(GV.as_stanimaTransferToSpell))); 
		//stats.Skills.Add("VelocityMeleeDecay" , new Skill ("VelocityMeleeDecay","str", stats.strength, stats, new AssStorage(GV.as_veloStabilityDecay)));
        
        AddSkill("meleeRangeEff",GV.Stats.Agi,stats);

        stats.Skills.Add("stanimaToForce", new Skill("stanimaToForce", GV.Stats.Agi, stats.agility, stats, new AssStorage(50, 25, 75, 50, GV.HorzAsym.MinToMax)));
        //stats.Skills.Add("maxStanimaUse", new Skill("maxStanimaUse", "agi", stats.agility, stats, new AssStorage(40, 9999, 99999, 50, GV.HorzAsym.MinToMax))); //max amount of stanima can use for sum of all abilities in a second.

		//stats.Skills.Add("jumpForce", new Skill("jumpForce", "agi", stats.agility, stats,new AssStorage(GV.JUMP_FORCE_SCALE,GV.JUMP_FORCE_MIN,GV.JUMP_FORCE_MAX,GV.HorzAsym.MinToMax)));
        stats.Skills.Add("acrobatics", new Skill("acrobatics", GV.Stats.Agi, stats.agility, stats, new AssStorage(GV.as_airDragTime))); //time before you lose full maneuverability while in air  (airdrag time)
		//stats.Skills.Add("jumpDuration", new Skill("jumpDuration", "agi", stats.agility, stats, new AssStorage(GV.JUMP_FORCE_DURATION_SCALE, GV.JUMP_FORCE_DURATION_MIN, GV.JUMP_FORCE_DURATION_MAX, GV.HorzAsym.MinToMax)));
        //stats.Skills.Add("jumpReflexes", new Skill("jumpReflexes", "agi", stats.agility, stats)); //affects length time you may move in air before you start losing horizontally force application
		//stats.Skills.Add("aimerSpinSpeed", new Skill("aimerSpinSpeed", "agi", stats.agility, stats, new AssStorage(GV.RETICLE_SPIN_SPD_SCALE, GV.RETICLE_SPIN_SPD_MIN, GV.RETICLE_SPIN_SPD_MAX, GV.HorzAsym.MinToMax)));
		//stats.Skills.Add("MaxAimAngle", new Skill("MaxAimAngle", "agi", stats.agility, stats, new AssStorage(GV.RETICLE_ANGLE_MEAN, GV.RETICLE_ANGLE_MIN, GV.RETICLE_ANGLE_MAX,GV.HorzAsym.MinToMax)));
        
        stats.Skills.Add("staminaEfficiency", new Skill("staminaEfficiency", GV.Stats.Agi, stats.agility, stats));
        stats.Skills.Add("getUpTime", new Skill("getUpTime", GV.Stats.Agi, stats.agility, stats));
        stats.Skills.Add("meleeSpeed", new Skill("punchSpeed", GV.Stats.Agi, stats.agility, stats, new AssStorage(GV.as_punchAnimSpeed)));
        stats.Skills.Add("climbSpeed", new Skill("grabSpeed", GV.Stats.Agi, stats.agility, stats, new AssStorage(GV.as_climbSpeed)));
        stats.Skills.Add("meleeDecay", new Skill("meleeDecay", GV.Stats.Agi, stats.agility, stats, new AssStorage(GV.as_meleeDecayCoeff)));

        //Const
        stats.Skills.Add("bodySize", new Skill("bodySize", GV.Stats.Const, stats.constitution, stats));
        
        stats.Skills.Add("durability", new Skill("durability", GV.Stats.Const, stats.constitution, stats));
        stats.Skills.Add("debuffResistance", new Skill("debuffResistance", GV.Stats.Const, stats.constitution, stats));
        AddSkill("maximumHP", GV.Stats.Const, stats); //scaling value, not total value
		AddSkill("maximumStamina", GV.Stats.Const, stats); //scaling value, not total value
		AddSkill("concussMax", GV.Stats.Const, stats); //scaling value, not total value
        AddSkill("dmgResist", GV.Stats.Const, stats);
        //Char

        //Int
        stats.Skills.Add("skillModChargeRate", new Skill("skillModChargeRate", GV.Stats.Int, stats.intelligence, stats, new AssStorage(GV.as_skillModChargeRate)));  //max energy to charge all skillmods per turn 
        stats.Skills.Add("skillModChargeEfficency", new Skill("skillModChargeEfficency", GV.Stats.Int, stats.intelligence, stats, new AssStorage(GV.as_skillModChargeEff)));
        stats.Skills.Add("energyPerForce", new Skill("energyPerForce", GV.Stats.Int, stats.intelligence, stats, new AssStorage(GV.as_energyToForce)));  //Force per unit of energy
        stats.Skills.Add("energyUseEfficiency", new Skill("energyUseEfficiency", GV.Stats.Int, stats.intelligence, stats, new AssStorage(GV.as_energyChargeLossFromMaterial)));  //Energy loss from material transfer
        stats.Skills.Add("sightRange", new Skill("sightRange", GV.Stats.Int, stats.intelligence, stats));
        
        stats.Skills.Add("spellFireSpeed", new Skill("spellFireSpeed", GV.Stats.Int, stats.intelligence, stats, new AssStorage(GV.as_spellCastAnimSpeed)));
        stats.Skills.Add("flatMinSpellUpkeep", new Skill("flatMinSpellUpkeep", GV.Stats.Int, stats.intelligence, stats, new AssStorage(GV.as_flatSpellEnergyUpkeep)));  //min energy loss in a spell
        stats.Skills.Add("percentSpellUpkeep", new Skill("percentSpellUpkeep", GV.Stats.Int, stats.intelligence, stats, new AssStorage(GV.as_percentSpellEnergyUpkeep)));
        AddSkill("spellPlacementSpeed", GV.Stats.Int, stats); //Used for speed a spell can move for "Hover","Placement","Follow" etc
        //stats.Skills.Add ("velocitySpellDecay", new Skill ("velocitySpellDecay","int", stats.intelligence, stats, new AssStorage(GV.as_veloStabilityDecay)));
        AddSkill("spellChargeRadio", GV.Stats.Int, stats);

        stats.Skills.Add("stabilityDecayRate", new Skill("stabilityDecayRate", GV.Stats.Int, stats.intelligence, stats, new AssStorage(GV.as_stabDecayRate)));
        stats.Skills.Add("stabilityRecoveryRate", new Skill("stabilityRecoveryRate", GV.Stats.Int, stats.intelligence, stats, new AssStorage(GV.as_stabRecovRate)));

		//stats.Skills.Add("reticleDistance", new Skill("reticleDistance", "int", stats.intelligence, stats));

        //Wis
        stats.Skills.Add("energyTransferRate", new Skill("energyTransferRate", GV.Stats.Wis, stats.wisdom, stats, new AssStorage(GV.as_energyToSpellChargeRate)));
        AddSkill("maximumEnergy", GV.Stats.Wis, stats);

        stats.addSkill("maxEnergyTransferToAbilities",GV.Stats.Wis,new AssStorage(30,1,50,80,GV.HorzAsym.MinToMax)); //max amount of mana that can be transfered to all abilities (think this was removed)

        //Physicality
        AddSkill("constBodyScaleX" , GV.Stats.Const, stats);
        AddSkill("constBodyScaleY" , GV.Stats.Const, stats);
        AddSkill("strBodyScaleX"   , GV.Stats.Str  , stats);
        AddSkill("strBodyScaleY"   , GV.Stats.Str  , stats);
        AddSkill("upperBody"       , GV.Stats.Str  , stats);
        AddSkill("lowerBody"       , GV.Stats.Const, stats); 

        //Regen
        AddSkill("staminaRegenRate" , GV.Stats.Agi  , stats);
        AddSkill("hpRegenRate"      , GV.Stats.Const, stats);
        AddSkill("energyRegenRate"  , GV.Stats.Int  , stats);

        //DamageType Resistances
        AddResistanceSkill("resistEarth"    , GV.Stats.Const, stats);
        AddResistanceSkill("resistEnergy"   , GV.Stats.Const, stats);
        AddResistanceSkill("resistAir"      , GV.Stats.Const, stats);
        AddResistanceSkill("resistWater"    , GV.Stats.Const, stats);
        AddResistanceSkill("resistFire"     , GV.Stats.Const, stats);
        AddResistanceSkill("resistIce"      , GV.Stats.Const, stats);
        AddResistanceSkill("resistAether"   , GV.Stats.Const, stats);
        AddResistanceSkill("resistNature"   , GV.Stats.Const, stats);
        AddResistanceSkill("resistMana"     , GV.Stats.Const, stats);
        AddResistanceSkill("resistKnockback", GV.Stats.Const, stats);
        AddResistanceSkill("resistConcusion", GV.Stats.Const, stats);


        stats.Skills.Add("concusionRecoverRate", new Skill("concusionRecoverRate", GV.Stats.Const, stats.constitution, stats, new AssStorage(GV.as_concussRecoverSpeed)));
        stats.Skills.Add("heatRecoverRate", new Skill("heatRecoverRate", GV.Stats.Const, stats.constitution, stats));
    }

    private static void AddResistanceSkill(string _name, GV.Stats _statType, BodyStats stats)
    {
        stats.Skills.Add(_name, new Skill(_name, _statType, stats.agility, stats, BalanceFormulaDict.Instance.GetFormula("resistance")));
    }

    private static void AddSkill(string _name, GV.Stats _statType, BodyStats stats)
    {
        stats.Skills.Add(_name, new Skill(_name, _statType, stats.agility, stats, BalanceFormulaDict.Instance.GetFormula(_name)));
    }

    public static List<string> GetAllSkills()
    {
        BodyStats bs = new BodyStats(true);
        BodyStatFiller.FillAvatarBodyStats(bs);
        return bs.Skills.Keys.ToList<string>();
    }
}
