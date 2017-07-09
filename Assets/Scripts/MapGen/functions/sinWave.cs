using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sinWave : functions  {

	public sinWave(){

	}

	public override float retY(float x){
		return Mathf.Sin (x / 10) * 5;
	}


}