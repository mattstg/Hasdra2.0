using UnityEngine;
using System.Collections;

public class WiggleWiggle : MonoBehaviour {

    public float jumpTimer = 3.8f;
    public float layerCounter = 10f;
    bool frontLayer = true;

	void Update () {
        jumpTimer -= Time.deltaTime;
        if (jumpTimer < 0)
        {
            this.GetComponent<Rigidbody2D>().AddForce(Random.Range(.2f, .4f) * new Vector2(Random.Range(-1,1), 1),ForceMode2D.Impulse);
            jumpTimer = 3.8f;
        }
       /* layerCounter -= Time.deltaTime;
        if (layerCounter <= 0)
        {
            layerCounter = 10;
            GetComponent<SpriteRenderer>().sortingOrder = (frontLayer)?4:2;
            frontLayer = !frontLayer;
        }*/
	}
}
