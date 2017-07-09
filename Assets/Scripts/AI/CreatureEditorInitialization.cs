using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureEditorInitialization : MonoBehaviour {

    public Habitat habitat;
    public string creatureName = "creature";
    public int startLevel = 1;
    public bool archapello = false; //auto look for name

	void Start () {
        Creature creature = (Creature)(TheForge.Instance.BuildNPC(creatureName, startLevel));
        creature.gameObject.GetComponent<SpriteRenderer>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite; //if this crashes, your creature doesnt have a sprite renderer
        creature.gameObject.transform.position = gameObject.transform.position;
        creature.gameObject.transform.localScale = gameObject.transform.localScale;
        creature.gameObject.transform.name = gameObject.transform.name;
        creature.GetComponent<BoxCollider2D>().size = transform.localScale;
        if (habitat)
        {
            habitat.AddCreature(creature);
            creature.gameObject.transform.SetParent(habitat.transform);
        }

        Destroy(gameObject);
	}
	
}
