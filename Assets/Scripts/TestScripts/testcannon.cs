using UnityEngine;
using System.Collections;

public class testcannon : MonoBehaviour {

    public GameObject ammunition;
    public float ammoMass = 1f;
    public float ammoForce = 1f;

    public void FireBullet()
    {
        GameObject toFire = Instantiate(ammunition, this.transform.position, Quaternion.identity) as GameObject;
        toFire.GetComponent<Rigidbody2D>().mass = ammoMass;
        toFire.GetComponent<testAmmo>().updateInfo = true;
        toFire.GetComponent<Rigidbody2D>().AddForce(new Vector2(ammoForce, 0), ForceMode2D.Impulse);
    }
}
