using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellLayerManager : MonoBehaviour {
    //http://answers.unity3d.com/questions/8715/how-do-i-use-layermasks.html

    SpellInfo si;
    float freshSpellTimer = GV.CASTER_IMMUNITY;
    bool fired = false;
    int pid;
    bool casterIsSpell;
    bool updating = false;

    string casterAllign;
    string spellState; 
    string ignoreAvatar;

    public void Initialize(int _pid, bool _casterIsSpell, SpellInfo _si)
    {
        pid = _pid;
        si = _si;
        casterIsSpell = _casterIsSpell; //doesnt matter anymore
        casterAllign = (pid == 0) ?"P":"O";  //player or opponent, where player allies are player
        spellState = "C"; //Since it's charging, not (A)ctive
        ignoreAvatar = (_si.interactionParams.Contains(GV.InteractionType.Avatar_Collision)) ? "IA" : "";
        //prefix += (casterIsSpell) ? "Spell" : ""; doesnt matter anymore
        GV.SetAllChildLayersRecurisvely(transform, GetSetLayerName());
    }

    private string GetSetLayerName()
    {
        return casterAllign + spellState + ignoreAvatar;
    }

    private int GetLayerMask(string _layerName)
    {
        int layerMask = 1 << LayerMask.NameToLayer(_layerName);
        return layerMask;
    }

    public void Update()
    {
        if (!updating)
            return;

       if (freshSpellTimer > 0) //Only upgdating for this
       {
           freshSpellTimer -= Time.deltaTime;
           if (freshSpellTimer <= 0)
               SpellMatures();
       }                
    }

    public void FireSpellThatIgnoresAllAvatars()
    {
        GV.SetAllChildLayersRecurisvely(transform, "SpellIgnoreAvatar");
        updating = false;
    }

    public void FireSpell()
    {
        updating = true;
    }

    public void SpellMatures()
    {
        spellState = "A";
        GV.SetAllChildLayersRecurisvely(transform, GetSetLayerName());
        updating = false;
    }
}
