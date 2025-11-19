using System.IO;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    /// <summary>
    /// Manages player profile data including stats, friends list, and save games
    /// Handles local storage and will integrate with backend services in future
    /// </summary>
    public static class PlayerProfileManager
    {
        private static PlayerProfile s_CurrentProfile;
        private const string PROFILE_FILE_NAME = "PlayerProfile.json";
        private const string SAVE_GAME_FILE_NAME = "SaveGame.dat";
        
        static PlayerProfileManager()
        {
            LoadProfile();
        }
        
        /// <summary>
        /// Get the current player's profile, creating default if none exists
        /// </summary>
        public static PlayerProfile GetCurrentProfile()
        {
            if (s_CurrentProfile == null)
            {
                LoadProfile();
            }
            return s_CurrentProfile;
        }
        
        /// <summary>
        /// Save the current profile to disk
        /// </summary>
        public static void SaveProfile()
        {
            if (s_CurrentProfile == null)
            {
                Debug.LogWarning("No profile to save");
                return;
            }
            
            string profilePath = Path.Combine(Application.persistentDataPath, PROFILE_FILE_NAME);
            string json = JsonUtility.ToJson(s_CurrentProfile, true);
            File.WriteAllText(profilePath, json);
            Debug.Log($"Profile saved to {profilePath}");
        }
        
        /// <summary>
        /// Load profile from disk or create default
        /// </summary>
        public static void LoadProfile()
        {
            string profilePath = Path.Combine(Application.persistentDataPath, PROFILE_FILE_NAME);
            
            if (File.Exists(profilePath))
            {
                string json = File.ReadAllText(profilePath);
                s_CurrentProfile = JsonUtility.FromJson<PlayerProfile>(json);
                Debug.Log($"Profile loaded from {profilePath}");
            }
            else
            {
                s_CurrentProfile = CreateDefaultProfile();
                SaveProfile();
            }
        }
        
        /// <summary>
        /// Create a default profile with sample data
        /// </summary>
        private static PlayerProfile CreateDefaultProfile()
        {
            var profile = new PlayerProfile("NewPlayer");
            
            // Add sample friends for demonstration
            profile.FriendsList.Add("AI_Player_1");
            profile.FriendsList.Add("AI_Player_2");
            profile.FriendsList.Add("TestUser");
            
            return profile;
        }
        
        /// <summary>
        /// Update player stats after a match
        /// </summary>
        public static void UpdateStats(bool won, int kills, int deaths = 0)
        {
            var profile = GetCurrentProfile();
            
            if (won)
                profile.Wins++;
            
            profile.Kills += kills;
            profile.Deaths += deaths;
            profile.GamesPlayed++;
            profile.LastPlayed = System.DateTime.Now;
            
            SaveProfile();
        }
        
        /// <summary>
        /// Add a friend to the player's friends list
        /// </summary>
        public static void AddFriend(string friendName)
        {
            var profile = GetCurrentProfile();
            
            if (!profile.FriendsList.Contains(friendName))
            {
                profile.FriendsList.Add(friendName);
                SaveProfile();
            }
        }
        
        /// <summary>
        /// Remove a friend from the player's friends list
        /// </summary>
        public static void RemoveFriend(string friendName)
        {
            var profile = GetCurrentProfile();
            
            if (profile.FriendsList.Remove(friendName))
            {
                SaveProfile();
            }
        }
        
        /// <summary>
        /// Check if a save game exists
        /// </summary>
        public static bool HasSaveGame()
        {
            string savePath = Path.Combine(Application.persistentDataPath, SAVE_GAME_FILE_NAME);
            return File.Exists(savePath);
        }
        
        /// <summary>
        /// TODO: Implement save game functionality
        /// This will save current game state for Continue feature
        /// </summary>
        public static void SaveGame(object gameState)
        {
            // Placeholder for future implementation
            Debug.Log("SaveGame - Not yet implemented");
        }
        
        /// <summary>
        /// TODO: Implement load game functionality
        /// This will load saved game state for Continue feature
        /// </summary>
        public static object LoadGame()
        {
            // Placeholder for future implementation
            Debug.Log("LoadGame - Not yet implemented");
            return null;
        }
    }
}
