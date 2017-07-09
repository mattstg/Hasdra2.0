using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class InputManager {
    protected PlayerControlScript pcs;
    Dictionary<KeyCode, string> inputSpellMap = new Dictionary<KeyCode,string>();
    Dictionary<KeyCode, string> defaultAbilities = new Dictionary<KeyCode, string>();  //eventaully to be removed
    //Dictionary<KeyCode, Ability> inputAbilityMap = new Dictionary<KeyCode, Ability>();
    public static float deltaTime = 0f;
    public bool inputDisabled = false;

	// Use this for initialization
    public InputManager(PlayerControlScript _pcs)
    {
        pcs = _pcs;
    }

    public void LoadControls(int controlScheme)
    {
        fillAbilityDictionary(controlScheme);
    }
	
	// Update is called once per frame
	public virtual void Update (float deltaTime) {
		//playerBody.isMoving = false;
        if (inputDisabled)
            return;

        List<KeyValuePair<KeyCode, Ability>> triggableKeyCodes = pcs.abilityManager.GetTriggableKeycodes();

        foreach (KeyValuePair<KeyCode, Ability> ability in triggableKeyCodes) //this could techincally be upgraded with a delegate, but then all the function calls would have to have same arguements, solve with default args?
        {
            if (ability.Value.IsTriggable(GV.AbilityTriggerType.Released) && Input.GetKeyUp(ability.Key))  //Checks if a key was released
            {
                pcs.abilityManager.UpdateAbility(ability.Value, GV.AbilityTriggerType.Released);
            }
            else if (ability.Value.IsTriggable(GV.AbilityTriggerType.Pressed) && Input.GetKeyDown(ability.Key))
            {
                pcs.abilityManager.UpdateAbility(ability.Value, GV.AbilityTriggerType.Pressed);
            }
            else if (ability.Value.IsTriggable(GV.AbilityTriggerType.Held) && Input.GetKey(ability.Key))
            {
                pcs.abilityManager.UpdateAbility(ability.Value, GV.AbilityTriggerType.Held);
            }
            if (ability.Value.IsTriggable(GV.AbilityTriggerType.Update))
            {
                pcs.abilityManager.UpdateAbility(ability.Value, GV.AbilityTriggerType.Update);
            }
        }
        
	    foreach (KeyValuePair<KeyCode,string> ability in defaultAbilities) //this could techincally be upgraded with a delegate, but then all the function calls would have to have same arguements, solve with default args?
        {
            if (Input.GetKey(ability.Key)) //Checks if a key is being held down
            {
                switch (ability.Value)
                {
                    case "down":
						//playerBody.isCrouched = true;
						pcs.Crouch();
                        break;
                    case "zoomIn":
                        pcs.cameraControl.ModZoom(-2 * Time.deltaTime);
                        break;
                    case "zoomOut":
                        pcs.cameraControl.ModZoom(2 * Time.deltaTime);
                        break;
                    default:
                        break;
                }
            }
        }
        
        foreach (KeyValuePair<KeyCode, string> spell in inputSpellMap)
        {
            if (Input.GetKeyUp(spell.Key))
                pcs.SpellReleased(spell.Value);
            else if (Input.GetKey(spell.Key))
                pcs.SpellPressed(spell.Value);
        }

	}

    public virtual void AddSpellAndKeyCode(string spellName, string keycodeAsString)
    {
        KeyCode _keycode = (KeyCode)System.Enum.Parse(typeof(KeyCode), keycodeAsString.ToUpper());
        if (inputSpellMap.ContainsKey(_keycode))
        {
            Debug.Log("spell: " + inputSpellMap[_keycode] + " overwritten by spell: " + spellName + " at keycode: " + _keycode.ToString());
            inputSpellMap[_keycode] = spellName;
        }
        else
        {
            inputSpellMap.Add(_keycode, spellName);
        }
    }

    /*
    public virtual void AddAbilityAndKeycode(Ability ability, string keycodeAsString)
    {
        KeyCode _keycode;
        try
        {
            _keycode = (KeyCode)System.Enum.Parse(typeof(KeyCode), keycodeAsString.ToUpper());
        }
        catch
        {
            try
            {
                _keycode = (KeyCode)System.Enum.Parse(typeof(KeyCode), keycodeAsString);
            }
            catch
            {
                Debug.Log("couldnt turn keycode: " + keycodeAsString + " into a keycode enum, ability: " + ability.abilityName + " not added");
                return;
            }
        }
        if (inputAbilityMap.ContainsKey(_keycode))
        {
            pcs.abilityManager.RemoveAbilityKey(inputAbilityMap[_keycode].abilityName, _keycode);
            inputAbilityMap[_keycode] = ability;
            pcs.abilityManager.AddAbilityKey(ability.abilityName, _keycode);
        }
        else
        {
            inputAbilityMap.Add(_keycode, ability);
            pcs.abilityManager.AddAbilityKey(ability.abilityName, _keycode);
        }
    }*/

    protected virtual void fillAbilityDictionary(int localPlayerNumber)
    {
        if (localPlayerNumber == 0)
        {
            pcs.abilityManager.LearnOrLevelAbility(GV.AbilityType.moveLeft, KeyCode.A.ToString(), GV.EXPERIENCE_PER_LEVEL);
            pcs.abilityManager.LearnOrLevelAbility(GV.AbilityType.moveRight, KeyCode.D.ToString(), GV.EXPERIENCE_PER_LEVEL);
            pcs.abilityManager.LearnOrLevelAbility(GV.AbilityType.jump, KeyCode.E.ToString(), GV.EXPERIENCE_PER_LEVEL);
            pcs.abilityManager.LearnOrLevelAbility(GV.AbilityType.dash, KeyCode.Q.ToString(), GV.EXPERIENCE_PER_LEVEL);
            pcs.abilityManager.LearnOrLevelAbility(GV.AbilityType.tradeIn, KeyCode.Z.ToString(), GV.EXPERIENCE_PER_LEVEL);
            pcs.abilityManager.LearnOrLevelAbility(GV.AbilityType.aimUp, KeyCode.W.ToString(), GV.EXPERIENCE_PER_LEVEL);
            pcs.abilityManager.LearnOrLevelAbility(GV.AbilityType.aimDown, KeyCode.S.ToString(), GV.EXPERIENCE_PER_LEVEL);

            //defaultAbilities.Add(KeyCode.C, "down");
            //defaultAbilities.Add(KeyCode.Q, "fly");
            defaultAbilities.Add(KeyCode.Minus, "zoomOut");
            defaultAbilities.Add(KeyCode.Equals, "zoomIn");
        }
        else
        {
            pcs.abilityManager.LearnOrLevelAbility(GV.AbilityType.moveLeft, KeyCode.LeftArrow.ToString(), GV.EXPERIENCE_PER_LEVEL);
            pcs.abilityManager.LearnOrLevelAbility(GV.AbilityType.moveRight, KeyCode.RightArrow.ToString(), GV.EXPERIENCE_PER_LEVEL);
            pcs.abilityManager.LearnOrLevelAbility(GV.AbilityType.tradeIn, KeyCode.Keypad0.ToString(), GV.EXPERIENCE_PER_LEVEL);
            pcs.abilityManager.LearnOrLevelAbility(GV.AbilityType.aimUp, KeyCode.UpArrow.ToString(), GV.EXPERIENCE_PER_LEVEL);
            pcs.abilityManager.LearnOrLevelAbility(GV.AbilityType.aimDown, KeyCode.DownArrow.ToString(), GV.EXPERIENCE_PER_LEVEL);
            pcs.abilityManager.LearnOrLevelAbility(GV.AbilityType.jump, KeyCode.O.ToString(), GV.EXPERIENCE_PER_LEVEL);
            pcs.abilityManager.LearnOrLevelAbility(GV.AbilityType.dash, KeyCode.P.ToString(), GV.EXPERIENCE_PER_LEVEL);
        }
    }

    public Dictionary<string,string> ListAllSpellsLearnt()
    {
        Dictionary<string,string> toRet = new Dictionary<string,string>();
        foreach(KeyValuePair<KeyCode,string> kv in inputSpellMap)
            toRet.Add(kv.Key.ToString(),kv.Value);
        return toRet;
    }

    public virtual int GetLoadoutValue()
    {
        return 0;
    }
}
