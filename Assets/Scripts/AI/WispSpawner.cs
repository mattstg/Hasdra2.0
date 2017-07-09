using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispSpawner : MonoBehaviour {

    public Vector2 lvlRange = new Vector2(3, 10);
    public float wispSpawnRate = 12f;
    public float spawnRange = 4;
    float wispSpawnCounter = 0;
	// Update is called once per frame
	void Update () {
        wispSpawnCounter -= Time.deltaTime;
        if(wispSpawnCounter <= 0)
        {
            Vector2 spawnOffset = new Vector2(Random.Range((float)(-spawnRange - 1), spawnRange + 1), Random.Range((float)(-spawnRange - 1), spawnRange + 1));
            Vector2 curPos = transform.position;
            Vector2 newPos = curPos + spawnOffset;
            int lvl = Random.Range((int)lvlRange.x, (int)lvlRange.y);
            lvl = Mathf.Min(lvl,Random.Range((int)lvlRange.x, (int)lvlRange.y));
            
            PlayerControlScript newPcs = TheForge.Instance.BuildNPC("Wisp", lvl);
            newPcs.transform.localScale = new Vector2(.1f, .1f) * lvl;
            newPcs.transform.localPosition = newPos;
            wispSpawnCounter = wispSpawnRate;
        }
	}
}
