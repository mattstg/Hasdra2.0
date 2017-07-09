using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gaia : MonoBehaviour {
    public int NumberOfEnviroments = 2;
    public float timeToUpdateEnviroments = 5f;
    float counterToUpdateEnviroments = 0;
    List<Biome> enviroments;

    public void Start()
    {
        counterToUpdateEnviroments = timeToUpdateEnviroments;
        CreateEnviroments(NumberOfEnviroments);

        XMLToPlant xmlToPlant = new XMLToPlant();
        /*foreach (string s in xmlToPlant.GetAllTreeList())
            Debug.Log("tree: " + s);*/

    }

    public void CreateEnviroments(int numberOfEnviros)
    {
        enviroments = new List<Biome>();
        for (int i = 0; i < numberOfEnviros; i++)
        {
            GameObject enviro = Instantiate(Resources.Load("Prefabs/enviroment/Enviroment"), new Vector3(i * GV.ENVIROMENT_SIZE + GV.ENVIROMENT_SIZE/2,GV.ENVIROMENT_SIZE/2,0), Quaternion.identity) as GameObject;
            enviro.GetComponent<Biome>().EnviromentID = i;
            enviroments.Add(enviro.GetComponent<Biome>());
        }
        for (int i = 0; i < enviroments.Count; i++)
        {
            try
            {
                enviroments[i].leftNeighbor = enviroments[i - 1];
            }
            catch
            {

            }
            try
            {
                enviroments[i].rightNeighbor = enviroments[i + 1];
            }
            catch
            {

            }
        }
    }

    public void Update()
    {
        counterToUpdateEnviroments += Time.deltaTime;
        if (counterToUpdateEnviroments >= timeToUpdateEnviroments)
        {
            UpdateEnviroments(counterToUpdateEnviroments);
            counterToUpdateEnviroments = 0;
        }

    }

    private void UpdateEnviroments(float timePassed)
    {
        foreach (Biome e in enviroments)
        {
            e.UpdateEnviroment(timePassed);
        }
    }

    public void LoadAllPlants()
    {

        foreach (string xmlPlantDnaPath in XMLEncoder.GetAllXMLFilePaths(GV.fileLocationType.Trees))
        {
            LoadPlant(xmlPlantDnaPath);
            Debug.Log("Plant loaded: " + xmlPlantDnaPath);
        }
    }

    private void LoadPlant(string xmlPath)
    {
        //from name, get enviro and pos
        //load dna into plant
        //plant the plant
    }
}
