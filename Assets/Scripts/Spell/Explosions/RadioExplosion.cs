using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RadioExplosion : Explosion {

    public override void InitializeExplosion(SpellInfo si, float _energy, List<SkillModifier> _storedSkillModifiers)
    {
        density = GV.EXPLOSION_RADIO_DENSITY;
        base.InitializeExplosion(si, _energy, _storedSkillModifiers);
    }

    public override void CreateAfterExplosion()
    {
    }
}
