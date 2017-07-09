using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplosionFactory : MonoBehaviour {

    static ExplosionFactory mInstance;

    public static ExplosionFactory Instance
    {
            get
            {
                if (mInstance == null)
                {
                    GameObject go = new GameObject();
                    go.name = "ExplosionFactory singleton";
                    mInstance = go.AddComponent<ExplosionFactory>();
                }
                return mInstance;
            }
    }


    public void MakeExplosion(SpellInfo si, Vector2 location, List<SkillModifier> storedSkillModifiers)
    {
        GameObject newExplosion = Instantiate(Resources.Load("Prefabs/Spell/Explosion"), location, Quaternion.identity) as GameObject;
        float energyInExplosion = (si.spellForm == GV.SpellForms.Physical) ? si.currentEnergy / GV.PHYSICALSPELL_EXPLOSION_REDUCTION : si.currentEnergy;

        newExplosion.AddComponent<Explosion>().InitializeExplosion(si, energyInExplosion, storedSkillModifiers);
        //Debug.Log("skill modes transfering: " + storedSkillModifiers.Count);
        newExplosion.transform.parent = this.transform;
        //newExplosion.transform.position = location;
        MakeParticleExplosion(si.spellForm, energyInExplosion, location, si.directionalDamageType, si.currentAngle);
    }

    /*private void MakeParticleExplosion(GV.MaterialType matType, float _energy, Vector2 pos, GV.DirectionalDamage explosionType, float spellsFacingAngle)
    {
        ParticleController particleController;
        try
        {
            particleController = ((GameObject)Instantiate(Resources.Load("Prefabs/Spell/ParticleEffects/Particle" + matType.ToString() + "Explosion"), pos, Quaternion.identity)).GetComponent<ParticleController>();
        }
        catch
        {
            Debug.LogError("Unhandled explosion particle type at location:  Prefabs/Spell/ParticleEffects/Particle" + matType.ToString() + "Explosion");
            particleController = ((GameObject)Instantiate(Resources.Load("Prefabs/Spell/ParticleEffects/ParticleFireExplosion"), transform.position, Quaternion.identity)).GetComponent<ParticleController>();
        }
        particleController.particleMaterial = matType;
        particleController.BurstToDeath(_energy,explosionType);
        
    }*/

    private void MakeParticleExplosion(GV.SpellForms spellForm, float _energy, Vector2 pos, GV.DirectionalDamage explosionType, float spellsFacingAngle)
    {
        ParticleController particleController;
        particleController = ((GameObject)Instantiate(Resources.Load("Prefabs/Spell/ParticleEffects/ParticleExplosion"), pos, Quaternion.identity)).GetComponent<ParticleController>();
        
        particleController.BurstToDeath(_energy, explosionType);
        particleController.SetParticleMaterial(spellForm);

    }

    public class SelfCastExplosionParam
    {

    }
    
}
