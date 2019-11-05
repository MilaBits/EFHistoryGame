using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SmartwallPackage.Language
{
    /// <summary>
    /// Attach this class to an item with a Image class from UnityEngine.UI to connect it to the langauge management system.
    /// Input the ID of the Image in the list of Images, if you don't link an Image to change via the inspector it will 
    /// search for an Image class on its gameobject, if non is found it will post a warning in console.
    /// 
    /// In order for the image swapping to work reliably you should assign one of the options for the image to the UI Image.
    /// </summary>
    public class L_Image : MonoBehaviour
    {
        public int IdOfImage;
        public Image ImageToChange;
    
        void Start()
        {
            //check if a Image has been linked
            if (ImageToChange == null)
            {
                ImageToChange = gameObject.GetComponent<Image>();
                if (ImageToChange == null) //Try to find an Image class
                {
                    Debug.LogWarning("L_Image | Start | Image changer has no image to change and can't find one on its gameobject: " + gameObject.name);
                    return;
                }
                else
                {
                    Debug.LogWarning("L_Image | Start | Image changer has no image to change but it has found a Image class on its gameobject: " + gameObject.name);
                }
            }
            //register for language change
            LanguageController.LanguageChangedEvent += GetImagePathFromLanguageControllerAndChangeImage;
            if (!LanguageController.LanguageLoaded.Equals(string.Empty))
            {
                GetImagePathFromLanguageControllerAndChangeImage();
            }
        }

        public void GetImagePathFromLanguageControllerAndChangeImage()
        {
            string newImgPath = LanguageController.GetImage(IdOfImage);
            if (newImgPath.Equals(string.Empty))
            {
                Debug.LogWarning("L_Image | GetSpriteFromLanguageControllerAndPlaceItInImage | Atempted to load a Image entry that is not availible in the language file, is the file up to date?");
            }
            else
            {
                StartCoroutine(LoadAndApplyTexture(newImgPath)); //WebRequest is made to function withing a coroutine and is dodgy outside of it
            }
        }
        private IEnumerator LoadAndApplyTexture(string newImgPath)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture("file:///" + Application.dataPath + Path.DirectorySeparatorChar + newImgPath))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError)
                {
                    Debug.LogError("L_Image | GetSpriteFromLanguageControllerAndPlaceItInImage | Loading file failed: " + www.error);
                }
                else
                {
                    Texture2D tex = DownloadHandlerTexture.GetContent(www);
                    ImageToChange.sprite = Sprite.Create(tex, ImageToChange.sprite.rect, ImageToChange.sprite.pivot, ImageToChange.sprite.pixelsPerUnit);
                }
            }
        }
    }
}
