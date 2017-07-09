using UnityEngine;
using System.Collections;
using System.Linq;

public class SpellDestabilization : MonoBehaviour {
    bool animationActivated = false;
	Rigidbody2D rbod;
	Spell spell;
	SpellInfo spellInfo { get {return spell.spellInfo;}}
	bool hasBeenFired = false;
    
    bool isMelee = false;
    float destabPerVeloMelee;

    /*public void Start()
    {
        rbod = GetComponent<Rigidbody2D>();
		spell = GetComponent<Spell> ();
        spellInfo.stability= 1;
    }*/

    public void Initialize(Rigidbody2D _rbod, Spell spellParent)
    {
        rbod = _rbod;
        spell = spellParent;
        spellInfo.stability = 1;
    }

    private float ChargeModifier()
    { //when mod stability, multiplies by this return value for collisions that are not entities (since those are 1 or 0)
        if (spell.spellInfo.spellState == GV.SpellState.Charging)
            return .5f;
        return 1;
    }

    /// <summary>
    /// returns true if explodes
    /// </summary>
    /// <returns></returns>
    public bool StabilizeUpkeep()
	{
        if (hasBeenFired)
        {
            if (isMelee)
                MeleeDecay();

            if (spellInfo.stability <= GV.STABILITY_DECAY_THRESHOLD)
                spellInfo.stability -= Random.Range(0, spell.bodyStats.getSkillValue("stabilityDecayRate") * Time.deltaTime);
            else if (spellInfo.stability >= GV.STABILITY_RECOVER_THRESHOLD)
                spellInfo.stability += Random.Range(0, spell.bodyStats.getSkillValue("stabilityRecoveryRate") * Time.deltaTime);

            StaticReferences.numericTextManager.CreateNumericDisplay(this, transform, "SpellStability", "", spellInfo.stability, Color.blue, true);
        }
        if (spellInfo.stability > 1)
            spellInfo.stability = 1;
        else if (spellInfo.stability <= 0 || spellInfo.stability != spellInfo.stability) //check for NaN
            return true; //explode

		return false;
    }

	public void MeleeDecay(){
        float decayToAdd = rbod.velocity.magnitude * Time.deltaTime * destabPerVeloMelee;
		spellInfo.stability -= decayToAdd;
	}

    //Absorbing, getting absorbed or taking dmg all utilize this, This formula seems a tad odd.
    public void Absorb(float energyBeingAbsorbed, GV.MaterialType matBeingAbsorbed, float otherSpeed, bool coresTouching, GV.BasicColiType colType)
    {
        //Get despellInfo.stabilityfrom absorbing, or from being absorbed, the faster your moving, the more destable youll get
        // (absorbed/devourer)*matDict*velo
        //if(GV.GetSpellFormByMaterialType(matBeingAbsorbed)== Spell.SpellForms.Physical)
        float percentIncrease = 1 + (rbod.velocity.magnitude+otherSpeed)*GV.DESTABILITY_PER_VELO;
        float percentIncreaseFromColiType = 1;
        if(colType == GV.BasicColiType.SolidMaterial)
        {
            percentIncreaseFromColiType = GV.DESTABILITY_TERRAIN_MODIFIER + GV.DESTABILITY_TERRAIN_PERVELO_MODIFIER * otherSpeed;
        }
        else if (colType == GV.BasicColiType.Explosion)
        {
            percentIncreaseFromColiType = GV.DESTABILITY_TERRAIN_MODIFIER;
        }
        float energyRatio = energyBeingAbsorbed / spellInfo.currentEnergy;
        float totalToSub = (energyRatio * MaterialDict.Instance.GetDistability(spellInfo.materialType, matBeingAbsorbed) * percentIncrease * percentIncreaseFromColiType);
        //Debug.Log(string.Format("Spell {0}, Total sub {1}, from energy ratio({2})*matDictDist({3})*percIncrease({4})*percIncreaseFromcoliType({5})", spellInfo.materialType, totalToSub, energyRatio, MaterialDict.Instance.GetDistability(spellInfo.materialType, matBeingAbsorbed),percentIncrease,percentIncreaseFromColiType));

        totalToSub *= (coresTouching)?GV.CORES_TOUCHING_DESTABLE_MULT:1;
        //Debug.Log("ratio: " + energyRatio + ", DestabilityMultiplier(MD): " + matDictValue + ", Velo Multiplier: " + percentIncrease + " Total: " + (energyRatio*matDictValue));
       // Debug.Log("current stab - total: " + spellInfo.stability+ " - " + totalToSub + " = " + (spellInfo.stability- totalToSub));
        spellInfo.stability -= totalToSub * ChargeModifier();
        //Debug.Log("spellInfo.stabilityloss: " + totalToSub + ", cores touch: " + coresTouching + ", percent increase from velo: " + percentIncrease);
        //setStability(spellInfo.stability- ((otherEnergy / spellEnergy) * MaterialDict.Instance.GetDistability(spellMatType, otherMatType)*percentIncrease));
   }

    //Takes from the current spell, change if needed
    public void EnergyVsPhysicalDestablize(float stampPower, float stamperDefense, bool coresTouching, float stamperSpeed)
    {
        float destab = (stamperDefense / stampPower) * Time.deltaTime * GV.DESTAB_MULTIPLIER * stamperSpeed;
        //Debug.Log(string.Format("stamperdef/stamppower = {0},  from {1}/{2}", destab, stamperDefense, stampPower));
        if (coresTouching)
            destab = 1; //core touching causes instant destab

        spellInfo.stability -= destab * ChargeModifier();
        //Debug.Log("current stability: " + spellInfo.stability);
    }

    public void CollideWithEntity()
    {
        spellInfo.stability -= MaterialDict.Instance.GetDestabilityVsEntity(spellInfo.materialType);  //ethier 1 or 0 to force explosion or not
    }

    public void TakeDamage(float energyBeingAbsorbed, GV.MaterialType matBeingAbsorbed)
    { //Destab from takeDamage 


        //float percentIncrease = 1 + rbod.velocity.magnitude * GV.DESTABILITY_PER_VELO; //my speed doesnt matter thats taken into account already
        float energyRatio = energyBeingAbsorbed / spellInfo.currentEnergy;
        float totalToSub = (energyRatio * MaterialDict.Instance.GetDistability(spellInfo.materialType, matBeingAbsorbed));
        spellInfo.stability -= totalToSub;
        //Debug.Log(string.Format("Stability {0} after losing {1}", spellInfo.stability, totalToSub));
    }

	public void spellFired(){
		hasBeenFired = true;
        if (spellInfo.isMelee)
        {
            //float finalRange = spellInfo.melee_maxRange * Mathf.Min((spellInfo.currentEnergy / spellInfo.melee_maxRange_energy),1);
            destabPerVeloMelee = (1 / spellInfo.melee_maxRange);
            isMelee = true;
        }
	}

    /*public void GetAbsorbed(float energyDevouring, GV.MaterialType materialDevouring, float energyBeingAbsorbed, GV.MaterialType matBeingAbsorbed)
    {
        //Get despellInfo.stabilityfrom absorbing, or from being absorbed, the faster your moving, the more destable youll get
        // (absorbed/devourer)*matDict*velo
        float percentIncrease = 1 + rbod.velocity.magnitude * GV.DESTABILITY_PER_VELO;
        float energyRatio = energyDevouring / energyBeingAbsorbed;
        float matDictValue = MaterialDict.Instance.GetDistability(materialDevouring, matBeingAbsorbed);
        float totalToSub = (energyRatio * matDictValue * percentIncrease);
        Debug.Log("ratio: " + energyRatio + ", matdictvalue: " + matDictValue + ", percentIncVelo: " + percentIncrease + " ratio*mactDict: " + (energyRatio * matDictValue));
        Debug.Log("current stab - total: " + spellInfo.stability+ " - " + totalToSub + " = " + (spellInfo.stability- totalToSub));
        setStability(spellInfo.stability- totalToSub);
        //setStability(spellInfo.stability- ((otherEnergy / spellEnergy) * MaterialDict.Instance.GetDistability(spellMatType, otherMatType)*percentIncrease));
    }*/

    

}
