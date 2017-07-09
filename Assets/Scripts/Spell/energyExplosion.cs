using UnityEngine;
using System.Collections;

public class energyExplosion : MonoBehaviour {

    float energyCounter;

    public void InitializeExplosion(float energy)
    {
        energyCounter = GV.EXPLOSION_FIZZLE_TIME_PER_ENERGY * energy;
        if (energy > 0)
        {
            float radius = Mathf.Sqrt(((1/GV.ENERGYEXPLOSION_DENSITY) * GV.ENERGY_SCALE_SIZE * energy) / Mathf.PI);
            this.transform.localScale = new Vector3(radius + GV.SPRITE_START_SCALE, radius + GV.SPRITE_START_SCALE, 1);
        }
    }

    public void Update()
    {
        energyCounter -= Time.deltaTime;
        if (energyCounter <= 0)
            GV.Destroyer(this.gameObject);
    }
}
