using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class Habitat : MonoBehaviour {

    List<Creature> creatures = new List<Creature>();
    public string habitatName;
    AudioManager audioManager;

    public void Start()
    {
        string habitatAudio = "Audio/Habitat/" + habitatName;
        if (File.Exists("Assets/Hasdra/Resources/" + habitatAudio + ".wav"))
        {
            audioManager = gameObject.AddComponent<AudioManager>();
            audioManager.Initialize();
            audioManager.PlaySound("Habitat/" + habitatName, true);
        }
        else
        {
            Debug.Log("Assets/Hasdra/Resources/" + habitatAudio + ".wav does not exist");
        }

    }

    public void AddCreature(Creature creature)
    {
        creatures.Add(creature);
    }
   /* HabitatPhysical physicalHabitat;
    Brain habitatBrain;
    List<EntityDNA> creatureDNA;*/

    /*public void Update(float dt) //called by habitatManager
    {

    }*/

}

