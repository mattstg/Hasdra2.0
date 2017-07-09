using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpellStorage{

    public SpellStateMachine stateMachine;
    public State startState;
    public SpellInfo spellInfo;
    public GV.MaterialType materialType;
    public List<SkillModSS> onChargeSkillMods;
    public string name;

    public SpellStorage()
    {        
    }

    /*public void StoreSpell(Spell toStore)
    {
        spellInfo = toStore.spellInfo.Clone();
        //stateMachine = toStore.stateMachine.clone()
    }*/
}
