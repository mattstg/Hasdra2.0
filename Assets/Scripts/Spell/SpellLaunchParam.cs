using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellLaunchParam  {

    public string spellName;
    public Vector2 displacement;

	public SpellLaunchParam(string _spellName, Vector2 _displacement)
    {
        spellName = _spellName;
        displacement = _displacement;
    }
}
