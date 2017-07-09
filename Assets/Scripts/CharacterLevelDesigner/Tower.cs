using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Tower : MonoBehaviour {

    public Transform levelUpSlotGrid;

    bool loopPreviewActive = false;
    float loopPreviewCountdown;
    List<Transform> loopPreviewSelected = new List<Transform>();
    
   /* public List<MTuple<string, string>> GetAllStacks()
    {
        List<MTuple<string, string>> toReturn = new List<MTuple<string, string>>();
        foreach (Transform lvlSlot in levelUpSlotGrid) //is this same as GetObjectOfType
        {
            LevelUpSlot lvs = lvlSlot.GetComponent<LevelUpSlot>();
            if(lvs.currentLevelOption != "")
                toReturn.Add(new MTuple<string, string>(lvs.currentLevelOption, lvs.keyTrigger));
        }
        return toReturn;
    }*/

    public void LoadStacks(Stack<string> levelStack)
    {
        //its a reference, so take what i need and give back, return false when empty
        //V.STAT_ALL_NAMES.Contains(value)  is how we check if the next value is a key or not
    }

    //ypos of how far down its scrolled plus the mouse
    public void AddAtPos(float yPos, string _name, GV.LevelPkgType lvlPkgType)
    {
        int index = (int)(yPos / GV.PLAYERLEVELDESIGN_SLOT_HEIGHT);
        CreateSlotAtIndex(index, _name, lvlPkgType);
    }

    public void CreateSlotAtIndex(int index,string _name, GV.LevelPkgType lvlPkgType, string _keycode = "")
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/CharacterProgression/LevelUpSlot")) as GameObject;
        go.GetComponent<LayoutElement>().preferredHeight = GV.PLAYERLEVELDESIGN_SLOT_HEIGHT;
        go.transform.SetParent(levelUpSlotGrid);
        go.transform.SetSiblingIndex(index);
        go.GetComponent<LevelUpSlot>().Initialize(_name, _keycode, lvlPkgType);        
        UpdateAllLevelText();
    }

    public void UpdateAllLevelText()
    {
        foreach (Transform t in levelUpSlotGrid)
            if (t != levelUpSlotGrid)
                t.GetComponent<LevelUpSlot>().level = t.GetSiblingIndex();
    }

    public void DisplayLoopAmtPreview()
    {
        ResetLoopPreview();
        int amtToPreview = int.Parse(GV.charLvlUI.loopAmtInput.text);
        amtToPreview = (amtToPreview > levelUpSlotGrid.childCount)? levelUpSlotGrid.childCount:amtToPreview;
        
        for (int i = levelUpSlotGrid.childCount - 1; i > levelUpSlotGrid.childCount - 1 - amtToPreview; i--)
            loopPreviewSelected.Add(levelUpSlotGrid.GetChild(i));            
     
        loopPreviewActive = true;
        loopPreviewCountdown = GV.PLAYERLEVELDESIGN_PREVIEW_COUNTDOWN;
    }

    public void Update()
    {
        if (loopPreviewActive)
        {
            foreach (Transform t in loopPreviewSelected)
            {
                if (t == null)
                {
                    loopPreviewCountdown = 0;
                    break;
                }
                t.GetComponent<Image>().color = Color.Lerp(Color.white, Color.red, loopPreviewCountdown / GV.PLAYERLEVELDESIGN_PREVIEW_COUNTDOWN);
            }
            loopPreviewCountdown -= Time.deltaTime;

            if (loopPreviewCountdown <= 0)
                ResetLoopPreview();
        }
    }

    private void ResetLoopPreview()
    {
        foreach (Transform t in loopPreviewSelected)
            if(t != null)
                t.GetComponent<Image>().color = Color.white;
        loopPreviewSelected.Clear();
        loopPreviewCountdown = GV.PLAYERLEVELDESIGN_PREVIEW_COUNTDOWN;
        loopPreviewActive = false;
    }

    public void LoadLevelTracker(LevelTracker lvlTracker)
    {
        ClearTower();
        GV.charLvlUI.loopAmtInput.text = lvlTracker.loopAmt.ToString();
        GV.charLvlUI.characterName.text = lvlTracker.avatarName;
        int indexCount = 0;
        while (lvlTracker.levelUpStack.Count != 0)
        {
            LevelPackage lvlPkg = lvlTracker.levelUpStack.Dequeue();
            if (GV.STAT_ALL_NAMES.Contains(lvlPkg.pkgName))
            {
                CreateSlotAtIndex(indexCount, lvlPkg.pkgName, GV.LevelPkgType.Stat);
            }
            else //else is spell, need the key
            {
                string _keycode = lvlPkg.activationKey;
                CreateSlotAtIndex(indexCount, lvlPkg.pkgName, lvlPkg.levelPkgType,  _keycode );
            }
            indexCount++;
        }
    }

    public void ClearTower()
    {
        foreach (Transform t in levelUpSlotGrid)
            if (t != levelUpSlotGrid)
                Destroy(t.gameObject);
        GV.charLvlUI.loopAmtInput.text = "0";
        GV.charLvlUI.characterName.text = "";
    }
}
