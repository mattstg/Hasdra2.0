using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuSlot : MonoBehaviour {

    public Text statusName;
    public InputField statusValue;

    public void Initialize(string _statusName, string _statusValue)
    {
        statusName.text = _statusName;
        statusValue.text = _statusValue;
    }
}
