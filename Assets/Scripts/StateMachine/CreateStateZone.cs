using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateStateZone : MonoBehaviour {
    public GameObject statePrefab;
    public Transform parentOfState;
    List<GameObject> currentColliders = new List<GameObject>();
    bool spotTaken = false;

    public void CreateState()
    {
        //Check here if an object is already inside
        if (currentColliders.Count == 0)
        {
            GameObject newState = Instantiate(statePrefab, this.transform.position, Quaternion.identity) as GameObject;
            newState.transform.parent = parentOfState;
            newState.transform.localScale = new Vector3(1,1,1);
            GameObject.FindObjectOfType<TreeTracker>().AddState(newState.GetComponent<State>());
        }
    }

    public void CreateState(string stateChosen)
    {
        GameObject newState = new GameObject();

        switch (stateChosen)
        {
            case "Action":
                newState = Instantiate(Resources.Load<GameObject>("Prefabs/StatePrefabs/ActionState"), this.transform.position, Quaternion.identity) as GameObject;
                break;
            case "ApplyVelo":
                newState = Instantiate(Resources.Load<GameObject>("Prefabs/StatePrefabs/VeloState"), this.transform.position, Quaternion.identity) as GameObject;
                break;
            case "Transform":
                newState = Instantiate(Resources.Load<GameObject>("Prefabs/StatePrefabs/TransformState"), this.transform.position, Quaternion.identity) as GameObject;
                break;
            case "Create":
                newState = Instantiate(Resources.Load<GameObject>("Prefabs/StatePrefabs/CreateState"), this.transform.position, Quaternion.identity) as GameObject;
                break;
            case "FaceDirection":
                newState = Instantiate(Resources.Load<GameObject>("Prefabs/StatePrefabs/FaceDir"), this.transform.position, Quaternion.identity) as GameObject;
                break;
            case "Empty":
                newState = Instantiate(Resources.Load<GameObject>("Prefabs/StatePrefabs/Empty"), this.transform.position, Quaternion.identity) as GameObject;
                break;
            default:
                Debug.LogError("Unvalid state passed in createStateZone");
                break;
        }
        newState.transform.SetParent(parentOfState);
        GameObject.FindObjectOfType<TreeTracker>().AddState(newState.GetComponent<State>());

    }

    public void OnTriggerEnter2D(Collider2D coli)
    {
        if (coli.CompareTag("State"))
        {
            if (!currentColliders.Contains(coli.gameObject))
                currentColliders.Add(coli.gameObject);
        }
    }

    public void OnTriggerExit2D(Collider2D coli)
    {
        if (coli.CompareTag("State"))
        {
            if (currentColliders.Contains(coli.gameObject))
                currentColliders.Remove(coli.gameObject);
        }
    }

    
}
