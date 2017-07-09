using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ridgeNoise : functions  {
	float offset;
	float amplitude;
	float wavelength;

	public ridgeNoise(float amp, float wavel, float offs){
		offset = offs;
		amplitude = amp;
		wavelength = wavel;
	}

	public override float retY(float x){
		return (Mathf.Abs(Mathf.PerlinNoise ((x + offset)/wavelength, offset/wavelength)) * -1 + 1)  * amplitude;
	}

}
