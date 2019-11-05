using System.Collections.Generic;
using UnityEngine;

namespace SmartwallPackage.Utils
{
    /// <summary>
    /// Container class for the colours linked to players. Using this will ensure 
    /// that the score screen and ingame score trackers use tha same colours for a player.
    /// </summary>
    public class PlayerColourContainer : MonoBehaviour
    {
        private static PlayerColourContainer _Instance;

        public Color HighscoreColour;
        public List<Color> PlayerColours = new List<Color>();

        private void Awake()
        {
            if (_Instance == null)
            {
                _Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.LogWarning("PlayerColourContainer | Awake | A second PlayerColourContainer was initialized, please make sure that there will be only one in a scene.");
                Destroy(this);
            }
        }

        /// <summary>
        /// Retrive the colour set for a player starting at 1. 0 = colour of last highscore.
        /// </summary>
        public static Color GetPlayerColour(int playerNumber)
        {
            if(playerNumber < 0 || playerNumber > _Instance.PlayerColours.Count)
            {
                Debug.LogError("PlayerColourContainer | GetPlayerColour | Player number is out of bounds: " + playerNumber.ToString() + ". Value clamped!");
                Mathf.Clamp(playerNumber, 0, _Instance.PlayerColours.Count);
            }
            if(playerNumber == 0)
            {
                return _Instance.HighscoreColour;
            }
            return _Instance.PlayerColours[playerNumber - 1];
        }
    }
}
