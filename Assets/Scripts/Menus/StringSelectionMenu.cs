using UnityEngine;
using System.Collections;

public class StringSelectionMenu {

    public delegate void DelegateFunc(string arg1);
	//Creates a menu system the loads a list of strings, or a file path, allowing you to chose. Can include folder structures as well
    public StringSelectionMenu(GV.fileLocationType fileLocTop,  DelegateFunc theFunc, bool allowFolders)
    {
        /*string loc = XMLEncoder.GetFilePathByType(fileLocTop);
        XMLEncoder xmlencoder = new XMLEncoder();
        List<string> toRet = new List<string>();

        foreach (string filenameAndExt in Directory.GetFiles(XMLEncoder.GetFilePathByType(GV.fileLocationType.Spells)))
        {

            if (Path.GetExtension(filenameAndExt) == ".xml")
            {
                string filename = Path.GetFileNameWithoutExtension(filenameAndExt);
                toRet.Add(filename);
            }
        }
        return toRet;*/
    }
}
