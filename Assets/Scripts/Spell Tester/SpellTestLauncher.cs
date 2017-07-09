using UnityEngine;
using System.Collections;

public class SpellTestLauncher : MonoBehaviour {

    public float moveSpeed = 2;
	// Update is called once per frame
	void Update () {
        Vector3 curPos = gameObject.transform.position;

        if (Input.GetKey(KeyCode.W))
            curPos.y += moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            curPos.y -= moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            curPos.x -= moveSpeed * Time.deltaTime;        
        if (Input.GetKey(KeyCode.D))
            curPos.x += moveSpeed * Time.deltaTime;

        gameObject.transform.position = curPos;
    }
}
