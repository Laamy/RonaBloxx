using System;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Diagnostics;
using System.Web;

class Program
{
    public static LauncherArgs la;
    public static MDIInIFile config = new MDIInIFile();

    public static bool CheckAdminPerms()
    {
        // use WindowsPrincipal to check if the program has been launched in administrator or not
        return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }

    static async Task MainLoop(string[] args)
    {
        // arguments meaning its probably roblox trying 2 launch
        Task.Factory.StartNew(() => Application.Run(new LauncherWindow()));

        // parse launcher arguments
        la = Launcher.ParseArgs(args[0]);

        string placeId = HttpUtility.UrlDecode(la.PlaceLauncherUrl).Split('&')[2].Split('=')[1];

        CheckGetUniverse(placeId);

        LauncherWindow.phase++;

        // store roblox client version for version comparing stuff
        RobloxProcess.version = await RobloxClient.GetRobloxVersionAsync();

        // get roblox install directory
        string robloxPath = await RobloxClient.GetInstallPathAsync();
        string robloxLatestPath = robloxPath + "\\" + RobloxProcess.version;

        if (!Directory.Exists(robloxLatestPath))
        {
            // latest roblox not installed
            LauncherWindow.handle.Hide();

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

        LauncherWindow.phase++;

        // start robloxplayerbeta with the parsed arguments & a copy of the unparsed ones (new or smth idK)
        StartRoblox(robloxLatestPath, args);

        // check & set roblox fps cap to unlimited
        RobloxClient.SetFPSAsync(robloxLatestPath, 999);

        // multi instance cuz im a fucking gay fuck
        RobloxClient.InitMutexAsync();

        LauncherWindow.phase++;

        // pause execution
        Thread.Sleep(-1);
    }

    private static async Task CheckGetUniverse(string placeId)
    {
        RobloxProcess.universe = await RobloxClient.GetMainUniverseAsync(placeId);

        LauncherWindow.phase++;
    }

    private static async Task StartRoblox(string robloxLatestPath, string[] args)
    {
        RobloxProcess.roblox = Process.Start(robloxLatestPath + "\\RobloxPlayerBeta.exe",
        $"--app " +
        $"-t {la.GameInfo} " +
        $"-j {HttpUtility.UrlDecode(la.PlaceLauncherUrl)} " +
        $"-b {la.TrackerId} " +
        $"--launchtime={la.LaunchTime} " +
        $"--rloc {la.RobloxLocale} " +
        $"--gloc {la.GameLocale} " +
        args[0]);
        LauncherWindow.phase++;
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
            MainLoop(args).GetAwaiter().GetResult();

            Console.ReadKey();
        }
    }
}