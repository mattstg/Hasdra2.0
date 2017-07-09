using UnityEngine;
using System.Collections;

public class treeKnob{
    public bool topKnob = false;
    public bool spotTaken = false;
    public Vector2 knobLocation;

    public treeKnob(bool _topKnob, Vector2 _knobLocation)
    {
        topKnob = _topKnob;
        knobLocation = _knobLocation;
    }
}
