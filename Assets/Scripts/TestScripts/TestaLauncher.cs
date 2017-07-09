using UnityEngine;
using System.Collections;

public class TestaLauncher : MonoBehaviour {
    public string spellName = "a";
    public float initialEnergy = 100f;
    public float direction = 0;
    public float velocity = 5;
    public SpellInitializer a;
	// Use this for initialization
	public void LaunchSpell () {
        /*a.InitializeSpell(GV.GetLiveSpell(spellName));
        a.GetComponent<Spell>().spellInfo.initialEnergy = initialEnergy;
        a.GetComponent<Spell>().spellInfo.initialHeadingAngle = direction;
        a.GetComponent<Spell>().spellInfo.velocity = velocity;
        a.GetComponent<Spell>().ToggleStateMachine(true);*/
	}
}
