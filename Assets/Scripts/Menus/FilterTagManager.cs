using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FilterTagManager : TagManager {

    List<string> allFilters;
    List<string> activeFilters;
    List<GameObject> visibleFilters;

    public override void Initialize(GV.fileLocationType _tableType)
    {
        activeFilters = new List<string>();
        allFilters = new List<string>();
        visibleFilters = new List<GameObject>();
        tagPrefab = "Prefabs/Menus/TagButtonFancy";
        maxViewableTags = 10;
        base.Initialize(_tableType);
        SetupFilters();
        SetVisible(false);
    }

    public void SetVisible(bool isVisible)
    {
        tagGrid.gameObject.SetActive(isVisible);
    }

    private void SetupFilters()
    {
        foreach (KeyValuePair<string, List<string>> kv in tagDict)
            foreach (string filter in kv.Value)
                if (!allFilters.Contains(filter))
                    allFilters.Add(filter);

        for (int i = 0; i < maxViewableTags && i < allFilters.Count; i++)
        {
            visibleFilters.Add(AddTagSlot(allFilters[i]));
        }
    }

    public void ToggleFilter(string filter)
    {
        if (activeFilters.Contains(filter))
            activeFilters.Remove(filter);
        else
            activeFilters.Add(filter);
        GameObject.FindObjectOfType<MainLevelDesignerScript>().RefreshBoxes(); //dirty way, but atm dont give a fuck
    }

    public void CycleFilters()
    {
        if (allFilters.Count > maxViewableTags)
        {
            Debug.Log("cycles");
        }
    }

    public List<string> FilterResults(List<string> toFilter)
    {
        if (activeFilters.Count == 0) //if no filters, all pass
            return new List<string>(toFilter);

        List<string> toRet = new List<string>();
        foreach (string s in toFilter)
        {
            if(tagDict.ContainsKey(s))
            {
                foreach(string tagName in tagDict[s])
                {
                    if(activeFilters.Contains(tagName))
                    {
                        if(!toRet.Contains(s))
                            toRet.Add(s);
                        break;
                    }
                }
            }
        }
        return toRet;
    }

    public GameObject AddTagSlot(string initialValue = "")
    {
        GameObject go = Instantiate(Resources.Load(tagPrefab)) as GameObject;
        go.transform.SetParent(tagGrid);
        go.GetComponent<TagButton>().Initialize(this, initialValue);
        return go;
    }
}
