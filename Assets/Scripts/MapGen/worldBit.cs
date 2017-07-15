using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worldBit : MonoBehaviour{
	public Vector2 point1 = new Vector2();
	public Vector2 point2 = new Vector2 ();
	public MAP_GV.BitType type;
	public float slope;
	public float lowerstPoint;
	public float hight;
	public Vector2 topLeftCorner = new Vector2();
	public Vector2 bottmLeftCorner = new Vector2();


	public Vector2 scaleOffset = new Vector2(1,1);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
		
	public void Initialize(Vector2 startPoint, Vector2 endPoint, float _lowerstPoint){
		lowerstPoint = _lowerstPoint;
		Initialize (startPoint, endPoint);
	}

	public void Initialize (Vector2 startPoint, Vector2 endPoint, float _lowerstPoint, float verticalOffset){
		hight = startPoint.y;
		point1 = startPoint;
		point2 = endPoint;
		slope = (endPoint.y - startPoint.y) / (endPoint.x - startPoint.x);
		type = getType (slope);
		Resize ();
		SetToWorldPos (startPoint + new Vector2(0,verticalOffset));
		topLeftCorner = retTopLeftCorner ();
		bottmLeftCorner = retBottomLeftCorner ();
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

	public static MAP_GV.BitType getType(Vector2 sP, Vector2 eP){
		float slope = (eP.y - sP.y) / (eP.x - sP.x);
		if (slope < MAP_GV._flatRange && slope > -MAP_GV._flatRange)
			return MAP_GV.BitType.block;
		else if (slope > 0)
			return MAP_GV.BitType.pos;
		else
			return MAP_GV.BitType.neg;
	}

	public static MAP_GV.BitType getType(float _slope){
		if (_slope < MAP_GV._flatRange && _slope > -MAP_GV._flatRange)
			return MAP_GV.BitType.block;
		else if (_slope > 0)
			return MAP_GV.BitType.pos;
		else
			return MAP_GV.BitType.neg;
	}

	public Vector2 retTopLeftCorner(){
		if (type == MAP_GV.BitType.pos)
			return new Vector2 (transform.position.x + 0.5f * scaleOffset.x, transform.position.y - 0.5f * scaleOffset.y);
		else
			return new Vector2 (transform.position.x + 0.5f * scaleOffset.x, transform.position.y + 0.5f * scaleOffset.y);
		//toSet.x - 0.5f * scaleOffset.x, toSet.y + 0.5f * scaleOffset.y

	}

	public Vector2 retBottomLeftCorner(){
		return new Vector2 (transform.position.x - 0.5f * scaleOffset.x, transform.position.y - 0.5f * scaleOffset.y);
	}
		
		
}
