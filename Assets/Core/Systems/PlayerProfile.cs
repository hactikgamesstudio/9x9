using System;
using System.Collections.Generic;

namespace Unity.Template.Multiplayer.NGO.Core.Systems
{
    /// <summary>
    /// Player profile data structure containing stats and social information
    /// </summary>
    [Serializable]
    public class PlayerProfile
    {
        public string PlayerName = "Player";
        public int Wins = 0;
        public int Kills = 0;
        public List<string> FriendsList = new List<string>();
        
        // Additional fields for future expansion
        public int Deaths = 0;
        public int GamesPlayed = 0;
        public DateTime LastPlayed = DateTime.Now;
        
        public PlayerProfile(string playerName)
        {
            PlayerName = playerName;
        }
        
        public PlayerProfile()
        {
            // Default constructor for serialization
        }
    }
}
