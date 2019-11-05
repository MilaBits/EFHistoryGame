using UnityEngine;
using UnityEngine.UI;

namespace SmartwallPackage.Language
{
    /// <summary>
    /// Attach this class to an item with a Text class from UnityEngine.UI to connect it to the langauge management system.
    /// Input the ID of the text in the list of text entries, if you don't link a Text to change via the inspector it will 
    /// search for a Text class on its gameobject, if non is found it will post a warning in console.
    /// If you are dealing with a TextMesh instead of a Unity.UI.Text use the L_TextMesh instead.
    /// </summary>
    public class L_Text : MonoBehaviour
    {
        public int IdOfTextEntry;
        public Text LabelToChangeTextOf;

        private void Start()
        {
            //Check if a Text class has been linked
            if (LabelToChangeTextOf == null)
            {
                LabelToChangeTextOf = gameObject.GetComponent<Text>(); //Try to find a Text class
                if (LabelToChangeTextOf == null)
                {
                    Debug.LogWarning("L_Text | Start | Text changer has no label to change and can't find one on its gameobject: " + gameObject.name);
                    return;
                }
                else
                {
                    Debug.LogWarning("L_Text | Start | Text changer has no label to change but it has found a Text class on its gameobject: " + gameObject.name);
                }
            }
            //register for language change
            LanguageController.LanguageChangedEvent += GetTextFromLanguageControllerAndPlaceItOnLabel;
            if (!LanguageController.LanguageLoaded.Equals(string.Empty))
            {
                GetTextFromLanguageControllerAndPlaceItOnLabel();
            }
        }

        private void GetTextFromLanguageControllerAndPlaceItOnLabel()
        {
            string newText = LanguageController.GetText(IdOfTextEntry);
            if (newText.Equals(string.Empty))
            {
                newText = "Entry is missing from language file!";
                Debug.LogError("L_Text | GetTextFromLanguageControllerAndPlaceItOnLabel | Atempted to load a text entry that is not availible in the language file, is the file up to date?");
            }
            LabelToChangeTextOf.text = newText;
        }
    }
}
