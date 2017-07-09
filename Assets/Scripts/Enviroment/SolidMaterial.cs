using UnityEngine;
using System.Collections;
//this class will hijack tags
//[RequireComponent(typeof(Destructible2D.D2dQuadFracturer))]
public class SolidMaterial : MonoBehaviour, DestructibleInterface {

    public SpellInfo spellInfo;
    public GV.MaterialType InitialMaterialType = GV.MaterialType.Rock;  //THIS IS ONLY USED BY TERRA, AND IS THIS WAY SO CAN BE EASILY EDITED IN EDITOR, ONLY USED ONCE ON TERRA INITIALIZATION
    public float InitialDensity = 1;                                    //Same as above, use spell infos instead.
    public bool DEBUG_ON = false;
    public bool indestructible = false;
    public bool disableOptimization = false;

    public bool canHover = true;
    float defenseScore = GV.GROUND_DEFENSE_SCORE; //same thing as life/enery/defense, resistance to being stamped i think
    float maxHp = GV.GROUND_MAX_HP;             //if takes that much dmg, then it fractures
    float dmgTaken = 0;                         //Im sure this gets reset by a clone, which is good so something doesnt infinite fracture, but split materials will "repair" them then
    bool materialIsKinematic = true;
    bool hasBeenInitialized = false;
    bool fadingAway = false;
    float maxArea;
    //public float massPerPixel;
    //public bool outputStuff = false;

    public void InitializeSplitChild(SpellInfo _spellinfo, int childsPixelCount)
    {
        spellInfo = _spellinfo;
        spellInfo.ModifyPixels(childsPixelCount);
        SetMaterialProperties(CalculateHover(childsPixelCount),false);
        //spellInfo.numOfPixels = childsPixelCount;
        //since tight squeezed ground from split and fractures can cause problems, shrink it minutly
        transform.localScale = transform.localScale - new Vector3(.01f, .01f, 0);
        if (materialIsKinematic)
            CheckIfEventaullyFractures();

        if (GetComponent<Rigidbody2D>())
            GetComponent<Rigidbody2D>().mass = spellInfo.mass;
    }
    /*
    public void InitializeSplitChild(float alphaCount, float _massPerPixel)
    {
        RecalculateCalculateMass(alphaCount, _massPerPixel);
        SetMaterialProperties(CalculateHover(alphaCount));
        //since tight squeezed ground from split and fractures can cause problems, shrink it minutly
        transform.localScale = transform.localScale - new Vector3(.01f, .01f, 0);
        if (materialIsKinematic)
            CheckIfEventaullyFractures();

    }

    public void InitializeMaterial(float desiredPixelCount, bool startsKinematic = true) //called if a parent material being created for first time
    {
        this.gameObject.tag = "Terrain"; //wonder if i still need this or if its been resolved by the clone?
        SetMaterialProperties(startsKinematic);
        CalculateInitialMass();
        //SetToDesiredPixels(desiredPixelCount);
        SetToDesiredPixels();
        RecalculateCalculateMass();
    }

    //This one should be called by Terra
    public void InitializeSplitChild()
    {
        SpellInfo _spellInfo = new SpellInfo();
        

        InitializeSplitChild(alphaCount, massPerPixel);
    }*/

    //This one should be called by Terra
    public void InitializeMaterial()
    {
        spellInfo = new SpellInfo();
        spellInfo.materialType = InitialMaterialType;
        SetMaterialProperties(true); //here the raw pixels are set
        float unitArea = spellInfo.numOfPixels / GV.PIXELS_PER_UNITAREA;
        float initialEnergy = unitArea * InitialDensity * transform.localScale.x * transform.localScale.y * GV.GROUND_ENERGY_PER_UNIT_AREA;
        float totalMass = MaterialDict.Instance.GetWeightPerEnergy(spellInfo.materialType) * initialEnergy;
        if(DEBUG_ON) Debug.Log("UnitArea: " + unitArea + ", initialEnergy: " + initialEnergy + ", totalMass: " + totalMass);
        spellInfo.mass = totalMass;
        spellInfo.currentEnergy = initialEnergy;
        defenseScore = MaterialDict.Instance.GetStampValue(spellInfo.materialType, false, spellInfo.currentEnergy, spellInfo.densityEffect);
        maxHp = spellInfo.currentEnergy;
        this.gameObject.tag = "Terrain"; //wonder if i still need this or if its been resolved by the clone?
        SetToDesiredPixels();
        if (GetComponent<Rigidbody2D>())
            GetComponent<Rigidbody2D>().mass = spellInfo.mass;
        if (DEBUG_ON) Debug.Log("Material Initialized : " + spellInfo);
    }

    //initialize a solidmaterial from a spell, or if its from a spell thats been split, pass the newNumberOfPixels, since an AlphaCount call at this stage would kill it
    public void InitializeMaterial(SpellInfo _spellInfo,int newNumberOfPixels = -1)
    {
        spellInfo = _spellInfo;
        if (spellInfo == null)
            Debug.Log("dif prob");
        bool isSplitChild = (newNumberOfPixels != -1);        
        defenseScore = MaterialDict.Instance.GetStampValue(spellInfo.materialType,false,spellInfo.currentEnergy, spellInfo.densityEffect);
        maxHp = spellInfo.currentEnergy;
        this.gameObject.tag = "Terrain"; //wonder if i still need this or if its been resolved by the clone?
        SetMaterialProperties(false, !isSplitChild);        
        if (GV.GetSpellFormByMaterialType(spellInfo.materialType) == GV.SpellForms.Energy) //Physical type optimize at a way earlier stage, energy at this stage is first time recieving destr
        {
            for (int i = 0; i < GV.SPELL_PIXEL_OPTIMZATION; i++)
            {
                GetComponent<Destructible2D.D2dDestructible>().HalveAlpha();
                spellInfo.pixelOptimizationLevel++;
            }
        }

        if (isSplitChild)  //if is a split child, initialize it as if it was the parent, then reduce to appriopriate size (should update mass and pixels)
        {
            //Debug.Log("the childs spellInfo: " + spellInfo);
            spellInfo.ModifyPixels(newNumberOfPixels);
        }

        if (GetComponent<Rigidbody2D>())
            GetComponent<Rigidbody2D>().mass = spellInfo.mass;
    }
    
    //Initialize a material from a normal spell into a solid material, does not come from splitting
    //can be energy or phys base
    /*public void InitializeMaterial(SpellInfo spellInfo)
    {
        defenseScore = 1;//spellInfo.currentEnergy;
        maxHp = spellInfo.currentEnergy;
        materialType = spellInfo.materialType;
        this.gameObject.tag = "Terrain"; //wonder if i still need this or if its been resolved by the clone?
        SetMaterialProperties(false);
        currentMass = GetComponent<Rigidbody2D>().mass;
        if (GV.GetSpellFormByMaterialType(spellInfo.materialType) == Spell.SpellForms.Energy) //Physical type optimize at a way earlier stage, energy at this stage is first time recieving destr
        {
            for (int i = 0; i < GV.SPELL_PIXEL_OPTIMZATION; i++)
                GetComponent<Destructible2D.D2dDestructible>().HalveAlpha();
        }
        massPerPixel = currentMass / GetComponent<Destructible2D.D2dDestructible>().AlphaCount;
    }*/

    void SetMaterialProperties(bool startsKinematic, bool allowPixelSet = true)
    {
        materialIsKinematic = startsKinematic;
        if (materialIsKinematic)
        {
                if (!GetComponent<Destructible2D.D2dEdgeCollider>())
                {
                    gameObject.AddComponent<Destructible2D.D2dEdgeCollider>();
                    Destroy(GetComponent<Rigidbody2D>());
                    Destroy(GetComponent<SolidMaterialCollision>());
                    Destroy(GetComponent<Destructible2D.D2dPolygonCollider>());
                    Destroy(GetComponent<Destructible2D.D2dQuadFracturer>());
                    if (GetComponent<EventaulFracture>()) GetComponent<EventaulFracture>().CancelFracturing();
                }
                
        }
        else
        {
                if (!GetComponent<Destructible2D.D2dPolygonCollider>())
                {
                    gameObject.AddComponent<Destructible2D.D2dPolygonCollider>();
                    if(!gameObject.GetComponent<Rigidbody2D>()) gameObject.AddComponent<Rigidbody2D>();
                    Destroy(gameObject.GetComponent<Destructible2D.D2dEdgeCollider>());
                    gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
                    gameObject.GetComponent<Rigidbody2D>().mass = spellInfo.mass;
                    gameObject.AddComponent<Destructible2D.D2dQuadFracturer>();
                }

                if (!gameObject.GetComponent<SolidMaterialCollision>())
                    gameObject.AddComponent<SolidMaterialCollision>().Initialize(spellInfo);
                else
                    gameObject.GetComponent<SolidMaterialCollision>().Initialize(spellInfo);
            
        }
        
        Destructible2D.D2dDestructible destr = GetComponent<Destructible2D.D2dDestructible>();
        //set the custom destructible values
        destr.InitializeDestructible(spellInfo.materialType, this, false);
        destr.SetIndestructible(indestructible);
        //set the destructible split properties
        destr.AutoSplit = Destructible2D.D2dDestructible.SplitType.Whole;
        destr.FeatherSplit = false;
        destr.MinSplitPixels = 0;
        if(allowPixelSet) spellInfo.numOfPixels = destr.AlphaCount;  //may not be allowed, especailly by split children since they are in the middle of a pixel calc
    }

    /*public void SetToDesiredPixels(float desiredAlphaCount, bool forceReset = false)
    { //halves until desired resolution is achieved
        Destructible2D.D2dDestructible destr = GetComponent<Destructible2D.D2dDestructible>();
        if (forceReset)
            destr.ResetAlpha();
        while (destr.AlphaCount > desiredAlphaCount)
        {
            destr.HalveAlpha();
            spellInfo.pixelOptimizationLevel++;
        }
        spellInfo.numOfPixels = destr.AlphaCount;
    }*/

    public void SetToDesiredPixels()
    { 
        //It will called AlphaOptimize a certain number of times, aiming to make a 1x1 optimize GV times, larger or smaller will scale a differnt number of times to match
        Destructible2D.D2dDestructible destr = GetComponent<Destructible2D.D2dDestructible>();
        float timesToOptimize = GV.SOLIDMATERIAL_PIXEL_OPTIMIZATION;
        
        //xy>=1 p = g - sqrt(xy/4)  
        //xy<1  p = g + sqrt(xy^-1/4)  
        float xy = transform.localScale.x * transform.localScale.y;
        if (xy >= 1)
        {
            int ixy = (int)xy;
            timesToOptimize = GV.SOLIDMATERIAL_PIXEL_OPTIMIZATION - (int)Mathf.Sqrt(ixy / 4);
        }
        else
        {
            timesToOptimize = GV.SOLIDMATERIAL_PIXEL_OPTIMIZATION + (int)Mathf.Sqrt(Mathf.Pow(xy,-1) / 4);
        }
        //    destr.HalveAlpha();

        if (disableOptimization)
            timesToOptimize = 0;
        while (destr.AlphaCount > GV.SOLIDMATERIAL_MINPIXEL_OPTIMIZATION && timesToOptimize > 0)
        {
            destr.HalveAlpha();
            spellInfo.pixelOptimizationLevel++;
            //if (outputStuff) Debug.Log("currentMass: " + destr.AlphaCount + ", times to opt: " + timesToOptimize);
            timesToOptimize--;
            
        }
        //if (outputStuff) RecalculateCalculateMass();
        //if (outputStuff) Debug.Log("current px: " + destr.AlphaCount + ", bonusScale: " + xy + ", TOTAL MASS: " + currentMass + " at MassPerPixel: " + massPerPixel);
        spellInfo.numOfPixels = destr.AlphaCount;
    }
          
    //returns energy the other spell absorbs, so energy base will have to call this
    public float GetAbsorbed(float _energyOfIncomingSpell, GV.MaterialType materialBeingAbsorbedBy)
    {
        /*
        float damageTaking = _energyOfIncomingSpell * MaterialDict.Instance.GetAbsorbtionResitance(materialType, materialBeingAbsorbedBy);
        damageTaking = (damageTaking > life) ? life : damageTaking;
        life -= damageTaking;
        //stability -= (energyConsuming / (life)) * MaterialDict.Instance.GetDistability(materialType, materialBeingAbsorbedBy) * Time.deltaTime;  no stability for ground.. right now
        return damageTaking;*/
        return 1;
    }

    public float HitBySpell(float dmgInflicted)
    {
        //Debug.Log("ground is hit by energy spell, takes dmg: " + dmgInflicted);
        dmgTaken += dmgInflicted;
        if (dmgTaken > maxHp)
            Fracture();
        //GetAbsorbed
        return 0;
    }

    public void Fracture()
    {
        if (!fadingAway)
        {
            if (materialIsKinematic)                    //if the material is an edge collider, to fracture, must become quad collider
                SetMaterialProperties(false);           //so if kinematic, become moveable
            gameObject.GetComponent<Destructible2D.D2dQuadFracturer>().Fracture();
            if (gameObject.GetComponent<EventaulFracture>()) Destroy(gameObject.GetComponent<EventaulFracture>());
        }
        dmgTaken = 0;
    }

    void DestroySolidMaterial()  //doesnt destroy right away, sets to fade mode
    {
        Destructible2D.D2dDestroyer d2 = gameObject.AddComponent<Destructible2D.D2dDestroyer>();
        d2.Life = 15f;
        d2.FadeDuration = 4f;
        d2.Fade = true;
    }

    bool CalculateHover(float alphaValue)
    {
        float chanceOfHover;
        float floatThreshold = GV.AMOUNT_OF_PIXELS_FOR_90_HOVER;
        if (alphaValue <= floatThreshold)
            chanceOfHover = (alphaValue - (.1f * floatThreshold)) / floatThreshold;
        else
            chanceOfHover = ((alphaValue - floatThreshold) / (2 * floatThreshold)) + .9f;
        return (chanceOfHover > Random.Range(0, 1f));
        //Returns true if the object should hover
    }

    void CheckIfEventaullyFractures() //d20, 5% chance of eventaully fracturing anyways,
    {
        /*if (!GetComponent<EventaulFracture>() && Random.Range(0f, 1f) < GV.GROUND_EVENTAULFRACTURE_CHANCE)
        {
            gameObject.AddComponent<EventaulFracture>().Initialize(GV.GROUND_EVENTAULFRACTURE_TIME, GV.GROUND_EVENTAULFRACTURE_COLOR);
        }*/
    }
    
    //DestructibleInterface Inherited functions
    public float GetEnergy()
    {
        return defenseScore;
    }

    public void AlphaCountTooLow() //when too small, ground will fade away and die
    {
        //Debug.Log("alpha count too low happens");
        if (fadingAway)
            return;
        DestroySolidMaterial();
        fadingAway = true;
    }

    public void PixelCountModified(int newPixelAmt) //Part of DestructibleInterface, if it is altered.
    {
        spellInfo.ModifyPixels(newPixelAmt);
        if (GetComponent<Rigidbody2D>())
            GetComponent<Rigidbody2D>().mass = spellInfo.mass;
        //RecalculateCalculateMass(newPixelAmt, massPerPixel);
    }

    public SpellInfo GetSpellInfo()
    {
        return spellInfo;
    }

    public SpellInfo CloneSpellInfo()
    {
        return spellInfo.Clone();
    }

    public float GetDensityEffect()
    {
        return spellInfo.densityEffect;
    }

    /// MASS CALCULATIONS

    /*void CalculateInitialMass()
    {
        float area = GetComponent<Destructible2D.D2dDestructible>().AlphaCount;
        float bonusScale = transform.localScale.x * transform.localScale.y;
        currentMass = (area / GV.PIXELS_PER_UNITAREA) * MaterialDict.Instance.GetWeightPerUnitArea(materialType) * bonusScale; //absolute temp fix, just trust me
        massPerPixel = currentMass / area;
        if (GetComponent<Rigidbody2D>())
            GetComponent<Rigidbody2D>().mass = currentMass;  //there is one unique circumstance in which this could occur in the future (If the initial soldimass is set to dynamic)
        //if(outputStuff) Debug.Log("current area: " + area + ", bonusScale: " + bonusScale + ", area/gv " + (area / GV.PIXELS_PER_UNITAREA) + ", mat density: " + MaterialDict.Instance.GetWeightPerUnitArea(materialType) + ", TOTAL MASS: " + currentMass + " at MassPerPixel: " + massPerPixel);
    }

    void RecalculateCalculateMass()
    {
        if (GetComponent<Rigidbody2D>())
            GetComponent<Rigidbody2D>().mass = spellInfo.mass;
    }*/

}
