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

        switch (si.materialType)
        {
            case GV.MaterialType.Charisma:
                newExplosion.AddComponent<CharismaExplosion>();
                break;
            case GV.MaterialType.Energy:
                newExplosion.AddComponent<EnergyExplosion>();
                break;
            case GV.MaterialType.Fire:
                newExplosion.AddComponent<FireExplosion>();
                break;
            case GV.MaterialType.Force:
                newExplosion.AddComponent<ForceExplosion>();
                break;
            case GV.MaterialType.Higgs:
                newExplosion.AddComponent<HiggsExplosion>();
                break;
            case GV.MaterialType.Ice:
                newExplosion.AddComponent<IceExplosion>();
                break;
            case GV.MaterialType.Lighting:
                newExplosion.AddComponent<LightingExplosion>();
                break;
            case GV.MaterialType.Mist:
                newExplosion.AddComponent<MistExplosion>();
                break;
            case GV.MaterialType.Nature:
                newExplosion.AddComponent<NatureExplosion>();
                break;
            case GV.MaterialType.Oil:
                newExplosion.AddComponent<OilExplosion>();
                break;
            case GV.MaterialType.Radio:
                newExplosion.AddComponent<RadioExplosion>();
                break;
            case GV.MaterialType.Rock:
                newExplosion.AddComponent<RockExplosion>();
                break;
            case GV.MaterialType.Smoke:
                newExplosion.AddComponent<SmokeExplosion>();
                break;
            case GV.MaterialType.Water:
                newExplosion.AddComponent<WaterExplosion>();
                break;
            default:
                Debug.LogError("Unhandled material type in explosion factory, Energy explosion default");
                newExplosion.AddComponent<EnergyExplosion>();
                break;
        }
        newExplosion.GetComponent<Explosion>().InitializeExplosion(si, energyInExplosion, storedSkillModifiers);
        //Debug.Log("skill modes transfering: " + storedSkillModifiers.Count);
        newExplosion.transform.parent = this.transform;
        //newExplosion.transform.position = location;
        MakeParticleExplosion(si.materialType, energyInExplosion, location, si.directionalDamageType, si.currentAngle);
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

    private void MakeParticleExplosion(GV.MaterialType matType, float _energy, Vector2 pos, GV.DirectionalDamage explosionType, float spellsFacingAngle)
    {
        ParticleController particleController;
        particleController = ((GameObject)Instantiate(Resources.Load("Prefabs/Spell/ParticleEffects/ParticleExplosion"), pos, Quaternion.identity)).GetComponent<ParticleController>();
        
        particleController.BurstToDeath(_energy, explosionType);
        particleController.SetParticleMaterial(matType);

    }

    public class SelfCastExplosionParam
    {

    }
    
}
