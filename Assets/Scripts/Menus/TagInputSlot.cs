using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TagInputSlot : MonoBehaviour {

    public InputField inputSlot;
    InputTagManager tagManager;  //okay for the weirdest reason ever this needs to be public, or the reference commits suicide
    string currentValue = "";

    public void Initialize(TagManager incomingTagManager, string initialValue)
    {
        tagManager = (InputTagManager)incomingTagManager;
        currentValue = initialValue;
        inputSlot.text = initialValue;
    }

    public void DeleteSlot()
    {
        tagManager.RemoveTag(currentValue);
        Destroy(this.gameObject);
    }

    public void ValueChanged()
    {
        string newValue = inputSlot.text;
        newValue = newValue.Replace(",","");  //cannot have, will fuck xml
        newValue = newValue.Replace("<", "");  //cannot have, will fuck xml
        newValue = newValue.Replace(">", "");  //cannot have, will fuck xml
        newValue = newValue.Replace(" ", "");  //cannot have, will fuck xml
        tagManager.RemoveTag(currentValue);
        inputSlot.text = newValue;
        if (newValue != "")
        {
            tagManager.AddTag(newValue);
            currentValue = newValue;
        }
    }
}
