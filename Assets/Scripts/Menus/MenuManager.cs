using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour {

    GenericPopup genericMenu;

    public void Start()
    {
        genericMenu = GV.worldUI.genericMenu;
    }

    public void OpenMenu(GV.MenuType menuTypeToOpen)
    {
        switch (menuTypeToOpen)
        {
            case GV.MenuType.debugControls:
                OpenDebugMenu();
                break;
            case GV.MenuType.spells:
                OpenSpellMenu();
                break;
            case GV.MenuType.statusEffects:
                OpenStatusEffectMenu();
                break;
            case GV.MenuType.stats:
                OpenStatsMenu();
                break;
            case GV.MenuType.level:
                break;
        }
    }

    private void OpenDebugMenu()
    {
        Dictionary<string, string> toMenu = new Dictionary<string, string>();
        toMenu.Add("Gain Xp, Spells lose gravity and destabilize", "PageUp");
        toMenu.Add("Lower concusion threshold","PageDown");
        toMenu.Add("Home", "Increase AGI by 10");
        toMenu.Add("Spells kinematic and then grow to 200","End");
        toMenu.Add("Zoom out","+");
        toMenu.Add("Zoom in","-");
        toMenu.Add("toggle mouse cutting ground tool", "0");
        toMenu.Add("visible debug numbers","\\");
        toMenu.Add("Hairline fractures","LeftClick & LeftShift");
        toMenu.Add("Skillmod menu","Tab");
        toMenu.Add("debug control keys","Keypad7");
        toMenu.Add("spell keys","Keypad8");
        toMenu.Add("stats menu","Keypad9");
        OpenMenu(toMenu, "Magic hotkeys");
    }

    private void OpenSpellMenu()
    {
        PlayerControlScript player = GV.worldUI.players[0];
        string _desc = "All currently learnt spells with their keymapping";
        OpenMenu(player.inputMap.ListAllSpellsLearnt(), _desc);
    }

    private void OpenStatusEffectMenu()
    {
        Dictionary<string, string> toOut = new Dictionary<string, string>();
        List<SkillModifier> playersSkillMods = GV.worldUI.players[0].stats.getAllSkillModifiers();
        string _desc = "All active SkillMods, Only shows effect power atm";

        foreach (SkillModifier sm in playersSkillMods)
            toOut.Add(sm.skillName, sm.effectPower.ToString());

        OpenMenu(toOut, _desc);
    }

    private void OpenStatsMenu()
    {
        Dictionary<string,string> toOut = new Dictionary<string,string>();
        Dictionary<string, Skill> skills = GV.worldUI.players[0].stats.Skills;
        string _desc = "All current values of stats, include all modifers";

        foreach (KeyValuePair<string, Skill> kv in skills)
            toOut.Add(kv.Key, kv.Value.get().ToString());

        OpenMenu(toOut, _desc);
    }

    private void OpenLevelMenu()
    {

    }

    private void OpenMenu(Dictionary<string, string> toOut, string desc)
    {
        genericMenu.CloseMenu();
        genericMenu.gameObject.SetActive(true);
        genericMenu.AddMenuItems(toOut);
        genericMenu.SetDesc(desc);
    }

    public void CloseMenu(GV.MenuType menuTypeToOpen)
    {

    }
}
