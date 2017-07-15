using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapRenderer : MonoBehaviour {
	public worldInstantiator map;
	Vector2 pos;
	float renderDist = MAP_GV.renderDistance;
	Vector2 renderRange;
	mapVBit[] mapArr;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
			mapArr = map.currentMapStorage.mapArray;
		pos = transform.position;
		renderRange = new Vector2 (map.currentMapStorage.xValToMapArr (pos.x - renderDist), map.currentMapStorage.xValToMapArr(pos.x + renderDist));
		if (renderRange.x < 0)
			renderRange.x = 0;
		if (renderRange.y > mapArr.Length)
			renderRange.y = mapArr.Length;
		for (int x = (int) renderRange.x; x <= renderRange.y; x++) {
			if (mapArr [x] == null) {
				Debug.Log ("Trying to render a null VBit. Maybe instantiate the VBit here.");
			} else if(mapArr [x].isRendered){
				//already rendered, we don't need to do anything
			}else{
				mapArr [x].renderVBit (true);
			}

		}
	}
}
