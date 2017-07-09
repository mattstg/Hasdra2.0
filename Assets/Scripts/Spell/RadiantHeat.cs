using UnityEngine;
using System.Collections;

//radiant heat only works with circles...
public class RadiantHeat : MonoBehaviour {

    public TemperatureSensitive sourceOfEnergy;
    public float heatEnergy = 1;
    CircleCollider2D heatColi;

    public RadiantHeat InitializeHeat(TemperatureSensitive _sourceOfEnergy, Transform parentTransform)
    {
        GameObject fireSystem = Instantiate(Resources.Load("Prefabs/Spell/ParticleEffects/FireBallParticleSystem")) as GameObject;
        fireSystem.GetComponent<centerOnParent>().parent = transform;
        fireSystem.transform.SetParent(transform);
        sourceOfEnergy = _sourceOfEnergy;
        heatColi = GetComponent<CircleCollider2D>();
        //heatCircle.transform.SetParent(transform);  stops colliders from merging
        GetComponent<centerOnParent>().parent = parentTransform;
        return this;
        
    }

    public void Update()
    {
        if(sourceOfEnergy.pastMinThreshold())
            heatEnergy += sourceOfEnergy.EnergyToHeat();
        SetSize();
        if(sourceOfEnergy == null)
            GV.Destroyer(gameObject);
            
    }

    private void SetSize()
    {
        if (heatEnergy > 0)
        {
            float radius = Mathf.Sqrt(heatEnergy / Mathf.PI)*GV.FIRE_SCALE_SIZE;
            heatColi.radius = radius;
            //this.transform.localScale = new Vector3(radius + GV.SPRITE_START_SCALE, radius + GV.SPRITE_START_SCALE, 1);
        }
    }

    public void OnCollisionStay2D(Collision2D coli)
    {
        if (coli.gameObject.GetComponent<TemperatureSensitive>() != null && (coli.gameObject.GetComponent<TemperatureSensitive>() != sourceOfEnergy))
        {
            ResolveCollision(coli.gameObject.GetComponent<TemperatureSensitive>());
        }
    }


    public void OnTriggerStay2D(Collider2D coli)
    {
        if (coli.gameObject.GetComponent<TemperatureSensitive>() != null && (coli.gameObject.GetComponent<TemperatureSensitive>() != sourceOfEnergy))
        {
            ResolveCollision(coli.gameObject.GetComponent<TemperatureSensitive>());
        }
    }



    public void ResolveCollision(TemperatureSensitive otherObj)
    {
        otherObj.ChangeTemperature(heatEnergy * Time.deltaTime, heatEnergy);
        //heatEnergy;
    }
}
