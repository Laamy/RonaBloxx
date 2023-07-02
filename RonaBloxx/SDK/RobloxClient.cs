using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Configuration;
using System.Web.Script.Serialization;

public class RobloxClient
{
    public static Mutex robloxMutex;

    public static void InitMutex()
    {
        robloxMutex = new Mutex(true, "ROBLOX_singletonMutex");
    }

    public static void UninitMutex()
    {
        if (robloxMutex != null)
        {
            robloxMutex.Close();
            robloxMutex.Dispose();
        }
    }

    public static void ExitApp() // this is called everytime we want to exit the application
    {
        UninitMutex();
        Process.GetCurrentProcess().Kill();
    }

    public static WebClient wc = new WebClient();

    public static void UpdateRoblox()
    {
        MDIDirectory.CheckCreate("Tmp");
        wc.DownloadFile("https://setup.rbxcdn.com/" + RobloxProcess.version + "-Roblox.exe",
            MDI.mdiBase + "\\Tmp\\RobloxPlayerLauncher.exe");
        Process.Start(MDI.mdiBase + "\\Tmp\\RobloxPlayerLauncher.exe");
    }

    // https://github.com/L8X/Roblox-Client-Optimizer/tree/main stole settings from these guys
    public static void SetFPS(string robloxPath, int fps)
    {
        JavaScriptSerializer jss = new JavaScriptSerializer();

        Directory.CreateDirectory(robloxPath + "\\ClientSettings");

        // roblox application settings
        ClientAppSettings settings = new ClientAppSettings();
        settings.DFIntTaskSchedulerTargetFps = fps;

        string json = jss.Serialize(settings);
        File.WriteAllText(robloxPath + "\\ClientSettings\\ClientAppSettings.json", json);
    }

    // fixed GetMainUniverse function
    public static RobloxUniverse GetMainUniverse(string placeId)
    {
        JavaScriptSerializer jss = new JavaScriptSerializer();

        // scrap html for universe id
        string result = wc.DownloadString($"https://www.roblox.com/games/{placeId}");
        string universeId = Regex.Match(result, @"universe-id=""(.*?)"">").Groups[1].Value;

        // get universe root as json then deserialize into C# object
        string json = wc.DownloadString("https://games.roblox.com/v1/games?universeIds=" + universeId);

        return jss.Deserialize<RobloxUniverse>(json);
    }

    public static string GetInstallPath()
    {
        string output = "";

        string robloxFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Roblox\\Versions";
        string robloxPFPath = "C:\\Program Files (x86)\\Roblox\\Versions";


        if (Directory.Exists(robloxFolder))
            output = robloxFolder;

        if (Directory.Exists(robloxPFPath))
            output = robloxPFPath;

        return output;
    }
}