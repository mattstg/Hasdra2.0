using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpellStorage{

    public SpellStateMachine stateMachine;
    public State startState;
    public SpellInfo spellInfo;
    public GV.SpellForms spellForm; //why isnt this in spellinfo?
    public List<SkillModSS> onChargeSkillMods;
    public string name;
    public bool isBasicSpell;

    public SpellStorage()
    {        
    }

    /*public void StoreSpell(Spell toStore)
    {
        spellInfo = toStore.spellInfo.Clone();
        //stateMachine = toStore.stateMachine.clone()
    }*/
}
