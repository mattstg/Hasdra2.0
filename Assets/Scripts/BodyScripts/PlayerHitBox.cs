using UnityEngine;
using System.Collections;

public class PlayerHitBox : MonoBehaviour {
	public Vector3 center;
	public GameObject chest; //used to access the chests location and place 
    public GameObject hitBox;
	// Use this for initialization
	void Start () {
        refreshPosition();

	}
	
	// Update is called once per frame
	void Update () {
        if (!hitBox.GetComponent<Rigidbody2D>().isKinematic)
        {
            refreshPosition();
        }
	}

    private void refreshPosition(){
        center = getNewCenter();
        chest.transform.position = center;
        //chest.transform.position = Vector3.zero;
    }


    public Vector3 getNewCenter()
    {
        return hitBox.transform.position;
    }
}
