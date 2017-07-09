using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TesterDebugMenu : MonoBehaviour {

    public InputField statStr;
    public InputField statCons;
    public InputField statAgi;
    public InputField statDex;
    public InputField statWis;
    public InputField statInt;
    public InputField statChar;

    Dictionary<GV.Stats, InputField> statFeilds;
    Dictionary<string, DebugUIComponent> uiGridComponent = new Dictionary<string, DebugUIComponent>();

    public Transform componentGrid;

    void Start()
    {
        statFeilds = new Dictionary<GV.Stats,InputField>();
        statFeilds.Add(GV.Stats.Str,statStr);
        statFeilds.Add(GV.Stats.Const,statCons);
        statFeilds.Add(GV.Stats.Agi,statAgi);
        statFeilds.Add(GV.Stats.Dex,statDex);
        statFeilds.Add(GV.Stats.Wis,statWis);
        statFeilds.Add(GV.Stats.Int,statInt);
        statFeilds.Add(GV.Stats.Char,statChar);
        SetToCurrentStatValues();
        AddRequestedUIGridComponent("Body Stats");
    }

    public void ValueAltered(string valueName)
    {
        GV.Stats statAltered = GV.ParseEnum<GV.Stats>(valueName);
        float newValue = float.Parse(statFeilds[statAltered].text);
        GV.worldUI.players[0].stats.setParentStat(statAltered, newValue);
    }

    public void SetToCurrentStatValues()
    {
        foreach (KeyValuePair<GV.Stats, InputField> kv in statFeilds)
            kv.Value.text = GV.worldUI.players[0].stats.getParentStat(kv.Key).ToString();
    }

    //Getting the list of 
    private void AddRequestedUIGridComponent(string _name)
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/Menus/GridMenuComponent")) as GameObject;
        go.transform.SetParent(componentGrid);
        DebugUIComponent duc = go.AddComponent<ducBodyStats>();
        uiGridComponent.Add(_name, duc );
        duc.Initialize(_name, DucType.BodyStats);
    }
    
}

