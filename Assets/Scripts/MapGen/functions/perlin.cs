using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class perlin : functions  {
	float offset;
	float amplitude;
	float wavelength;

	public perlin(float amp, float wavel, float offs){
		offset = offs;
		amplitude = amp;
		wavelength = wavel;
	}

	public override float retY(float x){
		return Mathf.PerlinNoise ((x + offset)/wavelength, offset/wavelength) * amplitude;
	}
		
}
