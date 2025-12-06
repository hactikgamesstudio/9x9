using System;
using Unity.Multiplayer;

namespace Unity.Template.Multiplayer.NGO.Runtime
{
    // Minimal command line parser supporting a few known args
    public class CommandLineArgumentsParser
    {
        public int ServerPort { get; } = -1;
        public string ServerIP { get; } = string.Empty;
        public int MaxPlayers { get; } = -1;
        public MultiplayerRoleFlags? Role { get; }

        public CommandLineArgumentsParser()
        {
            try
            {
                var args = Environment.GetCommandLineArgs();
                for (int i = 0; i < args.Length; i++)
                {
                    var a = args[i];
                    if (a.StartsWith("-port=") || a.StartsWith("--port="))
                    {
                        if (int.TryParse(a.Split('=')[1], out int p)) ServerPort = p;
                    }
                    else if ((a == "-port" || a == "--port") && i + 1 < args.Length)
                    {
                        if (int.TryParse(args[i + 1], out int p)) ServerPort = p;
                    }
                    else if (a.StartsWith("-ip=") || a.StartsWith("--ip="))
                    {
                        ServerIP = a.Split('=')[1];
                    }
                    else if ((a == "-ip" || a == "--ip") && i + 1 < args.Length)
                    {
                        ServerIP = args[i + 1];
                    }
                    else if (a.StartsWith("-maxPlayers=") || a.StartsWith("--maxPlayers="))
                    {
                        if (int.TryParse(a.Split('=')[1], out int m)) MaxPlayers = m;
                    }
                    else if (a == "-role" && i + 1 < args.Length)
                    {
                        Role = ParseRole(args[i + 1]);
                    }
                }
            }
            catch
            {
                // ignore parsing errors
            }
        }

        static MultiplayerRoleFlags? ParseRole(string v)
        {
            return v.ToLowerInvariant() switch
            {
                "client" => MultiplayerRoleFlags.Client,
                "server" => MultiplayerRoleFlags.Server,
                "host" => MultiplayerRoleFlags.ClientAndServer,
                _ => null
            };
        }
    }
}
