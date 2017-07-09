using UnityEngine;
using System.Collections;

public class UIPanelSizeFix : MonoBehaviour {

	// Use this for initialization

	void Start () {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.offsetMax = new Vector2(0, 0);
        rectTransform.offsetMin = new Vector2(0, 0);
	}
	
}
