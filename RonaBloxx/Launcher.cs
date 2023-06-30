using System.Collections.Generic;

namespace RonaBloxx
{
    public class LauncherArgs
    {
        public string GameInfo, PlaceLauncherUrl, RobloxLocale, GameLocale;
        public ulong LaunchTime, TrackerId;
    }

    class Launcher
    {
        public static LauncherArgs ParseArgs(string input)
        {
            LauncherArgs output = new LauncherArgs();

            Dictionary<string, string> gameArgs = new Dictionary<string, string>();

            string[] pairs = input.Split('+');
            foreach (string pair in pairs)
            {
                string[] parts = pair.Split(':');
                string name = parts[0];
                string value = parts[1];

                gameArgs[name] = value;
            }

            output.GameInfo = gameArgs["gameinfo"];
            output.LaunchTime = ulong.Parse(gameArgs["launchtime"]);
            output.PlaceLauncherUrl = gameArgs["placelauncherurl"];
            output.TrackerId = ulong.Parse(gameArgs["browsertrackerid"]);
            output.RobloxLocale = gameArgs["robloxLocale"];
            output.GameLocale = gameArgs["gameLocale"];

            return output;
        }
    }
}
