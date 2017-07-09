using UnityEngine;
using System.Collections;

public class Terra : MonoBehaviour {
    public Transform terrainParent;
    Vector2 mapSize = new Vector2(4,3);
    System.Collections.Generic.List<SolidMaterial> solidMaterialList = new System.Collections.Generic.List<SolidMaterial>();   //its public so I can attach shit manually from the scene for now for testing
        
	// Use this for initialization
	void Start () {

        foreach (SolidMaterial sm in GameObject.FindObjectsOfType<SolidMaterial>())
        {
            solidMaterialList.Add(sm);
            sm.InitializeMaterial();
        }
        /*
        foreach (SolidMaterial sm in terrainParent.GetComponentsInChildren<SolidMaterial>())
        {
            solidMaterialList.Add(sm);
            sm.InitializeMaterial();
        }*/
      
        //InitializeTerra();  not currently functioning properly, leave that until automated terrain generation

	}

    void InitializeTerra()
    {
        CreateAllSolidMaterials();
    }

    void CreateAllSolidMaterials()
    {
        //foreach(SolidMaterial sm in solidMaterialList)
        //    sm.InitializeMaterial(GV.STD_SOLIDMATERIAL_RESOLUTION);
        
        for(int y = 0; y < mapSize.y;y++)
            for (int x = 0; x < mapSize.x; x++)
            {
                Instantiate(Resources.Load("Prefabs/enviroment/BackgroundDirt"), new Vector2(x * GV.BASIC_GROUND_SIZE.x, -y * GV.BASIC_GROUND_SIZE.y - GV.BASIC_GROUND_SIZE.y / 2), Quaternion.identity);
                GameObject go = Instantiate(Resources.Load("Prefabs/enviroment/BasicGround"), new Vector2(x * GV.BASIC_GROUND_SIZE.x, -y * GV.BASIC_GROUND_SIZE.y - GV.BASIC_GROUND_SIZE.y/2), Quaternion.identity) as GameObject;
                go.GetComponent<SolidMaterial>().InitializeMaterial();
                solidMaterialList.Add(go.GetComponent<SolidMaterial>());
            }
        //create three borders
        GameObject obs = Instantiate(Resources.Load("Prefabs/enviroment/Obsidian"), new Vector2((mapSize.x * GV.BASIC_GROUND_SIZE.x)/2, mapSize.y * GV.BASIC_GROUND_SIZE.y - GV.BASIC_GROUND_SIZE.y), Quaternion.identity) as GameObject;
        obs.transform.localScale = new Vector2(mapSize.x, .3f);
        obs = Instantiate(Resources.Load("Prefabs/enviroment/Obsidian"), new Vector2(-1, (mapSize.y * GV.BASIC_GROUND_SIZE.y)/2), Quaternion.identity) as GameObject;
        obs.transform.localScale = new Vector2(mapSize.x, .3f);
    }


}
