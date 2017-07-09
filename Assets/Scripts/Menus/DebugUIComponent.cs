using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum DucType { BodyStats} 

public class DebugUIComponent : MonoBehaviour {

    public Text titleText;
    public Transform grid;
    DucType ducType;

    List<string> defaultToLoad = new List<string>() { "acrobatics", "concusionRecoverRate", "staminaRegenRate" };

    protected List<StatSlotUI> statSlots = new List<StatSlotUI>();

    public virtual void Initialize(string _title, DucType _ducType)
    {
        titleText = transform.Find("Title").GetComponent<Text>();
        grid = transform.Find("Grid");
        ducType = _ducType;
        titleText.text = _title;
    }

    public void AddStatSlots(int numberToAdd)
    {
        for (int i = 0; i < numberToAdd; i++)
        {
            GameObject go = Instantiate(Resources.Load("Prefabs/Menus/StatSlot")) as GameObject;
            go.GetComponent<StatSlotUI>().ducParent = this;
            go.transform.SetParent(grid);
            statSlots.Add(go.GetComponent<StatSlotUI>());
            //debug, remove later
            if (i < defaultToLoad.Count)
                go.GetComponent<StatSlotUI>().titleText.text = defaultToLoad[i];
        }
    }

    public virtual void Update()
    {

    }
}
