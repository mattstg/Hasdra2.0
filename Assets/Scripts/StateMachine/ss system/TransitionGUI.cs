using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class TransitionGUI : MonoBehaviour
{
    public Transform slotFrameParent;
    public Transition transition;
    public List<TransSlotStructGUI> slotStructGUIs = new List<TransSlotStructGUI>();
    public Vector2 fromOffset;
    public Vector2 toOffset;
    private Transform dynamicLineGUI;

    public void Initialize(Transition _trans, Vector2 _fromOffset, Vector2 _toOffset, Transform _dynamicLineGUI)
    {
        transition = _trans;
        fromOffset = _fromOffset;
        toOffset = _toOffset;
        dynamicLineGUI = _dynamicLineGUI;
    }

    public void TransitionPressed()
    {
        this.gameObject.SetActive(!this.gameObject.activeInHierarchy);
        if (this.gameObject.activeInHierarchy)
        {
            if (slotFrameParent.childCount != transition.slotStructs.Count)
                LoadAllSlotStructGUI();
        }
    }

    public void RemoveAllChildren()
    {
        var children = new List<GameObject>();
        foreach (Transform child in slotFrameParent)
            children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
    }

    public void LoadAllSlotStructGUI()
    {
        RemoveAllChildren();
        foreach (TransSlotStruct ss in transition.slotStructs)
        {
            LoadExistingSlotStruct(ss);
        }
    }   

    System.Collections.Generic.List<TransSlotStruct> GetSlotStruct()
    {
        return transition.GetSlotStructs();
    }

    public void ExitTransition()
    {
        this.gameObject.SetActive(false);
        SaveTransition();
        //Load trans box
        //fe ss load it
    }

    public void Open()
    {
        this.gameObject.SetActive(true);
        Debug.Log("i dont think this is called ever, which is fine");
    }

    public void DeleteTransition()
    {
        dynamicLineGUI.GetComponent<DynamicLine>().from.GetComponent<StateCompactGUI>().RemoveTransition(this);
        dynamicLineGUI.GetComponent<DynamicLine>().to.GetComponent<StateCompactGUI>().RemoveTransition(this);
        Destroy(this.gameObject);
        Destroy(dynamicLineGUI.gameObject);
        FindObjectOfType<SaveLoader>().treeTracker.Delete(this);
        //delete reference from both states
        //delete self from tree tracker
        
    }

   

    public void DeleteSlot(TransSlotStructGUI callingSSG)
    {
        if (slotStructGUIs.Count == 1)
            return;
        transition.RemoveSlotStruct(callingSSG.slotstruct);
        slotStructGUIs.Remove(callingSSG);
        GameObject.Destroy(callingSSG.gameObject);
    }

    private void SaveTransition()
    {
        foreach (TransSlotStructGUI ssg in slotStructGUIs)
            ssg.SaveAllValues();
    }

    private void LoadExistingSlotStruct(TransSlotStruct ss)
    {
        GameObject newSS = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/TransitionsInput/ss resource/SlotStruct"));
        newSS.transform.SetParent(slotFrameParent);
        TransSlotStructGUI ssg = newSS.GetComponent<TransSlotStructGUI>();
        ssg.InitializeExisting(ss,this);
        slotStructGUIs.Add(ssg);
    }

    public List<Dictionary<string, string>> ExportForXML()
    {
        return transition.ExportForXML(fromOffset,toOffset);
    }

    public void AddNewSlotStruct()
    {
        TransSlotStruct newSlotStruct = new TransSlotStruct();
        transition.AddSlotStruct(newSlotStruct);
        GameObject newSS = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/TransitionsInput/ss resource/SlotStruct"));
        newSS.transform.SetParent(slotFrameParent);
        TransSlotStructGUI ssg = newSS.GetComponent<TransSlotStructGUI>();
        ssg.InitializeNew(newSlotStruct, this);
        slotStructGUIs.Add(ssg);
        //add the empty one first
    }
}


