using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapRenderer : MonoBehaviour {
	public worldInstantiator map;
	Vector2 pos;
	float renderDist = MAP_GV.renderDistance;

	Vector2 renderRange,lastRenderRange,leftRender,rightRender;
	bool firstRender = true;


	// Use this for initialization
	void Start () {	
	
	}
	
	// Update is called once per frame
	void Update () {
		renderRange = returnRange ();
		//Debug.Log("newRenderRange " + renderRange.ToString());
		//Debug.Log ("lastRenderRange " + lastRenderRange.ToString ());
		if (firstRender) {
			firstRender = false;
			for (float x = renderRange.x; x < renderRange.y; x+= MAP_GV._xIncrement) {
				if (map.currentMapStorage.retVBitAtWorldX(x) == null) {
					//Debug.Log ("Trying to render a null VBit. Maybe instantiate the VBit here.");
					map.GenerateChunk (x);
				} else if(map.currentMapStorage.retVBitAtWorldX(x).isRendered){
					//already rendered, we don't need to do anything
				}else{
					if (map == null || map.currentMapStorage == null || map.currentMapStorage.retVBitAtWorldX (x) == null) {
						if (map == null)
							Debug.Log ("Map is NULL");
						if (map.currentMapStorage == null)
							Debug.Log ("Map Storage is NULL");
						if (map.currentMapStorage.retVBitAtWorldX (x) == null) {
							Debug.Log ("Map VBit is NULL");
						}
						Debug.Log ("x val is: " + x);
					}else
					map.currentMapStorage.retVBitAtWorldX(x).renderVBit (true);
				}
			}
		}else{
			//left
			leftRender = leftToChangeRange();
			//Debug.Log ("leftRender Range: " + leftRender.ToString ());
			for(float x = leftRender.x ; x < leftRender.y; x+= MAP_GV._xIncrement){
				if (map.currentMapStorage.retVBitAtWorldX(x) == null) {
					//Debug.Log ("Trying to render a null VBit. Maybe instantiate the VBit here.");
					map.GenerateChunk (x);
				}

				if (renderRange.x == leftRender.x) {
					if (map == null || map.currentMapStorage == null || map.currentMapStorage.retVBitAtWorldX (x) == null) {
						Debug.Log ("x " + x + "map " + map + " currentMapStorage " + map.currentMapStorage.outputArr() + " vBit " + map.currentMapStorage.retVBitAtWorldX(x));
					}else
					map.currentMapStorage.retVBitAtWorldX(x).renderVBit (true);
				} else {
					if (map == null || map.currentMapStorage == null || map.currentMapStorage.retVBitAtWorldX (x) == null) {
						if (map == null)
							Debug.Log ("Map is NULL");
						if (map.currentMapStorage == null)
							Debug.Log ("Map Storage is NULL");
						if (map.currentMapStorage.retVBitAtWorldX (x) == null) {
							Debug.Log ("Map VBit is NULL");
						}
						Debug.Log ("x val is: " + x);
					}else
					map.currentMapStorage.retVBitAtWorldX(x).renderVBit (false);
				}

			}

			//right
			rightRender = rightToChangeRange ();
			for(float x = rightRender.x; x < rightRender.y; x+= MAP_GV._xIncrement){
				if (map.currentMapStorage.retVBitAtWorldX(x) == null) {
					//Debug.Log ("Trying to render a null VBit. Maybe instantiate the VBit here.");
					map.GenerateChunk (x);
					//Debug.Log ("here at x loc: " + x);

				}

				if (renderRange.y == rightRender.x) {
					if (map == null || map.currentMapStorage == null || map.currentMapStorage.retVBitAtWorldX (x) == null) {
						Debug.Log ("x " + x + "map " + map + " currentMapStorage " + map.currentMapStorage.outputArr() + " vBit " + map.currentMapStorage.retVBitAtWorldX(x));
					}else
						map.currentMapStorage.retVBitAtWorldX(x).renderVBit (false);
				} else {
					if (map == null || map.currentMapStorage == null || map.currentMapStorage.retVBitAtWorldX (x) == null) {
						if (map == null)
							Debug.Log ("Map is NULL");
						if (map.currentMapStorage == null)
							Debug.Log ("Map Storage is NULL");
						if (map.currentMapStorage.retVBitAtWorldX (x) == null) {
							Debug.Log ("Map VBit is NULL");
						}
						Debug.Log ("x val is: " + x);
						//Debug.Log ("x " + x + "map " + map + " vBit " + map.currentMapStorage.retVBitAtWorldX(x) + " currentMapStorage " + map.currentMapStorage.ToString());
					}else
					map.currentMapStorage.retVBitAtWorldX(x).renderVBit (true);
				}
				//Debug.Log ("rightRender Range: " + rightRender.ToString ());
			}
		}
	}

	void LateUpdate(){
		lastRenderRange = renderRange;
	}

	public Vector2 returnRange(){
		pos = transform.position;
		float r1 = pos.x - renderDist;
		if (r1 < 0)
			r1 = 0;
		float r2 = pos.x + renderDist;
		if (r2 > map.currentMapStorage.mapArray.Length)
			r2 = map.currentMapStorage.mapArray.Length;
		return new Vector2 (r1,r2);
	}

	public Vector2 leftToChangeRange(){
		return leftToChangeRange (renderRange, lastRenderRange);
	}

	public Vector2 leftToChangeRange(Vector2 currV, Vector2 lastV){
		float lb, ub;
		if (currV.x < lastV.x) {
			lb = currV.x;
			ub = lastV.x;
		} else {
			lb = lastV.x;
			ub = currV.x;
		}
		return new Vector2 (lb, ub);
	}

	public Vector2 rightToChangeRange(){
		return rightToChangeRange (renderRange, lastRenderRange);
	}

	public Vector2 rightToChangeRange(Vector2 currV, Vector2 lastV){
		float lb, ub;
		if (currV.y < lastV.y) {
			lb = currV.y;
			ub = lastV.y;
		} else {
			lb = lastV.y;
			ub = currV.y;
		}
		return new Vector2 (lb, ub);

	}
}
