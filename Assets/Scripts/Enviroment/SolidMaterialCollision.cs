using UnityEngine;
using System.Collections;

public class SolidMaterialCollision : MonoBehaviour {

    //Solid material will be too intensive if it is always calculating collisions (since ground touches ground alot). However interactions between itself (falling hard enough to create hairlines) or hitting players
    //is also nesscary. (Players hitting ground too hard can be checked with sudden deaccel change and collider)
    //This script is added when a solidmaterial is dynamic, and removed when it becomes kinematic again
    //in here i can apply the same logic,
    
    SpellInfo spellInfo;

    public void Initialize(SpellInfo si)
    {
        spellInfo = si;
    }

    public void OnTriggerStay2D(Collider2D coli)
    {
        ResolveCollision(coli);
    }

    public void OnCollisionEnter2D(Collision2D coli)  //if this is "onStay", piles of fractures will resolve like crazy, but at same time.. the player... hmm..
    {
        ResolveCollision(coli.collider);
    }

    private void ResolveCollision(Collider2D coli)
    {
		
        PlayerControlScript pcs = coli.GetComponent<PlayerControlScript>();
        SolidMaterial sm = coli.GetComponent<SolidMaterial>();
        if (pcs)
        {
			if ((pcs.GetComponent<Rigidbody2D>().velocity - GetComponent<Rigidbody2D>().velocity).magnitude < 1) //if the ground is not moving, no dmg (stops dmg from walking ontop of ground or resting on player)
                return; // actually should be difference in their velocities! Wrong matt haha!  -  (ง •̀_•́)ง You win this round
			if (GetComponent<Rigidbody2D> ().velocity.magnitude < 1)
				return;
            //Debug.Log ("mass of spell " + spellInfo.mass + "   relative velo " + (pcs.GetComponent<Rigidbody2D>().velocity - GetComponent<Rigidbody2D>().velocity).magnitude.ToString());

            //float dmg = spellInfo.mass * (pcs.GetComponent<Rigidbody2D>().velocity - GetComponent<Rigidbody2D>().velocity).magnitude * Time.deltaTime * GV.SOLIDMATERIAL_PHYSICAL_MOMENTUEM_DAMAGE_MULTIPLIER;  //copied from Spell, more and more the similarities.. to be expected, but a final solution is needed
            float dmg = DealDamage(pcs.GetComponent<Rigidbody2D>().velocity);
            float dmgdealt = pcs.TakeDamage(dmg, spellInfo.materialType);
			//Debug.Log ("dmg = " + dmg);
            //Debug.Log("dmgdealt: " + dmgdealt);
            //Debug.Log("dmg sent to player from SMC: " + dmg + ", dmg taken by player: " + dmgdealt);
            //Debug.Log("mass: " + spellInfo.mass + ",dmg b4 dt: " + spellInfo.mass * (pcs.GetComponent<Rigidbody2D>().velocity - GetComponent<Rigidbody2D>().velocity).magnitude * GV.SPELLFORM_PHYSICAL_MOMENTUEM_DAMAGE_MULTIPLIER);
        }
        else if (sm)
        {
            Debug.Log("another terrain coli");
        }
    }

    private float DealDamage(Vector2 otherVelo)
    {
        Vector2 relativeVelo = otherVelo - GetComponent<Rigidbody2D>().velocity;
        if (Mathf.Abs(relativeVelo.x) < .2f && Mathf.Abs(relativeVelo.y) < .2f)  //see! no sqrt!
            return 0; //too little dmg

        float speed = relativeVelo.magnitude;
        float Ek = .5f * spellInfo.mass * speed * speed;
        return Ek * Time.deltaTime * spellInfo.densityEffect;
    }
	
}
