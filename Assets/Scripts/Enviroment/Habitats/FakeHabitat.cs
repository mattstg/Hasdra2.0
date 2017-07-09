using UnityEngine;
using System.Collections;

public class FakeHabitat : MonoBehaviour {

    float countdown = 10f;
    bool createOne = true;
	// Update is called once per frame
	void Update () {
        countdown -= Time.deltaTime;
        if (createOne && countdown <= 0)
        {
            createOne = false;
            TheForge.Instance.BuildNPC("Frog",5);
        }

	}
}
