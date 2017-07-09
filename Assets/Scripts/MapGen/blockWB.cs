using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blockWB : worldBit {
	//public float hight;

	/*
	public void Initialize(Vector2 startPoint, Vector2 endPoint){
		hight = startPoint.y;
		base.Initialize (startPoint, endPoint);
		//Resize ();
		//SetToWorldPos (startPoint);
	} */

	public override void Resize(){
		float totH = hight - lowerstPoint;
		scaleOffset = new Vector2 (MAP_GV._xIncrement, totH);
		Vector2 tempS = transform.localScale;
		transform.localScale = new Vector3(tempS.x * scaleOffset.x, tempS.y * scaleOffset.y, 1);
	}

	public void bigBlockResize(float _xDist, float bedRock, float _h){
		transform.localScale = new Vector3 (1, 1, 1);
		float totH = _h - bedRock; 
		scaleOffset = new Vector2 (_xDist, totH);
		Vector2 tempS = transform.localScale;
		transform.localScale = new Vector3(tempS.x * scaleOffset.x, tempS.y * scaleOffset.y, 1);
	}

	public override void SetToWorldPos(Vector2 toSet){
		transform.position = new Vector2 (toSet.x - 0.5f * scaleOffset.x, toSet.y - 0.5f * scaleOffset.y);
	}


}