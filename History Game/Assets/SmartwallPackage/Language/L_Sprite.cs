using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace SmartwallPackage.Language
{
    /// <summary>
    /// Attach this class to an item with a SpriteRendere class to connect it to the langauge management system.
    /// Input the ID of the Image in the list of Images, if you don't link a SpriteRenderer to change via the 
    /// inspector it will search for a SpriteRenderer class on its gameobject, if non is found it will post a 
    /// warning in console.
    /// 
    /// In order for the image swapping to work reliably you should assign one of the options for the image to the renderer.
    /// </summary>
    public class L_Sprite : MonoBehaviour
    {
        public int IdOfSprite;
        public SpriteRenderer SpriteToChange;

        void Start()
        {
            //check if a SpriteRenderer has been linked
            if (SpriteToChange == null)
            {
                SpriteToChange = gameObject.GetComponent<SpriteRenderer>();
                if (SpriteToChange == null) //Try to find an SpriteRenderer class
                {
                    Debug.LogWarning("L_Sprite | Start | Sprite changer has no sprite to change and can't find one on its gameobject: " + gameObject.name);
                    return;
                }
                else
                {
                    Debug.LogWarning("L_Sprite | Start | Sprite changer has no sprite to change but it has found a SpriteRenderer class on its gameobject: " + gameObject.name);
                }
            }
            //register for language change
            LanguageController.LanguageChangedEvent += GetImagePathFromLanguageControllerAndChangeSprite;
            if (!LanguageController.LanguageLoaded.Equals(string.Empty))
            {
                GetImagePathFromLanguageControllerAndChangeSprite();
            }
        }

        void GetImagePathFromLanguageControllerAndChangeSprite()
        {
            string newImgPath = LanguageController.GetImage(IdOfSprite);
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
                    SpriteToChange.sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), tex.width / SpriteToChange.sprite.texture.width * SpriteToChange.sprite.pixelsPerUnit);
                }
            }
        }
    }
}
