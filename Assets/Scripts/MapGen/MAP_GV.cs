using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAP_GV {

	public enum BitType {nul, pos, neg, block};

	public static float _xIncrement = 0.5f; //breaks when going below 0.5f for some reason, results in negative scale
	public static float _flatRange = 0.04f;
	public static int _incrementBatch = 20; 

	public static float renderDistance = 150;

	public static float _distToBedrock = -50; //from lowest point over whole map
	public static float _floorSafety = 0.25f; //internal peram
	public enum _tileType {W, B, Wn}

	public static float mapLimit = 20000;

}
