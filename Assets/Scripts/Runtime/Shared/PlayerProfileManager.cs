using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    public static class PlayerProfileManager
    {
        private static int m_TotalWins = 0;
        private static int m_TotalKills = 0;
        private static int m_TotalDeaths = 0;
        private static float m_TotalPlaytime = 0f;

        public static int TotalWins => m_TotalWins;
        public static int TotalKills => m_TotalKills;
        public static int TotalDeaths => m_TotalDeaths;
        public static float TotalPlaytime => m_TotalPlaytime;

        public static void UpdateStats(bool won, int kills, int deaths)
        {
            if (won) m_TotalWins++;
            m_TotalKills += kills;
            m_TotalDeaths += deaths;
            Debug.Log($"[PlayerProfile] Updated: {m_TotalWins}W, {m_TotalKills}K, {m_TotalDeaths}D");
        }

        public static void AddPlaytime(float seconds)
        {
            m_TotalPlaytime += seconds;
        }

        public static void ResetStats()
        {
            m_TotalWins = 0;
            m_TotalKills = 0;
            m_TotalDeaths = 0;
            m_TotalPlaytime = 0f;
        }

        public static void LoadGame()
        {
            // TODO: Implement save game loading from PlayerPrefs or persistent storage
            Debug.Log("[PlayerProfile] Loading game...");
        }

        public static bool HasSaveGame()
        {
            // TODO: Check if save game exists in persistent storage
            return false; // Placeholder: return false until save system is implemented
        }

        public static PlayerProfile GetCurrentProfile()
        {
            // TODO: Return actual player profile from persistence layer
            return new PlayerProfile
            {
                PlayerName = "Player",
                Wins = m_TotalWins,
                Kills = m_TotalKills,
                FriendsList = new System.Collections.Generic.List<string>()
            };
        }
    }

    /// <summary>
    /// Represents a player's profile data
    /// </summary>
    public class PlayerProfile
    {
        public string PlayerName { get; set; }
        public int Wins { get; set; }
        public int Kills { get; set; }
        public int Losses { get; set; }
        public System.Collections.Generic.List<string> FriendsList { get; set; } = new System.Collections.Generic.List<string>();
    }
}
