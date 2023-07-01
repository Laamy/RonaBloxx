using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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

    public static void SetFPS(string robloxPath, int fps)
    {
        if (Directory.Exists(robloxPath + "\\ClientSettings"))
        {
            // rewrite 2 file with new settings if it exists else creates it
            File.WriteAllText(robloxPath + "\\ClientSettings\\ClientAppSettings.json", $"{{\"DFIntTaskSchedulerTargetFps\":{fps}}}");
            
            return;
        }

        // config doesnt exist yet so we make our own
        Directory.CreateDirectory(robloxPath + "\\ClientSettings");
        File.WriteAllText(robloxPath + "\\ClientSettings\\ClientAppSettings.json", $"{{\"DFIntTaskSchedulerTargetFps\":{fps}}}");
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