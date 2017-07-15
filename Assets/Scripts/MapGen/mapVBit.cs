using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapVBit {
	public bool isRendered = true;
	public staticWorldBit topBit;
	public staticWorldBit midBit;
	public staticWorldBit baseBit;

	public mapVBit(staticWorldBit _top, staticWorldBit _mid, staticWorldBit _base){
		topBit = _top;
		midBit = _mid;
		baseBit = _base;
	}

	public staticWorldBit getBitAtHight(float y){
		float topH = topBit.getHighestPoint ();
		float midH = midBit.getHighestPoint ();
		float baseH = baseBit.getHighestPoint ();

		if (y > topH)
			Debug.Log ("Attempt to find worldBit, but input " + y + " is above all bits");
		else  if (topH <= y && y > midH)
			return topBit;
		else if (midH <= y && y > baseH)
			return midBit;
		else if(!baseBit.isNull)
			return baseBit;
		return null;
	}

	public GameObject getGObjAtHight(float y){
		return getBitAtHight (y).liveWorldBit;
	}

	public float getTopSlope(){
		if (!topBit.isBroken)
			return topBit.retSlope ();
		else
			return 0;
	}

	public void renderVBit(bool toRend){
		topBit.liveWorldBit.SetActive (toRend);
		midBit.liveWorldBit.SetActive (toRend);
		if (!baseBit.isNull)
			baseBit.liveWorldBit.SetActive (toRend);
	}
}

public class staticWorldBit {
	public bool isNull = false;

	public GameObject liveWorldBit; // holds the game object
	public bool isBroken = false;

	//internals
	public Vector2 point1 = new Vector2();
	public Vector2 point2 = new Vector2 ();
	public MAP_GV.BitType type;
	public float slope;
	public float lowerstPoint;
	public float hight;
	public Vector2 botLeftCorner = new Vector2();
	public Vector2 botRightCorner = new Vector2();

	public staticWorldBit(GameObject _liveWorldBit, MAP_GV.BitType _type, Vector2 _p1, Vector2 _p2, float _slope, float _lowestPoint, float _hight){
		liveWorldBit = _liveWorldBit;
		point1 = _p1;
		point2 = _p2;
		type = _type;
		slope = _slope;
		lowerstPoint = _lowestPoint;
		hight = _hight;
		Debug.Log ("You are using an outdated constructor.");
	}

	public staticWorldBit(worldBit _input){
		liveWorldBit = _input.gameObject;
		point1 = _input.point1;
		point2 = _input.point2;
		type = _input.type;
		slope = _input.slope;
		lowerstPoint = _input.lowerstPoint;
		hight = _input.hight;
		botLeftCorner = _input.topLeftCorner; 
		botRightCorner = _input.bottmLeftCorner;
	}

	public staticWorldBit(){
		isNull = true;
	}


	public float getHighestPoint(){
		if (type != MAP_GV.BitType.block) {
			if (point1.y > point2.y)
				return point1.y;
			else
				return point2.y;
		} else
			return point1.y;
	}

	public float retSlope(){
		if (type == MAP_GV.BitType.block)
			return 0;
		else
			return slope;
	}
}
