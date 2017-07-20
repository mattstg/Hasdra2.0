using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapVBit {
	public bool isRendered = true;
	public staticWorldBit[] bits = new staticWorldBit[3];

	public mapVBit(staticWorldBit _top, staticWorldBit _mid, staticWorldBit _base){
		bits[0] = _top;
		bits[1] = _mid;
		bits[2] = _base;
	}

	public staticWorldBit getBitAtHight(float y){
		float topH = bits[0].getHighestPoint ();
		float midH = bits[1].getHighestPoint ();
		float baseH = bits[2].getHighestPoint ();

		if (y > topH)
			Debug.Log ("Attempt to find worldBit, but input " + y + " is above all bits");
		else  if (topH <= y && y > midH)
			return bits[0];
		else if (midH <= y && y > baseH)
			return bits[1];
		else if(!bits[2].notInitialized)
			return bits[2];
		return null;
	}

	public GameObject getGObjAtHight(float y){
		return getBitAtHight (y).liveWorldBit;
	}

	public float getTopSlope(){
		if (!bits[0].isBroken || bits[0].notInitialized)
			return bits[0].retSlope ();
		else
			return 0;
	}

	public bool[] renderVBit(bool toRend){
		//returns the index value of destroyed world bits
		bool[] rendCheck = new bool[3];
		for (int c = 0; c < 3; c++) {
			if (bits [c] != null && !bits [c].notInitialized) {
				rendCheck [c] = true;
				bits [c].liveWorldBit.SetActive (toRend);
			} else {
				rendCheck [c] = false;
				Debug.Log ("#" + c + " bit not being loaded. Is it initialized? " + (!bits [c].notInitialized) + ". Is bit == null? " + (bits [c] == null));
			}
		}
		return rendCheck;
	}
}

public class staticWorldBit {
	public bool notInitialized = false;
	public bool isBroken = false;

	public GameObject liveWorldBit; // holds the game object


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
		notInitialized = true;
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

	public bool markAsDestroyed(){
		if(liveWorldBit != null && isBroken){
		liveWorldBit = null;
		isBroken = true;
			return true;
		}else{
			Debug.Log("Bit is already marked as destroyed.");
			return false;
		}
		Debug.Log ("Some weird case has occured...");
		return false;
	}
}
