using UnityEngine;
using UnityEngine.UI;

namespace SmartwallPackage.ScoreScreen
{
    public class ScoreBar : MonoBehaviour
    {
        public Image Bar;
        public GameObject Crown;
        public GameObject NewHighscore;
        public Text T_Score;

        private int _Score;
        private float _BarFillPerc;
        private float _TimeToRaise;
        private bool _Started = false;
        private bool _IsWinner = false;
        private bool _HasHighscore = false;
    
        public void SetNewBarColour(Color colour)
        {
            Bar.color = colour;
        }

        public void Begin(int score, float barFillPerc, float timeToRaiseAllInSeconds, bool hasHighscore, bool isWinner)
        {
            Begin(score, barFillPerc, timeToRaiseAllInSeconds, hasHighscore, isWinner, 0f);
        }
        public void Begin(int score, float barFillPerc, float timeToRaiseAllInSeconds, bool hasHighscore, bool isWinner, float delay)
        {
            _IsWinner     = isWinner;
            _HasHighscore = hasHighscore;
            _Score        = score;
            _BarFillPerc  = barFillPerc;
            _TimeToRaise  = timeToRaiseAllInSeconds;
            if (delay == 0)
            {
                RaiseBar();
            }
            else
            {
                Invoke("RaiseBar", delay);
            }
        }
        private void RaiseBar()
        {
            _Started = true;
            ScoreSoundsManager.PlayScoreSound();
        }

        // Update is called once per frame
        void Update()
        {
            if (_Started)
            {
                if(Bar.fillAmount < _BarFillPerc)
                {
                    Bar.fillAmount += (Time.deltaTime / _TimeToRaise);
                    if (Bar.fillAmount >= _BarFillPerc)
                    {
                        Bar.fillAmount = _BarFillPerc;
                        T_Score.text   = "<b>" + _Score.ToString() + "</b>";
                        Invoke("ShowTopIcons", 0.4f);
                    }
                }
            }
        }

        private void ShowTopIcons()
        {
            if (_IsWinner)
            {
                Crown.SetActive(true);
                ScoreSoundsManager.PlayWinSound();
            }
            if (_HasHighscore)
            {
                NewHighscore.SetActive(true);
            }
        }
    }
}
