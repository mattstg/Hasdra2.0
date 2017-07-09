using UnityEngine;
using System.Collections;
using System.Linq;


public class XMLToPlant : MonoBehaviour {

    public string[] GetAllTreeList()
    {
        return System.IO.Directory.GetFiles(XMLEncoder.GetFilePathByType(GV.fileLocationType.Trees),"*.xml");
    }

}
