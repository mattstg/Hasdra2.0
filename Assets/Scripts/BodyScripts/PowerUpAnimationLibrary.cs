using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//Given a type, gives all power up animations, however the way i maintain files may cause massive issues come not using Windows
public class PowerUpAnimationLibrary  {


    private static PowerUpAnimationLibrary instance;

    private PowerUpAnimationLibrary() { }

    public static PowerUpAnimationLibrary Instance
   {
      get 
      {
         if (instance == null)
         {
             instance = new PowerUpAnimationLibrary();
         }
         return instance;
      }
   }


    public static string[] GetPowerUpAnimations(string creatureName, float tier)
    {
        try
        {
            string[] animatorFiles = System.IO.Directory.GetFiles("Assets/Hasdra/Resources/Animations/PoweringUpAnimations/" + creatureName, "*.controller"); //gets all animations
            return GetAllStringsStartWith(animatorFiles,"t" + tier.ToString());
        }
        catch
        {
            Debug.LogError("Error getting powerup animations for: " + creatureName + " tier " + tier);
            return null;
        }
    }

    //Price we pay for genericness
    private static string[] GetAllStringsStartWith(string[] toFilter, string starts)
    {
        //The issue is the filename is at the end, I wanted to doa  single linq but fuck
        List<string> tempList = new List<string>();
        foreach (string s in toFilter)
        {
            string ts = System.IO.Path.GetFileName(s);
            if (ts.StartsWith(starts))
            {
                int startIndex = s.IndexOf("Animations");
                int endIndex = s.IndexOf(".controller");
                string toAdd = s.Substring(startIndex, endIndex - startIndex);
                tempList.Add(toAdd);
            }
        }
        return tempList.ToArray<string>();
    }
}
