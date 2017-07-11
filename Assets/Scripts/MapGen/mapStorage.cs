using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapStorage : MonoBehaviour {

	public mapVBit[] mapArray;
	private float zero; 

	// Use this for initialization
	void Start () {
		mapArray = new mapVBit[(int) MAP_GV.mapLimit * 2 + 1];
		zero = (MAP_GV.mapLimit * 2 + 1) / 2;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool loadMapBit(float x, mapVBit input){
		float trueCoord = zero + x;
		if (Mathf.Abs (trueCoord) <= MAP_GV.mapLimit) {
			Debug.Log ("Attempting to input a bit outside (" + x + ") of the map range limit.");
			return false;
		} else {
			mapArray [(int)trueCoord] = input;
			return true;
		}
	}

	public mapVBit getMapBit(float x){
		float trueCoord = zero + x;
		return mapArray [(int)trueCoord];
	}

}
