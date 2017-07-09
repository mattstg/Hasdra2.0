using UnityEngine;
using System.Collections;

public class HardSetDestructibleProperties : MonoBehaviour {

    public float defensivePower = 1;
    Destructible2D.D2dDestructible destr;

	void Start()
    {
        destr = GetComponent<Destructible2D.D2dDestructible>();
        destr.SetDefensivePower(defensivePower);
    }

    void Update()
    {
        destr.SetDefensivePower(defensivePower);
    }
}
