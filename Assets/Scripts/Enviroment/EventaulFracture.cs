using UnityEngine;
using System.Collections;

public class EventaulFracture : MonoBehaviour {

    public float timer = 10f;
    float startTimer = 10f;
    bool initialized = false;
    public bool isFracturing = false;
    Vector3 shakeIntensity = GV.GROUND_EVENTAULFRACTURE_SHAKE_INTENSITY;
    Color fadeToColor = Color.red;
    Color originalColor = Color.white;
    Destructible2D.D2dDestructible destructibleComponent;
   
	//This script slowly counts down, and causes the material to fracture
	void Start () {
        if (!GetComponent<SolidMaterial>() || !GetComponent<Destructible2D.D2dDestructible>())
        {
            Debug.LogError("Eventaul fracture placed on object without a solidMaterial or sprite, removing self");
            Destroy(this); //if the object is not valid, destroy self
        }
        else
        {
            destructibleComponent = GetComponent<Destructible2D.D2dDestructible>();
        }
	}

 
    public void Initialize(float _timer,Color _fadeToColor)
    {
        fadeToColor = _fadeToColor;
        timer = _timer;
        startTimer = timer;
        initialized = true;
        originalColor = GetComponent<Destructible2D.D2dDestructible>().Color;
    }

    public void Initialize(EventaulFracture toClone)
    {
        //Debug.Log("initializing from parent: " + toClone.timer + "/" + startTimer + ", Color " + toClone.fadeToColor + " from original color: " + originalColor);
        startTimer = toClone.startTimer;
        originalColor = toClone.originalColor;
        Initialize(toClone.timer, toClone.fadeToColor);
    }
	
	// Update is called once per frame
	void Update () {
        if (!initialized)    //So a fracture may clone the object with this component still attached, this will cause it to delete itself
        {
            Destroy(this);
            return;
        }
        timer -= Time.deltaTime;
        destructibleComponent.Color = Color.Lerp(Color.white, fadeToColor, 1 - (timer / startTimer));
        //transform.position += shakeIntensity;
        //shakeIntensity *= -1f;

        if (timer <= 0)
        {
            Debug.Log("timer was less than zero");
            isFracturing = true; //in case cloned child references this
            initialized = false; //since child will clone it, remove this
            destructibleComponent.Color = originalColor;
            GetComponent<SolidMaterial>().Fracture();
            Destroy(this);
        }
	}

    public void CancelFracturing()
    {
        destructibleComponent.Color = originalColor;
        Destroy(this);
    }
}
