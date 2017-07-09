using UnityEngine;
using System.Collections;

public class LiveWorldStats{

#region singleton
    private static LiveWorldStats instance;

    private LiveWorldStats() { }

    public static LiveWorldStats Instance
   {
      get 
      {
         if (instance == null)
         {
             instance = new LiveWorldStats();
         }
         return instance;
      }
   }
#endregion singleton

    float worldEnergy;

    //If this was made to be more generic, so it auto detected which enviroment it was coming from, thatd be wicked. This guy can process an enviromentInfo (enviroInfo will just be a datastruct)
    //itll be easier for everyone to call it, although a different call will be to pass your enviroment, to avoid a feedback loop.
    public void AddWorldEnergy(float energyToAdd)
    {
        //im sure theyll be complex ways of dividing energy in the future
        //   ______      /|
        //  / >.<  \____/ |
        //  \/    __   __|
        //   \/ \/  \/\/
        // for the void!
        worldEnergy += energyToAdd;
    }
}
