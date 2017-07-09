using UnityEngine;
using System.Collections;

public class LayerTimer : MonoBehaviour {
     int layerEnd;
     float timer;
     Transform objTrans;
    //Attach and initialize, after the timer is done, it will swap the layer and the layers beneath
    
    public void Initialize(Transform _objT, int _layerStart, int _layerEnd, float _timer, bool startEnabled)
    {
        objTrans = _objT;
        GV.SetAllChildLayersRecurisvely(objTrans, _layerStart);
        layerEnd = _layerEnd;
        timer = _timer;
        this.enabled = startEnabled;
    }

    public void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            GV.SetAllChildLayersRecurisvely(objTrans, layerEnd);
            Destroy(this);
        }
    }
}
