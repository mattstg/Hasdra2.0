using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerGuiController : MonoBehaviour {

    PlayerControlScript player;
    GUIOrganizer playerGUI;

    float initialUIHeight_hp, initialUIHeight_mana, initialUIHeight_stanima, initialUIHeight_concuss;

    public void Initialize(GUIOrganizer guiOrganizer, PlayerControlScript pcs, int playerNum)
    {
        player = pcs;
        playerGUI = guiOrganizer;
        playerGUI.gameObject.layer = LayerMask.NameToLayer("Player" + playerNum + "GUI");
        if (playerNum == 1)
        {
            Vector3 curPos = guiOrganizer.GetComponent<Transform>().position;
            curPos.y -= Screen.height / 2;
            guiOrganizer.GetComponent<Transform>().position = curPos;
        }
        playerGUI.statusEffectHud.Initialize(pcs);
    }

    public void UpdateDisplay()
    {
        float concussMax = player.stats.getSkillValue("concussMax");

        playerGUI.hpBar.value      = (player.stats.healthPoints / player.stats.maxHp);
        playerGUI.manaBar.value    = (player.stats.energy / player.stats.maxEnergy);
        playerGUI.staminaBar.value = (player.stats.stamina / player.stats.maxStamina);
        playerGUI.concussBar.value = (player.stats.concusionScore % concussMax) / concussMax;

        playerGUI.hpTxt.text = FormatBarOutput(player.stats.healthPoints,player.stats.maxHp);
        playerGUI.manaTxt.text    = FormatBarOutput(player.stats.energy,player.stats.maxEnergy);
        playerGUI.staminaTxt.text = FormatBarOutput(player.stats.stamina,player.stats.maxStamina);
        playerGUI.concussTxt.text = FormatBarOutput(player.stats.concusionScore % concussMax,concussMax);

        playerGUI.concussCount.text = ((int)(player.stats.concusionScore / concussMax)).ToString();

        playerGUI.levelBar.text = (player.levelUpManager.experienceStored / GV.EXPERIENCE_PER_LEVEL).ToString();
        playerGUI.loadoutText.text = player.inputMap.GetLoadoutValue().ToString();

        playerGUI.statusEffectHud.UpdateDisplay();
    }

    private string FormatBarOutput(float _value, float _max)
    {
        return (int)_value + "/" + (int)_max;
    }
}
