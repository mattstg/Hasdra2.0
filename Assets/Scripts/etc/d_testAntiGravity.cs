using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class d_testAntiGravity : MonoBehaviour {

    public Rigidbody2D rb2;
	
	// Update is called once per frame
	void Update () {
        Vector2 forceToCancelGravity = (Physics2D.gravity * rb2.mass) * -1 * Time.deltaTime;
        rb2.AddRelativeForce(forceToCancelGravity, ForceMode2D.Impulse);
        rb2.mass += Time.deltaTime * 5;
    }
}
