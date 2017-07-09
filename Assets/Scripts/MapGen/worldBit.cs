using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worldBit : MonoBehaviour {
	private bool selfDestruct = false;

	public enum BitType {nul, pos, neg, block};
	public Vector2 point1 = new Vector2();
	public Vector2 point2 = new Vector2 ();
	public BitType type;
	public float slope;
	public float lowerstPoint;
	public float hight;

	public Vector2 scaleOffset = new Vector2(1,1);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void LateUpdate(){
		if (selfDestruct)
			Destroy (this);
	}
		
	public void Initialize(Vector2 startPoint, Vector2 endPoint, float _lowerstPoint){
		lowerstPoint = _lowerstPoint;
		Initialize (startPoint, endPoint);
	}

	public void Initialize(Vector2 startPoint, Vector2 endPoint){
		hight = startPoint.y;
		point1 = startPoint;
		point2 = endPoint;
		slope = (endPoint.y - startPoint.y) / (endPoint.x - startPoint.x);
		type = getType (slope);
		Resize ();
		SetToWorldPos (startPoint);
	}

	virtual public void Resize(){	}

	virtual public void SetToWorldPos(Vector2 toSet){	}

	public static worldBit.BitType getType(Vector2 sP, Vector2 eP){
		float slope = (eP.y - sP.y) / (eP.x - sP.x);
		if (slope < MAP_GV._flatRange && slope > -MAP_GV._flatRange)
			return worldBit.BitType.block;
		else if (slope > 0)
			return worldBit.BitType.pos;
		else
			return worldBit.BitType.neg;
	}

	public static worldBit.BitType getType(float _slope){
		if (_slope < MAP_GV._flatRange && _slope > -MAP_GV._flatRange)
			return worldBit.BitType.block;
		else if (_slope > 0)
			return worldBit.BitType.pos;
		else
			return worldBit.BitType.neg;
	}

	public Vector2 retBaseCorner(){
		if (type == BitType.pos)
			return new Vector2 (transform.position.x + 0.5f * scaleOffset.x, transform.position.y - 0.5f * scaleOffset.y);
		else
			return new Vector2 (transform.position.x + 0.5f * scaleOffset.x, transform.position.y + 0.5f * scaleOffset.y);
		//toSet.x - 0.5f * scaleOffset.x, toSet.y + 0.5f * scaleOffset.y

	}

	public Vector2 retBottomCorner(){
		return new Vector2 (transform.position.x + 0.5f * scaleOffset.x, transform.position.y - 0.5f * scaleOffset.y);
	}

	public void markForDestruction(){
		selfDestruct = true;
	}
		
}
