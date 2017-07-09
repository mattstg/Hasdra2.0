using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MainLevelDesignerScript : MonoBehaviour {

    int availableVisibleSlots = 25;
    int currentPage = 0;

    XMLEncoder xmlEncoder;
    FilterTagManager tagManager;
    public Transform mainCanvas;
    List<string> allStatNames    = new List<string>();
    List<string> allSkillNames   = new List<string>();
    List<string> allSpellNames   = new List<string>();
    List<string> allAbilityNames = new List<string>();
    List<string> allPassiveNames = new List<string>();
    string categorySelected = "";
    string currentSearchBar = ""; //player inputed search bar


    public void Start()
    {
        tagManager = GameObject.FindObjectOfType<FilterTagManager>();
        xmlEncoder = new XMLEncoder();
        categorySelected = "Stats";
        LoadUpgradeSelectionBox();
        tagManager.Initialize(GV.fileLocationType.Spells);
    }

    /// ///// initialLoading
    private void LoadUpgradeSelectionBox()
    {
        LoadAllStats();
        LoadAllSpells();
        LoadAllAbilities();
        LoadAllSkills();
        LoadListToBoxes(allStatNames);
    }

    public void CategorySelected(string catName)
    {
        tagManager.SetVisible(catName == "Spells");
        categorySelected = catName;        
        RefreshBoxes();
        SetExpCost(catName);
    }

    private void SetExpCost(string catSelected)
    {
        float expCost = 0;
        switch (catSelected)
        {
            case "Stats":
                expCost = 10;
                break;
            case "Skills":
                expCost = 4;
                break;
            case "Spells":
                expCost = 20;
                break;
            case "Abilities":
                expCost = 10;
                break;
            case "Passives":
                expCost = 30;
                break;
            default:
                break;
        }
        GV.charLvlUI.expCost.text = "Exp Cost: " + expCost;
    }

    public void RefreshBoxes()
    {
        List<string> toLoad = GetList(categorySelected);
    
        if(categorySelected == "Spells")
            toLoad = GameObject.FindObjectOfType<FilterTagManager>().FilterResults(toLoad);

        if (currentSearchBar != "")
        {
            List<string> toLoadPostSearchFilter = new List<string>();
            string currentSearchBarFix = currentSearchBar.ToLower().ToString();
            foreach (string toFilter in toLoad)
            {
                string toFilterFix = toFilter.ToLower().ToString(); //stranegness require cast (not cause of case tho??)
                if (toFilterFix.Contains(currentSearchBarFix))
                    toLoadPostSearchFilter.Add(toFilter);
            }
            toLoad = toLoadPostSearchFilter;
        }
        LoadListToBoxes(toLoad);
    }

    private List<string> GetList(string category)
    {
        List<string> toRet;
        switch (category)
        {
            case "Stats":
                toRet = allStatNames;
                break;
            case "Skills":
                toRet = allSkillNames;
                break;
            case "Spells":
                toRet = allSpellNames;
                break;
            case "Abilities":
                toRet = allAbilityNames;
                break;
            case "Passives":
                toRet = allPassiveNames;
                break;
            default:
                Debug.LogError(string.Format("Unhandled switch case {0}", categorySelected));
                toRet = allStatNames;
                break;
        }
        return toRet;
    }

    private GV.LevelPkgType lvlPkgTypeFromCategory(string category)
    {
        switch (category)
        {
            case "Stats":
                return GV.LevelPkgType.Stat;            
            case "Spells":
                return GV.LevelPkgType.Spell;
            case "Abilities":
                return GV.LevelPkgType.Ability;
            case "Skills":
                return GV.LevelPkgType.Skill;
            case "Passives":            
            default:
                Debug.LogError(string.Format("Unhandled switch case {0}", categorySelected));
                return GV.LevelPkgType.Spell;
        }
    }

    private void LoadAllStats()
    {
        allStatNames.AddRange(GV.STAT_ALL_NAMES);
    }

    private void LoadAllAbilities()
    {
        List<string> abilityNames = AbilityDict.Instance.GetAllAbilityNames();
        foreach (string toAdd in abilityNames)
            allAbilityNames.Add(toAdd);
    }

    private void LoadAllSpells()
    {
        foreach (string file in Directory.GetFiles(XMLEncoder.GetFilePathByType(GV.fileLocationType.Spells)))
        {
            if (Path.GetExtension(file) == ".xml")
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                allSpellNames.Add(filename);
            }
        }
    }

    private void LoadAllSkills()
    {
        allSkillNames = BodyStatFiller.GetAllSkills();
    }

    private void LoadListToBoxes(List<string> toDisplay)
    {
        ClearBoxes();
        int indexAddition = (availableVisibleSlots * currentPage); //index added from current page number 
        for (int i = 0; i < availableVisibleSlots && i + indexAddition < toDisplay.Count; i++)
        {
            GameObject g = Instantiate(Resources.Load("Prefabs/CharacterProgression/LevelOption")) as GameObject;
            g.transform.SetParent(GV.charLvlUI.storeGrid);
            g.GetComponent<LevelUpOption>().Initialize(toDisplay[i + indexAddition], lvlPkgTypeFromCategory(categorySelected));
        }
    }

    private void ClearBoxes()
    {
        foreach (Transform t in GV.charLvlUI.storeGrid)
            if (t != GV.charLvlUI.storeGrid)
                Destroy(t.gameObject);
    }

    public void CreatingDraggingSlot(string slotName, GV.LevelPkgType pkgType)
    {
        GameObject g = Instantiate(Resources.Load("Prefabs/CharacterProgression/LevelOptionDragger")) as GameObject;
        g.transform.SetParent(GV.charLvlUI.canvas);
        g.GetComponent<LevelUpOption>().Initialize(slotName, pkgType);
        g.transform.position = Input.mousePosition;
    }

    public void DropDraggingSlot(LevelUpOption option)
    {
        Destroy(option.gameObject);
        float yLevel = GV.charLvlUI.tower.transform.position.y;

        if (Input.mousePosition.x <= GV.charLvlUI.tower.GetComponent<RectTransform>().rect.width)
            GV.charLvlUI.tower.AddAtPos(Mathf.Abs(yLevel) + Input.mousePosition.y, option.optionName, option.lvlPkgType);
    }
    /// ////////////////////

    public void SaveButton()
    {
        if (GV.charLvlUI.characterName.text == "")
            return;

        LevelTracker levelTracker = new LevelTracker(GV.charLvlUI.characterName.text,int.Parse(GV.charLvlUI.loopAmtInput.text));
        Transform gridTransform = GV.charLvlUI.tower.levelUpSlotGrid.transform;
        for (int i = 0; i < gridTransform.childCount; i++) 
        {
          LevelUpSlot lvlSlot = gridTransform.GetChild(i).GetComponent<LevelUpSlot>();
          LevelPackage toAdd;
          if (GV.STAT_ALL_NAMES.Contains(lvlSlot.slotName))
          {
              toAdd = new LevelPackage(GV.LevelPkgType.Stat, lvlSlot.slotName, "", GV.EXPERIENCE_PER_LEVEL);
          }
          else
          {
              toAdd = new LevelPackage(lvlSlot.lvlPkgType, lvlSlot.slotName, lvlSlot.keyTrigger, GV.EXPERIENCE_PER_LEVEL);
          }
          levelTracker.AddNewLevel(toAdd);
        }
        levelTracker.FinializeLoopAmt();
        xmlEncoder.LevelDictionaryToXML(levelTracker);
    }

    public void LoadButton()
    {
        string charName = GV.charLvlUI.characterName.text;
        List<string> allCharNames = XMLEncoder.LoadAllCharacterNames();

        if(allCharNames.Contains(charName))
        {
            LoadCharacter(charName);
        }
        else
        {
            OpenListOfLoadableCharacters();
        }
        
    }

    private void LoadCharacter(string charName)
    {
        GV.charLvlUI.characterName.text = charName;
        LevelTracker levelTracker = xmlEncoder.XMLToLevelTracker(charName, GV.fileLocationType.Characters);
        GV.charLvlUI.tower.LoadLevelTracker(levelTracker);
    }

    private void OpenListOfLoadableCharacters()
    {
        GenericPopup loadExistingSpellPopup = GenericPopup.CreateGenericPopup();
        loadExistingSpellPopup.SetAsCanvasLastChild();
        List<string> allCharNames = XMLEncoder.LoadAllCharacterNames();
        loadExistingSpellPopup.SetDesc("Loading char name does not exist, please select one of the following");
        
        foreach (string charName in allCharNames)
        {
            loadExistingSpellPopup.AddButton(charName, LoadCharacter, charName);
        }
    }

    public void TagPressed(string tag)
    {

    }

    public void SearchBarValueChanged()
    {
        currentSearchBar = GV.charLvlUI.searchBar.text;
        RefreshBoxes();
    }

    public void CycleButtonPressed(bool isNext)
    {
        int countOfActiveList = GetList(categorySelected).Count;
        //int pages
    }
}
