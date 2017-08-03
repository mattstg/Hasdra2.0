using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LiveSpellDict : MonoBehaviour {

    public Dictionary<string, SpellStorage> loadedSpells = new Dictionary<string, SpellStorage>();

    public SpellStorage GetSpell(string spellName)
    {
        spellName = spellName.ToLower();
        if (loadedSpells.ContainsKey(spellName))
        {           
            return loadedSpells[spellName];
        }
        Debug.LogError("Spell does not exist in LiveSpellDict: " + spellName);
        return null;
    }

    public void AddSpell(string _name, SpellStorage toAdd)
    {
        _name = _name.ToLower();
        if (!loadedSpells.ContainsKey(_name))
            loadedSpells.Add(_name, toAdd);
    }

    public void LoadAllSpells()
    {    
        foreach(string filenameAndExt in Directory.GetFiles(XMLEncoder.GetFilePathByType(GV.fileLocationType.Spells)))
        {

            if (Path.GetExtension(filenameAndExt) == ".xml")
            {
                string filename = Path.GetFileNameWithoutExtension(filenameAndExt);
                //Debug.Log("Loading: " + filename);
                AddSpell(filename, XMLDictListToSpell(filename, XMLEncoder.XmlToDictionaryList(filename, GV.fileLocationType.Spells),true));
            }
        }
        foreach (string filenameAndExt in Directory.GetFiles(XMLEncoder.GetFilePathByType(GV.fileLocationType.BasicSpells)))
        {

            if (Path.GetExtension(filenameAndExt) == ".xml")
            {
                string filename = Path.GetFileNameWithoutExtension(filenameAndExt);
                //Debug.Log("Loading: " + filename);
                AddSpell(filename, XMLDictListToSpell(filename, XMLEncoder.XmlToDictionaryList(filename, GV.fileLocationType.BasicSpells),true));
            }
        }
    }

    private string AllLoadedSpellNames()
    {
        string toOut = "";
        foreach (string k in loadedSpells.Keys)
            toOut += k + ",";
        return toOut;
    }


   /* public SpellStorage XMLBasicSpellToSpellStorage(string name, List<Dictionary<string, string>> spellDictToLoad)
    {
        //turn the list given into a dictionary       
        //Spell toReturn = new Spell();
        //Debug.Log("Loading spell: " + name);
        string spellType = "energy";
        TreeTracker treeTracker = new TreeTracker();
        foreach (Dictionary<string, string> dict in spellDictToLoad)
        {
            if (dict["SuperType"] == "State")
            {
                State toadd = new State();
                toadd.ImportFromDictionary(dict);
                treeTracker.AddState(toadd);
            }
        }

        List<SkillModSS> skillModSS = new List<SkillModSS>(); //chargables
        foreach (Dictionary<string, string> dict in spellDictToLoad)
        {
            if (dict["SuperType"] == "StateSlot")
            {
                int stateID = int.Parse(dict["parentID"]);
                GV.States slotType = GV.ParseEnum<GV.States>(dict["stateSlotType"]);
                State state = treeTracker.GetStateByID(stateID);
                StateSlot newStateSlot = StateSlot.CreateStateSlot(slotType, state);
                newStateSlot.ImportFromDictionary(dict, treeTracker);

                if (stateID == 0 && slotType == GV.States.SkillMod)
                {
                    newStateSlot.Initialize(null); //remove parent state
                    skillModSS.Add((SkillModSS)newStateSlot);
                }
                else
                {
                    state.AddStateSlot(newStateSlot);
                }

            }
        }


        List<SkillModSS> skillModSS = new List<SkillModSS>(); //chargables
      
        StateSlot startState = treeTracker.GetStartStateSlot();


        SpellStorage toReturn = new SpellStorage();
        toReturn.onChargeSkillMods = skillModSS;
        toReturn.spellForm = (GV.SpellForms)(int.Parse(spellDictToLoad["spellType"]));
        toReturn.startState = null;
        toReturn.stateMachine = null;
        toReturn.name = name;
        toReturn.spellInfo = new SpellInfo();
        toReturn.spellInfo.InitializeSpellInfo(startState);
        toReturn.isBasicSpell = true;
        return toReturn;
    }*/



    public SpellStorage XMLDictListToSpell(string name, List<Dictionary<string,string>> spellDictToLoad, bool initializeOnCreate)
    {
        //turn the list given into a dictionary       
        //Spell toReturn = new Spell();
        //Debug.Log("Loading spell: " + name);
        string spellType = "energy";
        TreeTracker treeTracker = new TreeTracker();        
        foreach (Dictionary<string, string> dict in spellDictToLoad)
        {
            if (dict["SuperType"] == "State")
            {
              State toadd = new State();
              toadd.ImportFromDictionary(dict);
              treeTracker.AddState(toadd);                        
            }            
        }
        foreach (Dictionary<string, string> dict in spellDictToLoad)
        {
            if (dict["SuperType"] == "Transition")
            {
                Transition t = new Transition(dict,treeTracker);
                treeTracker.AddTransition(t);
            }
        }

        foreach (Dictionary<string, string> dict in spellDictToLoad)
        {
            if (dict["SuperType"] == "TransSlotStruct")
            {
                Transition trans = treeTracker.GetTransitionByID(int.Parse(dict["parentID"]));
                TransSlotStruct newSlotStruct = new TransSlotStruct();
                newSlotStruct.ImportFromXML(dict);
                trans.AddSlotStruct(newSlotStruct);
            }
        }

        List<SkillModSS> skillModSS = new List<SkillModSS>(); //chargables
        foreach (Dictionary<string, string> dict in spellDictToLoad)
        {
            if (dict["SuperType"] == "StateSlot")
            {
                int stateID = int.Parse(dict["parentID"]);
                GV.States slotType = GV.ParseEnum<GV.States>(dict["stateSlotType"]);
                State state = treeTracker.GetStateByID(stateID);
                StateSlot newStateSlot = StateSlot.CreateStateSlot(slotType,state,initializeOnCreate);
                newStateSlot.ImportFromDictionary(dict, treeTracker);
                
                if(stateID == 0 && slotType == GV.States.SkillMod)
                {
                    newStateSlot.Initialize(null); //remove parent state
                    skillModSS.Add((SkillModSS)newStateSlot);
                }
                else
                {
                    state.AddStateSlot(newStateSlot);
                }
                
            }
        }

        StateSlot startState = treeTracker.GetStartStateSlot();
        SpellStorage toReturn = new SpellStorage();
        toReturn.onChargeSkillMods = skillModSS;
        toReturn.spellForm = startState.ssDict["SpellForm_Type"].CastValue<GV.SpellForms>();
        toReturn.startState = treeTracker.GetStartState();
        toReturn.stateMachine = new SpellStateMachine(treeTracker);
        toReturn.name = name;
        toReturn.spellInfo = new SpellInfo();
        toReturn.spellInfo.InitializeSpellInfo(startState);
        return toReturn;
    }

    //private StartState DictToStartState(Dictionary<string, string> startStateDict)
    //{
    //    StartState toReturn = new StartState();
    //    toReturn.ImportFromDictionary(startStateDict);
    //    return toReturn;
    //}


    //currently not in use
    //public void LoadAllSpells(List<string> spellsToLoad)
    //{
    //    foreach (string spellName in spellsToLoad)
    //    {
    //        if (File.Exists(XMLEncoder.GetFilePathByType(GV.fileLocationType.Spells) + "/" + spellName + ".xml"))
    //        {
    //            AddSpell(spellName, XMLDictListToSpell(spellName, XMLEncoder.XmlToDictionaryList(spellName, GV.fileLocationType.Spells)));
    //        }
    //        else
    //        {
    //        }
    //
    //    }
    //}

    public static List<string> GetAllSpellNames()
    {
        XMLEncoder xmlencoder = new XMLEncoder();
        List<string> toRet = new List<string>();

        foreach (string filenameAndExt in Directory.GetFiles(XMLEncoder.GetFilePathByType(GV.fileLocationType.Spells)))
        {

            if (Path.GetExtension(filenameAndExt) == ".xml")
            {
                string filename = Path.GetFileNameWithoutExtension(filenameAndExt);
                toRet.Add(filename);
            }
        }
        return toRet;
    }
}

