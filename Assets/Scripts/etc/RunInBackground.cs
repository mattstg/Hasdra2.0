﻿using UnityEngine;
using System.Collections;

public class RunInBackground : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Application.runInBackground = true;
	}
	
}
