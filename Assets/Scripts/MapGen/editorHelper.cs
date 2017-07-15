using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class editorHelper : MonoBehaviour {
	public worldInstantiator map;
	Vector2 pos;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		pos = transform.position;
		mapVBit temp = map.currentMapStorage.getMapVBit (pos.x);
		if (temp != null) {
			temp.topBit.liveWorldBit.GetComponent<SpriteRenderer> ().color = new Color (10, 0, 0);
			temp.midBit.liveWorldBit.GetComponent<SpriteRenderer> ().color = new Color (10, 0, 0);
			temp.baseBit.liveWorldBit.GetComponent<SpriteRenderer> ().color = new Color (10, 0, 0);
		}
	}
}
