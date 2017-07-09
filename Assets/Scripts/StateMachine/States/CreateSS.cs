using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class CreateSS : StateSlot {

    public override void Initialize(State _parentState)
    {
        base.Initialize(_parentState);
        string createStartValue = LiveSpellDict.GetAllSpellNames()[0];
        AddSSTuple("create", createStartValue, GV.StateVarType.ExistingSpells);
        AddSSTuple("use_Spell_Default_Param", "True", GV.StateVarType.Bool);
        AddSSTuple("displacedX", "0", GV.StateVarType.Float, "use_Spell_Default_ParamFalse");
        AddSSTuple("displacedY", "0", GV.StateVarType.Float, "use_Spell_Default_ParamFalse");
        AddSSTuple("fireAngle", "0", GV.StateVarType.Float,"use_Spell_Default_ParamFalse");
        AddSSTuple("fire_Ang_Relative", "Normal", GV.StateVarType.RelativeLaunchType, "use_Spell_Default_ParamFalse");
        AddSSTuple("energy", "0", GV.StateVarType.Float, "use_Spell_Default_ParamFalse");
        AddSSTuple("energyMode", "constant", GV.StateVarType.constantOrPercent, "use_Spell_Default_ParamFalse");
        AddSSTuple("repeatFire", "False", GV.StateVarType.Bool, "use_Spell_Default_ParamFalse");
    }

    public override void PerformStateAction(Spell spell)
    {
        SpellLaunchParam slp = new SpellLaunchParam(ssDict["create"].value, new Vector2(ssDict["displacedX"].CastValue<float>(), ssDict["displacedY"].CastValue<float>()));
        spell.CreateSpellState(slp);
        //Debug.Log("state action Create");
    }

    public override void ExitState(Spell spell)
    { //if charging a spell, fire it
        if (spell.spellInfo.isCreating)
        {
            spell.spellBridge.LaunchSpell();
            spell.spellInfo.isCreating = false;
        }
    }

}
