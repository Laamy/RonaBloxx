using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System;
using System.Reflection;
using System.Diagnostics;

partial class InstallerWindow : Form
{
    bool hasToRepair = false;
    public InstallerWindow(bool reinstall = false)
    {
        hasToRepair = reinstall;
        InitializeComponent();
    }

    private void InstallApp(object sender, EventArgs e) // this one is instant so it doesnt really matter
    {
        progressBar1.Value = 0;

        RobloxClient.ReplaceRobloxAsync();
        RobloxClient.CloneRonaBloxxToRoblox();
        Program.config.Write("RequiresReinstall", "0", "System"); // reset reinstall

        progressBar1.Value = 100;
    }

    private void RepairApp(object sender, EventArgs e)
    {
        progressBar1.Value = 0;

        Program.config.Write("RequiresReinstall", "1", "System"); // force reinstall

        string robloxPath = RobloxClient.GetInstallPathAsync().GetAwaiter().GetResult();

        if (!Directory.Exists($"{robloxPath}\\RonaBloxx"))
        {
            MessageBox.Show("Failed to find ronabloxx");
            return;
        }

        RobloxProcess.version = RobloxClient.GetRobloxVersionAsync().GetAwaiter().GetResult();

        RobloxClient.ReplaceRobloxAsync($"{robloxPath}\\{RobloxProcess.version}\\RobloxPlayerLauncher.exe").GetAwaiter();

        DirectoryInfo[] dicks = new DirectoryInfo(robloxPath).GetDirectories();
        float increaseBy = 100 / dicks.Length;

        foreach (DirectoryInfo folder in dicks) // this is so lazy
        {
            folder.Delete(true);
            progressBar1.Value += (int)increaseBy;
        }

        RobloxClient.UpdateRoblox();

        progressBar1.Value = 100;
    }

    private void UninstallApp(object sender, EventArgs e) // this one is instant so it doesnt really matter
    {
        progressBar1.Value = 0;

        Program.config.Write("RequiresReinstall", "1", "System");

        string robloxPath = RobloxClient.GetInstallPathAsync().GetAwaiter().GetResult();

        if (!Directory.Exists($"{robloxPath}\\RonaBloxx"))
        {
            MessageBox.Show("Failed to find ronabloxx");
            return;
        }

        RobloxProcess.version = RobloxClient.GetRobloxVersionAsync().GetAwaiter().GetResult();

        RobloxClient.ReplaceRobloxAsync($"{robloxPath}\\{RobloxProcess.version}\\RobloxPlayerLauncher.exe").GetAwaiter();

        Directory.Delete($"{robloxPath}\\RonaBloxx", true);

        if (File.Exists($"{robloxPath}\\{RobloxProcess.version}\\ClientSettings"))
        {
            Directory.Delete($"{robloxPath}\\{RobloxProcess.version}\\ClientSettings", true);
        }

        progressBar1.Value = 100;

        //string batchFile = Path.Combine(Path.GetTempPath(), "delete.bat");
        //string deleteCommand = $"ping 127.0.0.1 -n 2 > nul & del \"{Assembly.GetExecutingAssembly().Location}\"";
        //File.WriteAllText(batchFile, deleteCommand);
        //Process.Start(new ProcessStartInfo(batchFile) { WindowStyle = ProcessWindowStyle.Hidden });
        //RobloxClient.ExitApp();
    }

    private void InstallerWindow_FormClosing(object sender, FormClosingEventArgs e)
        => RobloxClient.ExitApp();

    private void InstallerWindow_Load(object sender, EventArgs e)
    {
        if (hasToRepair)
        {
            RobloxClient.UpdateRoblox();

            // restart the application with administrator
            string fileName = Assembly.GetExecutingAssembly().Location;
            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.FileName = fileName;
            procInfo.Verb = "runas";

            try
            {
                Process.Start(procInfo);
                RobloxClient.ExitApp();
            }
            catch
            {
                // CANCELLED
                MessageBox.Show("Failed to do setup", "RonaBloxx");
                RobloxClient.ExitApp();
            }
        }

        if (!Program.CheckAdminPerms())
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;

            Text += " (Requires Administrator)";
        }
    }
}