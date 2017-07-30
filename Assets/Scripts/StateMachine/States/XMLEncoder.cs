using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;

public class XMLEncoder {

    public static List<Dictionary<string, string>> XmlToDictionaryList(string fileName, GV.fileLocationType dictionaryType)
    {
        string filePath = GetFilePathByType(dictionaryType);
        bool firstElement = true;
        if (!File.Exists(filePath + "/" + fileName + ".xml"))
            return null;
        List<Dictionary<string, string>> toReturn = new List<Dictionary<string, string>>();
        Dictionary<string,string> currentlyFilling = new Dictionary<string,string>();

        using (XmlReader reader = XmlReader.Create(filePath + "/" + fileName + ".xml"))
        {
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "element":
                            if(firstElement)
                            {
                                firstElement = false;
                            } else {
                                toReturn.Add(currentlyFilling);
                                currentlyFilling = new Dictionary<string,string>();
                            }                            
                            break;
                        case "StartElement": //ignore
                            break;
                        default:
                            string key = reader.Name;
                             if (reader.Read())
                             {
                               string value = reader.Value.Trim();
                               currentlyFilling.Add(key,value);
                             }
                            break;
                    }
                }
            }
        }
        //when done, the last element is still instead of currentlyFilling
        toReturn.Add(currentlyFilling);
        return toReturn;
    }

    public void TransitionToXML(Transition toEncode)
    {


    }

    //No longer used, uses DictionaryListToXML, delete if dont need in future
    public static void DictionaryToXML<T>(string fileName, Dictionary<string, T> toXML, GV.fileLocationType dictionaryType)
    {
        if (!Directory.Exists(GetFilePathByType(dictionaryType)))
            Directory.CreateDirectory(GetFilePathByType(dictionaryType));

        if(File.Exists(GetFilePathByType(dictionaryType) + "/" + fileName + ".xml"))
            File.Delete(GetFilePathByType(dictionaryType) + "/" + fileName + ".xml");
        
        using (XmlWriter writer = XmlWriter.Create(GetFilePathByType(dictionaryType) + "/" + fileName + ".xml"))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("StartElement");
            writer.WriteStartElement("element");
            foreach (KeyValuePair<string, T> kv in toXML)
            {
                writer.WriteElementString(kv.Key,kv.Value.ToString());
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
    }

    public static void DictionaryToXML<T>(string fileName, Dictionary<string, List<T>> toXML, GV.fileLocationType dictionaryType)
    {
        if (File.Exists(GetFilePathByType(dictionaryType) + "/" + fileName + ".xml"))
            File.Delete(GetFilePathByType(dictionaryType) + "/" + fileName + ".xml");

        using (XmlWriter writer = XmlWriter.Create(GetFilePathByType(dictionaryType) + "/" + fileName + ".xml"))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("StartElement");
            writer.WriteStartElement("element");
            foreach (KeyValuePair<string, List<T>> kv in toXML)
            {
                string totalString = "";
                foreach (T t in kv.Value)
                    totalString += t + ",";
                totalString = totalString.TrimEnd(',');
                writer.WriteElementString(kv.Key, totalString);
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }
    }

    public static void DictionaryListToXML<T>(string fileName, List<Dictionary<string, T>> dictList, GV.fileLocationType dictionaryType)
    {
        if (File.Exists(GetFilePathByType(dictionaryType) + "/" + fileName + ".xml"))
            File.Delete(GetFilePathByType(dictionaryType) + "/" + fileName + ".xml");

        using (XmlWriter writer = XmlWriter.Create(GetFilePathByType(dictionaryType) + "/" + fileName + ".xml"))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("StartElement");
            foreach (Dictionary<string, T> dict in dictList)
            {
                writer.WriteStartElement("element");
                foreach (KeyValuePair<string, T> kv in dict)
                {
                    writer.WriteStartElement(kv.Key);
                    writer.WriteString(kv.Value.ToString());
                    writer.WriteFullEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

    }


   

    public List<Dictionary<string,string>> StateAndTransitionsToDictionaryList(List<StateCompactGUI> stateList, List<TransitionGUI> transitionList)
    {
        List<Dictionary<string, string>> toReturn = new List<Dictionary<string, string>>();
        foreach (StateCompactGUI s in stateList)
        {
            List<Dictionary<string, string>> stateWithStatetStructs = s.ExportForXML();
            toReturn.AddRange(stateWithStatetStructs);
        }
        foreach (TransitionGUI t in transitionList)
        {
            List<Dictionary<string, string>> transWithSlotStructs = t.ExportForXML();
            toReturn.AddRange(transWithSlotStructs);            
        }
        return toReturn;
    }

    public static string GetFilePathByType(GV.fileLocationType fileLoc)
    {
        switch(fileLoc)
        {
            case GV.fileLocationType.Spells:
                return "Assets/XMLs/Spells";
            case GV.fileLocationType.Characters:
                return "Assets/XMLs/Levels";
            case GV.fileLocationType.Trees:
                return "Assets/XMLs/Nature";
            case GV.fileLocationType.Xml:
                return "Assets/XMLs";
            case GV.fileLocationType.NPCs:
                return "Assets/XMLs/NPCs";
            case GV.fileLocationType.TagManagers:
                return "Assets/XMLs/TagManagers";
            case GV.fileLocationType.BasicSpells:
                return "Assets/XMLs/BasicSpell";
            default:
                Debug.LogError("unhandled file type location, please add case");
                return "";
        }

    }

    private void SlotStructToXML(TransSlotStruct ss)
    {
        /*
         * enum SlotType{boolType,floatType,stringType,operatorType};
    enum OperatorType { lessThan = 0, greaterThan = 1, equalTo = 2};
    bool bValue;
    float fValue;
    string strValue;
    string slotName;
    OperatorType oValue;
         * */

    }
    
    public void LevelDictionaryToXML(LevelTracker levelTracker)
    {
        if (File.Exists(GetFilePathByType(GV.fileLocationType.Characters) + "/" + levelTracker.avatarName + ".xml"))
            File.Delete(GetFilePathByType(GV.fileLocationType.Characters) + "/" + levelTracker.avatarName + ".xml");

        using (XmlWriter writer = XmlWriter.Create(GetFilePathByType(GV.fileLocationType.Characters) + "/" + levelTracker.avatarName + ".xml"))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("StartElement");

            while (levelTracker.levelUpStack.Count > 0)
            {
                LevelPackage pkgToWrite = levelTracker.levelUpStack.Dequeue();
                writer.WriteStartElement("element");
                WriteFullElementString(writer,"PkgType", pkgToWrite.levelPkgType.ToString());
                WriteFullElementString(writer,"PkgName", pkgToWrite.pkgName);
                WriteFullElementString(writer,"ActivationKey", pkgToWrite.activationKey);
                WriteFullElementString(writer,"ExpGiven", GV.EXPERIENCE_PER_LEVEL.ToString());
                writer.WriteEndElement();
            }
            /*
            while (levelTracker.oldLevelUpStack.Count > 0)
            {
                writer.WriteStartElement("element");
                string popped = levelTracker.oldLevelUpStack.Pop();
                writer.WriteElementString("Level",popped);
                if (!GV.STAT_ALL_NAMES.Contains(popped))
                {
                    writer.WriteElementString("KeyCode", levelTracker.oldLevelUpStack.Pop());
                }
                writer.WriteEndElement();
            }*/
            writer.WriteStartElement("element");
            writer.WriteElementString("LoopAmt", levelTracker.loopAmt.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("EndElement");
            writer.WriteEndDocument();
        }
    }

    public LevelTracker XMLToLevelTracker(string avatarName, GV.fileLocationType fileLoc)
    {
        LevelTracker levelTracker = new LevelTracker();
        levelTracker.avatarName = avatarName;
        Stack<string> upsidedownStack = new Stack<string>();
        string filePath = GetFilePathByType(fileLoc);
        if (!File.Exists(filePath + "/" + avatarName + ".xml"))
        {
            Debug.Log("XmlToLvlTracker failed to produce lvl tracker, missing xml: " + filePath + "/" + avatarName + ".xml");
            return null;
        }

        List<Dictionary<string, string>> levelXmlAsDict = XmlToDictionaryList(avatarName, fileLoc);
        foreach (Dictionary<string, string> dict in levelXmlAsDict)
        {
            if (dict.ContainsKey("LoopAmt"))
            {
                levelTracker.loopAmt = int.Parse(dict["LoopAmt"]);
            }
            else
            {
               LevelPackage lvlPkg = new LevelPackage(GV.ParseEnum<GV.LevelPkgType>(dict["PkgType"]), dict["PkgName"], dict["ActivationKey"], float.Parse(dict["ExpGiven"]));
               levelTracker.AddNewLevel(lvlPkg);
            }
        }
        return levelTracker;
    }

    public static List<string> GetAllXMLFileNames(GV.fileLocationType fileLoc)
    {
        List<string> toReturn = new List<string>();
        foreach (string s in GetAllXMLFilePaths(fileLoc))
            toReturn.Add(Path.GetFileNameWithoutExtension(s));
        return toReturn;
    }

    public static string[] GetAllFoldersInPath(GV.fileLocationType fileLoc)
    {
        return Directory.GetDirectories(Path.GetFileNameWithoutExtension(GetFilePathByType(fileLoc)));
    }

    public static string[] GetAllXMLFilePaths(GV.fileLocationType fileLoc)
    {
       return Directory.GetFiles(GetFilePathByType(fileLoc), "*.xml");
    }

    public static List<string> GetAllFilesInPath(string path)
    {
        List<string> toRet = new List<string>();

        string[] fullPaths = Directory.GetFiles(path);
        foreach (string s in fullPaths)
        {
            if(Path.GetExtension(s) != ".meta")
                toRet.Add(Path.GetFileNameWithoutExtension(s));
        }
        return toRet;
    }


    public static List<string> LoadAllCharacterNames()
    {
        string[] characterNames = Directory.GetFiles(XMLEncoder.GetFilePathByType(GV.fileLocationType.Characters));
        List<string> toRet = new List<string>();
        foreach (string filename in characterNames)
        {
            if (Path.GetExtension(filename) == ".xml")
                toRet.Add(Path.GetFileNameWithoutExtension(filename));
        }
        return toRet;
    }

    public static void WriteFullElementString(XmlWriter writer,
                                          string localName,
                                          string value)
    {
        writer.WriteStartElement(localName);
        writer.WriteString(value);
        writer.WriteFullEndElement();
    }


    #region TagManager
    public static Dictionary<string, List<string>> XMLToTagManagerDict(GV.fileLocationType tagManagerType)
    {
        //allow it to return an empty one
        Dictionary<string, string> rawDict = XmlToDictionaryList(tagManagerType + "TagManager", GV.fileLocationType.TagManagers)[0];
        Dictionary<string, List<string>> toRet = new Dictionary<string, List<string>>();
        foreach (KeyValuePair<string, string> kv in rawDict)
        {
            List<string> tagList = new List<string>();
            string[] splitstring = kv.Value.Split(',');
            foreach (string s in splitstring)
                if (s != "" && !tagList.Contains(s))
                    tagList.Add(s);
            toRet.Add(kv.Key, tagList);
        }
        return toRet;
    }

    public static void TagManagerDictToXML(Dictionary<string, List<string>> dict, GV.fileLocationType tagManagerType)
    {
        XMLEncoder.DictionaryToXML<string>(tagManagerType + "TagManager", dict, GV.fileLocationType.TagManagers);
    }
    #endregion

    public static string OutputDictTListT<T>(Dictionary<T, List<T>> dict)
    {
        string toOut = "";
        foreach (KeyValuePair<T, List<T>> kv in dict)
        {
            toOut += kv.Key.ToString() + "{";
            foreach (T t in kv.Value)
            {
                toOut += t.ToString() + ",";
            }
            toOut.TrimEnd(',');
            toOut += "}, ";
        }
        toOut.TrimEnd(',');
        return toOut;
    }
}
