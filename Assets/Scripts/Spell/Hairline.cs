using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hairline : MonoBehaviour {

    //GV
    public static readonly float randAngRange = 45;  //Angle at which it will randomly spawn
    public static readonly Vector2 spriteSize = new Vector2(50, 7);

    public void Initialize(int power, float ang)
    {
        transform.name = "Hairline" + power;
        power--;
        float randAngSpawn = Random.Range(-randAngRange, randAngRange);
        float setAngle = randAngSpawn + ang;
        transform.eulerAngles = new Vector3(0, 0 , setAngle); //to test
        

        GetComponent<Destructible2D.D2dExplosion>().ForceStamp();
        if (power >= 2) //cuz 1 will not initalize anything else
        {
            GameObject go = Instantiate(Resources.Load("Prefabs/Spell/Hairline")) as GameObject;
            float x = Mathf.Cos(setAngle * Mathf.Deg2Rad) * spriteSize.x / 100;
            float y = Mathf.Sin(setAngle * Mathf.Deg2Rad) * spriteSize.y / 100;
            go.transform.position = transform.position + new Vector3(x, y);
            go.GetComponent<Hairline>().Initialize(power, setAngle);
            
            Debug.Log(string.Format("Hairline initialized with {0} power and starting at {1} angle at pos {2}+{3} = {4}", power, setAngle, transform.position, new Vector3(x, y), go.transform.position));
        }

        //Destroy(gameObject);
    }

    
}
