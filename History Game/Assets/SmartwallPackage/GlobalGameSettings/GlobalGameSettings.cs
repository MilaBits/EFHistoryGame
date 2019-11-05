using System.Collections.Generic;
using System.IO;
using SmartwallPackage.Utils;
using UnityEngine;

namespace SmartwallPackage.GlobalGameSettings
{
    [System.Serializable]
    public class GlobalGameSetting
    {
        public string Label;
        public string Value;
        public List<string> PossibleValues;
        public string Default;
    }

    public class GlobalGameSettings : MonoBehaviour
    {
        #region static
        private static List<GlobalGameSetting> _Settings;
        /// <summary>
        /// Get a setting by its name. Returns empty string if the setting was not found or if there is no settings file.
        /// </summary>
        public static string GetSetting(string nameOfSetting)
        {
            if (_Settings == null)
            {
                _Settings = XML_to_Class.LoadClassFromXML<List<GlobalGameSetting>>("StreamingAssets" + Path.DirectorySeparatorChar + "SavedData" + Path.DirectorySeparatorChar + "GlobalGameSettings.xml");
            }
            if (_Settings == null) //file was missing, use default.
            {
                Debug.LogWarning("GlobalGameSettings | GetSetting | Settings File is missing! Please create one. Creating default now.");
                _Instance.CreateSettingFile();
                _Settings = XML_to_Class.LoadClassFromXML<List<GlobalGameSetting>>("StreamingAssets" + Path.DirectorySeparatorChar + "SavedData" + Path.DirectorySeparatorChar + "GlobalGameSettings.xml");
            }
            if(_Settings == null)
            {
                Debug.LogError("GlobalGameSettings | GetSetting | Default settings file could not be made!");
            }
            else {
                GlobalGameSetting temp = FindSettingByName(nameOfSetting);
                if(temp != null)
                {
                    return temp.Value;
                }
                Debug.LogWarning("GlobalGameSettings | GetSetting | No setting by that name was found: " + nameOfSetting);
            }
            return string.Empty;
        }

        static private GlobalGameSetting FindSettingByName(string name)
        {
            foreach(GlobalGameSetting sett in _Settings)
            {
                if (sett.Label.ToLower().Equals(name.ToLower()))
                {
                    return sett;
                }
            }
            return null;
        }
        #endregion

        private static GlobalGameSettings _Instance;
        public List<GlobalGameSetting> SettingToMake = new List<GlobalGameSetting>();

        private void Awake()
        {
            _Instance = this;
        }

        public void CreateSettingFile()
        {
            if(XML_to_Class.SaveClassToXML(SettingToMake, "StreamingAssets" + Path.DirectorySeparatorChar + "SavedData" + Path.DirectorySeparatorChar + "GlobalGameSettings.xml"))
            {
                Debug.Log("Settings file created.");
            }
            else
            {
                Debug.LogError("Failed to make settings file.");
            }
        }
    }
}