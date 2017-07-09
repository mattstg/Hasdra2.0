using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HabitatManager : MonoBehaviour {

    List<Habitat> habitats = new List<Habitat>();
	// Use this for initialization
	void Start () {
        Transform habitatParent = GV.worldUI.habitatParent;
        HabitatPhysical[] habPhys = habitatParent.GetComponentsInChildren<HabitatPhysical>();
        foreach (HabitatPhysical hp in habPhys)
            habitats.Add(hp.habitat);
	}

    /*public void Update()
    {
        foreach (Habitat hab in habitats)
            hab.Update(Time.deltaTime);
    }*/

    public void AddHabitat()
    {

    }
}
