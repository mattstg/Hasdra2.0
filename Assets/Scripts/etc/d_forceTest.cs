using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class d_forceTest : MonoBehaviour {

    public float startSpeed = 5;
    float counter = 2;
    Rigidbody2D rb;
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        float forceToApply = rb.mass * startSpeed;
        rb.AddForce(new Vector2(1, 0) * forceToApply,ForceMode2D.Impulse);
	}
	
	// Update is called once per frame
	void Update () {
        counter -= Time.deltaTime;
        Debug.Log("current speed: " + rb.velocity);
        if (counter <= 0)
        {
            counter = 10;
            float currentEstimatedForce = rb.mass * rb.velocity.magnitude;
            Debug.Log("applying force: " + currentEstimatedForce);
            rb.AddForce(currentEstimatedForce * new Vector2(-1, 0), ForceMode2D.Impulse);
        }
	}
}
