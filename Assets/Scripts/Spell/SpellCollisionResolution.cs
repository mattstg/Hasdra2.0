using UnityEngine;
using System.Collections;

public class SpellCollisionResolution  {

    

    #region singleton
    private static SpellCollisionResolution instance;


    public static SpellCollisionResolution Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SpellCollisionResolution();
            }
            return instance;
        }
    }
    #endregion

    public SpellCollisionResolution()
    {


    }
	
}
