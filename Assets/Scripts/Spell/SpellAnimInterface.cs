using UnityEngine;
using System.Collections;

public interface SpellAnimInterface {
    void InitializeSpellAnimation(SpellInfo spell, Transform animationTransform);
    bool UpdateInterface();
}
