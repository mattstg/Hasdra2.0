using UnityEngine;
using System.Collections;

public class HairlineSplitterFactory
{

    #region Singleton
    private static HairlineSplitterFactory instance;

    private HairlineSplitterFactory() { }

    public static HairlineSplitterFactory Instance
    {
        get{
            if(instance == null)
            {
                instance = new HairlineSplitterFactory();
            }
            return instance;
        }
    }
#endregion

    public void CreateStampingHairline(float defensivePower, float offensivePower, GameObject beingStamped,float incomingAngle, Vector2 pos)
    {
        return; //Currelty turned off
        //Debug.Log(string.Format("hairline requested for {0} at {1} defense vs {2} offensive",beingStamped.name,defensivePower,offensivePower));
        LayerMask oldLm = beingStamped.layer;
        beingStamped.layer = LayerMask.NameToLayer("Hairline");
        int hairlinePower = (int)(offensivePower / defensivePower);
        if (hairlinePower > 0) //cuz 1 will not initalize anything else
        {
            GameObject go = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Spell/Hairline")) as GameObject;
            go.transform.position = pos;
            go.GetComponent<Hairline>().Initialize(hairlinePower--, incomingAngle);
        }
        beingStamped.layer = oldLm;

        //Use the two to calculate direction of stamp
        //Have to temp swap layers to stamp

    }

    public void CreateHairline(SpellInfo spellInfo, Vector2 position, Vector2 direction)
    {
        GameObject emptyParent = new GameObject();
        emptyParent.name = "Hairline Parent";
        emptyParent.transform.position = position;
        HairlineSplitter hairline = MonoBehaviour.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Spell/Hairline")).GetComponent<HairlineSplitter>();
        hairline.InitializeHairline(spellInfo.currentEnergy, emptyParent.transform, direction, spellInfo.materialType);
    }



}
