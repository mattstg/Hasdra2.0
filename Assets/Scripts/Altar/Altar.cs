using UnityEngine;
using System.Collections;

public class Altar : MonoBehaviour {

    public void OnTriggerStay2D(Collider2D coli)
    {
        if (coli.gameObject.GetComponent<Spell>())
        {
            Spell spell = coli.gameObject.GetComponent<Spell>();
            spell.spellInfo.currentEnergy += GV.ALTAR_SPELL_ENERGY_GIFT * Time.deltaTime;
        }
        else if (coli.gameObject.GetComponent<PlayerControlScript>())
        {
            PlayerControlScript pcs = coli.gameObject.GetComponent<PlayerControlScript>();
            pcs.stats.energy       += GV.ALTAR_PCS_ENERGY_GIFT * Time.deltaTime;
            pcs.stats.healthPoints += GV.ALTAR_PCS_HP_GIFT * Time.deltaTime;
            pcs.stats.stamina      += GV.ALTAR_PCS_STANIMA_GIFT * Time.deltaTime;
        }
    }
}
