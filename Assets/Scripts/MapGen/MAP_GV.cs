using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAP_GV : MonoBehaviour {


	public static float _xIncrement = 0.5f; //breaks when going below 0.5f for some reason, results in negative scale
	public static float _flatRange = 0.04f;
	public static int _incrementBatch = 25; 

	public static float _bedrock = -50;
	public static float _floorSafety = 0.25f;


	enum _tileType {W, B, Wn}
	public static string tileType = "Wn"; // "W" , "", or "Wn"

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
