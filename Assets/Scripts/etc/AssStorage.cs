using UnityEngine;
using System.Collections;

public class AssStorage : BalanceFormula {
	float control;
	float min;
	float max;
	float percent;
    GV.HorzAsym horzAsymType;

    public AssStorage(float inC, float inMin,float inMax,float inPer, GV.HorzAsym inH){
		control = inC;
		min = inMin;
		max = inMax;
		percent = inPer;
		horzAsymType = inH;
	}

	public AssStorage(float inC, float inMin,float inMax, GV.HorzAsym inH){
		percent = -1;
		control = inC;
		min = inMin;
		max = inMax;
		horzAsymType = inH;
	}

	public AssStorage(AssStorage copyThis){
		control = copyThis.control;
		min = copyThis.min;
		max = copyThis.max;
		percent = copyThis.percent;
		horzAsymType = copyThis.horzAsymType;
	}

	public float ret(float lvl){
		if(percent == -1)
			return GV.MikesAsymptote(lvl,control,min,max,horzAsymType);
		return GV.MikesAsymptote(lvl,control,min,max,percent,horzAsymType);
	}

    public BalanceFormula CopyThis()
    {
        return new AssStorage(this);
    }
}
