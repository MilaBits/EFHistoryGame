#pragma warning disable 0168
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SmartwallPackage.Utils
{
    public class DevSwitchToInitialization : MonoBehaviour
    {
        private void Awake()
        {
            try { MainThreadDispatcher.Instance.Ping(); }
            catch(Exception ex)
            {
                SceneManager.LoadScene(0);
                Debug.LogWarning("DevSwitchToInitialization | Start | No gamemaster detected, did you start from an init scene?");
            }
        }
    }
}
