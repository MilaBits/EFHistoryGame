using UnityEngine;

namespace SmartwallPackage.ScoreScreen
{
    public class ScoreSoundsManager : MonoBehaviour
    {
        private static ScoreSoundsManager _Instance;
        public AudioSource WinSound;
        public AudioSource ScoreSound;

        private void Awake()
        {
            _Instance = this;
        }

        public static void PlayWinSound()
        {
            if (_Instance != null)
            {
                _Instance._PlayWinSound();
            }
            else
            {
                Debug.LogError("ScoreSoundsManager | PlayWinSound | Missing instance of ScoreSoundsManager.");
            }
        }

        public static void PlayScoreSound()
        {
            if (_Instance != null)
            {
                _Instance._PlayScoreSound();
            }
            else
            {
                Debug.LogError("ScoreSoundsManager | PlayScoreSound | Missing instance of ScoreSoundsManager.");
            }
        }

        private void _PlayWinSound()
        {
            if (WinSound != null)
            {
                if (!WinSound.isPlaying)
                {
                    WinSound.Play();
                }
            }
            else
            {
                Debug.LogError("ScoreSoundsManager | PlayWinSound | Missing link to audio source.");
            }
        }

        private void _PlayScoreSound()
        {
            if (ScoreSound != null)
            {
                if (!ScoreSound.isPlaying)
                {
                    ScoreSound.Play();
                }
            }
            else
            {
                Debug.LogError("ScoreSoundsManager | PlayScoreSound | Missing link to audio source.");
            }
        }
    }
}
