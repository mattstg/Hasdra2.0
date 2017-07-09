using UnityEngine;
using System.Collections;

public class FractureSpeedBurst : MonoBehaviour {

    public float energy;
    public Vector2 spellCenter;
    public SpellInfo si;
	// Use this for initialization
    public void Initialize(float totalEnergy, Vector2 _spellCenter, SpellInfo _si)
    {
        energy = totalEnergy;
        spellCenter = _spellCenter;
        si = _si;
    }

    public void Start() //this is only used for fracture burst, should delete itself on cleanup
    {
        Destroy(this);
    }
}
