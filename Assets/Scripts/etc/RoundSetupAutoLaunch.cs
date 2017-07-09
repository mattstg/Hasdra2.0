using UnityEngine;
using System.Collections;

public class RoundSetupAutoLaunch : MonoBehaviour {

    public int startingLevel = 100;
    public string startingCharacter = "PlayerOne";
    public Transform startPosTransform;
    public string defaultSkin = "Default";
    public string startPosName = "Origins";
	// Use this for initialization
	void Awake () {
        if (!FindObjectOfType<PersistantRoundSetupInfo>())
        {
            Vector2 startPos = (startPosTransform == null) ? new Vector2(0, 0) : new Vector2(startPosTransform.position.x,startPosTransform.position.y);
            GameObject persistantObject = new GameObject();
            PersistantRoundSetupInfo info = persistantObject.AddComponent<PersistantRoundSetupInfo>();
            GameObject.DontDestroyOnLoad(persistantObject);
            info.avatarBlueprints.Add(new AvatarBlueprint(0, startingCharacter, defaultSkin, startingLevel, startPosName));
            info.levelNameSelected = startPosName;
        }
	}
	
}
