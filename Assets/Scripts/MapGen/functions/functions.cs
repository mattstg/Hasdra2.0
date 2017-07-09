using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class functions  {

	public functions(){

	}

	virtual public float retY(float x){ return x;}

	public float lowestOverInterval(float start, float increment, float end){
		float lowest = retY (start);
		for(float i = start + increment; i < end; i += increment){
			if (retY (i) < lowest)
				lowest = retY (i);
		}
		return lowest - MAP_GV._floorSafety;
	}
}
