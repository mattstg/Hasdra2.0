using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TagManager : MonoBehaviour {

    protected float maxViewableTags;
    GV.fileLocationType tableType;
    public Transform tagGrid;
    protected string tagPrefab;
    
    protected Dictionary<string,List<string>> tagDict;    

    public virtual void Initialize(GV.fileLocationType _tableType)
    {
        tagDict = new Dictionary<string, List<string>>();
        tableType = _tableType;
        tagDict = XMLEncoder.XMLToTagManagerDict(tableType);
        DeleteAllMissingFromFiles();
    }

    private void DeleteAllMissingFromFiles()
    {
        List<string> allXMLs = XMLEncoder.GetAllXMLFileNames(tableType);
        List<string> toRemove = new List<string>();
        foreach(KeyValuePair<string,List<string>> kv in tagDict)
            if(!allXMLs.Contains(kv.Key))
            {
                Debug.LogError(string.Format("TagManager({0})contains key {1} which is no xml exists, removing from tagManager",tableType.ToString(),kv.Key));
                toRemove.Add(kv.Key);
            }

        foreach(string s in toRemove)
            tagDict.Remove(s);
    }

    public void SetValue(string keyName, List<string> values)
    {
        if (values.Count <= 0 && tagDict.ContainsKey(keyName))
        {
            tagDict.Remove(keyName);
        }
        else
        {
            if (tagDict.ContainsKey(keyName))
                tagDict[keyName] = new List<string>(values);
            else
                tagDict.Add(keyName, new List<string>(values));
        }
    }

    public List<string> GetValues(string keyName)
    {
        if (tagDict.ContainsKey(keyName))
            return tagDict[keyName];
        else
            return new List<string>();
    }

    public void SaveTagManager()
    {
        XMLEncoder.TagManagerDictToXML(tagDict, tableType);
    }

    public virtual void ClearAllTagSlots()
    {
        foreach (Transform child in tagGrid)
            if (child != tagGrid)
                Destroy(child.gameObject);
    }

    
}
