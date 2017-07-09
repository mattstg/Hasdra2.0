using UnityEngine;
using System.Collections;

public class DamageStruct  {
	public string id;
	public float damage;
	public float timeFirstDmg;

	public DamageStruct(string idIn, float dmgIn){
		damage = dmgIn;
		id = idIn;
		timeFirstDmg = Time.deltaTime;
	}

	public void takeDmg(float dmgIn){
		damage += dmgIn;
	}
}
