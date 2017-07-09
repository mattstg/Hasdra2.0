using UnityEngine;
using System.Collections;

public class ChargingScript : MonoBehaviour {

    public float energy = 0f;
    public float maxEnergy = 100f;
    public float energyDecay = 0.01f; //decay of energy (in %)
    public float radius = 0f;
    Vector2 head;
    public PlayerControlScript caster;
    Vector2 casterPosition;
    public Sprite ChargeSprite;

	// Use this for initialization
	void Start () {	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
        if (caster != null)
        {
            casterPosition = new Vector2(caster.GetComponent<Transform>().position.x, caster.GetComponent<Transform>().position.y);
            head = caster.head;
            transform.position = casterPosition + head;
            energy -= energyDecay * maxEnergy;
            radius = energy / maxEnergy * maxEnergy / 100; 
        }
	}

	void Update () {

        //should move to the reticle of player
        //should decay energy
        //should update radius based on energy
        //change head based on player movement? ofcourse.

	}

    public float takeEnergy(float toGive)
    {
        float energyTransfer = (energy + toGive <= maxEnergy)? toGive : maxEnergy - energy;
        energy += energyTransfer;
        return energyTransfer;
    }

    public void release()
    {
		Destroy(this);//should initiate start step in spell
    }
}
