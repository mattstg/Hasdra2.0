using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TheForge
{
    #region Singleton
    private static TheForge instance;

    public static TheForge Instance
    {
        get{
            if(instance == null)
            {
                instance = new TheForge();
            }
            return instance;
        }
    }
#endregion
    Dictionary<string,string> prefabLocation;  //name to prefab location

    public TheForge()
    {
        prefabLocation = new Dictionary<string,string>();
    }

    public PlayerControlScript BuildAvatarPlayer(AvatarBlueprint blueprint) //Creates
    {
        XMLEncoder xmlEncoder = new XMLEncoder();
        GameObject player = MonoBehaviour.Instantiate(Resources.Load("Prefabs/ActionScene/CharacterShell")) as GameObject;
        PlayerControlScript pcs = player.GetComponent<PlayerControlScript>();
        pcs.pid = blueprint.pid;

        if (pcs.pid == 0)
        {
            GV.SetAllChildLayersRecurisvely(player.transform, "Player");
        }
        else
        {
            GV.SetAllChildLayersRecurisvely(player.transform, "OppPlayer");
        }
        //order is important
        GameObject playerGUI = MonoBehaviour.Instantiate(Resources.Load("Prefabs/ActionScene/PlayerGUI")) as GameObject; //change location
        playerGUI.transform.SetParent(GV.worldUI.mainCanvas, false);
        player.GetComponent<PlayerGuiController>().Initialize(playerGUI.GetComponent<GUIOrganizer>(), pcs, pcs.pid);
        pcs.Initialize(xmlEncoder.XMLToLevelTracker(blueprint.characterSelected, GV.fileLocationType.Characters), blueprint.startingLevel,"player"+pcs.pid);
        pcs.LoadSkin(blueprint.skin);
        pcs.guiControl = player.GetComponent<PlayerGuiController>();
        GV.worldLinks.masterCameraManager.AddCameraToTrack(pcs.AddCamera(), pcs.pid);
        if (GV.NumOfPlayers > 1)
        {
            pcs.cameraControl.SetAsSplitScreen(true, pcs.pid);
        }
        return pcs;
    }

    public PlayerControlScript BuildNPC(string NPCName, int lvl)
    {
        string prefabPath = "Prefabs/NPCs/creature"; // + NPCName; re-enable in future when we have more complex prefabs
        XMLEncoder xmlEncoder = new XMLEncoder();
        GameObject player = MonoBehaviour.Instantiate(Resources.Load(prefabPath)) as GameObject;
        if(player == null)
            player = MonoBehaviour.Instantiate(Resources.Load("Prefabs/NPCs/creature")) as GameObject;
        PlayerControlScript pcs = player.GetComponent<PlayerControlScript>();
        GV.SetAllChildSortingLayerRecurisvely(player.transform, "Creatures");
        pcs.Initialize(xmlEncoder.XMLToLevelTracker(NPCName, GV.fileLocationType.NPCs), lvl * GV.EXPERIENCE_PER_LEVEL,NPCName);
        return pcs;
    }

    public void CreatePrefabFromDNA(EntityDNA dnablueprint)
    {
        //Create creature, if name already exists but same species, then add a _V1, _V2...
        //if name already exists but entirelly different species, then _a1  for alternate
        //svalbard might take care of this tho
    }
}
