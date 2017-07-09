using UnityEngine;
using System.Collections;

public interface DestructibleInterface {
    float GetEnergy();
    float GetDensityEffect();
    void AlphaCountTooLow();
    void PixelCountModified(int pxlAmt);
    SpellInfo GetSpellInfo();
}
