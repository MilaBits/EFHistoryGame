using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmartwallPackage.Utils
{
    public class MainThreadDispatcher : MonoBehaviour
    {
        private static MainThreadDispatcher _Instance;
        /// <summary>
        /// Get the instance of the dispatcher, throws a generic exception is no instance exsists.
        /// </summary>
        public static MainThreadDispatcher Instance { get { if (_Instance != null) { return _Instance; } else { throw new Exception("MainThreadDispatcher | Instance | Atempted use of MainThreaDispatcher with no instance present in scene, please make sure a MainThreadDispatcher is availible."); } } }

        private static Queue<Action> actionQueue = new Queue<Action>();

        void Awake()
        {
            if(_Instance == null)
            {
                _Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.LogWarning("MainThreadDispatcher | Awake | A second MainThreadDispatcher was initialized, please make sure that there will be only one in a scene.");
                Destroy(this);
            }
        }
    
        //run all enqueed actions for this frame
        void Update()
        {
            lock (actionQueue)
            {
                while(actionQueue.Count > 0)
                {
                    Action a = actionQueue.Dequeue();
                    a.Invoke();
                }
            }
        }

        /// <summary>
        /// Actions passed to this method will be executed on the next frame on the main thread.
        /// </summary>
        /// <param name="action">Action to be executed on the main thread.</param>
        public void RunOnMainThread(Action action)
        {
            lock (actionQueue)
            {
                actionQueue.Enqueue(action);
            }
        }

        /// <summary>
        /// Method exsists soley to see if an instance exists, an exception is thrown if it does not.
        /// </summary>
        public void Ping() { }
    }
}
