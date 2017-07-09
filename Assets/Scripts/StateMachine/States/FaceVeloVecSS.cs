using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class FaceVeloVecSS : StateSlot {

	public override void Initialize(State _parentState)
	{
		base.Initialize(_parentState);        
	}
		
	public override void PerformStateAction(Spell spell)
	{
		Vector2 currentVelocity = spell.GetComponent<Rigidbody2D> ().velocity;
		float desiredAngle = GV.GetAngleOfLineBetweenTwoPoints (Vector2.zero, currentVelocity);
		//float aglePerSec = GV.SPELL_ROTATION_RADS_PER_SEC * Time.deltaTime;
		//float actualAngle = 0f;
		if(currentVelocity.magnitude > 0.2f)
		spell.transform.rotation = Quaternion.Euler(0,0,desiredAngle);

	}
}
