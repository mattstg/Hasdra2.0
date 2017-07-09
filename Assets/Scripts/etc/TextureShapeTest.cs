using UnityEngine;
using System.Collections;

public class TextureShapeTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
         
        GetComponent<Renderer>().material = Resources.Load("testMat") as Material;
        GetComponent<MeshFilter>().mesh.RecalculateNormals();
        //Material mat = Resources.Load("testMat") as Material;

	}
	
}
