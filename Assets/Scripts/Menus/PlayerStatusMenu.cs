using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStatusMenu : MonoBehaviour {

    public Transform gridTransform;
    public Dictionary<string, MenuSlot> statusSlotDict = new Dictionary<string, MenuSlot>();

    public void SetStatus(string statusName, string statusValue)
    {
        if (!statusSlotDict.ContainsKey(statusName))
        {
           GameObject go = Instantiate(Resources.Load("Prefabs/Menus/StatusSlot")) as GameObject;
           go.transform.SetParent(gridTransform, false);
           MenuSlot statusSlot = go.GetComponent<MenuSlot>();
           statusSlot.Initialize(statusName, statusValue);
           statusSlotDict.Add(statusName, statusSlot);
        }
        else
        {
            statusSlotDict[statusName].Initialize(statusName, statusValue);
        }
    }

    public void ClearStatus(string statusName, string statusValue)
    {
        if (statusSlotDict.ContainsKey(statusName))
        {
            Destroy(statusSlotDict[statusName].gameObject);
            statusSlotDict.Remove(statusName);
        }
    }

}
