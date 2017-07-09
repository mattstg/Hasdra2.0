using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class RoundSetupMainScript : MonoBehaviour {

    
    public string startPosName = "Origins";
    public Text playerOneSelectedName;
    public Text playerTwoSelectedName;
    string characterSelected = "";
    string character2Selected = "";
    string skinSelected = "Default";
    public GameObject characterSelectionButtonPrefab;
    public Transform characterSelectionGridParent;
    public InputField startLevelInputFeild;
    int startLevel { get { return int.Parse(startLevelInputFeild.text); } }
    bool playerOneSelection = true; //temp until network setup
    bool secondPlayerActive = false;

    public void Start()
    {
        LoadAllCharacters();
    }

    public void ToggleMusic()
    {
        GV.AUDIO = !GV.AUDIO;
    }

    private void LoadAllCharacters()
    {
        List<string> allCharacters = XMLEncoder.LoadAllCharacterNames();

        foreach (string charName in allCharacters)
        {
           GameObject charButton = Instantiate(characterSelectionButtonPrefab) as GameObject;
           charButton.GetComponentInChildren<Text>().text = charName;
           charButton.GetComponent<CharacterSelection>().characterName = charName;
           charButton.transform.parent = characterSelectionGridParent;
        }
    }

    public void CharacterSelected(string charSelected)
    {
        if (playerOneSelection)
        {
            characterSelected = charSelected;
            playerOneSelectedName.text = charSelected;
        }
        else
        {
            character2Selected = charSelected;
            playerTwoSelectedName.text = charSelected;
        }
        if(secondPlayerActive)
            playerOneSelection = !playerOneSelection;
    }

    public void SkinSelected(string _skinSelected)
    {
        skinSelected = _skinSelected;
    }

    public void StartRoundButton()
    {
        if (characterSelected != "" && startPosName != "")
        {
            GameObject persistantObject = new GameObject();
            PersistantRoundSetupInfo info = persistantObject.AddComponent<PersistantRoundSetupInfo>();
            GameObject.DontDestroyOnLoad(persistantObject);
            info.avatarBlueprints.Add(new AvatarBlueprint(0, characterSelected,skinSelected,startLevel, startPosName));
            if(character2Selected != "")
                info.avatarBlueprints.Add(new AvatarBlueprint(1, character2Selected, "Default", startLevel, startPosName));
            info.levelNameSelected = startPosName;
            UnityEngine.SceneManagement.SceneManager.LoadScene("WorldScene");
        }

    }
   
    public void ActivatePlayer2()
    {
        secondPlayerActive = true;
    }

    public string[] RetrieveAllSkins()
    {
        Debugger.OutputList<string>("All skins: ",GV.ReduceFilepathToResoureFolderBase(Directory.GetDirectories(GV.SKINS_BASE_FOLDER_FULL)));
        return GV.ReduceFilepathToResoureFolderBase(Directory.GetDirectories(GV.SKINS_BASE_FOLDER_FULL));  //retrieves all folder paths for skin in the format //Textures...  (after rez)
    }

   



}
