using UnityEngine;
using System.Collections;

public class SpellAnimationManager : MonoBehaviour {

    SpellInfo spellInfo;
    System.Collections.Generic.List<SpellAnimInterface> spellAnimations = new System.Collections.Generic.List<SpellAnimInterface>();
    Transform animationsObject = null;

    public void InitializeAnimManager(SpellInfo _spellInfo, Spell parentSpell)
    {
        spellInfo = _spellInfo;
        this.transform.SetParent(parentSpell.transform);
    }

    public void RemoveAnimation(GV.SpellAnimationType animType)
    {

    }

    public void AddAnimation(GV.SpellAnimationType animType)
    {
        /*SpellAnimInterface newAnim;
        if (animationsObject == null)
            CreateNewAnimationParent();

        switch (animType)
        {
            case GV.SpellAnimationType.GatheringDebris:
                newAnim = CreateGatheringDebris(spellInfo);
                break;
            case GV.SpellAnimationType.Spotlight:
                newAnim = CreateSpotlight(spellInfo);
                break;
            case GV.SpellAnimationType.Unstable:
                newAnim = CreateUnstable(spellInfo);
                break;
            default:
                newAnim = null;
                Debug.LogError("Animation type not recognized: " + animType);
                break;
        }
        spellAnimations.Add(newAnim);*/
    }

    private void CreateNewAnimationParent()
    {
        if (animationsObject)
            Destroy(animationsObject.gameObject);
        animationsObject = (new GameObject()).transform;
        animationsObject.transform.SetParent(transform);
        animationsObject.transform.localPosition = new Vector3(0, 0, 0);
        animationsObject.transform.localScale = new Vector3(1, 1, 1);
        animationsObject.name = "Spell Animations";
    }

    private SpellAnimInterface CreateGatheringDebris(SpellInfo spellInfo)
    {
        SpellAnimInterface debris = new GatherDebrisAnimation();
        debris.InitializeSpellAnimation(spellInfo, animationsObject);
        return (SpellAnimInterface)debris;
    }

    private SpellAnimInterface CreateSpotlight(SpellInfo spellInfo)
    {
        SpellAnimInterface spotLights = new SpellSpotlightRotation();
        spotLights.InitializeSpellAnimation(spellInfo, animationsObject);
        return spotLights;
    }

    private SpellAnimInterface CreateUnstable(SpellInfo spellInfo)
    {
        SpellAnimInterface unstable = new DestabilizationAnimation();
        unstable.InitializeSpellAnimation(spellInfo, animationsObject);
        return unstable;
    }

    public void ClearAllAnimations()
    {
        if (animationsObject)
            Destroy(animationsObject.gameObject);
        CreateNewAnimationParent();
    }

    public void UpdateAnimations()
    {
        for (int i = spellAnimations.Count - 1; i >= 0; i--)
            if (!spellAnimations[i].UpdateInterface())
                spellAnimations.RemoveAt(i);  //removes self, but the animation needs to delete itself
    }
}
