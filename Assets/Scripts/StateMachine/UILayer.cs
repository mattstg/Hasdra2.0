using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class UILayer : MonoBehaviour {

    public Transform stateParent;
    public Transform transitionParent;
    public Transform transitionGUIParent;
    public Transform inputPopups;
    public Transform startStateStartPos;
    public Transform newStateStartPos;
    public Transform expandedStateParent;
    public Transform tagBar;
    public Transform tagBarGrid;
    public Text currentSpellLoaded;

    bool addTransitionActive = false;
    State currentlySelectedState = null;
    bool toolbarIsActive = false;
    bool toolbarIsShowingState = true;

    public void Start()
    {
        GV.smUiLayer = this;
    }

    public static void FillDropdown<T>(Dropdown toFill) where T : struct, System.IComparable, System.IConvertible, System.IFormattable
    {
        toFill.ClearOptions();
        foreach (T enumValue in System.Enum.GetValues(typeof(T)))
        {
            toFill.options.Add(new Dropdown.OptionData(enumValue.ToString()));
        }
    }

    public static void FillDropdown(Dropdown toFill, List<string> listElem)
    {
        toFill.ClearOptions();
        foreach(string s in listElem)
            toFill.options.Add(new Dropdown.OptionData(s));
    }

    public static int GetIndexOfValue(Dropdown dropdown, string valueToFind)
    {
        for(int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == valueToFind)
                return i;
        }
        Debug.LogError("v: " + valueToFind + " value not found");
        return 0;
    }

    
}
