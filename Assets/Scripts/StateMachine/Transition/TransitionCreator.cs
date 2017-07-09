using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

//Used for the GUI in SM, not for game
public class TransitionCreator : MonoBehaviour {
    SaveLoader saveLoader;
    int curTransID = 1;

    public void Start()
    {
        saveLoader = GetComponent<SaveLoader>();
    }

    public TransitionGUI CreateNewTransition(StateCompactGUI stateFrom, Vector2 fromOffset, StateCompactGUI stateTo, Vector2 toOffset, bool loadingFromXMLs = false)
    {
        //Create both a real and GUI transition
        if (stateFrom == null || stateTo == null)
        {
            Debug.LogError("CreateTransition didnt have a from or to state");
            return null;
        }
        
        //transition
        GameObject newObj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/TransitionsInput/Transition"));
        Transition t = new Transition(stateFrom.stateGUI.state, stateTo.stateGUI.state);
        newObj.transform.SetParent(GV.smUiLayer.transitionParent);
        
        //transition popup
        GameObject popupChild = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/TransitionsInput/ss resource/TransitionGUI"));
        popupChild.transform.SetParent(GV.smUiLayer.transitionGUIParent,false);
        popupChild.SetActive(false);
        GameObject.FindObjectOfType<SaveLoader>().AddNewTransGUI(popupChild.GetComponent<TransitionGUI>());

        //trans backend
        if (!loadingFromXMLs)
            t.transitionID = curTransID++; //if no value assigned (not from xml) give incrementing value
        stateFrom.stateGUI.state.AddTransition(t);
        GameObject.FindObjectOfType<TreeTracker>().AddTransition(t);
        
        //gui
        TransitionGUI transGUI = popupChild.GetComponent<TransitionGUI>();
        transGUI.Initialize(t, fromOffset, toOffset, newObj.transform);
        if(!loadingFromXMLs)
            popupChild.GetComponent<TransitionGUI>().AddNewSlotStruct();
        newObj.GetComponent<Button>().onClick.AddListener(() => transGUI.TransitionPressed());
        newObj.GetComponent<DynamicLine>().SetTransforms(stateFrom.transform, fromOffset, stateTo.transform, toOffset);
        newObj.transform.SetParent(GameObject.FindObjectOfType<UILayer>().transitionParent);

        stateFrom.AddTransitionGUI(transGUI);
        stateTo.AddTransitionGUI(transGUI);

        return transGUI;
    }
    

    public void CreateStartStateTransition(State _from, State _to)
    {/*
        GameObject newObj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/TransitionsInput/Transition"));
        newObj.AddComponent<GoNextTransition>();
        newObj.GetComponent<GoNextTransition>().SetTransition(_from, _to);
        newObj.GetComponent<Image>().color = Color.white;
        newObj.GetComponent<DynamicLine>().SetTransforms(_from.transform, _to.transform);
        newObj.transform.SetParent(GameObject.FindObjectOfType<UILayer>().transitionParent);
        _from.AddTransition(newObj.GetComponent<Transition>());
        GameObject.FindObjectOfType<TreeTracker>().AddTransition(newObj.GetComponent<Transition>());*/
    }

    //should have tree tracker be a singleton
    public void LoadTransitionFromXMLLoader(System.Collections.Generic.Dictionary<string,string> xmlDict, TreeTracker treeTracker)
    {
        Vector2 fromOffset = new Vector2(float.Parse(xmlDict["fromOffsetX"]), float.Parse(xmlDict["fromOffsetY"]));
        Vector2 toOffset = new Vector2(float.Parse(xmlDict["toOffsetX"]), float.Parse(xmlDict["toOffsetY"]));
        Transition newT = CreateNewTransition(saveLoader.GetStateGUICompactByID(int.Parse(xmlDict["LinkFrom"])),fromOffset, saveLoader.GetStateGUICompactByID(int.Parse(xmlDict["LinkTo"])), toOffset, true).transition;
        newT.transitionID = int.Parse(xmlDict["TransID"]);
        if(newT.transitionID >= curTransID)
            curTransID = newT.transitionID + 1;
    }
    
}
