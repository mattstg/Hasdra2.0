using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelTracker{

    public string avatarName;
    public Queue<LevelPackage> levelUpStack = new Queue<LevelPackage>();


    //public Stack<string> oldLevelUpStack = new Stack<string>();
    //public Stack<string> oldLevelUpStack2 = new Stack<string>();
    //public Stack<string> oldLevelUpStackCurrent;
    public PlayerControlScript pcs;
    public LevelUpManager lvlManager;

    BodyStats bodyStats {get{return pcs.stats;}}
    InputManager inputMap {get{return pcs.inputMap;}}   //since this is a monobehavior lined to PCS, I use it for the transform location of the numericText as well
    AbilityManager abilityManager { get { return pcs.abilityManager; } }
    
    public bool enableNumericText = false;
    bool isLooping = false;
    public int loopAmt;


    #region initializing
    public LevelTracker()
    {
        //oldLevelUpStackCurrent = oldLevelUpStack;
    }

    public LevelTracker(string charName, int _loopAmt)
    {
        avatarName = charName;
        loopAmt = _loopAmt;
    }

    public void Initialize(LevelUpManager manager, PlayerControlScript _pcs)
    {
        pcs = _pcs;
        lvlManager = manager;
    }

    public void AddNewLevel(LevelPackage newLevel)
    {
        //in here could parse to find if stat or skill
        levelUpStack.Enqueue(newLevel);
        //oldLevelUpStack.Push(value);
        //So bottom of the tree and up. Need to add Last stacks first. so level N -> 1.
    }
    /*
    public void AddNewLevelWithKey(string value, string key)
    {
        if (GV.STAT_ALL_NAMES.Contains(value))
        {
            Debug.Log("AddNewLevelWithKey with a stat based value is being added, not added, " + value);
            return;
        }

        LevelPackage toAdd;
        if(value.Contains("Ab:"))
        {
            value.Replace("Ab:", "");
            toAdd = new LevelPackage(GV.LevelPkgType.Ability,value,key);
        }
        else
        {
            toAdd = new LevelPackage(GV.LevelPkgType.Spell,value,key);
        }

        levelUpStack.Enqueue(toAdd);
        
        //oldLevelUpStack.Push(key);
        //oldLevelUpStack.Push(value);
    }*/

    public void FinializeLoopAmt()
    {
        loopAmt = (loopAmt > levelUpStack.Count) ? levelUpStack.Count : loopAmt;
        loopAmt = (loopAmt <= 0)? 1: loopAmt;
    }

    #endregion


    public void LevelUp(int levels)
    {
        //Once at the minimuim count, push the pop into alternating stacks
        string allLearns = "";
        for (int i = levels; i > 0; i--)
        {
            LevelPackage nextLevel = levelUpStack.Dequeue();
            switch (nextLevel.levelPkgType)
            {
                case GV.LevelPkgType.Ability:
                    LevelUpAbility(nextLevel.pkgName, nextLevel.activationKey, GV.EXPERIENCE_PER_LEVEL);  //by default atm gives one levels worth
                    break;
                case GV.LevelPkgType.Spell:
                    LevelUpSpell(nextLevel.pkgName, nextLevel.activationKey);
                    break;
                case GV.LevelPkgType.Stat:
                    LevelUpStat(nextLevel.pkgName);
                    break;
                case GV.LevelPkgType.Skill:
                    LevelUpSkill(nextLevel.pkgName);
                    break;
                default:
                    Debug.LogError("level up error, unhandled type: " + nextLevel.levelPkgType);
                    break;
            }

            if (isLooping)
            {
                levelUpStack.Enqueue(nextLevel);
            }
            else if (levelUpStack.Count() <= loopAmt)
            {
                isLooping = true;
            }
            allLearns += ", " + nextLevel.pkgName;
        }
        //Debug.Log("Learnt: " + allLearns);
    }

    private void LevelUpSkill(string skill)
    {
        if (bodyStats.ContainsSkill(skill))
        {
            bodyStats.modSkillValue(skill, 1);
        }
        else
        {
            Debug.LogError("Leveling Skill {0}, DNE in bodystats"); 
        }
    }

    private void LevelUpStat(string stat)
    {
        switch (stat)
        {//{ "Str", "Const", "Agi", "Wis", "Int", "Dex", "Char" };
            case "Str":
                bodyStats.strength++;
                break;
            case "Const":
                bodyStats.constitution++;
                break;
            case "Agi":
                bodyStats.agility++;
                break;
            case "Wis":
                bodyStats.wisdom++;
                break;
            case "Int":
                bodyStats.intelligence++;
                break;
            case "Dex":
                bodyStats.dexterity++;
                break;
            case "Char":
                bodyStats.charisma++;
                break;
            default:
                Debug.LogError("LevelupStat not understood: " + stat);
                break;
        }
        //Debug.Log("stat leveled up: " + stat);
        LevelUpNumericText(stat, "");
    }

    private void LevelUpSpell(string _spellName, string _keycode)
    {
        //Debug.Log("level up spell: " + _spellName + ", using keycode: " + _keycode);
        if (pcs.isPlayerControlled)
        {
            inputMap.AddSpellAndKeyCode(_spellName, _keycode);
            LevelUpNumericText(_spellName, _keycode);
        }
    }


    private void LevelUpAbility(string _abilityName, string _keycode, float expIncoming)
    {
        abilityManager.LearnOrLevelAbility(_abilityName, _keycode, expIncoming);
        //Debug.Log("level up spell: " + _abilityName + ", using keycode: " + _keycode);
        LevelUpNumericText(_abilityName, _keycode);
    }



    private void LevelUpNumericText(string prefix, string keycode)
    {
        if (!enableNumericText || !GV.ND_ON)
            return;
        //so, if there is no keycode, its a stat, just calling numericDisplay and passing a 1 will mean it levels up by one, but passing 1 if it already exist, will mod it to 2.. :D
        if (keycode == "")
        { //using pcs through body stats as the owner of the numericText
            StaticReferences.numericTextManager.CreateNumericDisplay(bodyStats.pcs, bodyStats.pcs.transform, prefix, prefix, 1f, Color.blue);
        }
        else
        {
            StaticReferences.numericTextManager.CreateDisposableNumericDisplay(Color.grey, "Learn " + prefix + " " + keycode, 5, bodyStats.pcs.transform,true);
        }

    }
}
