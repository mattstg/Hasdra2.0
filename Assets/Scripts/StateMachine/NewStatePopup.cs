using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewStatePopup : MonoBehaviour {

    public Transform gridLayout;
    StateCompactGUI stateGUICompact;

    public void ExitPressed()
    {
        Destroy(this.gameObject);
    }

    public void ButtonPressed(int id)
    {
        GV.States statePressed = (GV.States)id;
        stateGUICompact.AddNewStateSlot(statePressed);
        Destroy(this.gameObject);
    }

    public void Initialize(StateCompactGUI _sgc)
    {
        stateGUICompact = _sgc;
        //create all buttons
        foreach (GV.States enumValue in System.Enum.GetValues(typeof(GV.States)))
        {
            if (enumValue != GV.States.StartState)
            {
                GameObject go = Instantiate(Resources.Load("Prefabs/StatePrefabs/ButtonSelection")) as GameObject;
                Button but = go.GetComponent<Button>();
                go.GetComponentInChildren<Text>().text = enumValue.ToString();
                go.transform.SetParent(gridLayout);
                int enumAsInt = (int)enumValue;
                but.onClick.AddListener(() => ButtonPressed(enumAsInt));
            }
        }
    }
}
