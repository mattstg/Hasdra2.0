using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triangleWB : worldBit{

	/*
	public override void Initialize(Vector2 startPoint, Vector2 endPoint){
		base.Initialize (startPoint, endPoint);
		//Resize ();
		//SetToWorldPos (startPoint);
	} */

	public override void Resize(){
		scaleOffset = new Vector2 (MAP_GV._xIncrement, slope * MAP_GV._xIncrement);
		Vector2 tempS = transform.localScale;
		if(type == MAP_GV.BitType.pos)
			transform.localScale = new Vector3(tempS.x * scaleOffset.x  * -1, tempS.y * scaleOffset.y, 1);
		else
			transform.localScale = new Vector3(tempS.x * scaleOffset.x, -1 * tempS.y * scaleOffset.y, 1);
	}

	public override void SetToWorldPos(Vector2 toSet){
		if(type == MAP_GV.BitType.neg)
			transform.position = new Vector2 (toSet.x - 0.5f * scaleOffset.x, toSet.y + 0.5f * scaleOffset.y);
		else
			transform.position = new Vector2 (toSet.x - 0.5f * scaleOffset.x, toSet.y + 0.5f * scaleOffset.y);
	}


}
