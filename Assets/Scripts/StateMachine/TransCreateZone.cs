using UnityEngine;
using System.Collections;

public class TransCreateZone : MonoBehaviour {

    public StateCompactGUI sgc;

    public void ClickedOnZone()
    {
        Vector2 fromOffset = Input.mousePosition - sgc.gameObject.transform.position;
        GameObject go = Instantiate(Resources.Load("Prefabs/StatePrefabs/TransDragger")) as GameObject;
        go.transform.SetParent(GV.smUiLayer.gameObject.transform);
        go.GetComponent<TransitionDragger>().Initialize(this, fromOffset);
    }
}
