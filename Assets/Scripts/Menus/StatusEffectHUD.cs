using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatusEffectHUD : MonoBehaviour {

    string s = "Prefabs/Menus/StatusEffectHudElement";
    PlayerControlScript pcs;
    public Dictionary<string, StatusEffectHudElement> activePanels = new Dictionary<string, StatusEffectHudElement>();

    public void Initialize(PlayerControlScript _pcs)
    {
        pcs = _pcs;
    }

    private void OpenStatusEffectMenu()
    {
        /*Dictionary<string, string> toOut = new Dictionary<string, string>();
        List<SkillModifier> playersSkillMods = GV.worldUI.players[0].stats.getAllSkillModifiers();
        string _desc = "All active SkillMods, Only shows effect power atm";

        foreach (SkillModifier sm in playersSkillMods)
            toOut.Add(sm.skillName, sm.effectPower.ToString());

        OpenMenu(toOut, _desc);*/
    }

    private void OpenStatsMenu()
    {
        /*
        Dictionary<string, string> toOut = new Dictionary<string, string>();
        Dictionary<string, Skill> skills = GV.worldUI.players[0].stats.Skills;
        string _desc = "All current values of stats, include all modifers";

        foreach (KeyValuePair<string, Skill> kv in skills)
            toOut.Add(kv.Key, kv.Value.get().ToString());

        OpenMenu(toOut, _desc);*/
    }

    public void ToggleStatusMenu()
    {

    }

    public void UpdateDisplay()
    {
        List<SkillModifier> playersSkillMods = GV.worldUI.players[0].stats.getAllSkillModifiers();
        List<string> activeSkillModNames = new List<string>();

        foreach (SkillModifier sm in playersSkillMods)
        {
            if(!activePanels.ContainsKey(sm.skillName))
            {
                GameObject go = Instantiate(Resources.Load(s)) as GameObject;
                go.transform.SetParent(transform, false);
                StatusEffectHudElement sehe = go.GetComponent<StatusEffectHudElement>();
                sehe.timerBar.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
                activePanels.Add(sm.skillName, sehe);
            }
            activePanels[sm.skillName].displayText.text = sm.skillName + ":" + sm.getEff();
            activePanels[sm.skillName].timerBar.fillAmount = sm.percentTimeRemaining();
            activeSkillModNames.Add(sm.skillName);
            //Debug.Log("skill mod detected: " + sm.skillName);
        }

        List<string> toRemove = new List<string>();
        foreach(KeyValuePair<string,StatusEffectHudElement> kv in activePanels)
        {
            if (!activeSkillModNames.Contains(kv.Key))
                toRemove.Add(kv.Key);
        }

        foreach (string removeString in toRemove)
        {
            Destroy(activePanels[removeString].gameObject);
            activePanels.Remove(removeString);
        }
    }
}
