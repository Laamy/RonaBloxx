using Microsoft.Win32;

using System;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Diagnostics;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;

class Program
{
    public static LauncherArgs la;
    public static MDIInIFile config = new MDIInIFile();

    public static void ReplaceRoblox(string proc = null)
    {
        if (proc == null)
            proc = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName;

        RegistryKey key = Registry.ClassesRoot.OpenSubKey("roblox-player\\shell\\open\\command", true);
        key.SetValue(string.Empty, "\"" + proc + "\" %1");
        key.Close();
    }

    public static bool CheckAdminPerms()
    {
        // use WindowsPrincipal to check if the program has been launched in administrator or not
        return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }

    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            // opened normally so open installer
            Application.Run(new InstallerWindow());
        }
        else
        {
            // arguments meaning its probably roblox trying 2 launch
            Task.Factory.StartNew(() => Application.Run(new LauncherWindow()));

            // parse launcher arguments
            la = Launcher.ParseArgs(args[0]);

            // fixed using new roblox api (found via burp suite)
            JavaScriptSerializer jss = new JavaScriptSerializer();
            VersionRoot versionRoot = jss.Deserialize<VersionRoot>(RobloxClient.wc.DownloadString("https://clientsettingscdn.roblox.com/v2/client-version/WindowsPlayer"));

            string placeId = HttpUtility.UrlDecode(la.PlaceLauncherUrl).Split('&')[2].Split('=')[1];

            try
            {
                string result = RobloxClient.wc.DownloadString($"https://www.roblox.com/games/{placeId}");

                string name = Regex.Match(result, @"<title>(.*?)<\/title>").Groups[1].Value;

                // api was changed i have to now scrap the html for the name
                RobloxProcess.place = new PlaceRoot();
                RobloxProcess.place.name = name.Split('-')[0].Trim();
                RobloxProcess.place.placeId = long.Parse(placeId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // store roblox client version for version comparing stuff
            RobloxProcess.version = versionRoot.clientVersionUpload;

            // get roblox install directory
            string robloxPath = RobloxClient.GetInstallPath();
            string robloxLatestPath = robloxPath + "\\" + RobloxProcess.version;

            if (!Directory.Exists(robloxLatestPath))
            {
                // latest roblox not installed
                config.Write("RequiresReinstall", "1", "System");

                Task.Factory.StartNew(() => Application.Run(new InstallerWindow(true)));
                Thread.Sleep(-1);
            }
            else
            {
                //reinstall stuff
                if (File.Exists(MDI.mdiBase + "config.ini"))
                {
                    if (config.KeyExists("RequiresReinstall", "System")
                        && config.Read("RequiresReinstall", "System") != "0" && !CheckAdminPerms())
                    {
                        MessageBox.Show("Roblox cant start due to needing a reinstall", "BBRB");
                        RobloxClient.ExitApp();
                    }
                }
            }

            // multi instance cuz im a fucking gay fuck
            RobloxClient.InitMutex();

            // check & set roblox fps cap to unlimited
            RobloxClient.SetFPS(robloxLatestPath, 999);

            // start robloxplayerbeta with the parsed arguments & a copy of the unparsed ones (new or smth idK)
            Task.Factory.StartNew(() => {
                RobloxProcess.roblox = Process.Start(robloxLatestPath + "\\RobloxPlayerBeta.exe",
                $"--app " +
                $"-t {la.GameInfo} " +
                $"-j {HttpUtility.UrlDecode(la.PlaceLauncherUrl)} " +
                $"-b {la.TrackerId} " +
                $"--launchtime={la.LaunchTime} " +
                $"--rloc {la.RobloxLocale} " +
                $"--gloc {la.GameLocale} " +
                args[0]);
            });

            // pause execution
            Thread.Sleep(-1);
        }
    }
}