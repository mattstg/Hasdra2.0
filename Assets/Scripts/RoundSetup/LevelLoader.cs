using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelLoader : MonoBehaviour {


    public void Start()
    {
        //Retrieve persistant info (round setup info) that the round scene left for us to initialize with
        PersistantRoundSetupInfo persistanceInfo = GameObject.FindObjectOfType<PersistantRoundSetupInfo>();

        //Initialize physical world
        GameObject.FindObjectOfType<worldInstantiator>().InitializeWorld(persistanceInfo.worldInitParams);
        foreach (SolidMaterial sm in GameObject.FindObjectsOfType<SolidMaterial>())
            sm.InitializeMaterial();


        GV.NumOfPlayers = (persistanceInfo.avatarBlueprints.Count);
        if (persistanceInfo) // if persistance info doesnt exist, then i load a default character.. somewhere else  (search for persitance info)
        {
            LoadPlayers(persistanceInfo);
            FindAllSpellsToLoad(); //Just outputs the spells to load actaully....
            //World info like enviro
            //also starting level
            if (GV.NumOfPlayers == 2)
            {
                GV.worldUI.splitScreenBar.gameObject.SetActive(true);
            }
            if (persistanceInfo.GetComponent<PITestGuiTrigger>())
            {
                GV.worldUI.debugMenu.gameObject.SetActive(true);
            }
            GameObject.Destroy(persistanceInfo);
            this.gameObject.GetComponent<LiveSpellDict>().LoadAllSpells(); //Load only SpellList spells
        }
    }

    public void SetupCameras(PlayerControlScript pcs)
    {
        GV.worldLinks.masterCameraManager.AddCameraToTrack(pcs.AddCamera(),pcs.pid);
        if(GV.NumOfPlayers > 1)
        {
            Debug.Log("num of players: " + GV.NumOfPlayers);
            pcs.cameraControl.SetAsSplitScreen(true, pcs.pid);
        }
        
    }

    private void LoadPlayers(PersistantRoundSetupInfo persistanceInfo) 
    {
        foreach (AvatarBlueprint blueprint in persistanceInfo.avatarBlueprints)
        {
            PlayerControlScript newPcs = TheForge.Instance.BuildAvatarPlayer(blueprint);
            float xStart = blueprint.pid * 5 - 10;
            newPcs.transform.position = new Vector2(xStart, GameObject.FindObjectOfType<worldInstantiator>().GetSurfacePoint(xStart) + 10); //GameObject.FindObjectOfType<StartingLocations>().GetStartingPosition(blueprint.startingPosName);
            GV.worldUI.players.Add(newPcs.pid, newPcs); 
        }
    }

   //So this function doesnt actaully do anything except output the names of things to load, all spells are loaded elsewhere
    private void FindAllSpellsToLoad()
    {
        List<string> allSpellsToLoad = new List<string>();

        XMLEncoder xmlEncoder = new XMLEncoder();
        Queue<LevelPackage> levelTrackerStack = xmlEncoder.XMLToLevelTracker(GameObject.FindObjectOfType<PersistantRoundSetupInfo>().avatarBlueprints[0].characterSelected, GV.fileLocationType.Characters).levelUpStack;
        while (levelTrackerStack.Count > 0)
        {
            LevelPackage nextStack = levelTrackerStack.Dequeue();
            allSpellsToLoad.Add(nextStack.pkgName);
        }

        string toOut = "";
        foreach (string s in allSpellsToLoad)
        {
            toOut += s + ",";
        }

        Debug.Log("for char: " + GameObject.FindObjectOfType<PersistantRoundSetupInfo>().avatarBlueprints[0].characterSelected + " Things loaded: " + toOut);
    }

    private void LoadSplitScreenBar()
    {

    }

}
