using UnityEngine;
using System.Collections;

public class t1PowerUp : MonoBehaviour {

    bool doOnce = true;
    public Transform[] particleSys;
    float[] timeToAnimationUpgrade = new float[] { 0,6, 12};
    int animationActivated = -1;
    float timeAlive = 0;
    Color primaryColor, secondaryColor; //Cannot currently change... fuck


    public void Initialize(Color _primaryColor, Color _secondaryColor)
    {
        primaryColor = _primaryColor;
        secondaryColor = _secondaryColor;
        //It is possible to seralize the objects here, and then change them, an expensive operation however, unless t1 is always there just inactive :/
    }

    private void TurnOffAllParticleSys()
    {
        foreach(Transform t in particleSys)
            t.gameObject.SetActive(false);
    }

    void Update()
    {
        timeAlive += Time.deltaTime;
        for (int i = particleSys.Length - 1; i >= 0; --i)
        {
            if (timeAlive >= timeToAnimationUpgrade[i])
            {               
                if (animationActivated != i)
                {
                    TurnOffAllParticleSys();
                    particleSys[i].gameObject.SetActive(true);
                    animationActivated = i;
                }
                break;
            }
        }
    }
}
