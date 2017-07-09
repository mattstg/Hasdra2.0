using UnityEngine;
using System.Collections;

public class GatherDebrisAnimation : SpellAnimInterface{
    SpellInfo spellInfo;
    Transform parentTransform;
    ParticleSystem particleSys;
    int lastUpdatedPowerLevel = 0;  //based on scale
    bool activated = false;
    bool isStable = true;

    public void InitializeSpellAnimation(SpellInfo _spell, Transform _parentTransform)
    {
        spellInfo = _spell;
        parentTransform = _parentTransform;
    }

    public bool UpdateInterface()
    {
        if (!activated && spellInfo.currentEnergy >= GV.GATHERDEBRIS_INITIAL_ENERGY)
        {
            ActivateGatherAnimation();
            activated = true;
        }

        /*if (spellInfo.isStable != isStable)  cannot currently access shape.EmitFromEdge through scripts, post about it online, write the ppl
        {
            particleSys.startSpeed *= -1;
            isStable = spellInfo.isStable;
            particleSys.shape.
        } */

        /*if (spellInfo.spellState == GV.SpellState.Launched)
        {
            CleanupAnimation();
            return false;
        }*/
        else if (activated && spellInfo.spellState == GV.SpellState.Charging)
        {
            int desiredPowerLevel = (int)(parentTransform.lossyScale.x / GV.GATHERDEBRIS_SCALE_INTERVAL);
            if (desiredPowerLevel != lastUpdatedPowerLevel)
            {
                particleSys.startLifetime = GV.GATHERDEBRIS_LIFESPAN_PER_SCALE * parentTransform.lossyScale.x;
                var emission = particleSys.emission;  //shrukien system every time serouisly...
                var rate = emission.rate;
                rate.constantMax = GV.GATHERDEBRIS_EMIT_PER_ENERGY * spellInfo.currentEnergy;
                emission.rate = rate;
                lastUpdatedPowerLevel = desiredPowerLevel;
            }
        }
        return true;
    }

    private void ActivateGatherAnimation()
    {
        particleSys = SpellAnimationLibrary.Instance.GetDebrisGatherAnimation(spellInfo.materialType);
        particleSys.transform.SetParent(parentTransform);
        particleSys.transform.localScale = new Vector3(1, 1, 1);
        particleSys.transform.localPosition = new Vector3(0, 0, 0);
    }

    private void CleanupAnimation()
    {
        MonoBehaviour.Destroy(particleSys);
    }


    
}
