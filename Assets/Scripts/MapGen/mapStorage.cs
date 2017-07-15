using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapStorage {

	//the map will be oriented left to right along the array.
	//map starts at 0 on the left, and extends positively towards the right to the map limit along x in unity units
	//the map is split into smaller or larger bits of MAP_GV._xIncrement length (currently 0.5)
	//this means the mapArray will be of length  MAP_GV.mapLimit / MAP_GV._xIncrement (example 2000m / 0.5m increments = 4000 bits)
	public mapVBit[] mapArray;
	public float mapLimit = MAP_GV.mapLimit;
	public float mapIncrement = MAP_GV._xIncrement;

	public mapStorage(){
		if (mapLimit % mapIncrement != 0) {
			Debug.Log ("It seems the Default Map Limit is not evenlty devisible by Map Increment. Altering Map Limit to satisfy this requirement.");
			mapLimit -= mapLimit % mapIncrement;
		}
		mapArray = new mapVBit[(int) (mapLimit / mapIncrement)];
	}

	public mapStorage(float _mL, float _mI){
		mapLimit = _mL;
		mapIncrement = _mI;
		if (mapLimit % mapIncrement != 0) {
			Debug.Log ("Map Limit is not evenlty devisible by Map Increment. Altering Map Limit to satisfy this requirement.");
			mapLimit -= mapLimit % mapIncrement;
		}
		mapArray = new mapVBit[(int) (mapLimit / mapIncrement)];
	}

	public bool loadMapVBit(float x, mapVBit input){
		if (x < 0 || x > mapLimit) {
			Debug.Log ("Attempting to input a bit outside (" + x + ") of the map range limit.");
			Debug.Log ("Top bit " + input.topBit.point1 + " " + input.topBit.point2);
			Debug.Log ("Mid bit " + input.midBit.point1 + " " + input.midBit.point2);
			Debug.Log ("Base bit " + input.baseBit.point1 + " " + input.baseBit.point2);
			Debug.Break();
			return false;
		} else {
			mapArray [xValToMapArr(x)] = input;
			return true;
		}
	}

	public mapVBit getMapVBit(float _x){
		float x = _x + MAP_GV._xIncrement;
		if (x >= 0 && x < mapArray.Length) {
			if (mapArray [xValToMapArr (x)] != null)
				return mapArray [xValToMapArr (x)];
			else
				Debug.Log ("Attempting to retrive MapVBit outside of map's rendered storage");
			return null;
		} else {
			Debug.Log ("You are attempting to retreive a bit outside of the map's size limit.");
			return null;
		}
	}

	public int xValToMapArr(float x){
		//Debug.Log ("xValToMapArr: " + (x / mapIncrement));
		return (int) (x / mapIncrement);
	}

	public GameObject getGameObjAtPos(Vector2 pos){
		if (pos.x < 0 || pos.x > mapLimit) {
			Debug.Log ("Vector input is outside the bounds of the map limit.");
			return null;
		}else
		return getMapVBit(xValToMapArr(pos.x)).getGObjAtHight (pos.y);;
	}

	public string outputArr(){
		string output = "";
		int counter = 0;
		while (mapArray [counter] != null) {
			output += "mapStorage index: " + counter + " pos: " + mapArray[counter].topBit.point1.ToString() + "  ||  ";
			counter++;
		}
		return output;
	}
}
