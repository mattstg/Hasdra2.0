using UnityEngine;
using System.Collections;

public class SpellAnimationLibrary
{

    #region Singleton
    private static SpellAnimationLibrary instance;

    private SpellAnimationLibrary() { }

    public static SpellAnimationLibrary Instance
    {
        get{
            if(instance == null)
            {
                instance = new SpellAnimationLibrary();
            }
            return instance;
        }
    }
#endregion

    public GameObject OC_Prefab;
    string[] animatorFiles;

    public void Start()
    {
        GetAllDestabilizationAnimators();
    }

    #region Destabilization
    

    public void GetAllDestabilizationAnimators()
    {
        animatorFiles = System.IO.Directory.GetFiles("Assets/Resources/Animations/DestabilizationAnimations", "*.controller");
    }

    public GameObject LoadRandomAnimator()
    {
        if (animatorFiles == null)
            GetAllDestabilizationAnimators();
        int toLoad = Random.Range(0, animatorFiles.Length);
        GameObject OCObject = MonoBehaviour.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Spell/OC_Animations/OC"));
        OCObject.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/DestabilizationAnimations/" + System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetFileName(animatorFiles[toLoad])));
        //OCObject.GetComponent<SpriteRenderer>().color = GV.MaterialBasicColor(matType);
        return OCObject;
    }

    public int NumberOfUniqueAnimations()
    {
        if (animatorFiles == null)
            GetAllDestabilizationAnimators();
        return animatorFiles.Length;
    }

    
#endregion

    public ParticleSystem GetDebrisGatherAnimation(GV.SpellForms spellForm)
    {
        GameObject debrisGatherAnim = MonoBehaviour.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Spell/ParticleEffects/DebrisCharge"));
        ParticleSystem partSys = debrisGatherAnim.GetComponent<ParticleSystem>();
        Renderer render = partSys.GetComponent<Renderer>();
        render.material = Resources.Load("Textures/Spells/Gathering Debris Materials/" + spellForm + "Particle", typeof(Material)) as Material;
        //Debug.Log("debris animation fake made for: " + matType);
        return partSys;
    }

    public ParticleSystem GetExplosionAnimation(GV.SpellForms spellForm)
    {
        GameObject debrisGatherAnim = MonoBehaviour.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Spell/ParticleEffects/CircleEmit"));
        ParticleSystem partSys = debrisGatherAnim.GetComponent<ParticleSystem>();
        Renderer render = partSys.GetComponent<Renderer>();
        render.material = Resources.Load("Textures/Spells/Gathering Debris Materials/" + spellForm + "Particle", typeof(Material)) as Material;
        //Debug.Log("debris animation fake made for: " + matType);
        return partSys;
    }
}
