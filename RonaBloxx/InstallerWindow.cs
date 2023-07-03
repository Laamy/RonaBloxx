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
        Program.config.Write("RequiresReinstall", "0", "System"); // reset reinstall

        progressBar1.Value = 100;

        MessageBox.Show("Installed", "RonaBloxx Installer");
    }

    private void RepairApp(object sender, EventArgs e)
    {
        progressBar1.Value = 0;

        Program.config.Write("RequiresReinstall", "1", "System"); // force reinstall

        string robloxFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            + "\\Roblox\\Versions";
        string robloxPFPath = "C:\\Program Files (x86)\\Roblox\\Versions"; // some people have other folder so this fixes it ig

        WebClient wc = new WebClient();

        RobloxProcess.version = wc.DownloadString("https://setup.rbxcdn.com/version");

        string robloxPath = "";

        if (!Directory.Exists(robloxFolder + "\\" + RobloxProcess.version) && !Directory.Exists(robloxPFPath + "\\" + RobloxProcess.version))
        {
            Program.config.Write("RequiresReinstall", "1", "System");

            MessageBox.Show("Latest roblox version not detected (FATAL FAILURE)", "BBRB");
            return;
        }
        else
        {
            if (Directory.Exists(robloxFolder + "\\" + RobloxProcess.version))
                robloxPath = robloxFolder + "\\" + RobloxProcess.version;

            if (Directory.Exists(robloxPFPath + "\\" + RobloxProcess.version))
                robloxPath = robloxPFPath + "\\" + RobloxProcess.version;
        }

        List<string> folders = new List<string>();

        if (Directory.Exists(robloxFolder))
            folders.AddRange(Directory.GetDirectories(robloxFolder));

        if (Directory.Exists(robloxPFPath))
            folders.AddRange(Directory.GetDirectories(robloxPFPath));

        float increaseBy = 100 / robloxFolder.Length;

        foreach (string version in folders.ToArray()) // this is so lazy
        {
            Directory.Delete(version, true);
            progressBar1.Value += (int)increaseBy;
        }

        RobloxClient.UpdateRoblox(); // this is a massive flaw..

        progressBar1.Value = 100;

        MessageBox.Show("Reinstall roblox & better booga booga to finish repair", "RonaBloxx Installer");
    }

    private void UninstallApp(object sender, EventArgs e) // this one is instant so it doesnt really matter
    {
        progressBar1.Value = 0;

        var robloxVersions = Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Roblox\\Versions");

        Program.config.Write("RequiresReinstall", "1", "System");

        string robloxFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            + "\\Roblox\\Versions";
        string robloxPFPath = "C:\\Program Files (x86)\\Roblox\\Versions"; // some people have other folder so this fixes it ig

        WebClient wc = new WebClient();

        RobloxProcess.version = wc.DownloadString("https://setup.rbxcdn.com/version");

        string robloxPath = "";

        if (!Directory.Exists(robloxFolder + "\\" + RobloxProcess.version) && !Directory.Exists(robloxPFPath + "\\" + RobloxProcess.version))
        {
            Program.config.Write("RequiresReinstall", "1", "System");

            MessageBox.Show("Latest roblox version not detected (FATAL FAILURE)", "BBRB");
            return;
        }
        else
        {
            if (Directory.Exists(robloxFolder + "\\" + RobloxProcess.version))
                robloxPath = robloxFolder + "\\" + RobloxProcess.version;

            if (Directory.Exists(robloxPFPath + "\\" + RobloxProcess.version))
                robloxPath = robloxPFPath + "\\" + RobloxProcess.version;
        }
        RobloxClient.ReplaceRobloxAsync(robloxPath + "\\RobloxPlayerLauncher.exe");

        progressBar1.Value = 100;

        MessageBox.Show("Uninstalled", "RonaBloxx Installer");
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