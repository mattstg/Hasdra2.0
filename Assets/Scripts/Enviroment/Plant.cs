using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Plant : MonoBehaviour {

    List<Bush> bushSpots; //tree should take care of growing these
    List<Branch> branches; //tree can randomly pick one to grow
    PlantDNA DNA;

    float plantLevel = 1;

    float happiness = 100;//Getting planted should overwrite happiness
    float totalHapiness = 0;
    float growBigDesire = 0;
    float growBushDesire = 0;
    float produceDesire = 0;
    float seedDesire = 0;
    float splitDesire = 0;
    float totalDesireUpgradeLimit = 1000;
    
    float moistureStored;
    float maxMoistureStorage;
    float nutrientStored;
    float maxNutrientStorage;
    float ManaStored;
    float maxManaStorage;

    public void Start()
    {
        branches = new List<Branch>();
        branches.Add(GetComponent<Branch>());
    }

    public void Upkeep()
    {
        BalanceDesires();
        CalculateHappiness(); //needs to be before upgrade
        Upgrade();
        //consume internal moisture based on size
        //consume enviroment nutrients from the ground
        //Attempt production
        //attempt growth
    }

    private void BalanceDesires()
    {
        growBigDesire += totalHapiness * DNA.getDNA("growBigDesireDistribution");
        growBushDesire += totalHapiness * DNA.getDNA("growBushDesireDistribution");
        produceDesire += totalHapiness * DNA.getDNA("produceDesireDistribution");
        seedDesire += totalHapiness * DNA.getDNA("seedDesireDistribution");
        splitDesire += totalHapiness * DNA.getDNA("splitDesireDistribution");

    }

    #region actions
    private void Upgrade()
    {
        if (growBigDesire > totalDesireUpgradeLimit)
            GrowBig();
        if (growBushDesire > totalDesireUpgradeLimit)
            GrowBushes();
        if (produceDesire > totalDesireUpgradeLimit)
            Produce();
        if (seedDesire > totalDesireUpgradeLimit)
            Fruit();
        if (splitDesire > totalDesireUpgradeLimit)
            Split();

    }

    private void GrowBig()
    {
        //transform.localScale += new Vector3(amount * (1 - DNA.getDNA("geneBranchHeightVsWidth")), amount * DNA.getDNA("geneBranchHeightVsWidth"), 0) * (GV.TREE_ENERGY_TO_GROWTH / DNA.getDNA("numberOfBranches"));
    }

    private void GrowBushes()
    {
        //grows production amount AND bush sizes (so stacks with growBranches
        //productionRate += GV.TREE_PROD_INCREASE_PER_ENERGY * amount;
    }

    private void Split()
    {
       /* DNA.modDNA("splitBank",amount);
        int splitAmounts = (int)(DNA.getDNA("splitBank / GV.TREE_SPLIT_BANK_TO_ACTIVATE"));
        if (splitAmounts < 1)
            return;

        //in future can have algo for height preference
        for (int i = 0; i < splitAmounts; i++)
        {            
            if (!branches[Random.Range(0, branches.Count)].Split())
                branches[Random.Range(0, branches.Count)].Split();
        }*/
    }

    private void Fruit()
    {
        //step one, calculate cost of total fruit to be produced
        //if not enough, produce half of what could be produced
        //create at random bushes

    }

    private void Produce()
    {

    }
    #endregion

    private void CalculateHappiness() //should be also based upon amount of time that has passsed, atm 
    {
        /*happiness -= Mathf.Abs(DNA.getDNA("desiredTemp") - plantEnviroment.currentTemp); //atm good temp wont make him happy, just bad temp unhappy, fix it when can <<
        happiness += ManaStored / maxManaStorage;
        happiness += nutrientStored / maxNutrientStorage;
        happiness += moistureStored / maxMoistureStorage;*/
    }

    private void UptakeFromEnviro()
    {
       /* if (nutrientStored < maxNutrientStorage)
        {
            float d = maxNutrientStorage - nutrientStored;
            d = (d>plantLevel)?plantLevel:d; //desire of nutrients capped by plant level (intake speed)
            nutrientStored += plantEnviroment.TakeNutrients(d);
        }
        if (moistureStored < maxMoistureStorage)
        {
            float d = maxMoistureStorage - moistureStored;
            d = (d > plantLevel) ? plantLevel : d; //desire of nutrients capped by plant level (intake speed)
            moistureStored += plantEnviroment.TakeMoisture(d);
        }
        if (ManaStored < maxManaStorage)
        {
            float d = maxManaStorage - ManaStored;
            d = (d > plantLevel) ? plantLevel : d; //desire of nutrients capped by plant level (intake speed)
            ManaStored += plantEnviroment.TakeMana(d);
        }*/
    }

    private void UpdatePlantStats()
    { //could be changed with genes to store different amounts
        maxMoistureStorage = plantLevel * GV.PLANT_STORAGE_PER_LEVEL * DNA.getDNA("maxMoistureStorage");
        maxNutrientStorage = plantLevel * GV.PLANT_STORAGE_PER_LEVEL * DNA.getDNA("maxNutrientStorage");
        maxManaStorage = plantLevel * GV.PLANT_STORAGE_PER_LEVEL * DNA.getDNA("maxManaStorage");
    }
}
