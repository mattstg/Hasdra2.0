using UnityEngine;
using System.Collections;

public class FireSpell : Spell {

    RadiantHeat radiantHeat;


    public void Start()
    {
        /*
        GameObject heatCircle = Instantiate(Resources.Load("Prefabs/Spell/RadiantHeat")) as GameObject;
        radiantHeat = heatCircle.GetComponent<RadiantHeat>().InitializeHeat(this,transform);
        radiantHeat.transform.SetParent(transform);
        temperature = GV.SPELL_FIRE_START_TEMPERATURE + spellInfo.currentEnergy * GV.SPELL_FIRE_TEMP_PER_ENERGY;*/
        base.Start();
    }

    public override void FinishTransformingWithExcessEnergy()
    {
        base.FinishTransformingWithExcessEnergy();
    }

}
