﻿using UnityEngine;
using System.Collections;

public class PersistantRoundSetupInfo : MonoBehaviour {

    public System.Collections.Generic.List<AvatarBlueprint> avatarBlueprints = new System.Collections.Generic.List<AvatarBlueprint>();
    public string levelNameSelected;
    public worldInstantiator.WorldInitParams worldInitParams;  //Not currently filled by level select, no existing UI
}
