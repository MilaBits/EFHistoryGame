using SmartwallPackage.SmartwallInput;
using UnityEngine;
using UnityEngine.UI;

namespace SmartwallPackage.Utils
{
    /// <summary>
    /// Put this class on a GUI button to have it Click event fire on smartwall input.
    /// It will play will also look for a animation controller and a audiosource on the object to trigger when it is pressed.
    /// </summary>
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Collider2D))]
    public class SWButton : MonoBehaviour, I_SmartwallInteractable
    {
        Button _ButtonImOn;
        Animator _Anime;
        AudioSource _AS;
        public string AnimationTriggerName = "Clicked";

        private void Awake()
        {
            _ButtonImOn = gameObject.GetComponent<Button>();
            _Anime      = gameObject.GetComponent<Animator>();
            _AS         = gameObject.GetComponent<AudioSource>();
        }
    
        public void Hit(Vector3 location)
        {
            _AS?.Play();
            _Anime?.SetTrigger(AnimationTriggerName);
            _ButtonImOn.onClick.Invoke();
        }
    }
}
