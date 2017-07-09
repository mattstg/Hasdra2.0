using UnityEngine;
using System.Collections;

public class EnergySpell : Spell {

    private bool explosionCreated = false;
    SpellSpotlightRotation ssr;   //Disable this in "Low Proc Mode"

    public void Start()
    {
        //temperature = GV.SPELL_ENERGY_START_TEMPERATURE + spellInfo.currentEnergy * GV.SPELL_ENERGY_TEMP_PER_ENERGY;
        base.Start();
    }

    public override void FinishTransformingWithExcessEnergy()
    {
        base.FinishTransformingWithExcessEnergy(); //base call will delete current spell, so do explosion logic first
    }

    public void Update()
    {
        //temperature = GV.SPELL_ENERGY_START_TEMPERATURE + spellInfo.currentEnergy * GV.SPELL_ENERGY_TEMP_PER_ENERGY;
        base.Update();
    }

    public override void ActivateAnimations()
    {
        spellAnimManager.AddAnimation(GV.SpellAnimationType.Spotlight);
    }

}
