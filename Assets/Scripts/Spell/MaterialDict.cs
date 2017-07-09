using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialDict
{

    #region singleton
    private static MaterialDict instance;


    public static MaterialDict Instance
   {
      get 
      {
         if (instance == null)
         {
             instance = new MaterialDict();
         }
         return instance;
      }
   }
    #endregion
    enum StampType { DefenseConstant, OffenseMultiplier };
    Dictionary<GV.MaterialType, MaterialInfo> materialInfoDict = new Dictionary<GV.MaterialType, MaterialInfo>();
    Dictionary<GV.MaterialType, MaterialSpriteStyle> materialSpriteStyle;
    
    float[,] absorbtionResistanceMatrix;
    float[,] damageResistanceMatrix;
    float[,] damageDistributionMatrix;
    float[,] distabilityMatrix;
    float[,] stampMatrix;
    float[] damageMatrix;
    float[] heatResistance;   //resists heat change as well as resist giving energy into heat when on fire
    float[] explosionTime; //Time for explosion to fizzle after made
    float[] bodyResistKnockback;
    float[] weightMatrix;
    float[] gravityMatrix;
    float[] weightPerEnergyMatrix;
    float[] spellUpkeep;
    float[] energyConversionMatrix; //energy lost when transfering energy to that type
    float[] destabilityVsEntityMatrix;
    float[] spellKnockbackMatrix;
    float[] concussBonusMatrix;
    float[] scaleMultMatrix;
    float[] hairlineFractureMatrix; //how much damage it can resist before fractures

    private MaterialDict() //initial energyCreationEfficency and density are set here, they should be set in matrices
    { 
        //this is really old stuff, I should refactor this into a matrix like the rest. //gravity is not used
        materialInfoDict.Add(GV.MaterialType.Energy, new MaterialInfo(1, 1, "Textures/Spells/energy", .15f));
        materialInfoDict.Add(GV.MaterialType.Rock, new MaterialInfo(5, .8f, "Textures/Spells/rock", 1));
        materialInfoDict.Add(GV.MaterialType.Fire, new MaterialInfo(5, .9f, "Textures/Spells/fire", .1f));
        materialInfoDict.Add(GV.MaterialType.Charisma, new MaterialInfo(.5f, .8f, "Textures/Spells/charisma", .1f));
        materialInfoDict.Add(GV.MaterialType.Force, new MaterialInfo(.7f, .8f, "Textures/Spells/force", .1f));
        materialInfoDict.Add(GV.MaterialType.Higgs, new MaterialInfo(.7f, .8f, "Textures/Spells/higgs", 1));
        materialInfoDict.Add(GV.MaterialType.Ice, new MaterialInfo(.7f, .8f, "Textures/Spells/ice", 1));
        materialInfoDict.Add(GV.MaterialType.Lighting, new MaterialInfo(.7f, .8f, "Textures/Spells/lighting", .1f));
        materialInfoDict.Add(GV.MaterialType.Mist, new MaterialInfo(.7f, .8f, "Textures/Spells/mist", .1f));
        materialInfoDict.Add(GV.MaterialType.Nature, new MaterialInfo(.7f, .8f,"Textures/Spells/nature", 1));
        materialInfoDict.Add(GV.MaterialType.Oil, new MaterialInfo(.7f, .8f,   "Textures/Spells/oil", 1));
        materialInfoDict.Add(GV.MaterialType.Radio, new MaterialInfo(.7f, .8f, "Textures/Spells/radio", .1f));
        materialInfoDict.Add(GV.MaterialType.Smoke, new MaterialInfo(.7f, .8f, "Textures/Spells/smoke", .1f));
        materialInfoDict.Add(GV.MaterialType.Water, new MaterialInfo(.7f, .8f, "Textures/Spells/water", .1f));

        FillExplosionTimeMatrix();
        FillHeatResistanceMatrix();
        FillAbsorbtionResistanceMatrix();
        FillDamageResistanceMatrix();
        FillDamageDistributionMatrix();
        FillDestabilityMatrix();
        FillBodyResistKnockbackMatrix();
        FillStampMatrix();
        FillWeightMatrix();  //Not currently used, delete when sure securly phased out
        FillWeightPerEnergyMatrix();
        FillSpellUpkeepMatrix();
        FillEnergyConversionMatrix();
        FillGravityMatrix();
        FillDestabilityVsEntityMatrix();
        FillDamageMatrix();
        FillSpellKnockbackMatrix();
        FillGetMaterialSpriteStyleDict();
        FillConcussBonusMatrix();
        FillScaleMultMatrix();
        FillHairlineDefenseMatrix();
    }

    private void FillHairlineDefenseMatrix()
    {
        //How much dmg before it begins to hairline fracture, "fracture power" is offensiveDmg/defensivePower
        //Density multiplies as defense
        hairlineFractureMatrix = new float[]
        { 9999f,    //energy
            20f,    //rock
          9999f,    //force
          9999f,    //fire
          9999f,    //water
          9999f,    //lighting
            10f,    //nature
          9999f,    //oil
          9999f,    //charisma
          9999f,    //radio
            12f,    //ice
            30f,    //higgs
          9999f,    //smoke
          9999f };  //Mist
    }

    private void FillScaleMultMatrix()
    {
        scaleMultMatrix = new float[]
        {  1f,    //energy
          .9f,    //rock
         1.1f,    //force
           1f,    //fire
           1f,    //water
           1f,    //lighting
           1f,    //nature
           1f,    //oil
           1f,    //charisma
           1f,    //radio
          .9f,    //ice
          .8f,    //higgs
           1f,    //smoke
           1f };  //Mist
    }

    private void FillGetMaterialSpriteStyleDict()
    {
        materialSpriteStyle = new Dictionary<GV.MaterialType, MaterialSpriteStyle>()
        {
            {GV.MaterialType.Charisma, new MaterialSpriteStyle(Color.white,GV.SpriteStyle.Wave) },  //yup
            {GV.MaterialType.Energy, new MaterialSpriteStyle(255,239,28,GV.SpriteStyle.Solid) },
            {GV.MaterialType.Fire, new MaterialSpriteStyle(255,117,28,GV.SpriteStyle.Solid) }, //yup
            {GV.MaterialType.Force, new MaterialSpriteStyle(179,223,255,GV.SpriteStyle.Wave,96) }, //y
            {GV.MaterialType.Higgs, new MaterialSpriteStyle(34,28,103,GV.SpriteStyle.Solid) }, //
            {GV.MaterialType.Ice, new MaterialSpriteStyle(23,175,238,GV.SpriteStyle.Solid) },
            {GV.MaterialType.Lighting, new MaterialSpriteStyle(236,230,31,GV.SpriteStyle.Solid) },
            {GV.MaterialType.Mist, new MaterialSpriteStyle(101,234,240,GV.SpriteStyle.Gas,95) },
            {GV.MaterialType.Nature, new MaterialSpriteStyle(0,188,56,GV.SpriteStyle.Solid) },
            {GV.MaterialType.None, new MaterialSpriteStyle(Color.white,GV.SpriteStyle.Solid) },
            {GV.MaterialType.Oil, new MaterialSpriteStyle(73,24,133,GV.SpriteStyle.Solid) },
            {GV.MaterialType.Radio, new MaterialSpriteStyle(54,54,54,GV.SpriteStyle.Wave) },
            {GV.MaterialType.Rock, new MaterialSpriteStyle(113,63,11,GV.SpriteStyle.Solid) },
            {GV.MaterialType.Smoke, new MaterialSpriteStyle(90,90,90,GV.SpriteStyle.Gas,95) },
            {GV.MaterialType.Water, new MaterialSpriteStyle(0,165,233,GV.SpriteStyle.Solid) }
        };
    }

    private void FillEnergyConversionMatrix()
    {
        //Energy loss transfering energy to a material type, 1 means 100% loss, 0 = 0% loss
        energyConversionMatrix = new float[] 
        {  0f,   //energy
          .1f,   //rock
           0f,   //force
          .1f,   //fire
           0f,   //water
           0f,    //lighting
           0f,    //nature
           0f,    //oil
           0f,    //charisma
           0f,    //radio
          .1f,    //ice
           0f,    //higgs
           0f,    //smoke
           0f };  //Mist
    }

    private void FillConcussBonusMatrix()
    {
        //Multiplier to bonus concuss dealt
                                         //Earth, Energy,  Air, Water, Fire, Ice, Aether, Nature, Mana
        concussBonusMatrix = new float[] {  1.2f,      1,    1,   .2f, .33f, 1.1f,     0,      0,    0 };
    }

    private void FillSpellKnockbackMatrix()
    {
        //Force in N per energy (also affected by density), only energy forms will apply this
        spellKnockbackMatrix = new float[] 
        {20f,   //energy
          1f,   //rock
         40f,   //force
          1f,   //fire
          1f,   //water
          1f,    //lighting
          1f,    //nature
          1f,    //oil
          1f,    //charisma
          0f,    //radio
          1f,    //ice
          1f,    //higgs
          1f,    //smoke
          1f };  //Mist
    }

    private void FillGravityMatrix()
    {
        //everything is relative to GV.Gravuty_spell_Energy/Physical
        gravityMatrix = new float[] 
        {  .5f,    //energy
            1f,    //rock
           .2f,    //force
           .5f,    //fire
            1f,    //water
           .5f,    //lighting
            1f,    //nature
            1f,    //oil
            1f,    //charisma
            1f,    //radio
            1f,    //ice
            1f,    //higgs
            1f,    //smoke
            1f };  //Mist
    }

    private void FillSpellUpkeepMatrix()
    {
        //per second % cost of thier total energy, before player efficencys
        spellUpkeep = new float[] 
        { .1f,   //energy
         .02f,   //rock
          .1f,   //force
          .1f,   //fire
          .1f,   //water
          .2f,    //lighting
          .1f,    //nature
          .1f,    //oil
          .1f,    //charisma
          .1f,    //radio
         .05f,    //ice
         .05f,    //higgs
         .02f,    //smoke
         .02f };  //Mist
    }

    private void FillWeightMatrix()  //This is used by solid material, although energy should be used
    {
        //weight per meter^2 / 1x1 unity square
        weightMatrix = new float[] 
        { .1f,   //energy
           2f,   //rock
          .1f,   //force
          .1f,   //fire
           1f,   //water
          .1f,    //lighting
            1,    //nature
            1,    //oil
          .1f,    //charisma
          .1f,    //radio
            1,    //ice
            3,    //higgs
          .1f,    //smoke
          .1f };  //Mist
    }

    private void FillWeightPerEnergyMatrix()
    {
        //weight per unit of energy
        weightPerEnergyMatrix = new float[] 
        {  1f,   //energy
           2f,   //rock
          .5f,   //force
          .5f,   //fire
           1f,   //water
          .5f,    //lighting
            1,    //nature
            1,    //oil
         .25f,    //charisma
          .1f,    //radio
         1.8f,    //ice
         3.2f,    //higgs
          .5f,    //smoke
          .5f };  //Mist
    }
    
    private void FillDamageResistanceMatrix()
    {
        //Energy, Rock, Force, Fire, Water, Lighting, Tree , Oil , Charisma, Radio, Ice , Higgs, Smoke , Mist 
        damageResistanceMatrix = new float[,]{ //where resistance is 1-, example .2f is 80% resistance.
             {1,    1,    1,    1,    1,       1,       1,    1,      1,       1,    1,    1,      1,      1},//EnergyFormResistance
             {1,    1,    1,    1,    1,       1,       1,    1,      1,       1,    1,    1,      1,      1}//PhysicalFormResitance
        };
    }

    private void FillStampMatrix()
    {
           //Energy, Rock, Force, Fire, Water, Lighting, Tree , Oil , Charisma, Radio, Ice , Higgs, Smoke , Mist 
        stampMatrix = new float[,]{ //StampDefense(constant) vs StampOffense(constant + energy*multiplier)
             { 0.1f,  10,  0.1f, 0.1f,  0.1f,   0.1f,  0.1f,  0.1f,  0.1f,   0.1f,       8,     15,  0.1f,  0.1f}, //DefenseConstant --only for solid defense--
             {   1f,   0f,   .2f,   1f,   .5f,   1.5f,     0,   .4f,     0,      0,       0,      0,   .1f,   .1f} //OffenseMultiplier -for energy stamping or physical fracturing--
        };
        //cannot have 0 stamp defense, will break D2D code.
    }
    
    private void FillBodyResistKnockbackMatrix()
    {
      //Earth, Energy, Air, Water, Fire, Ice, Aether, Nature, Mana
        bodyResistKnockback = new float[] //where resistance is 1-, example .2f is 80% resistance.
         {1,    1,      50,    .2f,  .2f,   1,      0,      0,   0};        
    }

    private void FillDamageMatrix()
    {
        //points of dmg per energy
        //Earth, Energy, Air, Water, Fire, Ice, Aether, Nature, Mana
        damageMatrix = new float[] 
        {   1,     1,    .2f,  .2f,  .7f,  1,     0,      0,    0 };
    }

    private void FillDamageDistributionMatrix()
    {
        //Energy, Rock, Force, Fire, Water, Lighting, Tree , Oil , Charisma, Radio, Ice , Higgs, Smoke , Mist 
        damageDistributionMatrix = new float[,]{
             {0,    1f,   0,    0,    0,       0,       0,  .3f,      0,       0,  .3f,  .7f,      0,      0},//Earth
             {.9f,  0,    0,  .1f,    0,     .7f,       0,    0,      0,       0,    0,  .3f,      0,      0},//Energy
             {0,    0,    1,    0,    0,       0,       0,    0,      0,       0,    0,    0,    .5f,     5f},//Air
             {0,    0,    0,    0,   1f,       0,       0,  .7f,      0,       0,  .2f,    0,    .5f,    .5f},//water
             {.1f,  0,    0,  .9f,    0,     .3f,       0,    0,      0,       0,    0,    0,      0,      0},//fire
             {0,    0,    0,    0,    0,       0,       0,    0,      0,       0,  .5f,    0,      0,      0},//ice
             {0,    0,    0,    0,    0,       0,       0,    0,      1,       1,    0,    0,      0,      0},//Aether
             {0,    0,    0,    0,    0,       0,      1f,    0,      0,       0,    0,    0,      0,      0},//nature
             {0,    0,    0,    0,    0,       0,       0,    0,      0,       0,    0,    0,      0,      0}//Mana
        };
    }

    public void DEBUG_TurnEnergyConcusive()  //for testing, delete eventaully
    {
        damageDistributionMatrix[0, 0] = 1f;
        damageDistributionMatrix[1, 0] = 0f;
    }

    private void FillHeatResistanceMatrix() 
    { //also, the values are 1-heatResistance, so 0, means 100% resistance 
        //                          Energy, Rock, Force, Fire, Water, Lighting, Tree , Oil , Charisma, Radio, Ice , Higgs, Smoke , Mist 
        heatResistance = new float[] { 1f,  .2f,   .9f,    1,   .8f,        1,     1,    1,        0,     0,    1,   .2f,      1,  1 };
    }

    private void FillExplosionTimeMatrix()
    { //also, the values are 1-heatResistance, so 0, means 100% resistance 
        //                          Energy, Rock, Force, Fire, Water, Lighting, Tree , Oil , Charisma, Radio, Ice , Higgs, Smoke , Mist 
        explosionTime = new float[] 
        { 3f,   //energy
          3f,   //rock
          3f,   //force
          3f,   //fire
          3f,   //water
          3,    //lighting
          3,    //nature
          3,    //oil
          3,    //charisma
          3,    //radio
          3,    //ice
          3,    //higgs
          3,    //smoke
          3 };  //Mist
    }

    private void FillAbsorbtionResistanceMatrix()
    {
        //COLUMN: The Materialthat is absorbing!   Row: The material that is being absorbed and resisting by a certain %!
        //example, as rock, you resist being absorbed by air, 1 means 0% resistance, .75 -> 25%...
        //IN ALL CASES, Phsyical should have very low resistances to energy, else energy not absorb alot and pass through physical without destabilizing, energy will have high destablization when absorbing physical
        //Energy, Rock, Force, Fire, Water, Lighting, Tree , Oil , Charisma, Radio, Ice , Higgs, Smoke , Mist 
        absorbtionResistanceMatrix = new float[,]{
             { .4f,    1,    1,    1,    1,       1,       1,    1,      1,       1,    1,    1,      1,      1},//Energy
             {.25f,    1,    1,    1,    1,       1,       1,    1,      1,       1,    1,    1,      1,      1},//Rock
             {   1,    1,    1,    1,    1,       1,       1,    1,      1,       1,    1,    1,      1,      1},//Force
             {   1,    1,    1,    1,    1,       1,       1,    1,      1,       1,    1,    1,      1,      1},//Fire
             {   1,    1,    1,    1,    1,       1,       1,    1,      1,       1,    1,    1,      1,      1},//Water
             {   1,    1,    1,    1,    1,       1,       1,    1,      1,       1,    1,    1,      1,      1},//Lighting
             {   1,    1,    1,    1,    1,       1,       1,    1,      1,       1,    1,    1,      1,      1},//Tree
             {   1,    1,    1,    1,    1,       1,       1,    1,      1,       1,    1,    1,      1,      1},//Oil
             {   1,    1,    1,    1,    1,       1,       1,    1,      1,       1,    1,    1,      1,      1},//Charisma
             {   1,    1,    1,    1,    1,       1,       1,    1,      1,       1,    1,    1,      1,      1},//Radio
             { .8f,    1,    1,    1,    1,       1,       1,    1,      1,       1,    1,    1,      1,      1},//Ice
             {   1,    1,    1,    1,    1,       1,       1,    1,      1,       1,    1,    1,      1,      1},//Higgs
             {   1,    1,    1,    1,    1,       1,       1,    1,      1,       1,    1,    1,      1,      1},//Smoke
             {   1,    1,    1,    1,    1,       1,       1,    1,      1,       1,    1,    1,      1,      1},//Mist
        };
    }

    private void FillDestabilityMatrix()
    {
        //point of view of material doing the absorbing (the devourer), which is the ROWS viewpoint!!, Columns is the one being absorbed
        //This ratio is relative to "stability - (EnergyBeingAbsorbed/SpellInfo.currentEnergy) * thisValue" where stability [1,0]
        //4 would mean gaurenteed explosion at absorbing 25% your current energy, .25 would mean at 4x
        //Energy, Rock, Force, Fire, Water, Lighting, Tree , Oil , Charisma, Radio, Ice , Higgs, Smoke , Mist 
        distabilityMatrix = new float[,]{
             {2,    6,  .1f,    2,    2,       2,       2,    2,      2,       2,    5,    2,      2,      2},//Energy
             {2,  .5f,    2,    2,    2,       2,       2,    2,      2,       2, .25f,    2,      2,      2},//Rock
             {.1f,  2,    2,    2,    2,       2,       2,    2,      2,       2,    2,    2,      2,      2},//Force
             {2,    7, .25f,  .1f,    2,       2,       2,    2,      2,       2,    8,    2,      2,      2},//Fire
             {2,    2,    2,    2,    2,       2,       2,    2,      2,       2,    2,    2,      2,      2},//Water
             {2,    2,    2,    2,    2,       2,       2,    2,      2,       2,    2,    2,      2,      2},//Lighting
             {2,    2,    2,    2,    2,       2,       2,    2,      2,       2,    2,    2,      2,      2},//Tree
             {2,    2,    2,    2,    2,       2,       2,    2,      2,       2,    2,    2,      2,      2},//Oil
             {2,    2,    2,    2,    2,       2,       2,    2,      2,       2,    2,    2,      2,      2},//Charisma
             {2,    2,    2,    2,    2,       2,       2,    2,      2,       2,    2,    2,      2,      2},//Radio
             {2,  .5f,    2,    2,    2,       2,       2,    2,      2,       2, .25f,    2,      2,      2},//Ice
             {2,    2,    2,    2,    2,       2,       2,    2,      2,       2,    2,    2,      2,      2},//Higgs
             {2,    2,    2,    2,    2,       2,       2,    2,      2,       2,    2,    2,      2,      2},//Smoke
             {2,    2,    2,    2,    2,       2,       2,    2,      2,       2,    2,    2,      2,      2},//Mist
        };
    }

    private void FillDestabilityVsEntityMatrix()
    { 
        //flat destability (@ 1 explodes) cause by collision with a living entity like a player, velocity multipliers atm does not apply
        //no delta time affect, so ethier it explodes 1 or doesn't 0
        //this only applies to energy types, so ignore the physicals
        destabilityVsEntityMatrix = new float[] 
        { 1f,   //energy
          0f,   //rock
          0f,   //force
          1f,   //fire
          0f,   //water
          1,    //lighting
          1,    //nature
          0,    //oil
          1,    //charisma
          1,    //radio
          0,    //ice
          0,    //higgs
          0,    //smoke
          0 };  //Mist
    }
    //
    public float GetHairlineDefense(GV.MaterialType materialType, float densityEffect)
    {
        try
        {
            return hairlineFractureMatrix[(int)materialType] * densityEffect;
        }
        catch
        {
            Debug.LogError("hairline matrix failure for: " + materialType);
            return 1;
        }
    }

    public float GetGravity(GV.MaterialType materialType)
    {
        try
        {
            return gravityMatrix[(int)materialType];
        }
        catch
        {
            Debug.LogError("Gravity matrix failure for: " + materialType);
            return 1;
        }
    }


    public float GetExplosionTime(GV.MaterialType materialType)
    {
        try
        {
            return explosionTime[(int)materialType];
        }
        catch
        {
            Debug.LogError("Explosion time failure for: " + materialType);
            return 1;
        }
    }

    public float GetBodyResistKnockback(GV.DamageTypes dmgType)
    {
        try
        {
            return bodyResistKnockback[(int)dmgType];
        }
        catch
        {
            Debug.LogError("bodyResistKnockback failure for: " + dmgType);
            return 1;
        }
    }

    public float GetDamageResistance(GV.MaterialType materialTakingDamage, GV.SpellForms damagingSpellFormType)
    {
        try
        {
            return damageResistanceMatrix[(int)damagingSpellFormType,(int)materialTakingDamage];
        }
        catch
        {
            Debug.LogError("damageResistanceMatrix failure for: " + materialTakingDamage.ToString() + " & " + damagingSpellFormType.ToString());
            return 1;
        }
    }

    public float GetStampValue(GV.MaterialType materialRequesting, bool isAttacking, float energy, float densityEffect)
    {
        float toRet = 0; //Energy * MaterialMod + Constant ==> Offense/Defense
        try
        {
            if (!isAttacking)
                toRet = stampMatrix[(int)StampType.DefenseConstant, (int)materialRequesting];
            else
                toRet = stampMatrix[(int)StampType.OffenseMultiplier,(int)materialRequesting] * energy;
        }
        catch
        {
            Debug.LogError("StampValueMatrix failure for material: " + materialRequesting.ToString() + " & energy: " + energy + " is attacking: " + isAttacking);
        }
        //EnergyMonitoringSystem.Instance
        //Debug.Log("returning the stampvalue: " + toRet);
        return toRet * densityEffect;
    }

    public float GetDistability(GV.MaterialType materialConsumingEnergy, GV.MaterialType materialBeingAbsorbed)
    {
        try
        {
            return distabilityMatrix[(int)materialConsumingEnergy, (int)materialBeingAbsorbed];
        }
        catch
        {
            Debug.LogError("Distability Matrix failure for: " + materialConsumingEnergy.ToString() + " & " + materialBeingAbsorbed.ToString());
            return 1;
        }
    }

    public float GetEnergyConversion(GV.MaterialType convertingToMat)
    {
        try
        {
            return energyConversionMatrix[(int)convertingToMat];
        }
        catch
        {
            Debug.LogError("Energy Conversion Matrix failure for: " + convertingToMat.ToString());
            return 1;
        }
    }

    public float GetConcussBonus(GV.DamageTypes dmgType)
    {
        try
        {
            return concussBonusMatrix[(int)dmgType];
        }
        catch
        {
            Debug.LogError("Concuss bonus Matrix failure for: " + dmgType.ToString());
            return 1;
        }
    }

    public float GetDamage(GV.DamageTypes dmgType)
    {
        try
        {
            return damageMatrix[(int)dmgType];
        }
        catch
        {
            Debug.LogError("dmg Matrix failure for: " + dmgType.ToString());
            return 1;
        }
    }

    public float GetKnockback(GV.MaterialType matType)
    {
        try
        {
            return spellKnockbackMatrix[(int)matType];
        }
        catch
        {
            Debug.LogError("knockback Matrix failure for: " + matType.ToString());
            return 1;
        }
    }

    public float GetDamageDistribution(GV.MaterialType affectedMaterial, GV.DamageTypes damageType)
    {
        try
        {
            return damageDistributionMatrix[(int)damageType, (int)affectedMaterial];
        }
        catch
        {
            Debug.LogError("damageDistributionMatrix failure for: " + affectedMaterial.ToString() + " & " + damageType.ToString());
            return 1;
        }
    }

    public float GetAbsorbtionResitance(GV.MaterialType materialResisting, GV.MaterialType theMaterialAbsorbing)
    {
        try
        {
            return absorbtionResistanceMatrix[(int)materialResisting, (int)theMaterialAbsorbing];
        }
        catch
        {
            Debug.LogError("Absorbtion resistance Matrix failure for: " + materialResisting + " & " + theMaterialAbsorbing);
            return 1;
        }
    }

    public float GetHeatResistance(GV.MaterialType materialResisting)
    {
        try
        {
            return heatResistance[(int)materialResisting];
        }
        catch
        {
            Debug.LogError("HeatResistance failure for: " + materialResisting);
            return 1;
        }
    }

    public float GetDestabilityVsEntity(GV.MaterialType materialCollidingWithEntity)
    {
        try
        {
            return destabilityVsEntityMatrix[(int)materialCollidingWithEntity];
        }
        catch
        {
            Debug.LogError("Destability failure for: " + materialCollidingWithEntity);
            return 1;
        }
    }

    public float GetSpellUpkeep(GV.MaterialType materialType)
    {
        try
        {
            return spellUpkeep[(int)materialType];
        }
        catch
        {
            Debug.LogError("spell upkeep matrix failure for: " + materialType);
            return 1;
        }
    }

    public float GetWeightPerUnitArea(GV.MaterialType materialType)
    {
        try
        {
            return weightMatrix[(int)materialType];
        }
        catch
        {
            Debug.LogError("Weight matrix failure for: " + materialType);
            return 1;
        }
    }

    public float GetWeightPerEnergy(GV.MaterialType materialType)
    {
        try
        {
            return weightPerEnergyMatrix[(int)materialType];
        }
        catch
        {
            Debug.LogError("Weight per energy matrix failure for: " + materialType);
            return 1;
        }
    }

    public float GetScaleMult(GV.MaterialType materialType)
    {
        try
        {
            return scaleMultMatrix[(int)materialType];
        }
        catch
        {
            Debug.LogError("scale mult matrix failure for: " + materialType);
            return 1;
        }
    }

    public MaterialSpriteStyle GetMaterialSpriteStyle(GV.MaterialType matType)
    {
        return materialSpriteStyle[matType];
    }

    public MaterialInfo GetMaterialInfo(GV.MaterialType name )
    {
        if (materialInfoDict.ContainsKey(name))
            return materialInfoDict[name];
        Debug.LogError("MaterialInfoDict does not contain key: " + name + " returning null");
        return null;
    }

    public List<string> GetMaterialActions(GV.MaterialType matType)
    {
        List<string> toRet = new List<string>();
        if (GV.GetSpellFormByMaterialType(matType) == GV.SpellForms.Energy)
        {
            toRet.Add("Explode");
            toRet.Add("Implode");
            toRet.Add("DirectionalExplode");
        }
        else
        {
            toRet.Add("Explode");
            toRet.Add("Fracture");
        }

        switch (matType)
        {
            case GV.MaterialType.Energy:
            case GV.MaterialType.Force:
            case GV.MaterialType.Fire:
            case GV.MaterialType.Lighting:
            case GV.MaterialType.Charisma:
            case GV.MaterialType.Radio:
            case GV.MaterialType.Mist:
            case GV.MaterialType.Smoke:
            case GV.MaterialType.Water:
            case GV.MaterialType.Oil:
            case GV.MaterialType.Nature:
            case GV.MaterialType.Rock:
            case GV.MaterialType.Ice:
            case GV.MaterialType.Higgs:
                break;
            default:
                Debug.LogError("MaterialType: " + matType.ToString() + " not recognized, defaulted to energy, please add");
                break;
        }
        return toRet;
    }

    public class MaterialSpriteStyle
    {
        public Color color;
        public GV.SpriteStyle spriteStyle;

        public MaterialSpriteStyle(Color _color, GV.SpriteStyle _spriteStyle)
        {
            color = _color;
            spriteStyle = _spriteStyle;
        }

        public MaterialSpriteStyle(float r, float g, float b, GV.SpriteStyle _spriteStyle, float a = 255)
        {
            color = new Color(r/255,g/255,b/255,a/255);
            spriteStyle = _spriteStyle;
        }
    }
}
