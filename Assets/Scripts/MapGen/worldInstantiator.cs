﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worldInstantiator : MonoBehaviour {
	public float gain = 1.2f;
	public float lacunarity =  1.2f;
	public float octaves = 7;
	public float baseWave = 250;
	public float baseAmp = 250;

	public float mapMin = -500;
	public float mapMax = 500;
	public int tileType = 2;

	public float mapYOffset = 0;

	private float x;
	private Vector2 startPoint;
	private Vector2 endPoint;
	private float totalLowest;
	perlinManager currentMap;

    Transform unityParent; //Used to store transforms for hiearchy


	// Use this for initialization
	public void InitializeWorld (WorldInitParams initParams)
    {
        if(initParams != null) //If null use above/editor values, otherwise take values from round setup
        {
            gain = initParams.gain;
            lacunarity = initParams.lacunarity;
            octaves = initParams.octaves;
            baseWave = initParams.baseWave;
            baseAmp = initParams.baseAmp;
            mapMin = initParams.mapMin;
            mapMax = initParams.mapMax;
            tileType = initParams.tileType;
            mapYOffset = initParams.mapYOffset;
        }
        unityParent = new GameObject().transform;
        unityParent.name = "WhiteWorld";
				// x range, y range ......... gain  lacunarity   octaves   baseWave  baseAmp 
		currentMap = new perlinManager( gain, lacunarity, octaves, baseWave,  baseAmp);
		Generate (mapMin, mapMax, currentMap);
	}


	public void Generate(float startX, float endX, perlinManager _func){
		totalLowest = _func.lowestOverInterval (startX, MAP_GV._xIncrement, endX);
		_func.verticalOffset = totalLowest - mapYOffset;
		totalLowest = mapYOffset;
		x = startX;
		float lowestPoint = _func.lowestOverInterval (startX, MAP_GV._xIncrement, startX + (float) MAP_GV._incrementBatch * MAP_GV._xIncrement + 1);
		while (x < endX) {
			startPoint = new Vector2(x,_func.retY (x));
			endPoint = new Vector2(x + MAP_GV._xIncrement,_func.retY (x + MAP_GV._xIncrement));
			worldBit b = InstantiateWB (worldBit.getType (startPoint, endPoint));
			b.Initialize(startPoint,endPoint,lowestPoint);
			if (b.type == MAP_GV.BitType.neg || b.type == MAP_GV.BitType.pos) {
				worldBit _b = InstantiateWB (MAP_GV.BitType.block);
				Vector2 basePos = b.retBaseCorner ();
				_b.Initialize(basePos,new Vector2(basePos.x + MAP_GV._xIncrement,basePos.y),lowestPoint);
				_b.markForDestruction ();
				b = _b;
			}
				
			x += MAP_GV._xIncrement;
			if ((x - startX) % (MAP_GV._incrementBatch * MAP_GV._xIncrement) == 0) {
				lowestPoint = _func.lowestOverInterval (x, MAP_GV._xIncrement, x + (float)MAP_GV._incrementBatch * MAP_GV._xIncrement + 1);
				worldBit _b = InstantiateWB (MAP_GV.BitType.block);
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


	public worldBit InstantiateWB(MAP_GV.BitType type){
		string s = ((MAP_GV._tileType)tileType).ToString();
		GameObject _worldBit = null;
		switch (type) {
		case MAP_GV.BitType.pos:
			_worldBit = (GameObject) Instantiate (Resources.Load ("Prefabs/MapGen/Pos" + s));
			break;
		case MAP_GV.BitType.block:
			_worldBit = (GameObject) Instantiate (Resources.Load ("Prefabs/MapGen/Block" + s));
			break;
		case MAP_GV.BitType.neg:
			_worldBit = (GameObject) Instantiate (Resources.Load ("Prefabs/MapGen/Neg" + s));
			break;
		case MAP_GV.BitType.nul:
			Debug.Log ("Here??");
			break;
		default: 
			Debug.Log ("Here??!?!?");
			break;
		}
        _worldBit.transform.SetParent(unityParent);

        return _worldBit.GetComponent<worldBit> ();;
	}

	public float GetSurfacePoint(float x){
		return currentMap.retY (x);
	}

    public class WorldInitParams
    {
        public float gain, lacunarity, octaves, baseWave, baseAmp, mapMin, mapMax, mapYOffset;
        public int tileType;
    }

}
