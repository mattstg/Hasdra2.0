using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class perlinManager : functions {
	perlin[] pearls; 
	ridgeNoise ridges;
	public float verticalOffset = 0;

	public perlinManager(float _gain, float _lacunarity, float _octaves, float _baseWavelength, float _baseAmplitude){
		float offset = Random.Range(-1000,-10000);
		pearls = new perlin [(int) _octaves - 1];

		for(int c = 0; c < _octaves; c++){
			if(c == 0)
				ridges = (ridgeNoise) new ridgeNoise(_baseAmplitude,_baseWavelength,offset);
			else
				pearls[c - 1] = (perlin) new perlin(_baseAmplitude/(_gain * c),_baseWavelength/(_lacunarity * c),offset);
		}
	}

	public override float retY(float x){
		float sum = 0;
		foreach (perlin p in pearls){
			sum += p.retY (x);
		}
		sum += ridges.retY (x);
		return sum - verticalOffset;
	}

}
