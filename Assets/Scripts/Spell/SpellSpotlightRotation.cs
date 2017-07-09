using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Hard coded : This class contains a hardcoded value, the scale of the spotlights, odd bug prevents the default values from being used, also... spotlight count???? WHAT THE FUCK
public class SpellSpotlightRotation : SpellAnimInterface {

    SpellInfo spellInfo; //parent spell
    Transform parentTransform; //where the stuff get attached to

    float rotationSpeed = 100;
    const float MaxSpotLights = 8;
    List<Transform> spotlightTransforms = new List<Transform>();
    bool paired = false;

    public void InitializeSpellAnimation(SpellInfo _spell, Transform _parentTransform)
    {
        spellInfo = _spell;
        parentTransform = _parentTransform;
    }

    public bool UpdateInterface()
    {
        int spotCount = spotlightTransforms.Count; //not sure whats going on, but doesnt work using this value dynamically
        float workingEnergy = spellInfo.currentEnergy - GV.SPOTLIGHT_MIN_ENERGY;  //energy excess of min starting energy
        if (workingEnergy > 0) 
        {
            int desiredActiveBeams = (int)(workingEnergy / GV.SPOTLIGHT_ENERGY_PER_BEAM);
            if (desiredActiveBeams > spotCount && spotCount < MaxSpotLights)  //not enough beams
            {
                float toAdd = desiredActiveBeams - spotCount;
                for (int i = 0; i < toAdd; ++i)
                {
                    GameObject _spotlight = MonoBehaviour.Instantiate(Resources.Load("Animations/SpellAnimations/SpellSpotLight")) as GameObject;
                    _spotlight.transform.SetParent(parentTransform);
                    _spotlight.transform.localPosition = new Vector3(0, 0, 0);
                    float startRot = 0;
                    if (paired)
                    {
                        startRot = (spotlightTransforms[spotCount - 1].eulerAngles.z + 180) % 360;
                        paired = false;
                    }
                    else
                    {
                        startRot = Random.Range(0, 360);
                        paired = true;
                    }
                    _spotlight.transform.localEulerAngles = new Vector3(0, 0, startRot);
                    _spotlight.transform.localScale = new Vector3(.4f, 1.4f, 1); //Odd bug where all scales are set to 0 :/
                    spotlightTransforms.Add(_spotlight.transform);
                }
            }
            else if (desiredActiveBeams < spotCount)  //too many beams
            {
                MonoBehaviour.Destroy(spotlightTransforms[0].gameObject);
                spotlightTransforms.RemoveAt(0);
            }
        }
        foreach (Transform t in spotlightTransforms)
            t.Rotate(new Vector3(0, 0, 1), rotationSpeed * Time.deltaTime);
        return true;
    }

}
