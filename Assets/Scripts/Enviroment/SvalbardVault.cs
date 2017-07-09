using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SvalbardVault
{
    #region Singleton
    private static SvalbardVault instance;

    public static SvalbardVault Instance
    {
        get{
            if(instance == null)
            {
                instance = new SvalbardVault();
            }
            return instance;
        }
    }
#endregion

    Dictionary<string, DNA> vaultStorage = new Dictionary<string, DNA>();

    private SvalbardVault()
    { 
        XMLEncoder xmlEncoder = new XMLEncoder();
        //Load all DNA from xml
        string[] allNPCPaths = XMLEncoder.GetAllFoldersInPath(GV.fileLocationType.NPCs); //all folders are creature names
        foreach (string npcName in allNPCPaths)
        {
            Debug.Log("npc: " + npcName + "to be loaded in vault");

        }
    }

    private void ConstructNPCDNA(string npcName)
    {
        string fullPath = XMLEncoder.GetFilePathByType(GV.fileLocationType.NPCs) + "/" + npcName + "/";
        string emotionalDNA = fullPath + "emoDNA.xml";
        string cortexDNA = fullPath + "cortexDNA.xml";
        string memoryDNA = fullPath + "memoryDNA.xml";
        string senesesDNA = fullPath + "sensesDNA.xml";
        string brainStemDNA = fullPath + "brainStemDNA.xml";
        string levelTracker = fullPath + "levelTracker.xml";
        string bodyPartDNA = fullPath + "bodyPartsDNA.xml";

    }

    public DNA GetDNA(string dnaCarrierName, GV.DNAType dnaType)
    {
        //
        return null;
    }

    //private 
}

