using UnityEngine;
using System.Collections;

public interface TemperatureSensitive {

    void ChangeTemperature(float heatAmount, float maxHeat);
    float EnergyToHeat();
    bool pastMinThreshold();
    
}
