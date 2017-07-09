using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : PlayerControlScript {

    public static readonly float RAND_TIME_DELAY = 4f; //rand time delay before starts audio
        //GV

    string creatureName;
    AudioManager audioManager;
    float dyingCounter = 0;
    bool dying = false;
    float audioTimer = 0;
    public bool playAudio = true;
    float timeDelay;
    bool audioInitialized = false;

    public override void Initialize(LevelTracker lvlTracker, int level, string _name)
    {
        pcsName = _name;
        isPlayerControlled = false;
        stats = new BodyStats(this);
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/Creatures/" + _name);
        //inputMap = new InputManager(this);  construct brain instead
        abilityManager = new AbilityManager(this);
        timeDelay = Random.Range(0,RAND_TIME_DELAY);
        levelUpManager = new LevelUpManager(lvlTracker, this);
        levelUpManager.ForceTradeInExp(level * GV.EXPERIENCE_PER_LEVEL);
        playerRigidBody = GetComponent<Rigidbody2D>();
        spellBridge = GetComponent<SpellBridge>();
        spellBridge.InitializeSpellBridge(this, null);
        gameObject.GetComponent<BoxCollider2D>().size = transform.localScale;
        
    }

    private void AudioInitilization()
    {
        audioInitialized = true;
        audioManager = gameObject.AddComponent<AudioManager>();
        audioManager.Initialize();
        playAudio = System.IO.Directory.Exists("Assets/Hasdra/Resources/Audio/NPCs/" + pcsName);
        
        //inputMap.LoadControls(pid); //load ctrls requires abMngr is up, abMangr requires inputmap on cnstr, so order here important
    }

    protected override void Update()
    {
        timeDelay -= Time.deltaTime;
        if (dying && dyingCounter <= 0)
            DestroyNPC();

        if(!audioInitialized && timeDelay <= 0)
        {
            AudioInitilization();
        }

        if (stats.healthPoints <= 0 && !dying)
        {
            dyingCounter = 2.5f;
            dying = true;
            storedForce = new Vector2();
        }
        else
        {
            dying = false;
        }


        if (!dying)
        {
            if (Mathf.Abs(playerRigidBody.velocity.x) <= .1f && Mathf.Abs(playerRigidBody.velocity.y) <= .1f)
            {
                playerRigidBody.velocity = new Vector2(0, 0);
            }
            else
            {
                playerRigidBody.velocity = playerRigidBody.velocity - (new Vector2(playerRigidBody.velocity.x * GV.CREATURE_VELO_LOSS.x, playerRigidBody.velocity.y * GV.CREATURE_VELO_LOSS.y) * Time.deltaTime);
            }
        }
        else
        {
            dyingCounter -= Time.deltaTime;
            ApplyStoredForce();
        }

        if(audioInitialized)
            UpdateAudio();
    }

    protected void UpdateAudio()
    {
        if (!playAudio)
            return;

        if (audioTimer <= 0)
        {
            audioManager.PlayRandomSoundFromFolder("NPCs/" + pcsName, true, false);
            audioTimer = 9;
        }
    }

    public override void Concusion(bool isConcussed)
    {
        //Simple ragdoll later
    }

    public override bool IsHeadshot(GameObject otherObj) //Just to get rid of that annoying bug in meantime
    {
        return false;
    }

    void DestroyNPC()
    {
        Destroy(gameObject);
    }
}
