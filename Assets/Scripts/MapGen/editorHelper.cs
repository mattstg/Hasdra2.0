using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class editorHelper : MonoBehaviour {
	public worldInstantiator map;
	Vector2 pos;

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (10, map.currentMapCurve.retY (10) + 1, 0);
	}
	
	// Update is called once per frame
	void Update () {
		pos = transform.position;
		mapVBit temp = map.currentMapStorage.retVBitAtWorldX (pos.x);
		if (temp != null) {
			if (!temp.topBit.isNull)
				temp.topBit.liveWorldBit.GetComponent<SpriteRenderer> ().color = new Color (0, 1, 0);
			else
				Debug.Log ("top bit null?");
			if(!temp.midBit.isNull)
				temp.midBit.liveWorldBit.GetComponent<SpriteRenderer> ().color = new Color (0, 1, 0);
			if(!temp.baseBit.isNull)
				temp.baseBit.liveWorldBit.GetComponent<SpriteRenderer> ().color = new Color (0, 1, 0);
		}
	}
}
