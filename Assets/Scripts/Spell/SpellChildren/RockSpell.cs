using UnityEngine;
using System.Collections;

public class RockSpell : Spell {

    public void Start()
    {
        temperature = GV.SPELL_ROCK_START_TEMP;
        base.Start();
    }

    public override void FinishTransformingWithExcessEnergy()
    {
        
        base.FinishTransformingWithExcessEnergy();//deletes object
    }



}
