using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleController : MonoBehaviour {

    //actaully the explosion one, make this a child subclass of a higher up particle explosion later

    
    public List<ParticleSystem> particleSystems;
    //public ParticleCollisionEvent[] collisionEvents;
    public GV.MaterialType particleMaterial = GV.MaterialType.Energy; //Must be set by making obj

    public void BurstToDeath(float _energy, GV.DirectionalDamage explosionType)
    {
        Burst(_energy,explosionType);
        GameObject.Destroy(this.gameObject, 9f);
    }

    public void Burst(float _energy, GV.DirectionalDamage explosionType)
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            int toEmit = (int)((_energy / GV.ENERGY_PER_PARTICLE)) / particleSystems.Count;
            toEmit = (toEmit <= 0)?1:toEmit;
            if (explosionType == GV.DirectionalDamage.implosion)
            {
                //Debug.Log("yo");
                ps.startSpeed *= -1;
            }
            
            //Debug.Log("Emitting: " + toEmit + ", (" + _energy + "/" + GV.ENERGY_PER_PARTICLE + ")/" + particleSystems.Count);
            ps.Emit(toEmit);
        }
    }

    public void SetParticleMaterial(GV.MaterialType matType)
    {
        particleMaterial = matType;
        Renderer render = particleSystems[0].GetComponent<Renderer>();
        render.material = Resources.Load("Textures/Spells/Materials/" + matType.ToString().ToLower() + "Particle", typeof(Material)) as Material;
    }

    public void BurstInDirection(float _energy)
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            int toEmit = (int)((_energy / GV.ENERGY_PER_PARTICLE)) / particleSystems.Count;
            toEmit = (toEmit <= 0) ? 1 : toEmit;
            //Debug.Log("Emitting: " + toEmit + ", (" + _energy + "/" + GV.ENERGY_PER_PARTICLE + ")/" + particleSystems.Count);

            ps.Emit(toEmit);
        }
    }

   /* void OnParticlesCollision(GameObject other)
    {
        Debug.Log("particle collision!");
        foreach(ParticleSystem p in particleSystems)
            OnParticleEffect(other,_OnParticleCollision(other,p));
    }
    
    int _OnParticleCollision(GameObject other, ParticleSystem part) {
        int safeLength = part.GetSafeCollisionEventSize();
        if (collisionEvents.Length < safeLength)
            collisionEvents = new ParticleCollisionEvent[safeLength];
        return part.GetCollisionEvents(other, collisionEvents);
    }

    void OnParticleEffect(GameObject otherObj, int particlesTouched)
    {
        if (particlesTouched > 0)
        {
            if (otherObj.CompareTag("Player"))
                ParticleTouchPlayer(particlesTouched);
        }
    }

    void ParticleTouchPlayer(int amount)
    {
        Debug.Log(amount + " " + particleMaterial + " type particles have touched the player ");
        //deal dmg based on power * amount
    }*/

   

}
