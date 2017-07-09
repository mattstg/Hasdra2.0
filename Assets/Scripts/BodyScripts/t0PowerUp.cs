using UnityEngine;
using System.Collections;

public class t0PowerUp : MonoBehaviour {
    float growRate = .15f;

    public void Initialize()
    {

    }

    public void Update()
    {
        transform.localScale = transform.localScale + (new Vector3(growRate, growRate, 0) * Time.deltaTime);
    }
	
}
