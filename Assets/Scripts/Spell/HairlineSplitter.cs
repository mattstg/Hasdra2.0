using UnityEngine;
using System.Collections;

//old one
public class HairlineSplitter : MonoBehaviour {

    //I imagine hairline splitter in the future can build "solidmaterial/spells", which grow over time. This would be used for lighting, tree growth, and etc

    Transform hairlineMainParent;
    float energy;
    GV.MaterialType matType;
    Vector2 direction;

    public void InitializeHairline(float _energy,  Transform _parentTransform, Vector2 dir, GV.MaterialType _matType)
    {
        energy = _energy;
        direction = dir;
        matType = _matType;
        hairlineMainParent = _parentTransform;
        transform.SetParent(hairlineMainParent);

        //energy is area, 
        float ratiox = GV.HAIRLINE_SIZE_RATIO.y / GV.HAIRLINE_SIZE_RATIO.x;
        Vector2 newScale = new Vector2();
        newScale.x = Mathf.Sqrt(energy / ratiox);
        newScale.y = newScale.x * ratiox;
        transform.localScale = newScale;

        Destructible2D.D2dExplosion exp = gameObject.GetComponent<Destructible2D.D2dExplosion>();
        exp.StampHardness = MaterialDict.Instance.GetStampValue(matType, true, energy, 1);
        exp.StampSize = transform.localScale;
        /*exp.ForceStamp();
        Collider2D collider = gameObject.GetComponent<Collider2D>();
        collider.enabled = true;
        bool somethingRemains = (Physics2D.IsTouchingLayers(collider, (int)GV.Layers.Terrain) || Physics2D.IsTouchingLayers(collider, (int)GV.Layers.Spell));
        if (somethingRemains)
            Destroy(gameObject);
        else*/
        if(energy > GV.HAIRLINE_MIN_ENERGY)
            SpawnChildren();
        //Split (make sure are children of same class)
        //fade and die
    }

    private Vector2 GetEndPoint()
    {
        Vector2 endGoal = GV.MultVect(direction.normalized,transform.localScale);
        return endGoal;
    }

    private void SpawnChildren()
    {
        float percentPortion = 1 / (GV.HAIRLINE_MAX_SPLITS + 1); //to make less equal splits
        float[] energySplit = new float[GV.HAIRLINE_MAX_SPLITS];
        for (int i = 0; i <= GV.HAIRLINE_MAX_SPLITS; i++) //splitting percentage of power
        {
            energySplit[Random.Range(0, GV.HAIRLINE_MAX_SPLITS - 1)] += percentPortion;
        }
        for (int i = 0; i < energySplit.Length; i++)
            if (energySplit[i] > 0)
                CreateNewSplit(energySplit[i]);
    }

    private void CreateNewSplit(float percentageAllocated)
    {
        int degRot = Random.Range(0, GV.HAIRLINE_MAX_DEGREE_VARIATION);
        Vector2 newDir = GV.RotateV2(direction, degRot);
        HairlineSplitter childSplit = (Instantiate(Resources.Load<GameObject>("Prefabs/Spell/Hairline"), GetEndPoint(), Quaternion.identity) as GameObject).GetComponent<HairlineSplitter>();
        childSplit.InitializeHairline(percentageAllocated * energy, hairlineMainParent, newDir, matType);
    }

}
