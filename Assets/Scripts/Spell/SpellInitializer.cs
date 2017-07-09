using UnityEngine;
using System.Collections;

public class SpellInitializer : MonoBehaviour {

    public Transform rangedReticle;
    public Transform centerReticle;

    public void InitializeSpell(SpellStorage toClone, SpellBridge parentCaster)
    {
        Spell newSpell = null;
        //this will be like a giant switch case
        switch(toClone.materialType)//MaterialType
        {
            case GV.MaterialType.Energy:
                newSpell = this.gameObject.AddComponent<EnergySpell>();
                break;
            case GV.MaterialType.Rock:
                newSpell = this.gameObject.AddComponent<RockSpell>();
                break;
            case GV.MaterialType.Fire:
                newSpell = this.gameObject.AddComponent<FireSpell>();
                break;
            case GV.MaterialType.Charisma:
                newSpell = this.gameObject.AddComponent<CharismaSpell>();
                break;
            case GV.MaterialType.Force:
                newSpell = this.gameObject.AddComponent<ForceSpell>();
                break;
            case GV.MaterialType.Higgs:
                newSpell = this.gameObject.AddComponent<HiggsSpell>();
                break;
            case GV.MaterialType.Ice:
                newSpell = this.gameObject.AddComponent<IceSpell>();
                break;
            case GV.MaterialType.Lighting:
                newSpell = this.gameObject.AddComponent<LightingSpell>();
                break;
            case GV.MaterialType.Mist:
                newSpell = this.gameObject.AddComponent<MistSpell>();
                break;
            case GV.MaterialType.Nature:
                newSpell = this.gameObject.AddComponent<NatureSpell>();
                break;
            case GV.MaterialType.Oil:
                newSpell = this.gameObject.AddComponent<OilSpell>();
                break;
            case GV.MaterialType.Radio:
                newSpell = this.gameObject.AddComponent<RadioSpell>();
                break;
            case GV.MaterialType.Smoke:
                newSpell = this.gameObject.AddComponent<SmokeSpell>();
                break;
            case GV.MaterialType.Water:
                newSpell = this.gameObject.AddComponent<WaterSpell>();
                break;
            default:
                newSpell = this.gameObject.AddComponent<EnergySpell>(); //defaults to energy
                Debug.LogError("Unrecgonized spell type: " + toClone.materialType.ToString());
                break;
        }
        //!!!!!!!!this below will overrite the shape anyways!!!!!!!!!
        //RuntimeAnimatorController spellLiveAnimations = Resources.Load<RuntimeAnimatorController>("Textures/Spells/SpellAnimRuntimeControllers/" + toClone.materialType.ToString() + "AnimCntrl");
       /* RuntimeAnimatorController spellLiveAnimations = Resources.Load<RuntimeAnimatorController>("Textures/Spells/PixelMode/" + toClone.spellInfo.spellShape + "/" +  toClone.materialType.ToString() + "AnimCntrl");
        if (spellLiveAnimations) //if one exist, there is a live sprite version of the spell, instead of a static sprite
        {
           newSpell.gameObject.AddComponent<Animator>().runtimeAnimatorController = spellLiveAnimations;
        }*/
        newSpell.spellInfo = new SpellInfo(toClone.spellInfo);
        newSpell.spellInfo.materialType = toClone.materialType;
        newSpell.spellStateMachine = toClone.stateMachine;
        newSpell.spellInfo.currentState = toClone.startState;
        SetupChargingSkillMods(newSpell, toClone);
        //newSpell.InitializeSpellCore(toClone.spellInfo.shape);
        //GRAVITY getGravity materailDict, fix float first
        newSpell.GetComponent<Rigidbody2D>().gravityScale = newSpell.spellInfo.gravityScale = MaterialDict.Instance.GetGravity(newSpell.spellInfo.materialType);
        SetupSpriteAndMass(newSpell);
        //newSpell.SetSizeMassSpriteShape();
        Destroy(this);//was here
        
        if (GV.GetSpellFormByMaterialType(newSpell.spellInfo.materialType) == GV.SpellForms.Physical)
        {
            gameObject.AddComponent<Destructible2D.D2dPolygonCollider>();
            Destructible2D.D2dDestructible destr = GetComponent<Destructible2D.D2dDestructible>();
            destr.InitializeDestructible(newSpell.spellInfo.materialType, newSpell, true);
            destr.AutoSplit = Destructible2D.D2dDestructible.SplitType.Whole;
            destr.FeatherSplit = false;
            destr.MinSplitPixels = 0;
            if(!newSpell.spellInfo.colorAltered)
                newSpell.spellInfo.spellColor = MaterialDict.Instance.GetMaterialSpriteStyle(newSpell.spellInfo.materialType).color;
            destr.Color = newSpell.spellInfo.spellColor;
            /*for (int i = 0; i < GV.SPELL_PIXEL_OPTIMZATION; i++)
            {
                destr.HalveAlpha();
                newSpell.spellInfo.pixelOptimizationLevel++;
            }*/
            newSpell.spellInfo.numOfPixels = destr.AlphaCount;
        }
        else //energy based
        {
            GameObject spellCoreGo = Instantiate(Resources.Load("Prefabs/Spell/SpellCore")) as GameObject;
            spellCoreGo.transform.SetParent(gameObject.transform);
            spellCoreGo.transform.localPosition = new Vector3(0, 0, 0);
            spellCoreGo.transform.localScale = new Vector3(1, 1, 1);
            newSpell.spellCore = spellCoreGo.GetComponent<SpellCore>();
            newSpell.spellCore.Initialize(newSpell.spellInfo);

            Collider2D newCollider;
            switch(newSpell.spellInfo.spellShape)
            {
                case GV.SpellShape.Circle:
                    if(newSpell.spellInfo.setScale.x != newSpell.spellInfo.setScale.y)
                    {
                        CapsuleCollider2D capsule1 = gameObject.AddComponent<CapsuleCollider2D>();
                        capsule1.direction = (newSpell.spellInfo.setScale.y > newSpell.spellInfo.setScale.x) ? CapsuleDirection2D.Vertical : CapsuleDirection2D.Horizontal;
                        newCollider = capsule1;
                    }
                    else
                        newCollider = gameObject.AddComponent<CircleCollider2D>();
                    break;
                case GV.SpellShape.Square:
                case GV.SpellShape.Dragon:
                    newCollider = gameObject.AddComponent<BoxCollider2D>();
                    break;
                case GV.SpellShape.Trapezoid:
                    newCollider = gameObject.AddComponent<PolygonCollider2D>();
                    break;
                case GV.SpellShape.Crescent:
                    CapsuleCollider2D capsule = gameObject.AddComponent<CapsuleCollider2D>();
                    newCollider = capsule;
                    capsule.direction = (newSpell.spellInfo.setScale.y > newSpell.spellInfo.setScale.x) ? CapsuleDirection2D.Vertical : CapsuleDirection2D.Horizontal;
                    capsule.offset = new Vector2(GV.SPRITE_SIZE / 4, 0); //cuz of sprite size
                    capsule.size = new Vector2(GV.SPRITE_SIZE/2, GV.SPRITE_SIZE); //cuz of sprite size
                    break;
                default:
                    Debug.Log("switch unhandled for " + newSpell.spellInfo.spellShape);
                    newCollider = gameObject.AddComponent<PolygonCollider2D>();
                    break;
            }
            newCollider.isTrigger = true;
        }

        if (newSpell.spellInfo.isSelfCast)
            newSpell.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "SelfCastSpell";
        newSpell.centerReticle = centerReticle;
        newSpell.rangedReticle = rangedReticle;
        newSpell.Initialize(parentCaster);
    }


    private void SetupChargingSkillMods(Spell newSpell, SpellStorage spellStorage)
    {
        newSpell.spellInfo.onChargeSkillMods = new System.Collections.Generic.List<SkillModifier>();
        //Debug.LogError("skill mod storage: " + spellStorage.onChargeSkillMods.Count);
        foreach (SkillModSS ss in spellStorage.onChargeSkillMods)
            newSpell.spellInfo.onChargeSkillMods.Add(new SkillModifier(ss, newSpell.GetInstanceID().ToString()));
    }

    private void SetupSpriteAndMass(Spell toSet)
    {
        MaterialDict.MaterialSpriteStyle matSpriteStyle = MaterialDict.Instance.GetMaterialSpriteStyle(toSet.spellInfo.materialType);
        if (toSet.spellInfo.spellForm == GV.SpellForms.Energy)
        {
            bool sucessful = LoadAnimatedSprite(toSet, matSpriteStyle);
            if (!sucessful)
                LoadStaticSprite(toSet, matSpriteStyle);
        }
        else
        {
            LoadStaticSprite(toSet, matSpriteStyle);
        }

        if (!toSet.spellInfo.colorAltered)
            toSet.spellInfo.spellColor = matSpriteStyle.color;
        toSet.gameObject.GetComponent<SpriteRenderer>().color = toSet.spellInfo.spellColor;
        toSet.SetSizeAndMass();
    }

    private bool LoadAnimatedSprite(Spell toSet, MaterialDict.MaterialSpriteStyle matSpriteStyle)
    {
        string animatorLocation = "Textures/Spells/Animators/" + matSpriteStyle.spriteStyle.ToString() + toSet.spellInfo.spellShape + "_0";
        RuntimeAnimatorController spellLiveAnimations = Resources.Load<RuntimeAnimatorController>(animatorLocation);
        toSet.gameObject.AddComponent<Animator>().runtimeAnimatorController = spellLiveAnimations;
        Texture2D t2d = Resources.Load<Texture2D>("Textures/Spells/Stamps/" + toSet.spellInfo.spellShape + "Stamp");
        if (t2d == null)
            return false;
        toSet.GetComponent<Destructible2D.D2dRepeatStamp>().StampTex = Resources.Load<Texture2D>("Textures/Spells/Stamps/" + toSet.spellInfo.spellShape + "Stamp");
        return true;
    }

    private void LoadStaticSprite(Spell toSet, MaterialDict.MaterialSpriteStyle matSpriteStyle)
    {
        string spriteLoc = "Textures/Spells/Static/Static" + matSpriteStyle.spriteStyle.ToString() + toSet.spellInfo.spellShape;
        Sprite s = Resources.Load<Sprite>(spriteLoc);
        if(s == null) //if sprite fails, probably because of sprite style (gas, etc), load a solid one instead
        {
            s = Resources.Load<Sprite>("Textures/Spells/Static/StaticSolid" + toSet.spellInfo.spellShape);
        }
        toSet.GetComponent<SpriteRenderer>().sprite = s;
        Destroy(toSet.GetComponent<Destructible2D.D2dRepeatStamp>());
    }
}
