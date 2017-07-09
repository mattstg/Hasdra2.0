using UnityEngine;
using System.Collections;

public class NumericDisplay : MonoBehaviour {

    float displayValue;
    float counter = GV.DYNAMIC_TEXT_LIFESPAN;
    bool isFading = false;
    string textPrefix;
    bool hideNumber; //so its not a numeric display with this, just a text
    Transform parentTransform;
    Vector3 moveSpeed;
    Vector3 positionOffset;
    TextMesh textMesh;
    int parentObjectID;

    string stringToPass = "";
    System.Action<int,string> onDeathCallFunc;

    public void Initialize(Color textColor, string prefix, float startValue, Transform _parentTransform, Vector3 offset, bool _hideNumber = false)
    {
        textMesh = gameObject.AddComponent<TextMesh>();
        textMesh.fontSize = 24;
        textMesh.characterSize = .1f;
        textPrefix = prefix;
        hideNumber = _hideNumber;
        GetComponent<MeshRenderer>().sortingLayerName = "DynamicText";
        parentTransform = _parentTransform;
        displayValue = startValue;
        textMesh.text = textPrefix + System.Math.Truncate(displayValue).ToString(); 
        textMesh.color = textColor;
        positionOffset = offset;
    }
    
    //Pass a function for it to call when it dies, that takes a string as an arg, and returns void  (can fix the args to be more generic, ask Matt or Google)
    //the use of this is that the parent may want to know when the object has died, to remove it from any list it is using to track it
    public void LinkFunctionForCallOnDeath(System.Action<int, string> deathEventToCall, string _stringToPass, int _callingObjID)
    {
        parentObjectID = _callingObjID;
        stringToPass = _stringToPass;
        onDeathCallFunc = deathEventToCall;
    }

    void CallSubscribedFunction()
    {
        onDeathCallFunc(parentObjectID,stringToPass);
    }
	// Update is called once per frame
	void Update () {
        if (parentTransform == null)
            isFading = true;

        if (!isFading)
        {
            counter -= Time.deltaTime;
            ScaleText();
            string toOut = (displayValue < 1) ? string.Format("{0:0.#}", displayValue) : System.Math.Truncate(displayValue).ToString();
            textMesh.text = (hideNumber) ? textPrefix : textPrefix + toOut;
            transform.position = parentTransform.position + positionOffset;
            if (counter <= 0)
            {
                isFading = true;
                Destructible2D.D2dDestroyer destr = gameObject.AddComponent<Destructible2D.D2dDestroyer>();                
                destr.Fade = true;
                destr.FadeDuration = GV.DYNAMIC_TEXT_FADE_TIME;
                destr.Life = GV.DYNAMIC_TEXT_FADE_TIME;
                float dir = (parentTransform.position.x < transform.position.x) ? 1 : -1; //direction it shoots away to (away from parent)
                moveSpeed = new Vector3(GV.DYNAMIC_TEXT_DROP_HORZ_SPEED * dir, GV.DYNAMIC_TEXT_DROP_VERT_SPEED, 0);
                if (onDeathCallFunc != null) CallSubscribedFunction();
            }
        }
        else
        {
            this.transform.position += moveSpeed*Time.deltaTime;
            moveSpeed += new Vector3(0, (float)Physics2D.gravity.y*Time.deltaTime, 0);
        }
	}

    public bool ModValue(float addToValue)
    {
        if (isFading)
            return false;
        displayValue += addToValue;
        counter = GV.DYNAMIC_TEXT_LIFESPAN;
        return true;
    }

    public bool SetValue(float setToValue)
    {
        if (isFading)
            return false;
        counter = GV.DYNAMIC_TEXT_LIFESPAN;
        displayValue = setToValue;
        if (displayValue != displayValue)
            Debug.Log("1");
        return true;
    }

    void ScaleText()
    {
        float scaleSize = (counter/GV.DYNAMIC_TEXT_LIFESPAN)*GV.DYNAMIC_TEXT_SCALE_FROM_ADD + displayValue*GV.DYNAMIC_TEXT_SCALE_PER_POINT;
        if (scaleSize != scaleSize)
            Debug.Log("counter: " + counter + "disp: " + displayValue);
        scaleSize = (scaleSize > GV.NUMERIC_DISPLAY_MAX_SCALE)?GV.NUMERIC_DISPLAY_MAX_SCALE:scaleSize;
        scaleSize = (scaleSize < GV.NUMERIC_DISPLAY_MIN_SCALE) ? GV.NUMERIC_DISPLAY_MIN_SCALE : scaleSize;
        this.transform.localScale = new Vector3(scaleSize,scaleSize, 1);
    }
}
