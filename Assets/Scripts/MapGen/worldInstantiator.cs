using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worldInstantiator : MonoBehaviour {
	public float gain = 1.2f;
	public float lacunarity =  1.2f;
	public float octaves = 7;
	public float baseWave = 250;
	public float baseAmp = 250;

	public float initWorldLeftRender = 0; // must be > 0 and < MAP_GV.mapLimit
	public float initWorldRightRender = 500; // must be > 0
	public float mapLimit = MAP_GV.mapLimit;
	public int tileType = 2;

	public float renderWall;

	public float mapYOffset = 0; // how high above 0 you want the lowest dip over whole map to be

	//private float x;
	private Vector2 startPoint;
	private Vector2 endPoint;
	private float totalLowest;
	public perlinManager currentMapCurve;
	public mapStorage currentMapStorage;

    Transform unityParent; //Used to store transforms for hiearchy

	public void Start(){
		InitializeWorld (null);
	}

	public void Update(){	}

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
			initWorldLeftRender = initParams.spawnPosition - MAP_GV.renderDistance;
			initWorldRightRender = initParams.spawnPosition + MAP_GV.renderDistance;
            tileType = initParams.tileType;
            mapYOffset = initParams.mapYOffset;
        }
        unityParent = new GameObject().transform;
        unityParent.name = "WhiteWorld";
				// x range, y range ......... gain  lacunarity   octaves   baseWave  baseAmp 
		currentMapCurve = new perlinManager( gain, lacunarity, octaves, baseWave,  baseAmp);
		if (initWorldLeftRender < 0 || initWorldRightRender > mapLimit || initWorldLeftRender >= initWorldRightRender) {
			Debug.Log ("Some invalid parameters were entered into the Initialize World function. World NOT generating.");
		} else {
			currentMapStorage = new mapStorage (mapLimit, MAP_GV._xIncrement);
			totalLowest = currentMapCurve.lowestOverWholeRange (0, MAP_GV._xIncrement, mapLimit);
			currentMapCurve.verticalOffset = totalLowest - mapYOffset;
			totalLowest = mapYOffset;
			Generate (initWorldLeftRender, initWorldRightRender);
		}
	}

	public void GenerateChunk(float _d){
		float chunkLength = (MAP_GV._incrementBatch * MAP_GV._xIncrement);
		float chunkStart = _d - (_d % chunkLength);
		float chunkEnd = chunkStart + chunkLength;
		//totalLowest = currentMapCurve.lowestOverWholeRange(0, MAP_GV._xIncrement , MAP_GV.mapLimit);
		float lowestPoint = currentMapCurve.lowestOverInterval(chunkStart, MAP_GV._xIncrement , chunkEnd + 1);
		float d = chunkStart;
		staticWorldBit topBit = new staticWorldBit();
		staticWorldBit midBit = new staticWorldBit();
		staticWorldBit baseBit = new staticWorldBit();
		while(d < chunkEnd){
			//Debug.Log ("d " + d + " isNull? " + currentMapStorage.getMapVBit(d));
			if (currentMapStorage.retVBitAtWorldX(d) == null) {
				//the map array is empty at this location, make new vBit!
				//totalLowest = currentMapCurve.lowestOverWholeRange(0, MAP_GV._xIncrement , MAP_GV.mapLimit);
				///////////////////////
				startPoint = new Vector2 (d, currentMapCurve.retY (d));
				endPoint = new Vector2 (d + MAP_GV._xIncrement, currentMapCurve.retY (d + MAP_GV._xIncrement));
				worldBit b = InstantiateWB (worldBit.getType (startPoint, endPoint));
				b.Initialize (startPoint, endPoint, lowestPoint);
				topBit = new staticWorldBit(b);
				if (b.type == MAP_GV.BitType.neg || b.type == MAP_GV.BitType.pos) {
					worldBit _b = InstantiateWB (MAP_GV.BitType.block);
					Vector2 basePos = b.retTopLeftCorner ();
					_b.Initialize (basePos, new Vector2 (basePos.x + MAP_GV._xIncrement, basePos.y), lowestPoint);
					midBit = new staticWorldBit(_b);
					b = _b;
					Destroy (_b);
				}


				if (d % (MAP_GV._incrementBatch * MAP_GV._xIncrement) == 0) {
					blockWB _B = (blockWB)InstantiateWB (MAP_GV.BitType.block);
					Vector2 basePos = b.retBottomLeftCorner ();
					_B.Initialize (basePos, new Vector2 (basePos.x, basePos.y), totalLowest);
					_B.bigBlockResize (chunkLength, totalLowest + MAP_GV._distToBedrock + MAP_GV._floorSafety, basePos.y);
					_B.SetBIGBlockToWorldPos(basePos);
					baseBit = new staticWorldBit (_B);
					_B.gameObject.GetComponent<SpriteRenderer> ().color = new Color (0, 0, 50);
					Destroy (b);
					Destroy (_B);
				}

				/////////////////////



				if(!midBit.notInitialized){
					currentMapStorage.loadMapVBit (d, new mapVBit (topBit, midBit, baseBit));
				//currentMapStorage.loadMapVBit (x - MAP_GV._xIncrement, new mapVBit (topBit, midBit, baseBit));
				}else{
					currentMapStorage.loadMapVBit(d, new mapVBit(topBit,baseBit,new staticWorldBit()));
				//currentMapStorage.loadMapVBit(x - MAP_GV._xIncrement, new mapVBit(topBit,baseBit,new staticWorldBit()));
				}

			} else {
				// map array has a VBit in this location, so do nothing.

			}
			d += MAP_GV._xIncrement;
		}
	}

	public void Generate(float startX, float endX){
		float x = startX;
		staticWorldBit topBit = new staticWorldBit();
		staticWorldBit midBit = new staticWorldBit();
		staticWorldBit baseBit = new staticWorldBit();
		float lowestPoint = currentMapCurve.lowestOverInterval (startX, MAP_GV._xIncrement, startX + (float)MAP_GV._incrementBatch * MAP_GV._xIncrement + 1);
		while (x < endX || ((x % (MAP_GV._incrementBatch * MAP_GV._xIncrement) != 0) && x < mapLimit && x >= 0)) {
			if (x % (MAP_GV._incrementBatch * MAP_GV._xIncrement) == 0)
				lowestPoint = currentMapCurve.lowestOverInterval (x, MAP_GV._xIncrement, x + (float)MAP_GV._incrementBatch * MAP_GV._xIncrement + 1);
			startPoint = new Vector2 (x, currentMapCurve.retY (x));
			endPoint = new Vector2 (x + MAP_GV._xIncrement, currentMapCurve.retY (x + MAP_GV._xIncrement));
			worldBit b = InstantiateWB (worldBit.getType (startPoint, endPoint));
			b.Initialize (startPoint, endPoint, lowestPoint);
			topBit = new staticWorldBit(b);
			if (b.type == MAP_GV.BitType.neg || b.type == MAP_GV.BitType.pos) {
				worldBit _b = InstantiateWB (MAP_GV.BitType.block);
				Vector2 basePos = b.retTopLeftCorner ();
				_b.Initialize (basePos, new Vector2 (basePos.x + MAP_GV._xIncrement, basePos.y), lowestPoint);
				midBit = new staticWorldBit(_b);
				b = _b;
				Destroy (_b);
			}
				

			if (x % (MAP_GV._incrementBatch * MAP_GV._xIncrement) == 0) {
				//lowestPoint = _func.lowestOverInterval (x, MAP_GV._xIncrement, x + (float)MAP_GV._incrementBatch * MAP_GV._xIncrement + 1);
				blockWB _B = (blockWB)InstantiateWB (MAP_GV.BitType.block);
				Vector2 basePos = b.retBottomLeftCorner ();
				//b.gameObject.GetComponent<SpriteRenderer> ().color = new Color (0, 50, 0);
				_B.Initialize (basePos, new Vector2 (basePos.x, basePos.y), totalLowest);
				_B.bigBlockResize ((float)MAP_GV._incrementBatch * MAP_GV._xIncrement, totalLowest + MAP_GV._distToBedrock + MAP_GV._floorSafety, basePos.y);
				_B.SetBIGBlockToWorldPos(basePos);
				baseBit = new staticWorldBit (_B);
				_B.gameObject.GetComponent<SpriteRenderer> ().color = new Color (10, 0, 0);
				//_B.gameObject.GetComponent<SpriteRenderer> ().color = new Color (0, 0, 50);
				Destroy (b);
				Destroy (_B);
			}
		
			if(!midBit.notInitialized)
				currentMapStorage.loadMapVBit (x, new mapVBit (topBit, midBit, baseBit));
				//currentMapStorage.loadMapVBit (x - MAP_GV._xIncrement, new mapVBit (topBit, midBit, baseBit));
			else
				currentMapStorage.loadMapVBit(x, new mapVBit(topBit,baseBit,new staticWorldBit()));
				//currentMapStorage.loadMapVBit(x - MAP_GV._xIncrement, new mapVBit(topBit,baseBit,new staticWorldBit()));
			
			x += MAP_GV._xIncrement;
		}
		Debug.Log(currentMapStorage.outputArr ());
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
		return currentMapCurve.retY (x);
	}

    public class WorldInitParams
    {
        public float gain, lacunarity, octaves, baseWave, baseAmp, mapLimit, spawnPosition, mapYOffset;
        public int tileType;
    }

}
