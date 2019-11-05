using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace SmartwallPackage.Language
{
    /// <summary>
    /// Attach this class to an item with a AudioSource to connect it to the langauge management system.
    /// Input the ID of the clip in the list of clips, if you don't link a AudioSource to change via the inspector it will 
    /// search for a AudioSource on its gameobject, if non is found it will post a warning in console.
    /// </summary>
    public class L_Audio : MonoBehaviour
    {
        public int IdOfAudioFile;
        public AudioSource AudioToChange;

        void Start()
        {
            if (AudioToChange == null)
            {
                //check if a AudioSource has been linked
                AudioToChange = gameObject.GetComponent<AudioSource>();
                if (AudioToChange == null) //Try to find an Audiosource
                {
                    Debug.LogWarning("L_Audio | Start | Audio changer has no AudioSource to change and can't find one on its gameobject: " + gameObject.name);
                    return;
                }
                else
                {
                    Debug.LogWarning("L_Audio | Start | Audio changer has no AudioSource to change but it has found an AudioSource on its gameobject: " + gameObject.name);
                }
            }
            //register for language change
            LanguageController.LanguageChangedEvent += GetIAudioPathFromLanguageControllerAndChangeClip;
            if (!LanguageController.LanguageLoaded.Equals(string.Empty))
            {
                GetIAudioPathFromLanguageControllerAndChangeClip();
            }
        }

        public void GetIAudioPathFromLanguageControllerAndChangeClip()
        {
            string newAudioPath = LanguageController.GetAudio(IdOfAudioFile);
            if (newAudioPath.Equals(string.Empty))
            {
                Debug.LogWarning("L_Image | GetSpriteFromLanguageControllerAndPlaceItInImage | Atempted to load a Image entry that is not availible in the language file, is the file up to date?");
            }
            else 
            {
                StartCoroutine(LoadAndApplyAudio(newAudioPath));
            }
        }
        private IEnumerator LoadAndApplyAudio(string newAudioPath)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + Application.dataPath + Path.DirectorySeparatorChar + newAudioPath, AudioType.WAV))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError)
                {
                    Debug.LogError("L_Image | GetSpriteFromLanguageControllerAndPlaceItInImage | Loading file failed: " + www.error);
                }
                else
                {
                    AudioToChange.clip = DownloadHandlerAudioClip.GetContent(www);
                }
            }
        }
    }
}
