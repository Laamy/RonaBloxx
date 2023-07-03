using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Script.Serialization;

public class RobloxClient
{
    public static Mutex robloxMutex;

    public static void InitMutexAsync()
    {
        robloxMutex = new Mutex(true, "ROBLOX_singletonMutex");
    }

    public static void UninitMutexAsync()
    {
        if (robloxMutex != null)
        {
            robloxMutex.Close();
            robloxMutex.Dispose();
        }
    }

    public static void ExitApp() // this is called everytime we want to exit the application
    {
        UninitMutexAsync();
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
    public static async Task SetFPSAsync(string robloxPath, int fps)
    {
        JavaScriptSerializer jss = new JavaScriptSerializer();

        Directory.CreateDirectory(robloxPath + "\\ClientSettings");

        // roblox application settings
        ClientAppSettings settings = new ClientAppSettings();
        settings.DFIntTaskSchedulerTargetFps = fps;

        string json = jss.Serialize(settings);
        File.WriteAllText(robloxPath + "\\ClientSettings\\ClientAppSettings.json", json);
    }

    public static async Task ReplaceRobloxAsync(string proc = null)
    {
        if (proc == null)
            proc = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName;

        RegistryKey key = Registry.ClassesRoot.OpenSubKey("roblox-player\\shell\\open\\command", true);
        key.SetValue(string.Empty, "\"" + proc + "\" %1");
        key.Close();
    }

    // fixed GetMainUniverse function
    public static async Task<RobloxUniverse> GetMainUniverseAsync(string placeId)
    {
        string universeId;
        using (WebClient wc = new WebClient())
        {
            // scrap html for universe id
            string result = wc.DownloadString($"https://www.roblox.com/games/{placeId}");
            universeId = Regex.Match(result, @"universe-id=""(.*?)"">").Groups[1].Value;
            LauncherWindow.phase++;
        }

        using (WebClient wc = new WebClient())
        {
            // get universe root as json then deserialize into C# object
            string json = wc.DownloadString("https://games.roblox.com/v1/games?universeIds=" + universeId);

            // convert to C# object
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Deserialize<RobloxUniverse>(json);
        }
            
    }

    public static async Task<string> GetInstallPathAsync()
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