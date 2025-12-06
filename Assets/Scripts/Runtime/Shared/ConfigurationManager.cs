using System;
using Unity.Multiplayer;
using UnityEngine;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    // Minimal configuration manager with defaults and simple accessors
    public class ConfigurationManager
    {
        // Keys used around the project
        public const string k_EnableBots = "enableBots";
        public const string k_Autoconnect = "autoconnect";
        public const string k_MaxPlayers = "maxPlayers";
        public const string k_Port = "port";
        public const string k_ServerIP = "serverIP";
        public const string k_DevConfigFile = "devconfig.json";
        public const string k_AllowReconnection = "allowReconnection";

        readonly CustomNetworkManager m_Owner;
        readonly Action<ConfigurationManager> m_OnLoaded;

        // Simple store (could be extended to load from file/UGS/args)
        int m_MaxPlayers = 2;
        int m_Port = 7777;
        string m_ServerIP = "127.0.0.1";
        bool m_EnableBots = false;
        bool m_Autoconnect = false;
        bool m_AllowReconnection = true;
        MultiplayerRoleFlags m_Role = MultiplayerRoleFlags.ClientAndServer;

        public ConfigurationManager(CustomNetworkManager owner, string devConfigPath, Action<ConfigurationManager> onLoaded)
        {
            m_Owner = owner;
            m_OnLoaded = onLoaded;
            try
            {
                // Light command line override support
                var parser = new CommandLineArgumentsParser();
                if (parser.ServerPort != -1) m_Port = parser.ServerPort;
                if (!string.IsNullOrEmpty(parser.ServerIP)) m_ServerIP = parser.ServerIP;
                if (parser.MaxPlayers > 0) m_MaxPlayers = parser.MaxPlayers;
                if (parser.Role.HasValue) m_Role = parser.Role.Value;

#if UNITY_SERVER || ENABLE_UCS_SERVER
                m_Role = MultiplayerRoleFlags.Server;
#endif
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Config] Failed to parse command line: {e.Message}");
            }
            finally
            {
                m_OnLoaded?.Invoke(this);
            }
        }

        public int GetInt(string key)
        {
            return key switch
            {
                var k when k == k_MaxPlayers => m_MaxPlayers,
                var k when k == k_Port => m_Port,
                _ => 0
            };
        }

        public bool GetBool(string key)
        {
            return key switch
            {
                var k when k == k_EnableBots => m_EnableBots,
                var k when k == k_Autoconnect => m_Autoconnect,
                var k when k == k_AllowReconnection => m_AllowReconnection,
                _ => false
            };
        }

        public string GetString(string key)
        {
            return key switch
            {
                var k when k == k_ServerIP => m_ServerIP,
                _ => string.Empty
            };
        }

        public MultiplayerRoleFlags GetMultiplayerRole() => m_Role;
    }
}
