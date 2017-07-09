using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarPhysicalBody : MonoBehaviour
{
    //Hardcoded editor installation
    public Transform llarm;
    public Transform luarm;
    public Transform rlarm;
    public Transform ruarm;
    public Transform llleg; 
    public Transform luleg;
    public Transform rlleg;
    public Transform ruleg;
    public Transform ltorso;
    public Transform utorso;
    public Transform head;
    public Transform rfoot;
    public Transform lfoot;
    public Transform rhand;
    public Transform lhand;


    //public Dictionary<string, AvatarLimb> avatarLimbDict = new Dictionary<string, AvatarLimb>();
    public Dictionary<string, Transform> avatarLimbDict = new Dictionary<string, Transform>();

    public void Initialize()
    {
        avatarLimbDict.Add("leftlowleg" , llleg);
        avatarLimbDict.Add("leftuppleg" , luleg);
        avatarLimbDict.Add("rightlowleg", rlleg);
        avatarLimbDict.Add("rightuppleg", ruleg);
        avatarLimbDict.Add("leftupparm" , luarm);
        avatarLimbDict.Add("leftlowarm" , llarm);
        avatarLimbDict.Add("rightupparm", ruarm);
        avatarLimbDict.Add("rightlowarm", rlarm);
        avatarLimbDict.Add("head", head);
        avatarLimbDict.Add("ltorso", ltorso);
        avatarLimbDict.Add("utorso", utorso);
        avatarLimbDict.Add("rfoot", rfoot);
        avatarLimbDict.Add("rhand", rhand);
        avatarLimbDict.Add("lfoot", lfoot);
        avatarLimbDict.Add("lhand", lhand);
    }
}
