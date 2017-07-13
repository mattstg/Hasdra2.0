using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapVBit {
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

		if (topH <= y && y > midH)
			return topBit;
		else if (midH <= y && y > baseH)
			return midBit;
		else if (y > topH)
			Debug.Log ("Attempt to find worldBit, but input " + y + " is above all bits");
		else
			return baseBit;
		return null;
	}
}

public class staticWorldBit {
	public Object liveWorldBit; // holds the game object
	public bool isBroken = false;

	//internals
	public Vector2 point1 = new Vector2();
	public Vector2 point2 = new Vector2 ();
	public MAP_GV.BitType type;
	public float slope;
	public float lowerstPoint;
	public float hight;

	public staticWorldBit(Object _liveWorldBit, MAP_GV.BitType _type, Vector2 _p1, Vector2 _p2, float _slope, float _lowestPoint, float _hight){
		liveWorldBit = _liveWorldBit;
		point1 = _p1;
		point2 = _p2;
		type = _type;
		slope = _slope;
		lowerstPoint = _lowestPoint;
		hight = _hight;
	}


	public float getHighestPoint(){
		if (point1.y > point2.y)
			return point1.y;
		else
			return point2.y;
	}
}
