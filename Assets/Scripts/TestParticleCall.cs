using UnityEngine;
using System.Collections;

public class TestParticleCall : MonoBehaviour {

    void OnParticlesCollision(GameObject other)
    {
        Debug.Log("particle collision!");
    }

    void OnParticleCollision(GameObject go)
    {
        Debug.Log("come on");
    }
    void OnParticlesCollision()
    {
        Debug.Log("particle collision!");
    }
}
