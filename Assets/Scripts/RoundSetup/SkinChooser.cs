using UnityEngine;
using System.Collections;
using System.IO;

public class SkinChooser : MonoBehaviour {

    string resourceLoadPath = "Prefabs/RoundSetup/SkinChoice";

    public void Start()
    {
        LoadAllCharacterSkinButtons();
    }

    public void LoadAllCharacterSkinButtons()
    {
        string[] allSkins = Directory.GetDirectories(GV.SKINS_BASE_FOLDER_FULL);
        foreach (string s in allSkins)
        {
            GameObject go = Instantiate(Resources.Load(resourceLoadPath)) as GameObject;
            go.GetComponent<UnityEngine.UI.Text>().text = Path.GetFileName(s);
            go.transform.SetParent(transform);
        }
    }
}
