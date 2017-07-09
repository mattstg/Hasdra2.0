using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialInfo{

	public float density = 0;
    public float intialEnergyCreationEfficency = 1f;
    public string spriteLocation;
    public float gravity = 1f;

    public MaterialInfo(float _density, float _intialEnergyCreationEfficency, string _spriteLocation, float _gravity)
    {
        density = _density;
        intialEnergyCreationEfficency = _intialEnergyCreationEfficency;
        spriteLocation = _spriteLocation;
        gravity = _gravity;
    }
}
