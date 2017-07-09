using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerInputManager : InputManager {

    List<Dictionary<string, string>> controllerInputSpellMap;
    List<Dictionary<string, Ability>> controllerAbilityMap;
    Dictionary<string, string> thumbstickAbilityMap;
    List<string> KeysPressedLastCycle = new List<string>();
    int loadoutValue = 0;
    bool cycleLeftLock = false;
    bool cycleRightLock = false;

    public ControllerInputManager(PlayerControlScript _pcs) : base (_pcs){}

    protected override void fillAbilityDictionary(int localPlayerNumber)
    {
        /*controllerInputSpellMap = new List<Dictionary<string, string>>
        {
            new Dictionary<string,string>(),
            new Dictionary<string,string>(),
            new Dictionary<string,string>()
        };

        Dictionary<string, string> layerZeroInputMap = new Dictionary<string, string>();
        layerZeroInputMap.Add("ControllerA","jump");
        layerZeroInputMap.Add("ControllerBackButton","tradeIn");
        layerZeroInputMap.Add("ControllerLeftBumper", "cycleLeft");
        layerZeroInputMap.Add("ControllerRightBumper", "cycleRight");

        Dictionary<string, string> layerOneInputMap = new Dictionary<string, string>();
        layerOneInputMap.Add("ControllerA", "jump");
        layerOneInputMap.Add("ControllerLeftBumper", "cycleLeft");
        layerOneInputMap.Add("ControllerRightBumper", "cycleRight");

        Dictionary<string, string> layerTwoInputMap = new Dictionary<string, string>();
        layerTwoInputMap.Add("ControllerA", "jump");
        layerTwoInputMap.Add("ControllerLeftBumper", "cycleLeft");
        layerTwoInputMap.Add("ControllerRightBumper", "cycleRight");

        controllerAbilityMap = new List<Dictionary<string, string>>();
        controllerAbilityMap.Add(layerZeroInputMap);
        controllerAbilityMap.Add(layerOneInputMap);
        controllerAbilityMap.Add(layerTwoInputMap);

        thumbstickAbilityMap = new Dictionary<string, string>();  //manually checked
        thumbstickAbilityMap.Add("HorizontalLeft", "moveLeft");
        thumbstickAbilityMap.Add("HorizontalRight", "moveRight");
        thumbstickAbilityMap.Add("VerticalUp", "aimUp");
        thumbstickAbilityMap.Add("VerticalDown", "aimDown");*/
    }

    public override void Update(float dt)
    {
        /*CheckControllerAxis();

        List<string> valuesPressedThisTurn = new List<string>();
        foreach (KeyValuePair<string, string> kv in controllerInputSpellMap[loadoutValue])
        {
            float inputv = Input.GetAxis(kv.Key);
            if (inputv > .5f)
            {
                pcs.SpellPressed(kv.Value);
                valuesPressedThisTurn.Add(kv.Key);
            }
            else
            {
                if (KeysPressedLastCycle.Contains(kv.Key))
                {
                    pcs.SpellReleased(kv.Value);
                }
            }
        }

        foreach (KeyValuePair<string, string> kv in controllerAbilityMap[loadoutValue])
        {
            float inputv = Input.GetAxis(kv.Key);
            if (inputv > .5f)
            {
                AbilityPressed(kv.Value);
                valuesPressedThisTurn.Add(kv.Key);
            }
            else
            {
                if (KeysPressedLastCycle.Contains(kv.Key))
                    AbilityReleased(kv.Value);
            }
        }

        KeysPressedLastCycle = valuesPressedThisTurn;*/
    }

    private void AbilityPressed(string abilityName)
    {
        /*
        switch (abilityName)
        {
            case "jump":
                //playerBody.jumpedThisCycle = true;
                pcs.Jump();
                break;
            case "down":
                //playerBody.isCrouched = true;
                pcs.Crouch();
                break;
            case "grab":
                pcs.Climb();
                break;
            case "aim up":
                pcs.UpdateReticleAngle(1);
                break;
            case "aim down":
                pcs.UpdateReticleAngle(-1);
                break;
            case "fly":
                pcs.Fly();
                break;
            case "cycleLeft":
                if (!cycleLeftLock)
                {
                    loadoutValue--;
                    if (loadoutValue < 0)
                        loadoutValue = 2;
                    cycleLeftLock = true;
                }
                break;
            case "cycleRight":
                if (!cycleRightLock)
                {
                    loadoutValue++;
                    loadoutValue %= 3;
                    cycleRightLock = true;
                }
                break;
            
            default:
                break;
        }*/
    }

    public override void AddSpellAndKeyCode(string spellName, string keycodeAsString)
    {
        /*char inputLevelChar = keycodeAsString[0];
        int inputLevel = 0;
        string keycodeProper = keycodeAsString;

        try
        {
            inputLevel = int.Parse(inputLevelChar.ToString());
            keycodeProper = keycodeAsString.Substring(1, keycodeAsString.Length - 1);
        }
        catch
        {
            inputLevel = 0;
        }
        
        string axisKey = UserInputedKeycodeToAxisKeycode(keycodeProper);
        if (controllerInputSpellMap[inputLevel].ContainsKey(axisKey))
        {
            Debug.Log("keycode: " + axisKey + " for level: " + inputLevel + " already contained spell: " + controllerInputSpellMap[inputLevel][axisKey] + " replacing with: " + spellName);
            controllerInputSpellMap[inputLevel][axisKey] = spellName;
        }
        else
        {
            controllerInputSpellMap[inputLevel].Add(axisKey,spellName);
        }*/
    }

    private string UserInputedKeycodeToAxisKeycode(string userkeycode)
    {
        switch (userkeycode.ToUpper())
        {
            case "A":
                return "ControllerA";
            case "X":
                return "ControllerX";
            case "Y":
                return "ControllerY";
            case "B":
                return "ControllerB";
            case "RT":
                return "ControllerRightTrigger";
            case "RB":
                return "ControllerRightBumper";
            case "LB":
                return "ControllerLeftBumper";
            case "LT":
                return "ControllerLeftTrigger";
            default:
                Debug.LogError("userkeycode: " + userkeycode + " unhandled, set to default A key");
                return "ControllerA";
        }
    }

    private void AbilityReleased(string abilityName)
    {
        /*switch (abilityName)
        {
            case "cycleRight":
                cycleRightLock = false;
                break;
            case "cycleLeft":
                cycleLeftLock = false;
                break;
            default:
                break;
        }*/
    }

    private void CheckControllerAxis()
    {
        /*float horzValue = Input.GetAxis("Horizontal");
        float vertValue = Input.GetAxis("Vertical");

        if (horzValue > .5f)
            pcs.abilityManager.UpdateKeys("moveLeft", GV.AbilityTriggerType.Pressed);
        else if (horzValue < -.5f)
            pcs.abilityManager.UpdateKeys("moveLeft", GV.AbilityTriggerType.Pressed);

        if (vertValue > .5f)
            pcs.UpdateReticleAngle(1);
        else if (vertValue < -.5f)
            pcs.UpdateReticleAngle(-1);

        float dpadYValue = Input.GetAxis("ControllerDY");
        if (dpadYValue > .5f)
            pcs.cameraControl.ModZoom(-2 * Time.deltaTime);
        else if (dpadYValue < -.5f)
            pcs.cameraControl.ModZoom(2 * Time.deltaTime);*/

    }

    public override int GetLoadoutValue()
    {
        return loadoutValue;
    }

    public virtual void AddAbilityAndKeycode(Ability ability, string _keycode)
    {
       /* if (controllerAbilityMap.ContainsKey(_keycode))
        {
            pcs.abilityManager.RemoveAbilityKey(controllerAbilityMap[_keycode].abilityName, _keycode);
            controllerAbilityMap[_keycode] = ability;
            pcs.abilityManager.AddAbilityKey(ability.abilityName, _keycode);
        }
        else
        {
            controllerAbilityMap.Add(_keycode, ability);
            pcs.abilityManager.AddAbilityKey(ability.abilityName, _keycode);
        }*/
    }
}
