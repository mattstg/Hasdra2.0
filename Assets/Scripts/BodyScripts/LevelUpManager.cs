using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Ideally should contain the LevelUpTracker. But didnt work out that way. Keeping it seperate in case different creatures have different LevelUpManager types since it contains animations
//Level up manager handles the GUI and the feeding of experience into levels
public class LevelUpManager
{
    public PlayerControlScript pcs;
    public LevelTracker levelTracker;
    public float experienceStored;
    public float experienceTradedIn; //exp traded in waiting to be turned into levels
    float tradeInTime = 0;
    float tradeInRate = 0;
    bool[] animationsActive = new bool[] { false,false, false, false }; //tier 0,1,2,3 
    string[] tier0Animations; //Should be paths for RuntimeAnimatorControllers
    string tierZeroAnimationPrefab = "Prefabs/BodyPrefabs/PowerUpAnimation";
    string tier1Animation = "Animations/PoweringUpAnimations/Player/t1BasicLevel";
    Transform parentTransform;
    Color playerColorOne;
    Color playerColorTwo;


    public LevelUpManager(LevelTracker _lvlTracker, PlayerControlScript _pcs)
    {
        pcs = _pcs;
        levelTracker = _lvlTracker;
        levelTracker.Initialize(this, pcs);
        IntializeAnimations(pcs.transform, pcs.primaryColor, pcs.secondaryColor);
    }

    private void IntializeAnimations(Transform parent, Color color1, Color color2)
    {
        parentTransform = parent;
        tier0Animations = PowerUpAnimationLibrary.GetPowerUpAnimations("Player", 0);
        playerColorOne = color1;
        playerColorTwo = color2;
    }

    public void ForceTradeInExp(float amt)
    {
        int levelsGained = (int)(amt / GV.EXPERIENCE_PER_LEVEL);
        if (levelsGained > 0)
            levelTracker.LevelUp(levelsGained);
    }

    //maybe level up manager shoudln't be handling the animations for trading in exp...
    public void TradeInExp(float amtTradeIn)
    {
        tradeInTime += Time.deltaTime;
        if (!animationsActive[0])
            ActivateAnimations(0);
        if (!animationsActive[1] && tradeInRate >= GV.ACTIVATE_TIER_1_POWER_ANIMATION_CONSUMPTION_RATE)
            ActivateAnimations(1);
        if (!animationsActive[2] && tradeInRate >= GV.ACTIVATE_TIER_2_POWER_ANIMATION_CONSUMPTION_RATE)
            ActivateAnimations(2);
        if (!animationsActive[3] && tradeInRate >= GV.ACTIVATE_TIER_3_POWER_ANIMATION_CONSUMPTION_RATE)
            ActivateAnimations(3);

        if (experienceStored > amtTradeIn)
        {
            experienceTradedIn += amtTradeIn;
            experienceStored -= amtTradeIn;
        }
        else
        {
            experienceTradedIn += experienceStored;
            experienceStored = 0;
        }
        int levelsGained = (int)(experienceTradedIn / GV.EXPERIENCE_PER_LEVEL);
        if(levelsGained > 0)
            levelTracker.LevelUp(levelsGained);
        experienceTradedIn -= levelsGained * GV.EXPERIENCE_PER_LEVEL;
    }

    #region LevelingAnimations
    private void ActivateAnimations(int n)
    {
        GameObject newAnim;
        switch (n)
        {
            case 0:
                newAnim = ActivateAnimationZero();
                break;
            case 1:
                newAnim = ActivateAnimationOne();
                break;
            case 2:
                newAnim = ActivateAnimationTwo();
                break;
            case 3:
                newAnim = ActivateAnimationThree();
                break;
            default:
                Debug.LogError("Default case for Activate Animations: " + n);
                newAnim = null;
                break;
        }
        if (newAnim) //catch because not all are active atm
        {
            newAnim.transform.SetParent(parentTransform);
            newAnim.transform.localPosition = new Vector3(0, 0, 0);
            //newAnim.GetComponent<SpriteRenderer>().sortingLayerName = "PowerUpAnimation";
        }
    }
    private GameObject ActivateAnimationZero()
    {
        animationsActive[0] = true;
        int toLoad = Random.Range(0, tier0Animations.Length);
        GameObject newAnimation = GameObject.Instantiate(Resources.Load(tierZeroAnimationPrefab)) as GameObject;
        newAnimation.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(tier0Animations[toLoad]);
        t0PowerUp t0 = newAnimation.AddComponent<t0PowerUp>();
        newAnimation.GetComponent<SpriteRenderer>().sortingLayerName = "PowerUpAnimation";
        t0.Initialize();
        return newAnimation;
    }
    private GameObject ActivateAnimationOne()
    {
        animationsActive[1] = true;
        GameObject newAnimation = GameObject.Instantiate(Resources.Load(tier1Animation)) as GameObject;
        newAnimation.GetComponent<t1PowerUp>().Initialize(playerColorOne, playerColorTwo);
        return newAnimation;
    }
    private GameObject ActivateAnimationTwo()
    {
        animationsActive[2] = true;
        //Debug.Log("Activate Animation 2");
        return null;
    }
    private GameObject ActivateAnimationThree()
    {
        animationsActive[3] = true;
        //Debug.Log("Activate Animation 3");
        return null;
    }
    private void StopAllAnimations()
    {
        try
        {
            Transform[] t = GV.GetAllChildrenWithTag(parentTransform, "PowerUpAnimation");
            for(int i = 0; i < t.Length; ++i)
                GameObject.Destroy(t[i].gameObject);
        }
        catch
        {
            Debug.LogError("No children found");
        }
        animationsActive = new bool[] { false, false, false, false };
    }
    

    #endregion

    public void StopTradeIn()
    {
        experienceStored += experienceTradedIn;
        experienceTradedIn = 0;
        StopAllAnimations();
    }

    private void ForceTradeInExp()
    {
        int levelsGained = (int)(experienceStored / GV.EXPERIENCE_PER_LEVEL);
        if (levelsGained >= 1)
        {
            experienceStored -= levelsGained * GV.EXPERIENCE_PER_LEVEL;
            levelTracker.LevelUp(levelsGained);
        }
    }

    public void GainExp(float amount)
    {
        experienceStored += amount;
    }

    public void UpdateExpPerSecond(float timePassed)
    {
        experienceStored += GV.EXPERIENCE_GAIN_PER_SECOND * timePassed;
    }
}


