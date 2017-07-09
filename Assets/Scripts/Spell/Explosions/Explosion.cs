using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Explosion : MonoBehaviour {

    Destructible2D.D2dExplosion d2dExplosion;
    
    private float initialEnergy;
    protected float energy;
    public GV.MaterialType materialType;
    float explosionDirection; //only used when ExplosionType == Vector
    float countdown;
    float startCountdown = 9f;  //The time it takes to dissapate the explosion, could be variable or GV
    bool interactedWithEnviro = false;
    //float percentEnergyLossPerSec = GV.explo; //could be variable as well
    float explosionDissipationGrowth = .3f; //amount the explosion grows or shrinks after the total animation
    bool firstUpdate = true; //Resolve collision happens after the first update, so drain happens afterwards
    float radioFreq;
    List<SkillModifier> storedSkillModifiers;
    protected float density = GV.EXPLOSION_DENSITY; 
	private SpellInfo damageTagingSI; //used for damage mitigation function to regulate damage (in PCS.TakeDamage( as arg))

    //for self casts and punches
    bool forceEffectsCaster     = true;
    bool skillModsEffectsCaster = true;
    bool damageEffectsCaster    = true;
    bool casterSpecailTreatment = false;
    GameObject caster           = null;

    //exp directional params
    GV.DirectionalDamage explosionType = GV.DirectionalDamage.explosion;
    bool useDefaultForce = true;
    float cappedExplosiveForce = 0;
    float dmgDirTrueAng = 0;

    /// <summary>creates an explosion, do not have to give it </summary>
    public virtual void InitializeExplosion(SpellInfo si,  float _energy, List<SkillModifier> _storedSkillModifiers)
    {

		damageTagingSI = new SpellInfo(si); //theses two are for hack for damage taken. To see which spell exploded exactly.
		damageTagingSI.currentEnergy = _energy;
		damageTagingSI.uniqueSpellID += "_EXPLO";

        //Debug.Log("energy in explosion: " + _energy);
        //here could also summon the apprioate particle affect
        radioFreq = si.radioFreq;
        d2dExplosion = GetComponent<Destructible2D.D2dExplosion>();
        explosionType = si.directionalDamageType;
        useDefaultForce = si.useDefaultForce;
        cappedExplosiveForce = si.cappedExplosiveForce;
        dmgDirTrueAng = si.dmgDirTrueAng;
        initialEnergy = energy = _energy;
        startCountdown = MaterialDict.Instance.GetExplosionTime(si.materialType);
        countdown = startCountdown = GV.EXPLOSION_DISSIPATE_VISUAL_TIME_KM.x + GV.EXPLOSION_DISSIPATE_VISUAL_TIME_KM.y * energy; ;
        explosionDirection = (explosionType == GV.DirectionalDamage.specifiedDir) ? dmgDirTrueAng : si.currentAngle;
        materialType = si.materialType;
        storedSkillModifiers = _storedSkillModifiers;
        string explosionSprite = "Textures/Spells/Explosions/" + si.materialType.ToString() + "Explosion";
        try
        {
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(explosionSprite);
        }
        catch
        {
            Debug.Log("Explosion texture not found at : " + explosionSprite);
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/Spells/Explosions/EnergyExplosion");
        }
        ScaleSize();
        //Debug.Log("explosion mat type: " + _materialType + " at e: " + _energy + " returned stamp power: " + MaterialDict.Instance.GetStampValue(_materialType, true, _energy));
        GetComponent<Destructible2D.D2dExplosion>().StampHardness = MaterialDict.Instance.GetStampValue(si.materialType, true, _energy, density); // *density;
        //add the numeric display
        if(!GV.ND_ON)
            StaticReferences.numericTextManager.CreateDisposableNumericDisplay(Color.red,"",energy, this.transform);

        casterSpecailTreatment = (si.castType == GV.CastType.SelfCast);
        if (casterSpecailTreatment)
        {
            caster = si.GetNCastersBack(1).gameObject;
            forceEffectsCaster = si.sc_selfApplyKnockback;
            skillModsEffectsCaster = si.sc_selfApplySkillMod;
            damageEffectsCaster = si.sc_selfApplySpell;
            density *= GV.SELFCAST_EXPLOSION_DENSITY_DEFAULT;
        }
        
    }

    public void ScaleSize()
    {
        float scaleSize = initialEnergy + initialEnergy * explosionDissipationGrowth * (1 - (countdown / startCountdown));
        transform.localScale = GV.CircleScale(scaleSize, density, materialType);
        d2dExplosion.StampSize.x = transform.localScale.x;
        d2dExplosion.StampSize.y = transform.localScale.y;
    }

    public float GetAbsorbed(float totalEnergyOfConsumer, GV.MaterialType materialDevouring)
    {  //get absorbed by other spells
     float energyLosing = totalEnergyOfConsumer * Time.deltaTime * MaterialDict.Instance.GetAbsorbtionResitance(materialType, materialDevouring);
     energyLosing = (energyLosing > energy) ? energy : energyLosing;
     energy -= energyLosing;
     return energyLosing;
    }

    public void Update()
    {
		damageTagingSI.currentEnergy = energy;
        if (firstUpdate && countdown >= 0 && energy >= 0)
        {
            firstUpdate = false;
            return;
        }

        if (countdown > 0 && energy > 0)
        {



            /*
            float energyPercentLoss = GV.EXPLOSION_DISSIPATE_ENERGY_KM.x + GV.EXPLOSION_DISSIPATE_ENERGY_KM.y * energy;
            energyPercentLoss = (energyPercentLoss<= GV.EXPLOSION_DISSIPATE_MIN_RATE)?GV.EXPLOSION_DISSIPATE_MIN_RATE:energyPercentLoss;
            float energyLoss = Time.deltaTime *energyPercentLoss*initialEnergy;
            energy -= energyLoss;
            energy = (energy<=0)?0:energy;
            countdown -= Time.deltaTime;
            GetComponent<SpriteRenderer>().material.color = new Color(1, 1, 1, countdown/startCountdown); */
             ScaleSize(); 
        }
        else
        {
            Die();
        }
    }

    public void Die()
    {
        EnviromentConsquence();  //I think it already does that with resolve coli
        GV.Destroyer(gameObject);
        //then get deleted or something
    }

    public void OnCollisionStay2D(Collision2D coli)
    {
        ResolveCollision(coli.gameObject, coli);
    }
    public void OnTriggerStay2D(Collider2D coli)
    {
        ResolveCollision(coli.gameObject);
    }

    public void ResolveCollision(GameObject otherObj, Collision2D coli = null)
    {
        Vector2 damageDirection = GV.GetDamageDirection(transform.position, explosionType, explosionDirection, otherObj.transform.position);
        if (otherObj.CompareTag("Player"))
        {
            bool applySpecialTreament = (casterSpecailTreatment && caster == otherObj);
            PlayerControlScript pcs = otherObj.GetComponent<PlayerControlScript>();
			if (!applySpecialTreament || damageEffectsCaster) {
				pcs.TakeDamage (energy * Time.fixedDeltaTime, materialType);
				//Debug.Log ("hereexplo");
			}
            if (!applySpecialTreament || forceEffectsCaster)
            {
                float energyUsing = energy;
                //if (!useDefaultForce)
                //    energyUsing = Mathf.Min(energy, cappedExplosiveForce / MaterialDict.Instance.GetKnockback(materialType));
                Debug.Log(string.Format("energy in spell {0}, but acting like {1} for knockback", energy, cappedExplosiveForce));
                pcs.KnockBackFromDamage(energyUsing * Time.deltaTime, damageDirection, materialType, true);
            }
            if (!applySpecialTreament || skillModsEffectsCaster)
                foreach (SkillModifier skillmod in storedSkillModifiers)
                    pcs.AddSkillModifier(skillmod);
        }
        else if (!interactedWithEnviro && otherObj.CompareTag("Enviroment"))
        {
            otherObj.GetComponent<Biome>().TakeExplosionDamage(materialType, energy); //dont multiple by delta time, assume it as if the explosion hits the enviro with full force to save on overtime calcs
            interactedWithEnviro = true;  //this means it will only hit one enviroment even if covers multiple
        }
        else if (otherObj.CompareTag("Spell"))
        {
            Spell otherSpell = otherObj.GetComponent<Spell>();
            otherSpell = (otherSpell != null) ? otherSpell : otherObj.transform.parent.GetComponent<Spell>();
            bool applySpecialTreament = (casterSpecailTreatment && caster == otherObj);
            SpellInfo otherSpellInfo = otherSpell.spellInfo;
            if (!applySpecialTreament || damageEffectsCaster)
                otherSpell.Absorb(GetAbsorbed(otherSpellInfo.currentEnergy, otherSpellInfo.materialType), materialType, 0, otherSpell.CoresTouching(transform), GV.BasicColiType.Explosion,true);
            if (!applySpecialTreament || forceEffectsCaster)
            {
                Vector2 dmgDir = GV.GetDamageDirection(transform.position, explosionType, explosionDirection, otherSpell.transform.position);
                //Debug.Log(string.Format("{0} explosion pushing spell @ dmgDir({1}) with {2} energy", materialType, dmgDir, energy * Time.deltaTime * density));
                otherSpell.TakeKnockBackDamage(dmgDir, energy * Time.deltaTime * density, materialType);
            }
            otherSpellInfo.lastBasicColiType = GV.BasicColiType.Explosion;
            otherSpellInfo.lastMaterialColiType = materialType;
            if (radioFreq != -1)
                otherSpell.spellInfo.lastRadioFreqRecieved = radioFreq;
        }
        else if (otherObj.CompareTag("Terrain") && otherObj.GetComponent<SolidMaterial>())
        {
            //go.GetComponent<SolidMaterial>().HitBySpell(
        }
    }

    public virtual void CreateParticleEffect()
    {
        //so for each inherited type, specify which one to load, and the apprioate power if any is applied
    }

    public virtual void CreateAfterExplosion()
    {
        //things like shrapnel
    }

    public void EnviromentConsquence()
    {
        //Affect enviroment with DamageType,Energy @ vector
    }

}

