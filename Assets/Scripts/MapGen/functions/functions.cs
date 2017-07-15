using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class functions  {
	private bool lowestBool = false;
	public float lowestOverWholeInterVal;


	public functions(){

	}

	virtual public float retY(float x){ return x;}

	public float lowestOverInterval(float start, float increment, float end){
		float lowest = retY (start);
		for (float i = start + increment; i < end; i += increment) {
			if (retY (i) < lowest)
				lowest = retY (i);
		}
		return lowest - MAP_GV._floorSafety;
	}

	public float lowestOverWholeRange(float start, float increment, float end){
		if (lowestBool) {
			return lowestOverWholeInterVal;
		} else {
			lowestBool = true;
			lowestOverWholeInterVal = lowestOverInterval (start, increment, end);
			return lowestOverWholeInterVal;
		}
	}
}
