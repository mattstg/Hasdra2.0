using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Biome : MonoBehaviour {
    //to decrease memory, find a way to limit the decimal places to 2 or 3. 

    public Biome leftNeighbor;
    public Biome rightNeighbor;
    public int EnviromentID = 0;
    public float enviromentEnergy = 0;
    public float enviromentHeatStored = 0f;
    public float currentTemp = GV.ENVIROMENT_START_HEAT;
    public float desiredTemp = GV.ENVIROMENT_START_HEAT;
    //3 nutrient types
    public float storedMoisture = GV.ENVIROMENT_START_MOISTURE;      //Upkeep drains this
    public float storedMana = GV.ENVIROMENT_START_MANA;              //Production drains this
    public float storedNutrient = GV.ENVIROMENT_START_NUTRIENT;      //Upkeep and production drain this
    List<Plant> plants = new List<Plant>();
    
    public void TakeExplosionDamage(GV.MaterialType matType, float amount)
    {
        ConsumeManaEnergy(MaterialDict.Instance.GetDamageDistribution(matType, GV.DamageTypes.Energy) * amount);
        ConsumeManaEnergy(MaterialDict.Instance.GetDamageDistribution(matType, GV.DamageTypes.Mana) * amount);
        ConsumeNatureEnergy(MaterialDict.Instance.GetDamageDistribution(matType, GV.DamageTypes.Nature) * amount);
        ConsumeFireEnergy(MaterialDict.Instance.GetDamageDistribution(matType, GV.DamageTypes.Fire) * amount);
        ConsumeIceEnergy(MaterialDict.Instance.GetDamageDistribution(matType, GV.DamageTypes.Ice) * amount);
        ConsumeWaterEnergy(MaterialDict.Instance.GetDamageDistribution(matType, GV.DamageTypes.Water) * amount);
    }

    public void ConsumeNatureEnergy(float amount)
    {
        storedNutrient += amount;
    }

    public void ConsumeManaEnergy(float amount)
    {
        storedMana += amount;
    }

    public void ConsumeFireEnergy(float amount)
    {
        enviromentHeatStored += amount;
    }

    public void ConsumeIceEnergy(float amount)
    {
        enviromentHeatStored -= amount;
    }

    public void ConsumeWaterEnergy(float amount)
    {
        storedMoisture += amount;
    }

    public void GrowPlant(string plantName)
    {
        //plants
    }

    public float TakeNutrients(float amount)
    {
        if(amount > storedNutrient*GV.ENVIROMENT_TRANSFER_TO_PLANT_MAX_CAP)
        {
            storedNutrient -= storedNutrient * GV.ENVIROMENT_TRANSFER_TO_PLANT_MAX_CAP;
            return storedNutrient * GV.ENVIROMENT_TRANSFER_TO_PLANT_MAX_CAP;
        }
        return amount;
    }

    public float TakeMoisture(float amount)
    {
        if (amount > storedMoisture * GV.ENVIROMENT_TRANSFER_TO_PLANT_MAX_CAP)
        {
            storedMoisture -= storedMoisture * GV.ENVIROMENT_TRANSFER_TO_PLANT_MAX_CAP;
            return storedMoisture * GV.ENVIROMENT_TRANSFER_TO_PLANT_MAX_CAP;
        }
        return amount;
    }
    public float TakeMana(float amount)
    {
        if (amount > storedMana * GV.ENVIROMENT_TRANSFER_TO_PLANT_MAX_CAP)
        {
            storedMana -= storedMana * GV.ENVIROMENT_TRANSFER_TO_PLANT_MAX_CAP;
            return storedMana * GV.ENVIROMENT_TRANSFER_TO_PLANT_MAX_CAP;
        }
        return amount;
    }

    #region UpdateEnviroment
    public void UpdateEnviroment(float timePassed)
    {
        TurnFireEnergyIntoDesiredTemp();
        ProgressCurrentTemp(timePassed);
        BalanceDesiredHeat(timePassed);
        GiveNeighborExcessNutrient(timePassed);
        GiveNeighborsExcessMana(timePassed);
        GiveNeighborsExcessMoisture(timePassed);
    }

    private void TurnFireEnergyIntoDesiredTemp()
    {
        if (enviromentHeatStored != 0)
        {
            desiredTemp += enviromentHeatStored * GV.ENVIROMENT_HEAT_STORED_TO_TEMP_INCREASE;
            //If consume alot of heat, should increase risk of fire
            enviromentHeatStored = 0;
        }
    }

    private void ProgressCurrentTemp(float timePassed)
    {
        currentTemp += ((desiredTemp - currentTemp) / GV.ENVIROMENT_TIME_TO_DESIRED_HEAT) * timePassed;
    }

    private void BalanceDesiredHeat(float timePassed)
    {
        desiredTemp += ((currentTemp - desiredTemp) / GV.ENVIROMENT_TIME_TO_CORRECT_DESIRED_HEAT) * timePassed;
        if (leftNeighbor)
            _BalanceNeighborDesiredHeat(timePassed, desiredTemp - leftNeighbor.desiredTemp, ref leftNeighbor);
        if(rightNeighbor)
            _BalanceNeighborDesiredHeat(timePassed, desiredTemp - rightNeighbor.desiredTemp, ref rightNeighbor);
    }

    private void GiveNeighborExcessNutrient(float timepassed)
    {
        if (leftNeighbor)
            _BalanceNeighborNutrient(timepassed, storedNutrient - leftNeighbor.storedNutrient, ref leftNeighbor);
        if (rightNeighbor)
            _BalanceNeighborNutrient(timepassed, storedNutrient - rightNeighbor.storedNutrient, ref rightNeighbor);
    }
    
    private void GiveNeighborsExcessMoisture(float timepassed)
    {
        if (leftNeighbor)
            _BalanceNeighborMoisture(timepassed, storedMoisture - leftNeighbor.storedMoisture, ref leftNeighbor);
        if (rightNeighbor)
            _BalanceNeighborMoisture(timepassed, storedMoisture - rightNeighbor.storedMoisture, ref rightNeighbor);
    }

    private void GiveNeighborsExcessMana(float timepassed)
    {
        if(leftNeighbor)
            _BalanceNeighborMana(timepassed,storedMana - leftNeighbor.storedMana,ref leftNeighbor);
        if(rightNeighbor)
            _BalanceNeighborMana(timepassed,storedMana - rightNeighbor.storedMana,ref rightNeighbor);
    }

    private void _BalanceNeighborMana(float timePassed, float difference, ref Biome neighbor)
    {
        if (difference > GV.ENVIROMENT_TRANSFER_MANA_NEIGHBORS * timePassed * 2)
        {
            neighbor.ConsumeManaEnergy(GV.ENVIROMENT_TRANSFER_MANA_NEIGHBORS * timePassed);
            storedMana -= GV.ENVIROMENT_TRANSFER_MANA_NEIGHBORS * timePassed;
        }
        else if (difference > GV.ENVIROMENT_TRANSFER_MANA_MIN_NEIGHBORS)
        {
            neighbor.ConsumeManaEnergy(difference / 2);
            storedMana -= difference / 2;
        }
    }

    private void _BalanceNeighborMoisture(float timePassed, float difference, ref Biome neighbor)
    {
        if (difference > GV.ENVIROMENT_TRANSFER_MOISTURE_NEIGHBORS * timePassed * 2)
        {
            neighbor.ConsumeWaterEnergy(GV.ENVIROMENT_TRANSFER_MOISTURE_NEIGHBORS * timePassed);
            storedMoisture -= GV.ENVIROMENT_TRANSFER_MOISTURE_NEIGHBORS * timePassed;
        }
        else if (difference > GV.ENVIROMENT_TRANSFER_MOISTURE_MIN_NEIGHBORS)
        {
            neighbor.ConsumeWaterEnergy(difference / 2);
            storedMoisture -= difference / 2;
        }
    }
    
    private void _BalanceNeighborNutrient(float timePassed, float difference, ref Biome neighbor)
    {
        if (difference > GV.ENVIROMENT_TRANSFER_NUTRIENT_NEIGHBORS * timePassed * 2)
        {
            neighbor.ConsumeNatureEnergy(GV.ENVIROMENT_TRANSFER_NUTRIENT_NEIGHBORS * timePassed);
            storedNutrient -= GV.ENVIROMENT_TRANSFER_NUTRIENT_NEIGHBORS * timePassed;
        }
        else if (difference > GV.ENVIROMENT_TRANSFER_NUTRIENT_MIN_NEIGHBORS)
        {
            neighbor.ConsumeNatureEnergy(difference / 2);
            storedNutrient -= difference / 2;
        }
    }

    private void _BalanceNeighborDesiredHeat(float timePassed, float difference, ref Biome neighbor)
    {
        if (difference > GV.ENVIROMENT_TRANSFER_HEAT_NEIGHBORS * timePassed * 2)
        {
            neighbor.desiredTemp += GV.ENVIROMENT_TRANSFER_HEAT_NEIGHBORS * timePassed;
            desiredTemp -= GV.ENVIROMENT_TRANSFER_HEAT_NEIGHBORS * timePassed;
        }
        else if (difference > GV.ENVIROMENT_TRANSFER_HEAT_MIN_NEIGHBORS)
        {
            neighbor.desiredTemp += difference / 2;
            desiredTemp -= difference / 2;
        }
    }
    
    //in function that pulls from enviroment to the tree, include a limiter based on percentage of max stored.
#endregion
 }
