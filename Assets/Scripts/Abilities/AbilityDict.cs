using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityDict  {

    ///<summary></summary></summary>
    #region Singleton
    private static AbilityDict instance;


    public static AbilityDict Instance
    {
        get{
            if (instance == null)
            {
                instance = new AbilityDict();
            }
            return instance;
        }
    }
#endregion



    private AbilityDict()
    {

    }

    public Ability GetAbilityByType(GV.AbilityType abilityType)
    {
        switch (abilityType)
        {
		    case GV.AbilityType.dash:
            	return new AvAbDash();
		    case GV.AbilityType.jump:
		    	return new AvAbJump();
            case GV.AbilityType.superJump:
                return new AvAbSuperJump();
            case GV.AbilityType.backDash:
		    	return new AvAbBackDash();
		    case GV.AbilityType.climb:
		    	return new AvAbClimb();
            case GV.AbilityType.zoom:
                    return new AvAbZoom();
            case GV.AbilityType.moveLeft:
                    return new AvAbMoveLeft();
            case GV.AbilityType.moveRight:
                    return new AvAbMoveRight();
            case GV.AbilityType.tradeIn:
                    return new AvAbTradeIn();
            case GV.AbilityType.aimUp:
                    return new AvAbAimUp();
            case GV.AbilityType.aimDown:
                    return new AvAbAimDown();
            case GV.AbilityType.fly:
                    return new AvAbFly();
            case GV.AbilityType.invis:
                return new AvAbInvis();
            case GV.AbilityType.block:
                return new AvAbBlock();
            default:
                Debug.LogError("Ability type: " + abilityType.ToString() + " not found");
                return new AvAbDash();
        }
    }

    public List<string> GetAllAbilityNames()
    {
        List<string> toRet = new List<string>();
        foreach (GV.AbilityType enumValue in System.Enum.GetValues(typeof(GV.AbilityType)))
        {
            toRet.Add(enumValue.ToString());
        }
        return toRet;
    }
}
