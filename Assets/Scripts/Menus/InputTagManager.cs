using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputTagManager : TagManager {

    string activeKey = "";
    List<string> currentOpenTags;
    

    public override void Initialize(GV.fileLocationType _tableType)
    {
        tagPrefab = "Prefabs/Menus/TagSlotInput";
        base.Initialize(_tableType);
        maxViewableTags = 10;
        currentOpenTags = new List<string>();
    }

    public void SetCurrentKey(string _keyName)
    {
        activeKey = _keyName;
    }

    public void LoadAllTagSlots()
    {
        ClearAllTagSlots();
        if (tagDict.ContainsKey(activeKey))
            foreach (string s in tagDict[activeKey])
            {
                AddTagSlot(s);
                AddTag(s);
            }
    }

    public override void ClearAllTagSlots()
    {
        base.ClearAllTagSlots();
        currentOpenTags = new List<string>();
    }

    public void AddTag(string tagName)
    {
        if (!currentOpenTags.Contains(tagName))
            currentOpenTags.Add(tagName);
    }

    public void RemoveTag(string tagName)
    {
        currentOpenTags.Remove(tagName);
    }

    public void DeleteAllTags()
    {
        currentOpenTags = new List<string>();
        ClearAllTagSlots();
    }

    public void Save()
    {
        if (activeKey == "")
            return;
        SetValue(activeKey, currentOpenTags);
        SaveTagManager();        
    }

    public void AddTagSlot(string initialValue = "")
    {
        GameObject go = Instantiate(Resources.Load(tagPrefab)) as GameObject;
        go.transform.SetParent(tagGrid);
        go.GetComponent<TagInputSlot>().Initialize(this, initialValue);
    }
}
