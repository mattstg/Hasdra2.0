using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Linq;

public class CharacterSkinLoader : MonoBehaviour {

	//given a character and a name of the skin, loads it
   
    public void LoadCharacterSkin(string skinName,GameObject character)
    {
        string[] allFiles;
        try
        {
            allFiles = Directory.GetFiles(GV.SKINS_BASE_FOLDER_FULL + "/" + skinName);
            SpriteRenderer[] allRenderers = character.GetComponentsInChildren<SpriteRenderer>();
            foreach (string spritePath in allFiles)
            {
                try
                {
                    if (Path.GetExtension(spritePath) != ".meta")
                    {
                        string bodyPart = Path.GetFileNameWithoutExtension(spritePath);
                        string truncatedPath = GV.ReduceFilepathToResoureFolderBase(spritePath);
                        allRenderers.SingleOrDefault(x => x.gameObject.name == bodyPart).sprite = Resources.Load<Sprite>(truncatedPath); //Resources.Load<Sprite>(truncatePath[1]);
                    }
                }
                catch
                {
                    //Debug.LogError("Catch: cannot load sprite: " + spritePath);  re add if skins weird, but means that the files in folder dont match bodypart name
                }
            }
        }
        catch
        {
            Debug.LogError("skin error while loading, aborting: " + skinName);
            return;
        }
    }

    
}
