using System.Collections;
using System.Collections.Generic;
using SmartwallPackage.SmartwallInput;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SmartwallPackage.Utils.StartScreen
{
    public class Countdown : MonoBehaviour, I_SmartwallInteractable
    {
        public List<GameObject> Numbers = new List<GameObject>();
        public AudioSource Numberbeep;
        public AudioSource FinishBeep;
        public Image I_HitMe;
        bool started = false;
        public UnityEvent CountdownFinished = new UnityEvent();

        IEnumerator CountDown()
        {
            foreach (GameObject go in Numbers)
            {
                go.SetActive(true);
                go.GetComponent<Animation>().Play();
                Numberbeep.Play();
                yield return new WaitForSeconds(1);
                go.SetActive(false);
            }
            FinishBeep.Play();
            CountdownFinished.Invoke();
            gameObject.GetComponent<Image>().enabled         = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }

        public void Hit(Vector3 hitPosition)
        {
            if (!started)
            {
                I_HitMe.enabled = false;
                StartCoroutine(CountDown());
                started = true;
            }
        }
    }
}
