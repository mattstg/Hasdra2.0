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

	public mapStorage(){
		if (mapLimit % MAP_GV._xIncrement != 0) {
			Debug.Log ("It seems the Default Map Limit is not evenlty devisible by Map Increment. Altering Map Limit to satisfy this requirement.");
			mapLimit -= mapLimit % MAP_GV._xIncrement;
		}
		mapArray = new mapVBit[(int) (mapLimit / MAP_GV._xIncrement)];
		Debug.Log ("Map Array Length " + mapArray.Length);
	}

	public mapStorage(float _mL, float _mI){
		mapLimit = _mL;
		MAP_GV._xIncrement = _mI;
		if (mapLimit % MAP_GV._xIncrement != 0) {
			Debug.Log ("Map Limit is not evenlty devisible by Map Increment. Altering Map Limit to satisfy this requirement.");
			mapLimit -= mapLimit % MAP_GV._xIncrement;
		}
		mapArray = new mapVBit[(int) (mapLimit / MAP_GV._xIncrement)];
		//Debug.Log ("mapArr size is set to " + (int) (mapLimit / MAP_GV._xIncrement) );
	}

	public bool loadMapVBit(float x, mapVBit input){
		x = worldXtoArrIndex (x);
		if (x < 0 || x > mapArray.Length) {
			Debug.Log ("Attempting to input a bit outside (" + x + ") of the map range limit.");
			for (int c = 0; c < 3; c++) {
				Debug.Log ("Bit# " + c + " Points: " + input.bits[c].point1 + " " + input.bits[c].point2);
			}
			Debug.Break ();
			return false;
		} else if(input.bits[0] == null || input.bits[1] == null || input.bits[2] == null){
			Debug.Log ("Attempting to input an unfilled VBit. Some Bit inside is null.");
			return false;
		}else if (input == null) {
			Debug.Log ("Attempting to input a null Map Vertical Bit: " + input.ToString());
			return false;
		}else{
			if (x < 0 || x > mapArray.Length) {
				Debug.Log ("your input x point is outside of the mapArr range: " + worldXtoArrIndex (x) + " mapArr L: " + mapArray.Length);
				return false;
			} else {
				//Debug.Log ("mapArr index: " + x + " mapArr L: " + mapArray.Length);
				mapArray [(int) x] = input;
			}
			
			return true;
		}
	}

	public bool refillLiveWorldBit(float x, int index, GameObject input){
		x = worldXtoArrIndex (x);
		if (x < 0 || x > mapArray.Length) {
			Debug.Log ("Attempting to input a bit outside (" + x + ") of the map range limit.");
			return false;
		} else if (input == null) {
			Debug.Log ("Attempting to input a null GameObject.");
			return false;
		} else if (index > 3 || index < 0) {
			Debug.Log ("Attempting to input into non existent array index. Max == 3. Current == " + index);
			return false;
		}else if (mapArray [(int)x].bits [index].liveWorldBit == null && mapArray [(int)x].bits [index].isBroken) {
			//This should relink the liveWorldBit with new one to replace a broken one
			mapArray [(int)x].bits [index].liveWorldBit = input;
			return true;
		}else{
			Debug.Log ("Trying to refill a worldBit with a live GameObject, but it is already linked to one...");
			return false;
		}
	}

	//Probably useless...
	public bool loadMapBit(float x, int index, staticWorldBit input){
		x = worldXtoArrIndex (x);
		if (x < 0 || x > mapArray.Length) {
			Debug.Log ("Attempting to input a bit outside (" + x + ") of the map range limit.");
			Debug.Log ("Bit# " + index + " Points: " + input.point1 + " " + input.point2);
			return false;
		} else if(input == null){
			Debug.Log ("Attempting to input a null staticWorldBit");
			return false;
		}else if (mapArray [(int)x].bits [index] != null && input.liveWorldBit != null) {
			//This should relink the liveWorldBit with 
			mapArray [(int)x].bits [index].liveWorldBit = input.liveWorldBit;
			return true;
		}else{
			mapArray [(int) x].bits[index] = input;
			return true;
		}
	}

	public mapVBit retVBitAtWorldX(float _x){
		int x = worldXtoArrIndex(_x);
		if (x >= 0 && x < mapArray.Length) {
			if (mapArray [x] != null)
				return mapArray [x];
			else
				//Debug.Log ("Attempting to retrive MapVBit outside of map's rendered storage");
			return null;
		} else {
//			Debug.Log ("x = " + x + " and mapArr.Length = " + mapArray.Length);
//			Debug.Log ("You are attempting to retreive a bit outside of the map's size limit.");
			return null;
		}
	}

	public int worldXtoArrIndex(float x){
		//Debug.Log ("xValToMapArr: " + (x / MAP_GV._xIncrement));
		return (int) (x / MAP_GV._xIncrement + MAP_GV._xIncrement);
	}
		
	public GameObject getGameObjAtPos(Vector2 pos){
		if (pos.x < 0 || pos.x > mapLimit) {
			Debug.Log ("Vector input is outside the bounds of the map limit.");
			return null;
		}else
			return retVBitAtWorldX(pos.x).getGObjAtHight (pos.y);
	}

	public string outputArr(){
		string output = "";
		int counter = 0;
		while (mapArray [counter] != null) {
			output += "mapStorage index: " + counter + " pos: " + mapArray[counter].bits[0].point1.ToString() + "  ||  ";
			counter++;
		}
		return output;
	}
}
