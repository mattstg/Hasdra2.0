using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;


public class StartState : State {
    /*
    public Toggle castOnChargeInput;
    public Toggle repeatAfterCastInput;
    public InputField energyInput;
    int energy = 1;
    bool castOnCharge = false;
    bool repeatAfterCast = false;
    
    public InputField directionXInput;
    public InputField directionYInput;
    public InputField speedInput;
    Vector2 direction = new Vector2(1,0);
    float speed = 1;

    public Toggle isMeleeInput;
    bool isMelee = false;

    public InputField shapeInput;
    string shape = "ball";

    public InputField densityInput;
    float density = 1;

    public InputField typeInput;
    string materialType = "energy";

    public override void RetrieveInfo()
    {
        energy = int.Parse(energyInput.text);
        castOnCharge = castOnChargeInput.isOn;
        repeatAfterCast = repeatAfterCastInput.isOn;
        direction = new Vector2(float.Parse(directionXInput.text), float.Parse(directionYInput.text));
        speed = float.Parse(speedInput.text);
        shape = shapeInput.text;
        isMelee = isMeleeInput.isOn;
        density = float.Parse(densityInput.text);
        materialType = typeInput.text;
    }

    public System.Collections.Generic.Dictionary<string, string> ExportForXML()
    {
        Dictionary<string, string> toReturn = new Dictionary<string, string>();
        toReturn.Add("Type", "StartState");
        toReturn.Add("Energy", energy.ToString());
        toReturn.Add("CastOnCharge", castOnCharge.ToString());
        toReturn.Add("RepeatAfterCast", repeatAfterCast.ToString());
        toReturn.Add("DirX", direction.x.ToString());
        toReturn.Add("DirY", direction.y.ToString());
        toReturn.Add("Speed", speed.ToString());
        toReturn.Add("Shape", shape);
        toReturn.Add("Density", density.ToString());
        toReturn.Add("MaterialType", materialType);
        toReturn.Add("IsMelee", isMelee.ToString());
        return null;
        //return toReturn.Concat(base.ExportForXML()).ToDictionary(x => x.Key, x => x.Value);
    }

    public void ImportFromDictionary(System.Collections.Generic.Dictionary<string, string> importDict)
    {
        StateID = -1;
        energy = int.Parse(importDict["Energy"]);
        castOnCharge = bool.Parse(importDict["CastOnCharge"]);
        repeatAfterCast = bool.Parse(importDict["RepeatAfterCast"]);
        direction = new Vector2(float.Parse(importDict["DirX"]),float.Parse(importDict["DirY"]));
        speed = float.Parse(importDict["Speed"]);
        shape = importDict["Shape"];
        try
        {
            isMelee = bool.Parse(importDict["IsMelee"]);
        }
        catch
        {
            isMelee = false;
            string toOut = "";
            foreach (string key in importDict.Keys)
            {
                toOut += key + ", ";
            }
            Debug.LogError("isMelee failed, all keys = " + toOut);
        }

        density = float.Parse(importDict["Density"]);
        materialType = importDict["MaterialType"];
        base.ImportFromDictionary(importDict);
    }

    public void FillGUIFromInternalVars()
    {
        energyInput.text = energy.ToString();
        castOnChargeInput.isOn = castOnCharge;
        repeatAfterCastInput.isOn = repeatAfterCast;
        directionXInput.text = direction.x.ToString();
        directionYInput.text = direction.y.ToString();
        speedInput.text = speed.ToString();
        isMeleeInput.isOn = isMelee;
        shapeInput.text = shape;
        densityInput.text = density.ToString();
        typeInput.text = materialType;
    }

    public void ClearStartState()
    {
        energy = 1;
        castOnCharge = true;
        repeatAfterCast = true;
        direction = new Vector2(0,1);
        speed = 1;
        shape = "Ball";
        density = 1;
        materialType = "Energy";
        isMelee = false;
        FillGUIFromInternalVars();
    }

    public SpellInfo ExtractSpellInfo()
    {
        SpellInfo toReturn = new SpellInfo();
        toReturn.energyLimit = energy;
        toReturn.castOnCharge = castOnCharge;
        toReturn.repeatAfterCast = repeatAfterCast;
        toReturn.initialHeadingAngle = direction;
        toReturn.velocity = speed;
        toReturn.shape = shape;
        toReturn.density = density;
        toReturn.materialType = GV.GetMaterialTypeFromString(materialType); // (GV.MaterialType)System.Enum.Parse(typeof(GV.MaterialType), materialType, true);
        toReturn.isMelee = isMelee;
        return toReturn;
    }*/

}
