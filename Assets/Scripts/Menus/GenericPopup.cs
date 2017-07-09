using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GenericPopup : MonoBehaviour {

    public Transform gridTransform;
    public Text menuDescText;
    public Dictionary<string, MenuSlot> menuSlots = new Dictionary<string, MenuSlot>();
    List<Button> buttons;

    public delegate void DelegateFunc(string arg1);

    public void SetDesc(string desc)
    {
        menuDescText.text = desc;
    }

    public void AddButton(string buttonText, System.Action callbackFunc)
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/Menus/GenericButton")) as GameObject;
        go.transform.SetParent(gridTransform);
        UnityEngine.Events.UnityAction castedCallback = new UnityEngine.Events.UnityAction(callbackFunc);
        UnityEngine.Events.UnityAction castedCallbackClose = new UnityEngine.Events.UnityAction(CloseMenu);
        go.GetComponent<Button>().onClick.AddListener(castedCallback);
        go.GetComponent<Button>().onClick.AddListener(castedCallbackClose);
        go.GetComponent<Button>().GetComponentInChildren<Text>().text = buttonText;
    }


    public void AddButton(string buttonText)
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/Menus/GenericButton")) as GameObject;
        go.transform.SetParent(gridTransform);
        UnityEngine.Events.UnityAction castedCallback = new UnityEngine.Events.UnityAction(CloseMenu);
        go.GetComponent<Button>().onClick.AddListener(castedCallback);
        go.GetComponent<Button>().GetComponentInChildren<Text>().text = buttonText;
    }

    public void AddButton(string buttonText, DelegateFunc theFunc, string arg1)
    {
        AddButton(buttonText, theFunc, arg1, Color.white);
    }

    public void AddButton(string buttonText, DelegateFunc theFunc, string arg1, Color _color)
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/Menus/GenericButton")) as GameObject;
        go.transform.SetParent(gridTransform);
        UnityEngine.Events.UnityAction castedCallbackClose = new UnityEngine.Events.UnityAction(CloseMenu);
        Button btn = go.GetComponent<Button>();
        btn.onClick.AddListener(delegate { theFunc(arg1); });
        btn.onClick.AddListener(castedCallbackClose);
        btn.GetComponentInChildren<Text>().text = buttonText;
        go.GetComponent<Image>().color = _color;
    }

    public void AddMenuItems(Dictionary<string, string> toAdd)
    {
        foreach (KeyValuePair<string, string> kv in toAdd)
            AddMenuItem(kv.Key, kv.Value);
    }

    public void AddMenuItem(string statusName, string statusValue)
    {
        if (!menuSlots.ContainsKey(statusName))
        {
           GameObject go = Instantiate(Resources.Load("Prefabs/Menus/MenuSlot")) as GameObject;
           go.transform.SetParent(gridTransform, false);
           MenuSlot statusSlot = go.GetComponent<MenuSlot>();
           statusSlot.Initialize(statusName, statusValue);
           menuSlots.Add(statusName, statusSlot);
        }
        else
        {
            menuSlots[statusName].Initialize(statusName, statusValue);
        }
    }

    public void CloseMenu()
    {
        GV.DeleteAllChildren(gridTransform);
        menuSlots.Clear();
        gameObject.SetActive(false);
    }

    public void RemoveMenuItem(string statusName, string statusValue)
    {
        if (menuSlots.ContainsKey(statusName))
        {
            Destroy(menuSlots[statusName].gameObject);
        }
    }

    public void SetAsCanvasLastChild()
    {
        transform.SetParent(FindObjectOfType<Canvas>().transform);
        transform.SetAsLastSibling();
    }

    public static GenericPopup CreateGenericPopup()
    {
        GameObject go = Instantiate(Resources.Load("Prefabs/Menus/GenericMenu")) as GameObject;
        Canvas mainCanvas = GameObject.FindObjectOfType<Canvas>();
        go.transform.SetParent(mainCanvas.transform,false);
        go.transform.SetAsLastSibling();
        return go.GetComponent<GenericPopup>();
    }

}
