using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worldInstantiator : MonoBehaviour {
	public float gain = 1.2f;
	public float lacunarity =  1.2f;
	public float octaves = 7;
	public float baseWave = 250;
	public float baseAmp = 250;

	public float renderLeftBound = 0; // must be > 0 and < MAP_GV.mapLimit
	public float renderRightBound = 500; // must be > 0
	public float mapLimit = MAP_GV.mapLimit;
	public int tileType = 2;

	public float renderWall;

	public float mapYOffset = 0; // how high above 0 you want the lowest dip over whole map to be

	private float x;
	private Vector2 startPoint;
	private Vector2 endPoint;
	private float totalLowest;
	perlinManager currentMapCurve;
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
			renderLeftBound = initParams.spawnPosition - MAP_GV.renderDistance;
			renderRightBound = initParams.spawnPosition + MAP_GV.renderDistance;
            tileType = initParams.tileType;
            mapYOffset = initParams.mapYOffset;
        }
        unityParent = new GameObject().transform;
        unityParent.name = "WhiteWorld";
				// x range, y range ......... gain  lacunarity   octaves   baseWave  baseAmp 
		currentMapCurve = new perlinManager( gain, lacunarity, octaves, baseWave,  baseAmp);
		if (renderLeftBound < 0 || renderRightBound > mapLimit || renderLeftBound >= renderRightBound) {
			Debug.Log ("Some invalid parameters were entered into the Initialize World function. World NOT generating.");
		} else {
			currentMapStorage = new mapStorage (mapLimit, MAP_GV._xIncrement);
			Generate (renderLeftBound, renderRightBound, currentMapCurve);
		}
	}

	public void Generate(float startX, float endX, perlinManager _func){
		totalLowest = _func.lowestOverWholeRange(startX, MAP_GV._xIncrement , endX);
		_func.verticalOffset = totalLowest - mapYOffset;
		totalLowest = mapYOffset;
		x = startX;
		staticWorldBit topBit = new staticWorldBit();
		staticWorldBit midBit = new staticWorldBit();;
		staticWorldBit baseBit = new staticWorldBit();;
		float lowestPoint = _func.lowestOverInterval (startX, MAP_GV._xIncrement, startX + (float)MAP_GV._incrementBatch * MAP_GV._xIncrement);
		while (x < endX) {				
			startPoint = new Vector2 (x, _func.retY (x));
			endPoint = new Vector2 (x + MAP_GV._xIncrement, _func.retY (x + MAP_GV._xIncrement));
			worldBit b = InstantiateWB (worldBit.getType (startPoint, endPoint));
			if(b.type == MAP_GV.BitType.block && (x % (MAP_GV._incrementBatch * MAP_GV._xIncrement - 1) == 0))
				lowestPoint = _func.lowestOverInterval (x, MAP_GV._xIncrement, x + (float)MAP_GV._incrementBatch * MAP_GV._xIncrement);
			b.Initialize (startPoint, endPoint, lowestPoint);
			topBit = new staticWorldBit(b);
			if (b.type == MAP_GV.BitType.neg || b.type == MAP_GV.BitType.pos) {
				if (x % (MAP_GV._incrementBatch * MAP_GV._xIncrement - 1) == 0){
					lowestPoint = _func.lowestOverInterval (x, MAP_GV._xIncrement, x + (float)MAP_GV._incrementBatch * MAP_GV._xIncrement);
				}
				worldBit _b = InstantiateWB (MAP_GV.BitType.block);
				Vector2 basePos = b.retTopLeftCorner ();
				_b.Initialize (basePos, new Vector2 (basePos.x + MAP_GV._xIncrement, basePos.y), lowestPoint);
				midBit = new staticWorldBit(_b);
				b = _b;
				Destroy (_b);
			}
				

			if (x % (MAP_GV._incrementBatch * MAP_GV._xIncrement -1) == 0) {
				lowestPoint = _func.lowestOverInterval (x, MAP_GV._xIncrement, x + (float)MAP_GV._incrementBatch * MAP_GV._xIncrement);
				//lowestPoint = _func.lowestOverInterval (x, MAP_GV._xIncrement, x + (float)MAP_GV._incrementBatch * MAP_GV._xIncrement + 1);
				blockWB _B = (blockWB)InstantiateWB (MAP_GV.BitType.block);
				Vector2 basePos = b.retBottomLeftCorner ();
				//b.gameObject.GetComponent<SpriteRenderer> ().color = new Color (0, 50, 0);
				_B.Initialize (basePos, new Vector2 (basePos.x, basePos.y), totalLowest);
				_B.bigBlockResize ((float)MAP_GV._incrementBatch * MAP_GV._xIncrement -1, totalLowest + MAP_GV._distToBedrock + MAP_GV._floorSafety, basePos.y);
				_B.SetBIGBlockToWorldPos(basePos);
				baseBit = new staticWorldBit (_B);
				//_B.gameObject.GetComponent<SpriteRenderer> ().color = new Color (0, 0, 50);
				Destroy (b);
				Destroy (_B);
			}
		
			if(!midBit.isNull)
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
