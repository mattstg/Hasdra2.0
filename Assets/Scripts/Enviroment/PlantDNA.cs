using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlantDNA : DNA{
    //this could be turned into a dictionary holding instead, like BodyStats. probably more memory efficent?
    Dictionary<string, float> genes = new Dictionary<string, float>() {};/*
    public float desiredTemp;
    public float desiredHeight; //or banch height, num of branches and such
    public float desiredMoistureStorage;
    public float desiredProductionRate;

    public float upgradeBank;   //this is just the amount of desire to do something, when it reaches full, it should do it.
    public float upgradeBankMax;
    public float splitBank; //amount of splits

    public float geneBranchHeightVsWidth = .6f;

    public float growDistribution;
    public float splitDistribution;
    public float seedDistribution;
    public float produceDistribution;

    public float numberOfBranches;
    public float productionRate;*/

    string[] productionTags;

    public void SaveToXML(int enviroNumber, Vector2 location)
    {
        //XMLEncoder xmlEncoder = new XMLEncoder();
        //xmlEncoder.DictionaryToXML<float>(enviroNumber + location.ToString(), genes, GV.fileLocationType.Trees);
        /*

        Dictionary<string, float> DNADictionary = new Dictionary<string, float>(){{"desiredTemp",desiredTemp},{"desiredHeight",desiredHeight},{"desiredMoistureStorage",desiredMoistureStorage},{"desiredProductionRate",desiredMoistureStorage},
            {"desiredProductionRate",desiredProductionRate},{"upgradeBank",upgradeBank},{"upgradeBankMax",upgradeBankMax},{"splitBank",splitBank},{"geneBranchHeightVsWidth",geneBranchHeightVsWidth},{"growDistribution",growDistribution},
            {"splitDistribution",splitDistribution},{"seedDistribution",seedDistribution},{"produceDistribution",produceDistribution},{"numberOfBranches",numberOfBranches},{"productionRate",productionRate}};
        
        xmlEncoder.DictionaryToXML<float>(enviroNumber + location.ToString(), DNADictionary, GV.fileLocationType.Trees);*/
    }

    public void LoadDNAFromXML(string DNAName, float mutationAllowance)
    {
        //This should be upgraded to create a probability cluster towards center, to do that, at the moment use averages
        /*XMLEncoder xmlEncoder = new XMLEncoder();
        try
        {
            List<Dictionary<string, string>> DNADictionary = xmlEncoder.XmlToDictionaryList(DNAName, GV.fileLocationType.Trees);
            foreach (KeyValuePair<string,string> s in DNADictionary[0])
            {
                genes.Add(s.Key, float.Parse(s.Value));
            }
            /*
            desiredTemp = float.Parse(DNADictionary[0]["desiredTemp"] + mutationValue(mutationAllowance));
            desiredHeight = float.Parse(DNADictionary[0]["desiredHeight"] + mutationValue(mutationAllowance)); 
            desiredMoistureStorage = float.Parse(DNADictionary[0]["desiredMoistureStorage"] + mutationValue(mutationAllowance)); 
            desiredProductionRate = float.Parse(DNADictionary[0]["desiredProductionRate"] + mutationValue(mutationAllowance)); 
            upgradeBank = float.Parse(DNADictionary[0]["upgradeBank"] + mutationValue(mutationAllowance));   
            upgradeBankMax = float.Parse(DNADictionary[0]["upgradeBankMax"] + mutationValue(mutationAllowance)); 
            splitBank = float.Parse(DNADictionary[0]["splitBank"] + mutationValue(mutationAllowance)) ; 
            geneBranchHeightVsWidth = float.Parse(DNADictionary[0]["geneBranchHeightVsWidth"] + mutationValue(mutationAllowance));
            growDistribution = float.Parse(DNADictionary[0]["growDistribution"] + mutationValue(mutationAllowance));
            splitDistribution = float.Parse(DNADictionary[0]["splitDistribution"] + mutationValue(mutationAllowance));
            seedDistribution = float.Parse(DNADictionary[0]["seedDistribution"] + mutationValue(mutationAllowance));
            produceDistribution = float.Parse(DNADictionary[0]["produceDistribution"] + mutationValue(mutationAllowance));
            numberOfBranches = float.Parse(DNADictionary[0]["numberOfBranches"] + mutationValue(mutationAllowance));
            productionRate = float.Parse(DNADictionary[0]["productionRate"] + mutationValue(mutationAllowance));
        }
        catch
        {
            Debug.LogError("DNA could not be loaded properly: " + DNAName);
        }*/
        //should set position from DNA as well
    }

    private float mutationValue(float range)
    {
        float mutation = (Random.Range(-range, range) + Random.Range(-range, range)) / 2;
        Debug.Log("mutation variance: " + mutation);
        return mutation;        
            //return ((Random.Range(-range, range) + Random.Range(-range, range)) / 2);  Return this func when proven works
    }

    public float getDNA(string name)
    {
        try{
            return genes[name];
        }
        catch{
            Debug.LogError("DNA Name: " + name + " not found");
            return 1;
        }
    }

    public void setDNA(string name, float value)
    {
        try
        {
            genes[name] = value;
        }
        catch
        {
            Debug.LogError("DNA Name: " + name + " not found");
        }
    }

    public void modDNA(string name, float value)
    {
        try
        {
            genes[name] += value ;
        }
        catch
        {
            Debug.LogError("DNA Name: " + name + " not found");
        }
    }
}
