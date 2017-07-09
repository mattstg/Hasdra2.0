using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class SaveLoader : MonoBehaviour {
    public int currentStateID = 1; //the one being allocated to new
    public TreeTracker treeTracker;
    public InputField spellNameInput;
    public InputTagManager tagManager;
    XMLEncoder xmlEncoder = new XMLEncoder();
    TransitionCreator transitionCreator;
    List<TransitionGUI> allTransGuis = new List<TransitionGUI>();
    List<StateCompactGUI> stateGUIs = new List<StateCompactGUI>();  //top level for state guis

    public void Start()
    {
       transitionCreator = GameObject.FindObjectOfType<TransitionCreator>();
       //create default start state
       StateCompactGUI sgc = CreateNewState();
       sgc.AddNewStateSlot(GV.States.StartState);
       tagManager = gameObject.GetComponent<InputTagManager>();
       tagManager.Initialize(GV.fileLocationType.Spells);
    }

    public void SaveButtonPressed()
    {
        if (spellNameInput.text == "")
            return;
        foreach (TransitionGUI tg in allTransGuis)
            tg.ExitTransition(); //which will also save the values of each SS
        foreach (StateCompactGUI sgc in stateGUIs)
            sgc.ContractPressed(); //closes and saves all

        GV.smUiLayer.currentSpellLoaded.text = spellNameInput.text;
        xmlEncoder.DictionaryListToXML<string>(spellNameInput.text, xmlEncoder.StateAndTransitionsToDictionaryList(stateGUIs, allTransGuis), GV.fileLocationType.Spells);
        tagManager.SetCurrentKey(spellNameInput.text);
        tagManager.Save();
    }

    private void LoadSpell(string spellToLoad)
    {
        ClearCurrentStateMachine(false);
        spellNameInput.text = spellToLoad;
        List<Dictionary<string, string>> xmlAsDictionary = XMLEncoder.XmlToDictionaryList(spellToLoad, GV.fileLocationType.Spells);
        if (xmlAsDictionary == null)
        {
            Debug.Log(spellToLoad + " spell does not exist");
            return;
        }
        //the reason for 3 foreach loops is to ensure loading order
        foreach (Dictionary<string, string> dict in xmlAsDictionary)
        {
            if (dict["SuperType"] == "State")
            {
                GameObject go = Instantiate(Resources.Load("Prefabs/StatePrefabs/StateCompactGUI")) as GameObject;
                go.transform.SetParent(GV.smUiLayer.stateParent);
                go.transform.position = new Vector3(float.Parse(dict["PosX"]), float.Parse(dict["PosY"]), 0);

                StateCompactGUI sgc = go.GetComponent<StateCompactGUI>();
                State loadedState = new State();
                loadedState.StateID = int.Parse(dict["ID"]);
                treeTracker.AddState(loadedState);
                loadedState.ImportFromDictionary(dict);
                loadedState.FillGUIFromInternalVars();
                AddStateGUICompact(sgc);
                sgc.LoadExisting(loadedState);

                if (currentStateID <= int.Parse(dict["ID"]))
                    currentStateID = int.Parse(dict["ID"]) + 1;
            }
        }

        foreach (Dictionary<string, string> dict in xmlAsDictionary)
            if (dict["SuperType"] == "Transition")
                transitionCreator.LoadTransitionFromXMLLoader(dict, treeTracker);


        foreach (Dictionary<string, string> dict in xmlAsDictionary)
        {
            if (dict["SuperType"] == "TransSlotStruct")
            {
                Transition trans = treeTracker.GetTransitionByID(int.Parse(dict["parentID"]));
                TransSlotStruct newSlotStruct = new TransSlotStruct();
                newSlotStruct.ImportFromXML(dict);
                trans.AddSlotStruct(newSlotStruct);
            }
        }

        foreach (Dictionary<string, string> dict in xmlAsDictionary)
        {
            if (dict["SuperType"] == "StateSlot")
            {
                State state = treeTracker.GetStateByID(int.Parse(dict["parentID"]));
                StateSlot newStateSlot = StateSlot.CreateStateSlot(GV.ParseEnum<GV.States>(dict["stateSlotType"]), state);
                newStateSlot.ImportFromDictionary(dict, treeTracker);
                StateCompactGUI sgc = GetStateGUICompactByID(int.Parse(dict["parentID"]));
                sgc.stateGUI.AddExistingStateSlot(newStateSlot);
                sgc.AddCompactListElement(newStateSlot);
            }
        }
        
        GV.smUiLayer.currentSpellLoaded.text = spellToLoad;
        tagManager.SetCurrentKey(spellToLoad);
        tagManager.LoadAllTagSlots();
    }

    public void LoadButtonPressed()
    {
        string spellName = spellNameInput.text; 
        List<string> allSpellNames = LiveSpellDict.GetAllSpellNames();

        if(allSpellNames.Contains(spellName))
        {
            LoadSpell(spellNameInput.text);     
        }
        else
        {
            OpenLoadExistingMenu();
        }
           
    }

    public void ClearCurrentStateMachine(bool remakeStartState)
    {
        treeTracker.ClearTree();
        allTransGuis.Clear();
        stateGUIs.Clear();
        spellNameInput.text = "";
        currentStateID = 1;
        if (remakeStartState)
        {
            StateCompactGUI sgc = CreateNewState();
            sgc.AddNewStateSlot(GV.States.StartState);            
        }
        GV.smUiLayer.currentSpellLoaded.text = "None";
        tagManager.SetCurrentKey("");
        tagManager.ClearAllTagSlots();
    }

    public void AddNewTransGUI(TransitionGUI newGUI)
    {
        if (!allTransGuis.Contains(newGUI))
            allTransGuis.Add(newGUI);
    }

    public void RemoveTransGUI(TransitionGUI newGUI)
    {
        allTransGuis.Remove(newGUI);
    }

    public void AddStateGUICompact(StateCompactGUI newGUI)
    {
        if (!stateGUIs.Contains(newGUI))
            stateGUIs.Add(newGUI);
        else
            Debug.Log("double thing");
    }

    public void RemoveStateGUICompact(StateCompactGUI newGUI)
    {
        stateGUIs.Remove(newGUI);
    }

    public StateCompactGUI GetStateGUICompactByID(int id)
    {
        return stateGUIs.Single(item => item.stateGUI.state.StateID == id);
    }


    public void CreateNewStateButtonPressed()
    {
        CreateNewState();
    }
    //when the create new state button is pressed, an empty state
    public StateCompactGUI CreateNewState()
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/StatePrefabs/StateCompactGUI")) as GameObject;
        StateCompactGUI sgc = go.GetComponent<StateCompactGUI>();
        sgc.InitializeNew();
        sgc.stateGUI.state = new State();
        AddStateGUICompact(sgc);
        sgc.stateGUI.state.StateID = currentStateID++;
        go.transform.SetParent(GV.smUiLayer.stateParent);
        go.transform.position = GV.smUiLayer.newStateStartPos.position;
        
        treeTracker.AddState(sgc.stateGUI.state);
        return sgc;
    }

    private void OpenLoadExistingMenu()
    {
        GenericPopup loadExistingSpellPopup = GenericPopup.CreateGenericPopup();
        loadExistingSpellPopup.SetDesc("Spell trying to load does not exist, select from the following existing spells");
        List<string> allSpellNames = LiveSpellDict.GetAllSpellNames();
        foreach (string spellname in allSpellNames)
        {
            loadExistingSpellPopup.AddButton(spellname, LoadSpell, spellname);
        }
    }

    public void LoadTestLevel()
    {
        string spellToTest = spellNameInput.text;
        if (spellToTest != "")
        {
            SaveButtonPressed();
            CreateTestCharacter(spellToTest);
            GameObject persistantObject = new GameObject();
            PersistantRoundSetupInfo info = persistantObject.AddComponent<PersistantRoundSetupInfo>();
            persistantObject.AddComponent<PITestGuiTrigger>();
            GameObject.DontDestroyOnLoad(persistantObject);
            info.avatarBlueprints.Add(new AvatarBlueprint(0, "spellTesterCharacter","Default",7, "Origins"));
            info.levelNameSelected = "Origins";
            UnityEngine.SceneManagement.SceneManager.LoadScene("WorldScene");
        }
    }

    private void CreateTestCharacter(string spellNameToTest)
    {
        LevelTracker levelTracker = new LevelTracker("spellTesterCharacter", 2);
        LevelPackage[] lvlPackages = new LevelPackage[8];
        lvlPackages[0] = new LevelPackage(GV.LevelPkgType.Spell, spellNameToTest, "F", GV.EXPERIENCE_PER_LEVEL);
        lvlPackages[1] = new LevelPackage(GV.LevelPkgType.Ability, "Ab:jump", "G", GV.EXPERIENCE_PER_LEVEL);
        lvlPackages[2] = new LevelPackage(GV.LevelPkgType.Stat, "Str", "", GV.EXPERIENCE_PER_LEVEL);
        lvlPackages[3] = new LevelPackage(GV.LevelPkgType.Stat, "Agi", "", GV.EXPERIENCE_PER_LEVEL);
        lvlPackages[4] = new LevelPackage(GV.LevelPkgType.Stat, "Wis", "", GV.EXPERIENCE_PER_LEVEL);
        lvlPackages[5] = new LevelPackage(GV.LevelPkgType.Stat, "Int", "", GV.EXPERIENCE_PER_LEVEL);
        lvlPackages[6] = new LevelPackage(GV.LevelPkgType.Stat, "Const", "", GV.EXPERIENCE_PER_LEVEL);
        lvlPackages[7] = new LevelPackage(GV.LevelPkgType.Stat, "Char", "", GV.EXPERIENCE_PER_LEVEL);

        foreach (LevelPackage lvlPkg in lvlPackages)
            levelTracker.AddNewLevel(lvlPkg);

        XMLEncoder xmlEncoder = new XMLEncoder();
        xmlEncoder.LevelDictionaryToXML(levelTracker);
    }

    public void AddTagPressed()
    {

    }

    public void TagRemoved()
    {

    }
}
