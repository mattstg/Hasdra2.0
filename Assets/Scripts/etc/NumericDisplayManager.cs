using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NumericDisplayManager : MonoBehaviour {
    //This class should handle the text, allowing certain things to be rendered or not. The enemy can make requests given thier int, to have it become visible (for thier local) as well.
    //If you give yourself as a calling class, you will access and create displays belonging to you.
	// Use this for initialization
    Dictionary<int, Dictionary<string, NumericDisplay>> numericDisplays = new Dictionary<int, Dictionary<string, NumericDisplay>>();
    Vector3 lastOffset = new Vector3(-GV.NUMERIC_DISPLAY_OFFSET_RANGE.x, -GV.NUMERIC_DISPLAY_OFFSET_RANGE.y, 0);

    /// <summary>
    /// Numeric display that when created, does not get stored in NumericDisplayManager, the calling object can manage it itself, or let it decay
    /// </summary>
    /// <param name="textColor"></param>
    /// <param name="startValue"></param>
    /// <param name="_parentTransform"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public NumericDisplay CreateDisposableNumericDisplay(Color textColor, string textPrefix, float startValue, Transform _parentTransform, bool hideNumber = false)
    {
        GameObject go = new GameObject();
        go.transform.SetParent(this.transform);
        go.AddComponent<NumericDisplay>().Initialize(textColor, textPrefix, startValue, _parentTransform, OffsetCreator(),hideNumber);
        return go.GetComponent<NumericDisplay>();
    }

    public void CreateNumericDisplay(Object callingObj, Transform _parentTransform, string displayName,string textPrefix, float _value, Color color, bool hardSet = false)
    {
        if (!GV.ND_ON)
            return;
        int objID = callingObj.GetInstanceID();
        Dictionary<string, NumericDisplay> activeNumericDisplays = GetCallingObjsDict(objID);
        if (!activeNumericDisplays.ContainsKey(displayName))
        {
            activeNumericDisplays.Add(displayName, StaticReferences.numericTextManager.CreateDisposableNumericDisplay(color,textPrefix ,_value, _parentTransform)); //hardcoded for now
            activeNumericDisplays[displayName].LinkFunctionForCallOnDeath(NumericDisplayDied, displayName, objID);
        }
        else
        {
            if (hardSet)
                activeNumericDisplays[displayName].SetValue(_value);
            else
                activeNumericDisplays[displayName].ModValue(_value);
        }
    }

    private Dictionary<string, NumericDisplay> GetCallingObjsDict(int objInstanceID)
    {
        //first check if the object has ever called this before
        if (!numericDisplays.ContainsKey(objInstanceID))
            numericDisplays.Add(objInstanceID, new Dictionary<string, NumericDisplay>());
        return numericDisplays[objInstanceID];
    }

    private void NumericDisplayDied(int objID, string deceasedName)
    {
        if (numericDisplays.ContainsKey(objID))
            if(numericDisplays[objID].ContainsKey(deceasedName))
            {
                numericDisplays[objID].Remove(deceasedName);
                if (numericDisplays[objID].Count == 0)
                {
                    numericDisplays.Remove(objID);
                }
                return;
            }
        Debug.LogError("numeric display removal attempt, not found for objID,name: " + objID + "," + deceasedName);
    }


    private Vector2 OffsetCreator()
    {
        lastOffset.x += GV.NUMERIC_DISPLAY_SPACE;
        if (lastOffset.x > GV.NUMERIC_DISPLAY_OFFSET_RANGE.x)
        {
            lastOffset.x = -GV.NUMERIC_DISPLAY_OFFSET_RANGE.x;
            lastOffset.y += GV.NUMERIC_DISPLAY_SPACE;
        }
        if (lastOffset.y > GV.NUMERIC_DISPLAY_OFFSET_RANGE.y)
            lastOffset.y = -GV.NUMERIC_DISPLAY_OFFSET_RANGE.y;
        return lastOffset;
    }

}
