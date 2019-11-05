using System.Collections;
using System.Collections.Generic;
using System.IO;
using SmartwallPackage.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SmartwallPackage.ScoreScreen
{
    public class HighscoreContainer
    {
        public int Highscore;
        public HighscoreContainer(int score)
        {
            Highscore = score;
        }
        public HighscoreContainer() { }
    }

    /// <summary>
    /// This Class manages the score display scene. It is Important that the scene is still called "Scores".
    /// You may wish to switch the replay button arrow, it is under C_UIRoot > P_Replay > BT_Replay
    /// </summary>
    public class ScoreScreenController : MonoBehaviour
    {
        private static List<int> Scores = new List<int>();
        /// <summary>
        /// Current highscore, publicly availible incase you want to use it for something.
        /// </summary>
        public static int Highscore { get { return _Highscore; } }
        private static int _Highscore = 0;

        public static int IndexOfSceneToMoveTo = 1;
        [HideInInspector]
        public float BarRiseAnimationTime = 0.7f;
        public GameObject P_Scoring;
        public GameObject ReplayButton;
        public GameObject ScoreBarBase;
    
        /// <summary>
        /// Moves to the scores scene to display the final scores and declare a winner and/or new highscore.
        /// Please set in the scene if you wish to use the continue or replay arrow on the button. The sceneIndex
        /// parameter is for determining what scen to move to after the scores have been shown.
        /// </summary>
        /// <param name="sceneIndex">Scene to move to from score scene, defaults to one.</param>
        public static void MoveToScores(List<int> scores, int sceneIndex = 1)
        {
            if (scores == null)
            {
                Debug.LogError("ScoreScreenController | MoveToScores | No scores have been stored in the scores list!");
            }
            else if (scores.Count == 0)
            {
                Debug.LogError("ScoreScreenController | MoveToScores | No scores have been stored in the scores list!");
            }
            IndexOfSceneToMoveTo = sceneIndex;
            Scores               = scores;
            SceneManager.LoadScene("Scores");
        }

        void Start()
        {
            //load highscore from file
            if(GlobalGameSettings.GlobalGameSettings.GetSetting("Reset Highscore").Equals("No"))
            {
                LoadHighscore();
            }
            else if(GlobalGameSettings.GlobalGameSettings.GetSetting("Reset Highscore").Equals(string.Empty))
            {
                if (GlobalGameSettings.GlobalGameSettings.GetSetting("Reset HS").Equals("No"))
                {
                    LoadHighscore();
                }
            }

            //check if we have all requirements linked
            if(ScoreBarBase == null) { Debug.LogError("ScoreScreenController | Start | Missing base object for score bars."); }
            if(P_Scoring == null) { Debug.LogError("ScoreScreenController | Start | Missing Link to perant panel."); }
            if(ReplayButton == null) { Debug.LogError("ScoreScreenController | Start | Missing Link to replay button."); }

            if (Scores == null)
            {
                Debug.LogError("ScoreScreenController | Start | No scores have been stored in the static Scores list!");
            }
            else
            {
                int numberOf0Scores = 0;
                int highestScore    = 0;
                foreach (int score in Scores)
                {
                    if (score == 0) { numberOf0Scores++; }
                    if (score > highestScore) { highestScore = score; }
                }
                if (Scores.Count == 0)
                {
                    Debug.LogError("ScoreScreenController | Start | No scores have been stored in the static Scores list!");
                    return;
                }
                else if(Scores.Count - numberOf0Scores == 1)
                {
                    SetupSinglePlayer(Scores.IndexOf(highestScore));
                }
                else if (Scores.Count - numberOf0Scores > 1)
                {
                    SetupMultiPlayer(highestScore);
                }
                if(highestScore > Highscore)
                {
                    _Highscore = highestScore;
                    SaveHighscore();
                }
            }
            Invoke("EnableReplay", BarRiseAnimationTime + 1f);
        }

        private void SetupSinglePlayer(int playerNr)
        {
            int highestScore;
            if(Scores[playerNr] > Highscore)
            {
                highestScore = Scores[playerNr];
            }
            else
            {
                highestScore = Highscore;
            }
            ScoreBar temp = Instantiate(ScoreBarBase, P_Scoring.transform).GetComponent<ScoreBar>();
            temp.SetNewBarColour(PlayerColourContainer.GetPlayerColour(playerNr+1));
            temp.Begin(Scores[playerNr], (float)Scores[playerNr] / (float)highestScore, BarRiseAnimationTime, Scores[playerNr] > Highscore, Scores[playerNr] > Highscore, 0.1f);

            temp = Instantiate(ScoreBarBase, P_Scoring.transform).GetComponent<ScoreBar>();
            temp.SetNewBarColour(PlayerColourContainer.GetPlayerColour(0));
            temp.Begin(Highscore, (float)Highscore / (float)highestScore, BarRiseAnimationTime, false, false, 0.1f);
        }

        private void SetupMultiPlayer(int highestScore)
        {
            for(int i = 0; i < Scores.Count; i++)
            {
                if (Scores[i] > 0)
                {
                    ScoreBar temp = Instantiate(ScoreBarBase, P_Scoring.transform).GetComponent<ScoreBar>();
                    temp.SetNewBarColour(PlayerColourContainer.GetPlayerColour(i+1));
                    temp.Begin(Scores[i], (float)Scores[i] / (float)highestScore, BarRiseAnimationTime, Scores[i] > Highscore && Scores[i] == highestScore, Scores[i] == highestScore, 0.1f);
                }
            }
        }

        private void SaveHighscore()
        {
            XML_to_Class.SaveClassToXML(new HighscoreContainer(Highscore), "StreamingAssets"+ Path.DirectorySeparatorChar + "HighScore");
        }

        private void LoadHighscore()
        {
            HighscoreContainer temp = XML_to_Class.LoadClassFromXML<HighscoreContainer>("StreamingAssets"+ Path.DirectorySeparatorChar +"HighScore");
            if(temp == null)
            {
                _Highscore = 0;
            }
            else
            {
                _Highscore = temp.Highscore;
            }
        }

        private void EnableReplay()
        {
            ReplayButton.SetActive(true);
        }

        public void BT_Replay_Clicked()
        {
            SceneManager.LoadScene(IndexOfSceneToMoveTo);
        }
    }
}