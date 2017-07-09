using UnityEngine;
using System.Collections;

public class AnimatorEventManager : MonoBehaviour {
	private PlayerControlScript pc;
	//private string spellName;

	// Use this for initialization
	void Start () {
		pc = GetComponentInParent<PlayerControlScript> ();
		//spellName = "";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Fire(){
		pc.AnimatorFiresSpell();
		//spellName = "";
	}

	/*
	public void SpellUnlock(){
		pc.unlockFromAnim ();
		//spellName = "";
	}*/

	public void EndOfFireAnim(){
		pc.unlockFromAnim ();
	}

	public void KickEnd(){
		Debug.Log ("Do something with me! FROM KICK END");
	}

	//public void setSpellName(string input){
	//	spellName = input;
	//}

	//public string getSpellName(){
	//	return spellName;
	//}

}
