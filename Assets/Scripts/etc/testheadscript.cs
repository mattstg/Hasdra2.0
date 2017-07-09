using UnityEngine;
using System.Collections;

public class testheadscript : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D coli)
    {
        Debug.Log("specifically the head");
    }
}
