using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispPcs : PlayerControlScript {

    public static bool WispsActivated = false;

    AudioManager audioManager;
    float dyingCounter = 0;
    bool dying = false;
    float audioTimer = 0;

    protected override void Start()
    {
    }

    public override void Initialize(LevelTracker lvlTracker, int level, string _name)
    {
        pcsName = _name;
        isPlayerControlled = false;
        stats = new BodyStats(this);
        //inputMap = new InputManager(this);  construct brain instead
        abilityManager = new AbilityManager(this);
        audioManager = gameObject.AddComponent<AudioManager>();
        audioManager.Initialize();
        //inputMap.LoadControls(pid); //load ctrls requires abMngr is up, abMangr requires inputmap on cnstr, so order here important
        levelUpManager = new LevelUpManager(lvlTracker, this);
        levelUpManager.ForceTradeInExp(level * GV.EXPERIENCE_PER_LEVEL);
        playerRigidBody = GetComponent<Rigidbody2D>();
        spellBridge = GetComponent<SpellBridge>();
        spellBridge.InitializeSpellBridge(this, null);
    }

    protected override void Update()
    {
        if (dying && dyingCounter <= 0)
            DestroyNPC();

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
            playerRigidBody.velocity = new Vector2(0, 0);
        else
        {
            dyingCounter -= Time.deltaTime;
            ApplyStoredForce();
        }

        UpdateAudio();
    }

    protected void UpdateAudio()
    {
        if(audioTimer <= 0)
        {
            audioManager.PlayRandomSoundFromFolder("NPCs/Wisp", true, false);
            audioTimer = 9;
        }
    }
    
    void DestroyNPC()
    {
        Destroy(gameObject);
    }
}
