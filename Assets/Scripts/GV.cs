using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//Might have to remove this and make a singleton if too much static memory is bad for a program

public class GV : MonoBehaviour
{
    //links
    public static UILayer smUiLayer;
    public static CharLevelUI charLvlUI;
    public static WorldUILinks worldUI;
    public static WorldSingletonLinks worldLinks;
    public static int NumOfPlayers;

    //by default enums are ordered ints, dont need to be implicit (start at 0)
    public enum DamageTypes { Earth = 0, Energy = 1, Air = 2, Water = 3, Fire = 4, Ice = 5, Aether = 6, Nature = 7, Mana = 8}
    public enum MaterialType { Energy = 0, Rock = 1, Force = 2, Fire = 3, Water = 4, Lighting = 5, Nature = 6, Oil = 7, Charisma = 8, Radio = 9, Ice = 10, Higgs = 11, Smoke = 12, Mist = 13, None }; //changing these values will kill shit, dont do it
    public enum BasicColiType { Spell, Player, SolidMaterial, Explosion, None}; //used for "lastColiHit" in spellinfo/state machine, make sure numbers match
    public enum SpellAnimationType { Spotlight, GatheringDebris, Unstable }
    public enum SpellState { Charging, Launched, Exploding, FinishedExplosion };
    public enum SpellForms { Energy = 0, Physical = 1 };
    public enum SpellShape { Circle, Square, Crescent, Trapezoid, Dragon };
    public enum States { StartState, Empty, Explode, Create, Rotate, Velo, Variable, Follow, Position, VeloVector, Radio, SkillMod, DmgVelo, FaceVeloVec, Fracture, IgnoreColi, SetAlpha, SetColor, ConvertToSolidMaterial, Destroy };
	public enum SkillModScalingType {forceEff, forceTime};
    public enum ConstantOrPercent { constant, percent };
    public enum DirectionalDamage { explosion, implosion, towardsFace, specifiedDir};
    public enum CastType { Normal, Melee, SelfCast };
    public enum MeleeCastType { kick, basicPunch }
    public enum EnergyLimitType { None, Constant, PercentOfCasterMax};
    public enum Stats { Str, Const, Agi, Wis, Int, Dex, Char }
    public enum fileLocationType { Spells, Characters, Trees, Xml, NPCs, TagManagers }
    public enum DNAType { Player, NPC }

    //Slot structs
    public enum SpellInfoDataType {GoNext, Energy, Intelligence, Wisdom, Time, Angle, Velocity, Speed, Stability, Density, Mass, BasicColiType, SpellColiType ,Altitude, Variable, Pos, Radio, PosX, PosY};
    public enum CastOnCharge { None, Hold, CastNoRepeat, CastWithRepeat }
    public enum VarType { Float, Bool, SpellType, String, BasicColiType, Vector2 };
    public enum StateVarType { Float, Bool, String, BasicColiType, RelativeType, ExistingSpells, MatType, Shape, Rotate, ModOPType, ModVarTime, RelativeLaunchType, IgnoreXY, SkillMod, SkillModType, constantOrPercent, damageDirectionType, castType, energyLimitType, Button, ColiMetaTypes, RadioOption, InteractionType, CastOnCharge, MeleeCastType }; //used for state machine
    public enum SlotDataType { boolType, floatType, stringType, operatorType };
    public enum OperatorType { equalTo, lessThan, greaterThan};
    public enum RelativeType { Normal, SpellLaunched, StateStart, World };
    public enum SlotStructRestrictions { none, noRelative, onlyEquals, all };
    public enum RelativeLaunchType { Normal, World };
    public enum IgnoreXY { None, IgnoreX, IgnoreY };
    public enum LevelPkgType { Ability, Stat, Spell, Skill }
    public enum RadioStateType { OnExplosion, Emit }
    public enum InteractionType { None, Caster_Knockback, Caster_Damage, Caster_SkillMod, Avatar_Collision }

    //related to state choices
    public enum statechoice_face { faceAngle, continousRotate };
    public enum statechoice_modVar { mod, set };
    public enum statechoice_modVarTime { perSecond, perCycle };
    public enum ColiMetaType { Basic, Material };

    public enum AbilityTriggerType { Pressed, Released, Held, Update };

    public enum AbilityType { dash , jump, backDash, climb, zoom, moveLeft, moveRight, tradeIn, aimUp, aimDown, fly, invis, superJump, block}

    public enum MenuType { statusEffects, debugControls, spells, level, stats };
    public enum AbilityResourceRequestType { stanima, energy, hp, exp }

    public enum SpriteStyle { Solid, Gas, Wave}

    public static readonly float SPELL_DIRECT_HIT_DMG_MODIFIER = 1.2f; //20% dmg increase, and will deal a full second worth of damage
    public static readonly bool DESTRUCTIBLE_TERRAIN = true;
    public static readonly float GROUND_DEFENSE_SCORE = 1;//13;
    public static readonly float GROUND_MAX_HP = 200;  //max dmg it can take before fracturing
    public static readonly float RETICLE_DISTANCE = 2f; //This is temp, as it will be replaced by something using int
    public static readonly float PHYSICALSPELL_EXPLOSION_REDUCTION = 1000f; //An explosion still appears from physical stuff Action1, however it is /N weaker
    
    public static readonly int HAIRLINE_MAX_SPLITS = 4;
    public static readonly float HAIRLINE_SCALE_PER_ENERGY = .2f;
    public static readonly Vector2 HAIRLINE_SIZE_RATIO = new Vector2(1, 4);
    public static readonly int HAIRLINE_MAX_DEGREE_VARIATION = 45;
    public static readonly int HAIRLINE_MIN_ENERGY = 5;  //less than 5 isnt allowed to split

    public static readonly List<string> STARTING_LOCATIONS = new List<string>() { "Wedge", "The Hole", "The Mines", "Origins", "Witches", "Plains", "Lag City", "Where the sidewalk ends", "Desert Biome", "Dungeon", "DungeonLarge"};

    #region Altar
    public static readonly float ALTAR_SPELL_ENERGY_GIFT = 1; //N energy per second given to spells in range
    public static readonly float ALTAR_PCS_ENERGY_GIFT = 1;  //N energy  per second given to pcs in range
    public static readonly float ALTAR_PCS_HP_GIFT = 1;      //N hp      per second given to pcs in range
    public static readonly float ALTAR_PCS_STANIMA_GIFT = 1; //N stanima per second given to pcs in range
    #endregion

    #region world
    public static readonly float MINTHRESHOLD_FOR_HEAT_PRODUCTION = 10f; //a flammable must have at least this much energy to give radiantHeat energy, prevents summoning spells and them burning out


    public static readonly float WORLD_FIRE_CONSUMPTION_RATE = .1f; //multiplied by heat of fire to consume that much energy
    public static readonly float FIRE_SCALE_SIZE = 4; // inverse, the size fire scales at in terms of heats radius

    public static readonly float ENVIROMENT_SIZE = 10f;
    public static readonly Vector2 ENVRIOMENT_SIZE_VECTOR = new Vector2(10, 10);
    public static readonly float ENVIROMENT_HEAT_STORED_TO_TEMP_INCREASE = .1f; //how much one unit of heat stored (from energy) increases the temperature by
    public static readonly float ENVIROMENT_TIME_TO_DESIRED_HEAT = 30f; //takes 30 seconds to go from current to desired
    public static readonly float ENVIROMENT_TIME_TO_CORRECT_DESIRED_HEAT = 10f; //It takes 10 seconds for the desired heat to go to the current heat
    public static readonly float ENVIROMENT_DESIRED_HEAT_MATCH_NEIGHBOR_TIME = 45f; //It takes n seconds for this desired heat to tend towards neighbors current heat
    public static readonly float ENVIROMENT_HEAT_DIFF_TRIGGER_TRANSFER = 1f;

    public static readonly float ENVIROMENT_START_HEAT = 24f;
    public static readonly float ENVIROMENT_TRANSFER_HEAT_NEIGHBORS = .1f;
    public static readonly float ENVIROMENT_TRANSFER_HEAT_MIN_NEIGHBORS = 1f;

    public static readonly float ENVIROMENT_START_MOISTURE = 100f;
    public static readonly float ENVIROMENT_MAX_MOISTURE = 1000f;
    public static readonly float ENVIROMENT_TRANSFER_MOISTURE_NEIGHBORS = 2f;
    public static readonly float ENVIROMENT_TRANSFER_MOISTURE_MIN_NEIGHBORS = 2f;

    public static readonly float ENVIROMENT_START_MANA = 100f;
    public static readonly float ENVIROMENT_MAX_MANA = 1000f;
    public static readonly float ENVIROMENT_TRANSFER_MANA_NEIGHBORS = 2f;
    public static readonly float ENVIROMENT_TRANSFER_MANA_MIN_NEIGHBORS = 1f;

    public static readonly float ENVIROMENT_START_NUTRIENT = 100f;
    public static readonly float ENVIROMENT_MAX_NUTRIENT = 1000f;
    public static readonly float ENVIROMENT_TRANSFER_NUTRIENT_NEIGHBORS = 2f;
    public static readonly float ENVIROMENT_TRANSFER_NUTRIENT_MIN_NEIGHBORS = 1f;

    public static readonly float ENVIROMENT_TRANSFER_TO_PLANT_MAX_CAP = .05f; //means the max amount, like 100, means can only give 5 to a plant

    public static readonly float PLANT_ENERGY_TO_GROWTH = .05f;
    public static readonly float PLANT_PROD_INCREASE_PER_ENERGY = .1f;

    public static readonly float PLANT_NUTRIENTS_TO_SPLIT = 10f;
    public static readonly float PLANT_WATER_TO_SPLIT = 10f;
    public static readonly float PLANT_MANA_TO_SPLIT = 10f;

    public static readonly float PLANT_SPLIT_BANK_TO_ACTIVATE = 100f; //amount of desire to grow one split
    public static readonly float PLANT_STORAGE_PER_LEVEL = 10;
    #endregion

    #region Mechanics
    public static bool AUDIO = true;  //Cheap implementation
    public static readonly float GRAVITY_SPELL_PHYSICAL = .7f;  // %
    public static readonly float GRAVITY_SPELL_ENERGY =    1f;

    public static readonly float SPELL_ROTATION_RADS_PER_SEC = Mathf.PI/2;
    public static readonly float ABILITY_TIME_HELD_TILL_KEY_LOCK = 1.5f;

    public static readonly float CONCUSS_TIME_RESET = 2f; //time before start recovering from concuss
    public static readonly float CONCUSS_THRESHOLD  = 2f;
    public static readonly float CONCUSS_START_COUNT  = 3f; //max number of stacked concussions, 1 sec per
    public static readonly float CONCUSS_MAX_COUNT = 6f; //max number of stacked concussions, 1 sec per
    public static readonly float CONCUSS_CLEANSE_STACK_TIME = 1f; //How long it takes to cleanse one stack of concuss, 1 second
    #endregion

    #region energy / spell
    public static readonly Vector2 SPELL_MAXMIN_SET_SCALE = new Vector2(.1f, 5); //clamped values for set scale, not allowed beyond and below
    public static readonly float HAIRLINE_SPEED_MIN = 4f; //Spells going under this speed won't cause hairline fractures. The reason for this is to prevent continous checking when rolling on sm
    public static readonly float HAIRLINE_COOLDOWN  = .16f; //how often can make a hairline, acnt do it every second else a mess

    public static readonly float EXPLOSIVE_FRACTURE_SPEED = 15f;
    public static readonly float SPRITE_SIZE = .2f; //not implemented everywhere yet, search "sprite size"
    public static readonly float DENSITY_EFFECT_MIN = .66f;

    public static readonly float SPELL_MAX_ENERGY = 9999;
    public static readonly float ENERGY_PER_PARTICLE = 2.5f;
    public static readonly float MASS_PER_AREA_MODIFIER = 12.5f; //increase the mass per volume for a spell
    
    public static readonly float ENERGY_SCALE_SIZE = .01f;//why does changing this cause it to shrink faster
    //public static readonly float ENERGY_LOSS_PERCENT = .05f;
    public static readonly float ENERGY_UPKEEP_LOSS_MINIMUIM = .25f; //spells lose at min .25 per scond    
    public static readonly float SPRITE_START_SCALE = .2f; //using 100x100 sprites

    public static readonly float EXPLOSION_UPKEEP_ENERGY_LOSS_CONSTANT = 1f;
    public static readonly float EXPLOSION_DENSITY = .01f;
    public static readonly float EXPLOSION_RADIO_DENSITY = .001f;
    public static readonly float ENERGYEXPLOSION_DENSITY = .1f;
    public static readonly float EXPLOSION_FIZZLE_TIME_PER_ENERGY = .01f;
    public static readonly Vector2 EXPLOSION_DISSIPATE_VISUAL_TIME_KM = new Vector2(3, .01f); //const,multiplier*energy  how long it takes for explosions to VISUALLY dissapate, not same as energy
    public static readonly Vector2 EXPLOSION_DISSIPATE_ENERGY_KM = new Vector2(.5f, -.001f);  //percent per second the explosions energy dissipates, Constant(percent of total) and energy multiplier
    public static readonly float EXPLOSION_DISSIPATE_MIN_RATE = .25f;  //if explosion has alot of energy, it should still disspate at most this rate, no slower
    public static readonly float SELFCAST_EXPLOSION_DENSITY_DEFAULT = .5f;  //multiples EXPLOSION_DENSITY

    public static readonly float STABILITY_DECAY_THRESHOLD = .3f; //at 30% begins to decay
    public static readonly float STABILITY_DECAY_MAX_RATE = .05f;  //decays at random between 0 and 5%
    public static readonly float STABILITY_RECOVER_RATE = .02f;
    public static readonly float STABILITY_RECOVER_THRESHOLD = .75f;
    public static readonly float CORES_TOUCHING_DESTABLE_MULT = 2f;

    public static readonly float EXPLOSIVEFRACTURE_ENERGY_PERCENT = .25f;  //Amount of energy that is put into the fracture

    public static readonly float ENERGY_DESTABLE_GLOW_THRESHOLD = .4f;
    public static readonly float DESTAB_MULTIPLIER = 1.75f;  //destabilization formula multiplies by this number cause too weak on its own

    //public static readonly float ENERGYFORM_ABSORBPTION_RATE = .4f; //amount of energy a type of EnergyForm.Energy will attempt to absorb each second from colliding energy (currentEnergy*this)

    public static readonly float SPELLFORM_PHYSICAL_MOMENTUEM_DAMAGE_MULTIPLIER = 50;
    public static readonly float SOLIDMATERIAL_PHYSICAL_MOMENTUEM_DAMAGE_MULTIPLIER = 2f;  //ground weighs a hella lot, not yet fully porportianal with spell since area of squares is ye ta thing

    public static readonly float CASTER_IMMUNITY = 1.5f; //Time until a charing spell becomes a mature spell, time caster is immune to a spell,(warning physical will go through ground at this point)

    public static readonly float HEAT_DISPERSION_RATE = 3f;

    public static readonly float DESTABILITY_PER_VELO = .8f;  //increase destability by N% per velo  (1 = 100% per point of velo)
    public static readonly float DESTABILITY_TERRAIN_MODIFIER = 2; //Nx destability from terrain types  (read n times)
    public static readonly float DESTABILITY_TERRAIN_PERVELO_MODIFIER = 4; //Nx prevents very fast spells from going through too far, * per point of velo
    public static readonly float DESTABILITY_EXPLOSION_MODIFIER = 2; //Nx destability from explosion types  (read n times)

    public static readonly float CORE_SIZE = .25f; //N * the scale of the normal spell 

    public static readonly float SPELL_SSLAYER_TIMER = 1.5f;  //How long a spell summoned by a spell will be immune to it's parent after launch

    public static readonly bool SPELL_ART_PIXEL_MODE = true;
    #region Temperatures
    public static readonly float SPELL_ENERGY_START_TEMPERATURE = 200f;
    public static readonly float SPELL_ENERGY_TEMP_PER_ENERGY = 1f;

    public static readonly float SPELL_ROCK_START_TEMP = 22f;

    public static readonly float SPELL_NATURE_START_TEMPERATURE = 22f;
    public static readonly float SPELL_NATURE_IGNITION_TEMPERATURE = 70f;

    public static readonly float SPELL_FORCE_START_TEMPERATURE = 22f;

    public static readonly float SPELL_FIRE_START_TEMPERATURE = 200f;
    public static readonly float SPELL_FIRE_TEMP_PER_ENERGY = 3f;

    public static readonly float SPELL_LIGHTING_START_TEMPERATURE = 300f;
    public static readonly float SPELL_LIGHTING_TEMP_PER_ENERGY = 2f;

    public static readonly float SPELL_CHARISMA_START_TEMPERATURE = 22f;

    public static readonly float SPELL_ICE_START_TEMPERATURE = 22f;
    public static readonly float SPELL_ICE_TEMP_PER_ENERGY = -1f;
    #endregion
    #endregion

    //body controller relevant GV
    #region playerStats
    public static readonly Vector2 PLAYER_SCALE_BODY = new Vector2(.3f,.3f);
    public static readonly Vector2 PLAYER_SCALE_UPPERBODY = new Vector2(.7f,.65f);
    public static readonly Vector2 PLAYER_SCALE_LOWERBODY = new Vector2(.9f,1);

    public static readonly Vector2 PLAYER_MAX_SPEED = new Vector2(10, 20);
    public static readonly float PLAYER_SPEED_CREATE_EXPLOSION = 22;
    public static readonly float PLAYER_SPEED_CREATE_EXPLOSION_EK_MULT = .001f; //so a body going 22m/s at 50kg is 1100. Which is way too much energy for an explosion, so /100
    public static readonly float PLAYER_SPEED_CREATE_EXPLOSION_REFRESH_RATE = .16f; //so it can happen every X seconds, not every colision which would be diaster
    public static readonly float PLAYER_REVIVAL_HP_PERC = .33f;

    public static readonly bool DEATH_PERMANENT_ENABLED = false;
	public static readonly float SELF_CAST_ANIM = 300; //so, this controls the animation of self cast power, when spell has 300 energy your maxed out in how powerful it looks.

    public static readonly float PLAYER_START_WEIGHT = 54f; //kg, temp until a sys around const & str is made
    public static readonly float PLAYER_HEADSHOT_BONUS = 1.2f; //2x dmg and concuss

    public static readonly float ENERGY_TO_SPELLVELO = 10f; //For every point in spellInfo.velocityEnergyTransfer(int) allows you to transfer this much energy

    public static readonly float RESISTANCE_CONSTANT = 20f;

    //public static readonly float STAMINA_COST = 0.05f; //cost per force applied (so 1f costs 1e), also used to recover when getting up (using force n weight)

    //Skill mods
    public static readonly float SPELL_MIN_ENERGY_BEFORE_CHARGE_SKILLMODS = .5f; //else spells may cause issues for adding values so small it counts as 0
    public static readonly float SPELL_MAX_PERCENT_CONSUMED_FOR_SKILLMOD = .9f; //max percent of total incoming energy allowed to be distributed to skill mods

    /// ///////////////////////////////////////////////////////////              SCALE  MIN   MAX    TYPE
    public static readonly AssStorage as_runForce = new AssStorage(200f, 280f, 1000, GV.HorzAsym.MinToMax);

    // //////////////////////////////////////////////////////////////   COMPLEX ASS
    /// ///////////////////////////////////////////////////////////              @control   MIN   MAX   PERCENT   TYPE
    public static readonly AssStorage as_energyToSpellChargeRate      = new AssStorage(100 ,    1f,  100,  50,  GV.HorzAsym.MinToMax);
    public static readonly AssStorage as_energyChargeLossFromMaterial = new AssStorage(50,    .20f,    1,  80,  GV.HorzAsym.MaxToMin);
    public static readonly AssStorage as_stanimaTransferToSpell       = new AssStorage(100,    1f,  100,  50,  GV.HorzAsym.MinToMax);
    public static readonly AssStorage as_stanimaToEnergyRatio         = new AssStorage(100,     1.2f,     2.5f,  50,  GV.HorzAsym.MinToMax);
	public static readonly AssStorage as_meleeDecayCoeff              = new AssStorage(50,     1,  2,  80,	GV.HorzAsym.MaxToMin);   //multiplier for how much faster it loses energy if melee
    public static readonly AssStorage as_energyTransferEfficency      = new AssStorage(50 ,  .5f,    1.2f,  80,  GV.HorzAsym.MaxToMin);  //energy loss from different material types
    public static readonly AssStorage as_energyToForce                = new AssStorage(50,    100,  500f,  80,  GV.HorzAsym.MinToMax); //
    public static readonly AssStorage as_skillModChargeEff            = new AssStorage(10,   .85f,    1f,  90,  GV.HorzAsym.MinToMax);
    public static readonly AssStorage as_skillModChargeRate           = new AssStorage(50 ,   .5f,   250,  50,  GV.HorzAsym.MinToMax);  //how much energy can be charged into skill mods
    //public static readonly AssStorage as_meleeRangeEff                = new AssStorage(50,   .05f,   .2f,  50,  GV.HorzAsym.MaxToMin);  //energy loss when charging a melee spell based on range calculations
    public static readonly AssStorage as_airDragTime                  = new AssStorage(50,     1f,    2f,  50,  GV.HorzAsym.MaxToMin);  //time in air before lose mobility
    public static readonly AssStorage as_concussRecoverSpeed          = new AssStorage(50,    .5f,     2,  50,  GV.HorzAsym.MinToMax);

    public static readonly AssStorage spellSize = new AssStorage(30, 1f, 8, 10, GV.HorzAsym.MinToMax);

    //public static readonly AssStorage as_maximumHp = new AssStorage(50, 100, 2500, HorzAsym.MinToMax);
    //public static readonly AssStorage as_maximumMana = new AssStorage(50, 100, 5000, HorzAsym.MinToMax);
    //public static readonly AssStorage as_maximumStanima = new AssStorage(50, 100, 1250, HorzAsym.MinToMax);

    ///ABILITIES
    //RUN                                                        @cntrl  MIN   MAX   PERCENT   TYPE
    //public static readonly AssStorage as_runForce = new AssStorage(20,   55f,  550,       80     ,GV.HorzAsym.MinToMax);


	//spell decay stuff
	public static readonly float EXPECTED_DMG_PER_ENERGY = 2f;
	public static readonly AssStorage as_flatSpellEnergyUpkeep = new AssStorage     (80,  .02f,  .75f, 80,	GV.HorzAsym.MaxToMin);  //min energy loss in a spell per second
	public static readonly AssStorage as_percentSpellEnergyUpkeep = new AssStorage  (100, 0.01f, 0.03f, 80, GV.HorzAsym.MaxToMin);
	//public static readonly AssStorage as_veloStabilityDecay = new AssStorage        (100,  1f,   .8,	50,	 GV.HorzAsym.MinToMax);  then your punches will not go the ranges the player specifys 
	public static readonly AssStorage as_stabDecayRate = new AssStorage             (50,  .02f,  .07f,	90,	 GV.HorzAsym.MaxToMin);
	public static readonly AssStorage as_stabRecovRate = new AssStorage             (50,  .02f,  .07f,	90,	 GV.HorzAsym.MinToMax);
	public static readonly float MELEE_VELOCITY_DEDUCTION = 3f;
    public static readonly float MELEE_RANGE_CONSTANT = 2f;  //see in the calculation to understand
    public static readonly float MELEE_RANGE_MAX = 3f;  //Cannot be above 3m range
    public static readonly float MELEE_RANGE_EFF_LOSS_PER_METER = .2f;  //flat eff loss per meter

	public static readonly float PRCNT_MOV_SPD_IS_SLIDE_STOP = 0.7f;

    public static readonly float JUMP_FORCE_MIN = 1000f;
	public static readonly float JUMP_FORCE_MAX = 2500f;
    public static readonly float JUMP_FORCE_SCALE = 100f;

	public static readonly float TIME_BETWEEN_CROUCH = 0.5f;

	public static readonly float LAND_FORCE_TO_FALL_ANIM = 300f; // the force at which player needs to hit ground to trigger land animation
	public static readonly float MIN_TIME_BETWEEN_JUMP_ANIM = 0.2f;
	public static readonly float MIN_TIME_BETWEEN_LAND_ANIM = 0.2f;
	public static readonly float VELO_FALL_TRIGG_ANIM = 4f;

    /*public static readonly float RETICLE_SPIN_SPD_MIN = 37f;
	public static readonly float RETICLE_SPIN_SPD_MAX = 90f;
    public static readonly float RETICLE_SPIN_SPD_SCALE = 80f;

    public static readonly float RETICLE_ANGLE_MIN = 75f;
	public static readonly float RETICLE_ANGLE_MAX = 90f;
	public static readonly float RETICLE_ANGLE_MEAN = 50f; 
    //public static readonly float MAX_RETICLE_ANGLE_SCALE = 1f;*/

    public static readonly float JUMP_FORCE_DURATION_SCALE = 140f;
    public static readonly float JUMP_FORCE_DURATION_MIN = 0.65f;
	public static readonly float JUMP_FORCE_DURATION_MAX = 3f;
    
    //public static readonly float GRAB_TO_JUMP_COOLDOWN_BASE = 0.05f;
    //public static readonly float GRAB_TO_JUMP_COOLDOWN_SCALE = 0.01f;

    //public static readonly float GRAB_STRENGTH_BASE = 800f;
    //public static readonly float GRAB_STRENGTH_SCALE = 15f;
    //public static readonly float MAX_UP_VELO_FROM_GRAB_BASE = 0.3f;

	public static readonly AssStorage as_ClimbStrength = new AssStorage(100, 800, 8000, 33, GV.HorzAsym.MinToMax);

	//public static readonly float GRAB_FORCE_SCALE = 200f;
	//public static readonly float GRAB_FORCE_MIN = 800f;
	//public static readonly float GRAB_FORCE_MAX = 3200f;

	//public static readonly float GRAB_VELO_MIN = 0.3f;
	//public static readonly float GRAB_VELO_MAX = 5.0f;
	//public static readonly float GRAB_VELO_SCALE = 200f;

	public static readonly AssStorage as_climbSpeed = new AssStorage(100, 0, 20, 33, GV.HorzAsym.MinToMax); 

	public static readonly AssStorage as_punchAnimSpeed = new AssStorage (50, 0.2f, 1, 80, GV.HorzAsym.MaxToMin);
	public static readonly AssStorage as_spellCastAnimSpeed = new AssStorage (200, 0.5f, 1.3f, 80, GV.HorzAsym.MaxToMin);


	public static readonly float MIN_EFFECT_DURATION = 0.5f;
	public static readonly float MIN_EFFECT_POWER = 1f;
	public static readonly bool EFFECTS_CAN_DECREASE_STATS = true;

    //public static readonly float MAX_UP_VEL_GRAB_SCALE = 0.02f;

    public static readonly float EXPERIENCE_GAIN_PER_SECOND = 1;
    public static readonly float EXPERIENCE_BANK_INTREST_PER_SECOND = .01f; //percent increase per second
    public static readonly int EXPERIENCE_PER_LEVEL = 10;

    public static readonly float CONCUSION_RECOVER_RATE = 2f; //multipled by the persons concs resistance
    public static float CONCUSION_KNOCKOUT_THRESHOLD = 100f;    //set to non-constant because changed for testing
    public static readonly float CONCUSION_RECOVERY_THRESHOLD = 70f;
    public static readonly float ENERGY_TO_CONCUSIVE_FORCE = 1f; //points of energy to concusive, concuss

    public static readonly float HEAT_RECOVER_RATE = 1f;
    public static readonly float PLAYER_TEMPERATURE_START = 37f;
    #endregion

    #region LevelUp
    public static readonly float BASE_EXP_CONSUME_PER_SECOND = GV.EXPERIENCE_PER_LEVEL / 3;
    public static readonly float BASE_EXP_CONSUME_INCREASE_PER_SECOND = GV.EXPERIENCE_PER_LEVEL / 3;
    public static readonly float ACTIVATE_TIER_1_POWER_ANIMATION_CONSUMPTION_RATE = GV.EXPERIENCE_PER_LEVEL;  //When you get a level per second, you unlock second animation, Tier 0 always active
    public static readonly float ACTIVATE_TIER_2_POWER_ANIMATION_CONSUMPTION_RATE = GV.EXPERIENCE_PER_LEVEL * 2; //When you get 2 level per second, you unlock third animation
    public static readonly float ACTIVATE_TIER_3_POWER_ANIMATION_CONSUMPTION_RATE = GV.EXPERIENCE_PER_LEVEL * 4; //When you get 4 level per second, you unlock fourth animation
    public static readonly float REMAINING_LEVEL_STACK_BEFORE_LOOP = 5f;                                               //Last  N levels loop on level up stack, we can make it a player selected variable 

    public static readonly float TIER_1_MAX_CHARGE_TIME = 8f; //after charging tier one for 8 seconds, animation stops growing.
    public static readonly Vector2 TIER_1_LIFETIME_GR = new Vector2(.3f, 1.8f);
    public static readonly Vector2 TIER_1_RADIUS_GR = new Vector2(1, 13);
    public static readonly Vector2 TIER_1_EMISSION_GR = new Vector2(10, 120);
    public static readonly Vector2 TIER_1_MINSIZE_GR = new Vector2(.1f, .2f);
    public static readonly Vector2 TIER_1_MAXSIZE_GR = new Vector2(.2f, .5f);
    #endregion

    #region FolderPaths
    public static readonly string SKINS_BASE_FOLDER_FULL = "Assets/Hasdra/Resources/Textures/PlayerSkins";
    public static readonly string SKINS_BASE_FOLDER_TRUNC_REZ = "Textures/PlayerSkins";
    #endregion

    #region D2d
    public static readonly float GROUND_DESTROY_MIN_PIXEL = 30f; //anything less than N pixels is destroyed, will figure out something a lil better for that
    public static readonly float PERCENT_ALPHA_REMAINING_BEFORE_DESTROY = .3f;
    public static readonly float AMOUNT_OF_PIXELS_FOR_90_HOVER = 2000;//200;  //at X pixels, ground has 90% hovering on split, and then increases by half that amount 
    public static readonly float STD_SOLIDMATERIAL_RESOLUTION = 6000; //it will be at max 6000 pixels, in the future, we can calculate the area of the sprite and set the appriopriate resolution in Terra
    
    public static readonly float GROUND_EVENTAULFRACTURE_CHANCE = .05f; //percent chance kinematic ground fractures over time
    public static readonly float GROUND_EVENTAULFRACTURE_TIME = 10f; //Time it takes for the eventaul fracture to occur
    public static readonly Color GROUND_EVENTAULFRACTURE_COLOR = Color.red; //Color it changes to over time
    public static readonly Vector3 GROUND_EVENTAULFRACTURE_SHAKE_INTENSITY = new Vector3(.3f,0,0); //How much it shakes            GOAL:over time, will Increase to this shake intensity per second from 0
    
    //THIS SHOULD BE 10 000, set to less in meantime while area shit is worked out
    public static readonly float PIXELS_PER_UNITAREA = 2500f;  //this is a actaul true constant related to unity editor (can be changed, but not here) amount of pixels that creates a 1x1 unit square area, useful for calculating mass
    public static readonly float SOLIDMATERIAL_PIXEL_OPTIMIZATION = 3f; //by default a 1x1 piece will be optimized N times.
    public static readonly float SOLIDMATERIAL_MINPIXEL_OPTIMIZATION = 16f; //A piece cannot be shrunk once it reaches this value

    public static readonly float SPELL_PIXEL_OPTIMZATION = 2f; //optimize a spell N times 
    #endregion

    #region UI
    public static readonly float DYNAMIC_TEXT_LIFESPAN  = 2f;  //dynamic text will refresh its lifespan if it recieves a new value, when reached it locks, and fades
    public static readonly float DYNAMIC_TEXT_FADE_TIME = .75f;   //time for the text to fade
    public static readonly float DYNAMIC_TEXT_SCALE_PER_POINT = .01f; //base scale grows by that much per point
    public static readonly float DYNAMIC_TEXT_SCALE_FROM_ADD = 1.1f; //grows by this scale when a value is added oover the GROW_CYCLE
    public static readonly float DYNAMIC_TEXT_DROP_ACCEL = .1f; //acceleration speed for the text to drop when fading
    public static readonly float DYNAMIC_TEXT_DROP_HORZ_SPEED = .6f; //speed it shoots away from center when fade starts
    public static readonly float DYNAMIC_TEXT_DROP_VERT_SPEED = 1.5f; //speed it shoots vertically when fade starts
    public static readonly float NUMERIC_DISPLAY_MAX_SCALE = 1.3f;
    public static readonly float NUMERIC_DISPLAY_MIN_SCALE = .5f;
    public static readonly Vector2 NUMERIC_DISPLAY_OFFSET_RANGE = new Vector2(.6f,.6f); //[-x,x],[-y,y]
    public static readonly float NUMERIC_DISPLAY_SPACE = .6f; //space between placements
    public static readonly Vector4 UI_STATE_EXPANDED_LRTB = new Vector4(-325,-158,-289,-88);
    public static readonly float TAG_MIN_COLOR = 50; //min color of r+g+b
    public static readonly float TAG_UNSELECTED_TRANSPARENCY = .5f; //color of tag button when not selected
    #endregion

    #region Enviroment
    public static readonly Vector2 BASIC_GROUND_SIZE = new Vector2(10, 4);
    public static readonly float GROUND_ENERGY_PER_UNIT_AREA = 100f;
    public static readonly Vector2 CREATURE_VELO_LOSS = new Vector2(.5f,.25f);  //max % loss per second
    //Habitat
    public static readonly float HABITAT_UPDATE_TIME;
    #endregion

    #region debug
    public static bool ND_ON = false;
    #endregion

    #region Animations
    public static readonly float SPOTLIGHT_MIN_ENERGY = 80f; //at N spotlights will begin
    public static readonly float SPOTLIGHT_ENERGY_PER_BEAM = 10f; //Every N energy creates one beam 
    public static readonly float SPOTLIGHT_MAX_BEAMS       =  8f; //Max light beams from spotlight animation

    // curEnergy/N is the powerlevel, once updated, it requires a difference of TWO powerlevels before updating again, this prevents thrashing.
    public static readonly float GATHERDEBRIS_SCALE_INTERVAL = .25f; //A difference in N scale will cause an update to the particleSys (lifespan,emission)
    public static readonly float GATHERDEBRIS_INITIAL_ENERGY = 10f;  //amount of energy before system activates
    public static readonly float GATHERDEBRIS_LIFESPAN_PER_SCALE = 1.25f; //for every scale of spell, the lifespan of the particles last that much longer
    public static readonly float GATHERDEBRIS_EMIT_PER_ENERGY = 2f; //for every scale of spell, the lifespan of the particles last that much longer

    public static readonly float UNSTABLE_LEVELS = 4;  //how many levels of unstability exist that will cause animations, number of animations between GV.STABILITY_DECAY_THRESHOLD and 0, split evenly.
    public static readonly Vector2 UNSTABLE_SIZRANGE = new Vector2(.9f, 1.2f); //It will have a random scaled range between these values
    #endregion

    #region Player Level Design
    public static readonly float PLAYERLEVELDESIGN_SLOT_HEIGHT = 90;
    public static readonly float PLAYERLEVELDESIGN_PREVIEW_COUNTDOWN = 3;
    public static readonly float PLAYERLEVELDESIGN_SCROLL_SPEED = 100;
    #endregion

    #region StateSSVars
    public static readonly float SS_POSITION_ACCEPTABLE_DISTANCE = .1f;  //within n units is fine, doesnt apply velo

    public static readonly Vector2 SSMINOR_SIZE = new Vector2(145, 60);
    public static readonly Vector2 STATESLOT_MIN_SIZE = new Vector2(444, 166);

    #endregion

    #region Camera
    public static readonly float CAMERA_OFFSET_Y = .9f;
    #endregion

    public static string[] spellLayers = new string[] {"Spell","SpellChargingSpell","SpellFreshSpell","OppChargingSpell","OppSpellChargingSpell","OppFreshSpell","OppSpellFreshSpell"};
    public static readonly string[] STAT_ALL_NAMES = new string[] { "Str", "Const", "Agi", "Wis", "Int", "Dex", "Char" };

    public static Transform GetTopMostParent(Transform childObj)
    {
        Transform topLayer = childObj;
        while (topLayer.parent != null)
            topLayer = topLayer.parent;
        return topLayer;
    }

    public static List<Transform> GetAllParents(Transform childObj)
    {
        List<Transform> childParents = new List<Transform>();
        Transform topLayer = childObj;
        while (topLayer.parent != null)
        {
            topLayer = topLayer.parent;
            childParents.Add(topLayer);
        }
        return childParents;
    }

    public static void DeleteAllChildren(Transform _parent)
    {
        foreach (Transform _child in _parent)
        {
            if (_child != _parent)
                Destroy(_child.gameObject);
        }
    }

    public static float GetAngleOfLineBetweenTwoPoints(Vector2 p1, Vector2 p2)
    { 
        float xDiff = p2.x - p1.x;
        float yDiff = p2.y - p1.y;
        return Mathf.Atan2(yDiff, xDiff) * (180 / Mathf.PI);
    } 
   // - See more at: http://wikicode.wikidot.com/get-angle-of-line-between-two-points#sthash.69QBOx3c.dpuf
    //TOADD function to get all children transforms (branched out deep)

    /// <summary>
    /// all inputfeild objects must have unique names
    /// </summary>
    public static Dictionary<string, InputField> ExtractAllInputFeilds(GameObject go)
    {
        Dictionary<string, InputField> toReturn = new Dictionary<string, InputField>();
        foreach (InputField inputfeild in go.GetComponentsInChildren<InputField>())
        {            
            toReturn.Add(inputfeild.gameObject.name, inputfeild);
        }
        return toReturn;

    }

    public static SpellStorage GetLiveSpell(string spellName)
    {
        Debug.Log("1: " + spellName);
        return StaticReferences.mainScriptsGO.GetComponent<LiveSpellDict>().GetSpell(spellName);
    }

    public static GV.SpellForms GetSpellFormByMaterialType(GV.MaterialType materialType)
    {
        switch (materialType)
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
                return GV.SpellForms.Energy;
            case GV.MaterialType.Rock:
            case GV.MaterialType.Ice:
            case GV.MaterialType.Higgs:
                return GV.SpellForms.Physical;
            default:
                Debug.LogError("MaterialType: " + materialType.ToString() + " not recognized, defaulted to energy, please add");
                break;
        }
        return GV.SpellForms.Energy;
    }

    public static void Destroyer(GameObject toDestroy)
    {
        StaticReferences.mainScriptsGO.GetComponent<ObjectDestroyer>().CustomDestroyObject(toDestroy);
        if (toDestroy.GetComponent<Spell>())
            toDestroy.GetComponent<Spell>().spellBridge.parentCaster = null;
    }

    public static Vector3 CircleScale(float energy, float density, GV.MaterialType materialType)
    {
        if (energy>= 0)
        {
            float radius = Mathf.Sqrt(((1 / MaterialDict.Instance.GetMaterialInfo(materialType).density) * (1 / density) * GV.ENERGY_SCALE_SIZE * energy) / Mathf.PI);
            radius = (float)System.Math.Round(radius, 2);
            return new Vector3(radius + GV.SPRITE_START_SCALE, radius + GV.SPRITE_START_SCALE,1);
        }
        return new Vector3(.1f,.1f,1);

    }

    public static Vector3 SquareScale(float energy, float density, GV.MaterialType materialType)
    {
        if (energy >= 0)
        {
            float scale = Mathf.Sqrt(((1 / MaterialDict.Instance.GetMaterialInfo(materialType).density) * (1 / density) * GV.ENERGY_SCALE_SIZE * energy));
            return new Vector3(scale + GV.SPRITE_START_SCALE, scale + GV.SPRITE_START_SCALE, 1);
        }
        return new Vector3(.1f, .1f, 1);

    }

    public static Vector2 GetRandomNormalizedVector()
    {
        Vector2 toRet = new Vector2(0,0);
        while(toRet.x == 0 && toRet.y == 0)
           toRet = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        return toRet.normalized;
    }

    public static Transform[] GetAllChildrenWithTag(Transform parent, string tag)
    {
        try
        {
            return parent.Cast<Transform>().Where(c => c.gameObject.tag == tag).ToArray();
        }
        catch
        {
            Debug.Log("obsv");
            return null;
        }
    }

    public static string[] GetAnimationString(string creatureName, float tier)
    {
        return PowerUpAnimationLibrary.GetPowerUpAnimations(creatureName, tier);
    }

    public static string ReduceFilepathToResoureFolderBase(string filepath, bool withFileExt = false)
    {
        //so messy, sure it can be done so much better...
        string extension = System.IO.Path.GetExtension(filepath);
        string result = filepath.Substring(0, filepath.Length - extension.Length);
        int index = result.IndexOf("Resources/");
        return result.Substring(index + 10);        
    }

    public static string[] ReduceFilepathToResoureFolderBase(string[] filepath, bool withFileExt = false)
    {
        //so messy, sure it can be done so much better...
        string[] toRet = new string[filepath.Length];
        for(int i = 0; i < filepath.Length; ++i)
        {
            string extension = System.IO.Path.GetExtension(filepath[i]);
            string result = filepath[i].Substring(0, filepath[i].Length - extension.Length);
            int index = result.IndexOf("Resources/");
            toRet[i] = result.Substring(index + 10); ;
        }
        return toRet;
    }
    //remove this after
    public static void DebugOutAllStrings(string[] s)
    {
        _DebugOutAllStrings(s);
    }
    public static void DebugOutAllStrings(List<string> s)
    {
        _DebugOutAllStrings(s);
    }

    public static string DebugConcatToOneOutputString(string[] s)
    {
        string toret = "";
        for (int i = 0; i < s.Length; i++)
        {
            toret += s[i] + ",";
        }
        return toret;
    }

    public static string DebugConcatToOneOutputString(IEnumerable s)
    {
        string toret = "";
        foreach (string ss in s)
            toret += ss + ",";
        return toret;
    }

    private static void _DebugOutAllStrings(IEnumerable s)
    {
        foreach (string ss in s)
            Debug.Log(ss);
    }


    public static Vector2 MultVect(Vector2 v2, Vector3 v3)
    {
        return new Vector2(v2.x * v3.x, v2.y * v3.y);
    }

    public static Vector2 RotateV2(Vector2 v, float degrees)
    {
        return Quaternion.Euler(0, 0, degrees) * v;
    }

    public static Color MaterialBasicColor(GV.MaterialType matType)
    {
        switch (matType)
        {
            case GV.MaterialType.Charisma:
                return Color.clear;
            case GV.MaterialType.Energy:
                return Color.yellow;
            case GV.MaterialType.Fire:
                return Color.red;
            case GV.MaterialType.Force:
                return Color.clear;
            case GV.MaterialType.Higgs:
                return Color.white;
            case GV.MaterialType.Ice:
                return Color.cyan;
            case GV.MaterialType.Lighting:
                return Color.yellow;
            case GV.MaterialType.Mist:
                return new Color32(32, 175, 233, 255);
            case GV.MaterialType.Nature:
                return Color.green;
            case GV.MaterialType.Oil:
                return new Color32(65, 0, 178, 255); //Dark purple
            case GV.MaterialType.Radio:
                return Color.clear;
            case GV.MaterialType.Rock:
                return new Color32(255, 90, 0, 255);
            case GV.MaterialType.Smoke:
                return Color.clear;
            case GV.MaterialType.Water:
                return Color.cyan;
            default:
                return Color.white;
        }
    }

    // www.desmos.com/calculator
    /*
     * \frac{-1}{\frac{\left(x-1\right)}{\left(k-1\right)\left(h-l\right)}+\frac{1}{h-l}}+h
     * \frac{1}{\frac{\left(x-1\right)}{\left(k-1\right)\left(h-l\right)}+\frac{1}{h-l}}+l
     */

	public static readonly bool OvRide = true;
	public static float MikesArcTan(float x, float control, float min, float max, float percent, HorzAsym horzAsymType){
		//More versatile version where we will be at percent% completion at x == control value
		float toRet = 0;
		float percentForm = (control - 1) * (100 / percent - 1); //makes percent have range of (0,100)  <-- note non inclusive brackets
		float mikesVar = (2/Mathf.PI)*(Mathf.Atan((x-1)/percentForm)); //this controls percent completion from max to min
		if (max <= min || control <= 1 || x < 0 || percent > 99 || percent < 1) {
			Debug.Log ("Some Variable in Mike's Asymptote is out of bounds... Returning 0...");
			Debug.Log (min + " " +  max + " " + control + " " + x + " " + percent);
			return 0;
		}

		if (horzAsymType == HorzAsym.MinToMax)
		{
			toRet = mikesVar * (max - min) + min;
		}
		else if (horzAsymType == HorzAsym.MaxToMin)
		{
			toRet = - mikesVar * (max - min) + max; 
		}
		//Debug.Log ("using ArcTan " + toRet);
		return toRet;
	}

    public enum HorzAsym { MinToMax, MaxToMin };
    public static float MikesAsymptote(float x, float control, float min, float max, HorzAsym horzAsymType)
    {
		//Default, assumes you want to be at 50% completion at control value == x
		return MikesAsymptote (x, control, min, max, 50f, horzAsymType);
    }

	public static float MikesAsymptote(float x, float control, float min, float max, float percent, HorzAsym horzAsymType){
		if (OvRide == true) {
			return MikesArcTan (x, control,min,max,percent, horzAsymType);
		}

		//More versatile version where we will be at percent% completion at x == control value
		float toRet = 0;
		float percentForm = (control-1) * (100 / percent - 1); //makes percent have range of (0,100)  <-- note non inclusive brackets
		float mikesVar = 1/((x-1) / percentForm + 1); //this controls percent completion from max to min
		if (max <= min || control <= 1 || x < 0 || percent > 99 || percent < 1) {
			Debug.Log ("Some Variable in Mike's Asymptote is out of bounds... Returning 0...");
			Debug.Log (min + " " +  max + " " + control + " " + x + " " + percent);
			return 0;
		}

		if (horzAsymType == HorzAsym.MinToMax)
		{
			toRet = - mikesVar * (max - min) + max;
		}
		else if (horzAsymType == HorzAsym.MaxToMin)
		{
			toRet = mikesVar * (max - min) + min; 
		}
		//Debug.Log ("arc Tan Val " + MikesArcTan(x,control,min,max,percent,horzAsymType));
		//Debug.Log ("control Val " + toRet);
		return toRet;
	}

	public static float MikesAsymptoteZero(float x, float control, float min, float max, HorzAsym horzAsymType){
		//Same as above (control at 50%) while also being able to start at x = 0!
		return MikesAsymptoteZero (x, control, min, max, 50f, horzAsymType);
	}

	public static float MikesAsymptoteZero(float x, float control, float min, float max, float percent, HorzAsym horzAsymType){
		//same as Mike's Assymptote, but allows for x starting at 0!
		return MikesAsymptote(x+1,control+1,min,max,percent,horzAsymType);
	}

    public static void SetAllChildSortingLayerRecurisvely(Transform _obj, string _sortingLayerName)
    {
        SpriteRenderer sr = _obj.gameObject.GetComponent<SpriteRenderer>();
        if(sr)
         sr.sortingLayerName = _sortingLayerName;
        foreach (Transform t in _obj)
            SetAllChildSortingLayerRecurisvely(t, _sortingLayerName);
    }

    public static void SetAllChildLayersRecurisvely(Transform _obj, string _layerName)
    {
        int layerNum = LayerMask.NameToLayer(_layerName);
        SetAllChildLayersRecurisvely(_obj, layerNum);
    }

    public static void SetAllChildLayersRecurisvely(Transform _obj, int _layer)
    {
        _obj.gameObject.layer = _layer;
        foreach( Transform t in _obj)
            SetAllChildLayersRecurisvely(t, _layer);
    }

    public static void SetAllChildTagRecurisvely(Transform _obj, string _tag)
    {
        _obj.gameObject.tag = _tag;
        foreach (Transform t in _obj)
            SetAllChildTagRecurisvely(t, _tag);
    }

    public static void SetAllChildTagNLayersRecurisvely(Transform _obj, int _layer, string _tag)
    {
        _obj.gameObject.layer = _layer;
        _obj.gameObject.tag = _tag;
        foreach (Transform t in _obj)
            SetAllChildTagNLayersRecurisvely(t, _layer,_tag);
    }


	/*
	public class AssStorage {
		float control;
		float min;
		float max;
		float percent; 
		HorzAsym horzAsymType;

		public AssStorage(float inC, float inMin,float inMax,float inPer, HorzAsym inH){
			control = inC;
			min = inMin;
			max = inMax;
			percent = inPer;
			horzAsymType = inH;
		}

		public AssStorage(float inC, float inMin,float inMax, HorzAsym inH){
			percent = -1;
			control = inC;
			min = inMin;
			max = inMax;
			horzAsymType = inH;
		}

		public AssStorage(AssStorage copyThis){
			control = copyThis.control;
			min = copyThis.min;
			max = copyThis.max;
			percent = copyThis.percent;
			horzAsymType = copyThis.horzAsymType;
		}

		public float ret(int x){
			if(percent == -1)
				return MikesAsymptote(x,control,min,max,horzAsymType);
			return MikesAsymptote(x,control,min,max,percent,horzAsymType);
		}
	}
	*/

    public static T ParseEnum<T>(string value)
    {
        return (T)System.Enum.Parse(typeof(T), value, true);
    }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return GV.RadianToVector2(degree * Mathf.Deg2Rad);
    }

	public static float Vector2ToAngle(Vector3 right){
		float angleToRet = Vector2.Angle(new Vector2(1, 0), right);
		if (right.y < 0) {
			return -angleToRet;
		} else
			return angleToRet;
	}

	public static float Vector2ToAngle(Vector2 forward){
		return Vector2ToAngle (new Vector3(forward.x,forward.y,0));
	}

    public static Vector3 RemoveZ(Vector3 v)
    {
        return new Vector3(v.x,v.y,0);
    }

    public static Vector2 V3toV2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }

    public static Vector2 V2FromAngle(float ang)
    {
        //http://www.systemroot.ca/2011/08/xna-converting-from-angle-to-vector2-and-back/
        if (ang <= 0)
            ang = (ang + 360) % 360;
        float myAngleInRadians = ang * Mathf.Deg2Rad;
        Vector2 toRet = new Vector2(
            (float)Mathf.Cos(myAngleInRadians),
            (float)Mathf.Sin(myAngleInRadians));
        return toRet;
    }

    public static string OutputDictToString(string title, Dictionary<string, string> outputDict)
    {
        string toOut = title + ": ";
        foreach (KeyValuePair<string, string> kv in outputDict)
            toOut += "\"" + kv.Key + "\": " + kv.Value + ", ";
        return toOut;
    }

    public static float Limit(float v, float limit)
    {
        return (v > limit) ? limit : v;
    }

    public static Vector2 GetPositionScreenPercentage(Vector2 pos)
    {
       return new Vector2(pos.x / Screen.width, pos.y / Screen.height);
    }

    public static Vector2 GetDamageDirection(Vector2 objDealingDmg, GV.DirectionalDamage objDealingDmgType, float specialAngle ,Vector2 objectTakingDamage)
    {
        switch (objDealingDmgType)
        {
            case DirectionalDamage.explosion:
                return objectTakingDamage - objDealingDmg;
            case DirectionalDamage.implosion:
                return objDealingDmg - objectTakingDamage;
            case DirectionalDamage.towardsFace:
            case DirectionalDamage.specifiedDir:
                return GV.V2FromAngle(specialAngle);
        }
        Debug.LogError("unhandled dmg dir: " + objDealingDmgType);
        return objectTakingDamage - objDealingDmg;
    }

}
