using UnityEngine;
using System.Collections;

public class DestabilizationAnimation : SpellAnimInterface {

    SpellInfo spellInfo;
    Transform parentTransform;
    float valueBetweenAnimations;
    int activeAnimations{get{return destableAnimations.Count;}}
    System.Collections.Generic.List<Transform> destableAnimations = new System.Collections.Generic.List<Transform>();


    public void InitializeSpellAnimation(SpellInfo _spell, Transform _parentTransform)
    {
        valueBetweenAnimations = GV.STABILITY_DECAY_THRESHOLD / (GV.UNSTABLE_LEVELS + 1); //plus one to not have one of the values be at 0
        //Debug.Log("value between anims: " + valueBetweenAnimations);
        spellInfo = _spell;
        parentTransform = _parentTransform;
    }

    public bool UpdateInterface()
    {
        //throw new System.NotImplementedException();
        if (spellInfo.stability < GV.STABILITY_DECAY_THRESHOLD)
        {
            int desiredActiveAnim = (int)((GV.STABILITY_DECAY_THRESHOLD - spellInfo.stability) / valueBetweenAnimations);
            //Debug.Log(string.Format("{0},{1},{2}", GV.STABILITY_DECAY_THRESHOLD, spellInfo.stability, valueBetweenAnimations));
            //Debug.Log("stab/vba: " + spellInfo.stability + "/" + valueBetweenAnimations + "=" + desiredActiveAnim);
            if (desiredActiveAnim != activeAnimations && desiredActiveAnim >= 0) //in case negative shennagins
            {
                if (desiredActiveAnim > activeAnimations)
                { //add an animation
                    GameObject newAnimationChild = SpellAnimationLibrary.Instance.LoadRandomAnimator(spellInfo.materialType);
                    newAnimationChild.transform.SetParent(parentTransform);
                    float scaleDif = Random.Range(GV.UNSTABLE_SIZRANGE.x, GV.UNSTABLE_SIZRANGE.y);
                    newAnimationChild.transform.localPosition = new Vector3(0, 0, 0);
                    newAnimationChild.transform.localScale = new Vector3(1 * scaleDif, 1 * scaleDif, 1);
                    newAnimationChild.name = "DestableAnim";
                    destableAnimations.Add(newAnimationChild.transform);
                }
                else
                { //remove an animation
                    MonoBehaviour.Destroy(destableAnimations[activeAnimations - 1].gameObject);  
                    destableAnimations.RemoveAt(activeAnimations - 1); //just remove the last one
                }
            }
        }
        else if (activeAnimations > 0)
            RemoveAllAnimations();
        
        return true;
    }

    public void RemoveAllAnimations()
    {
        for(int i = destableAnimations.Count - 1; i >= 0; i--)
        {
            MonoBehaviour.Destroy(destableAnimations[i].gameObject);
        }
        destableAnimations.Clear();
        Debug.Log("destab called");
    }
}
