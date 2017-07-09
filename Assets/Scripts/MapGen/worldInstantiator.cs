using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worldInstantiator : MonoBehaviour {
	public functions defaultFunc = new secondPower();

	private float x;
	private Vector2 startPoint;
	private Vector2 endPoint;
	private float totalLowest;



	// Use this for initialization
	void Start () {
				// x range, y range ......... gain  lacunarity   octaves   baseWave  baseAmp 
		Generate (-4000, 4000,new perlinManager(1.5f     ,     1.5f   ,  6,        250,       150));


	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Generate(float startX, float endX, functions _func){
		totalLowest = _func.lowestOverInterval (startX, MAP_GV._xIncrement, endX);
		if (totalLowest < MAP_GV._bedrock)
			Debug.Log ("perlin extends below _bedrock");
		x = startX;
		float lowestPoint = _func.lowestOverInterval (startX, MAP_GV._xIncrement, startX + (float) MAP_GV._incrementBatch * MAP_GV._xIncrement);
		while (x < endX) {
			startPoint = new Vector2(x,_func.retY (x));
			endPoint = new Vector2(x + MAP_GV._xIncrement,_func.retY (x + MAP_GV._xIncrement));
			worldBit b = InstantiateWB (worldBit.getType (startPoint, endPoint));
			b.Initialize(startPoint,endPoint,lowestPoint);
			if (b.type == worldBit.BitType.neg || b.type == worldBit.BitType.pos) {
				worldBit _b = InstantiateWB (worldBit.BitType.block);
				Vector2 basePos = b.retBaseCorner ();
				_b.Initialize(basePos,new Vector2(basePos.x + MAP_GV._xIncrement,basePos.y),lowestPoint);
				_b.markForDestruction ();
				b = _b;
			}
				
			x += MAP_GV._xIncrement;
			if ((x - startX) % (MAP_GV._incrementBatch * MAP_GV._xIncrement) == 0) {
				lowestPoint = _func.lowestOverInterval (x, MAP_GV._xIncrement, x + (float)MAP_GV._incrementBatch * MAP_GV._xIncrement);
				worldBit _b = InstantiateWB (worldBit.BitType.block);
				Vector2 basePos = b.retBottomCorner ();
				_b.Initialize(basePos,new Vector2(basePos.x,basePos.y),totalLowest);
				blockWB temp = (blockWB) _b;
				temp.bigBlockResize ((float)MAP_GV._incrementBatch * MAP_GV._xIncrement, totalLowest + MAP_GV._bedrock + MAP_GV._floorSafety,basePos.y);
				temp.SetToWorldPos (basePos);
				_b.markForDestruction ();
				temp.markForDestruction ();
			}
			b.markForDestruction ();

		}

	}

	public void Generate(float startX, float endX){
		Generate (startX, endX, defaultFunc);
	}



	public worldBit InstantiateWB(worldBit.BitType type){
		string s = MAP_GV.tileType;
		GameObject _worldBit = null;
		switch (type) {
		case worldBit.BitType.pos:
			_worldBit = (GameObject) Instantiate (Resources.Load ("Prefabs/MapGen/Pos" + s));
			break;
		case worldBit.BitType.block:
			_worldBit = (GameObject) Instantiate (Resources.Load ("Prefabs/MapGen/Block" + s));
			break;
		case worldBit.BitType.neg:
			_worldBit = (GameObject) Instantiate (Resources.Load ("Prefabs/MapGen/Neg" + s));
			break;
		case worldBit.BitType.nul:
			Debug.Log ("Here??");
			break;
		default: 
			Debug.Log ("Here??!?!?");
			break;
		}
		return _worldBit.GetComponent<worldBit> ();;
	}

}
