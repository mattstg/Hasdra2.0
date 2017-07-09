using UnityEngine;
using System.Collections;



public class Concussion{
    //const
    
    //Basic concussion, take concuss dmg = temp concuss

    public BodyStats bs;
    //private float _concusionScore = 0;
    public float curConcuss = 0;//{get { return _concusionScore;}}
    float concussTimer;
    //public AssStorage concussMax = new AssStorage(30, 100, 500, 50,GV.HorzAsym.MinToMax); //Move to body stats as concussionMax

    public bool isConcussed = false;
    bool recoveringFromConcuss = false;          //currently recovering from a concuss, will recover at 0
    public bool resistingFurtherConcuss = false; //reached max concuss, immune to future concuss
    
    float concussRecievedTimer;
    public bool animationRecoverComplete = true;
    public float maxConcussBars = GV.CONCUSS_START_COUNT;
    
    public Concussion(BodyStats _bs)
    {
        bs = _bs;
    }

    public void UpdateConcusionScore()
    {
        if (!animationRecoverComplete)  //recovering from concuss, getting up
            return;

        //float concussRecoverStat = bs.getSkillValue("concusionRecoverRate");
        float dt = Time.deltaTime;
        /*curConcuss -= concussRecoverStat * dt;
        curConcuss = Mathf.Max(curConcuss, 0);
        curConcuss = Mathf.Min(curConcuss, bs.getSkillValue("concussMax"));*/

        concussRecievedTimer -= dt;

        if (concussRecievedTimer <= 0)
        { 
            curConcuss -= (bs.getSkillValue("concussMax") / GV.CONCUSS_CLEANSE_STACK_TIME) * dt;
            curConcuss = Mathf.Max(0, curConcuss);
            //Debug.Log(string.Format("recovering {0},  {1}/{2}", (bs.getSkillValue("concussMax") / GV.CONCUSS_CLEANSE_STACK_TIME), curConcuss, bs.getSkillValue("concussMax"))); 
            if (isConcussed && curConcuss <= 0)
            {   //then recover concuss
                //Debug.Log("recovered");
                bs.pcs.Concusion(false);
                resistingFurtherConcuss = true; //resist further until animation complete
                animationRecoverComplete = recoveringFromConcuss = isConcussed = false;
            }
            concussRecievedTimer = 0;
        }
    }

    public void RecoverAnimationComplete()
    {
        resistingFurtherConcuss = false;
        curConcuss = 0;
        animationRecoverComplete = true;
        bs.pcs.ToggleInputMap(true);
    }

    public void RecieveDamage(float dmgAmt)
    {
        //already took dmg at this point, so 0 hp is feasible
        //1 hp dmg = 1 concuss, if 0 hp, full concuss recieved
        float totalConcussRecieved = dmgAmt * bs.getSkillValue("resistConcusion");
        if (!resistingFurtherConcuss)
        {
            //Debug.Log(string.Format("Recieved raw {0} concuss, resistance: {1}", concussForceRecieved, bs.getSkillValue("resistConcusion")));
            curConcuss += totalConcussRecieved; // *bs.getSkillValue("resistConcusion");

            if (curConcuss >= bs.getSkillValue("concussMax") && !isConcussed)
            {
                isConcussed = true;
                bs.pcs.Concusion(true);
                //Debug.Log("concussed");
            }

            concussRecievedTimer = (isConcussed) ? 0 : GV.CONCUSS_TIME_RESET;

            if (curConcuss >= maxConcussBars * bs.getSkillValue("concussMax") || bs.hp == 0)
            {
                curConcuss = maxConcussBars * bs.getSkillValue("concussMax");
                resistingFurtherConcuss = true;
                //Debug.Log("resisting further concuss");
            }

            StaticReferences.numericTextManager.CreateNumericDisplay(bs.pcs.gameObject, bs.pcs.transform, "concussTaken", "", curConcuss, Color.magenta, true);
        }
    }

    public void ModMaxConcussBars(int i)
    {
        maxConcussBars = Mathf.Min(maxConcussBars + i, GV.CONCUSS_MAX_COUNT);
    }

    /*public void RecieveConcussion(float concussForceRecieved)
    {
        if (!resistingFurtherConcuss)
        {
            //Debug.Log(string.Format("Recieved raw {0} concuss, resistance: {1}", concussForceRecieved, bs.getSkillValue("resistConcusion")));
            curConcuss += concussForceRecieved; // *bs.getSkillValue("resistConcusion");

            if (curConcuss >= bs.getSkillValue("concussMax") && !isConcussed)
            {
                isConcussed = true;
                bs.pcs.Concusion(true);
                //Debug.Log("concussed");
            }

            concussRecievedTimer = (isConcussed)?0:GV.CONCUSS_TIME_RESET;

            if (curConcuss >= GV.CONCUSS_MAX_COUNT * bs.getSkillValue("concussMax"))
            {
                curConcuss = GV.CONCUSS_MAX_COUNT * bs.getSkillValue("concussMax");
                resistingFurtherConcuss = true;
                //Debug.Log("resisting further concuss");
            }

            StaticReferences.numericTextManager.CreateNumericDisplay(bs.pcs.gameObject, bs.pcs.transform, "concussTaken", "", curConcuss, Color.magenta, true);
        }
        
    }*/
}
