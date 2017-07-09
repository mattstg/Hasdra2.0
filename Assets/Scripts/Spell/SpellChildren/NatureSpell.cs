using UnityEngine;
using System.Collections;

public class NatureSpell : Spell {

    RadiantHeat radiantHeat; //clean up spell on delete is needed for flammable objects

    public void Start()
    {
        temperature = GV.SPELL_NATURE_START_TEMPERATURE;
        base.Start();
    }

    public override void FinishTransformingWithExcessEnergy()
    {
        base.FinishTransformingWithExcessEnergy();
    }

    public override void PostChangeTemperature()
    {
        if(temperature > GV.SPELL_NATURE_IGNITION_TEMPERATURE && radiantHeat == null)
        {
            GameObject heatCircle = Instantiate(Resources.Load("Prefabs/Spell/RadiantHeat")) as GameObject;
            heatCircle.transform.SetParent(transform);
            radiantHeat = heatCircle.GetComponent<RadiantHeat>().InitializeHeat(this, transform);
        }
    }

}
