﻿using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using System;
using System.Threading.Tasks;
using System.Threading;

public partial class LauncherWindow : Form
{
    public static LauncherWindow handle;

    public LauncherWindow()
    {
        handle = this;
        InitializeComponent();
    }

    public static int phase = 0;

    private void RobloxTimer_Tick(object sender, EventArgs e)
    {
        foreach (Process proc in Process.GetProcesses())
        {
            if (proc.MainWindowTitle == "Roblox")
            {
                robloxTimer.Stop();
                Hide(); // hide for now

                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000); // ??

                    RobloxClient.SetTitle(proc, $"Roblox - (RonaBloxx)"); // {RobloxProcess.version}

                    RobloxClient.ExitApp();
                });
            }
        }

        if (RobloxProcess.universe != null)
        {
            Datum datum = RobloxProcess.universe.data.First();
            label2.Text = datum.name;
            label3.Text = $"{datum.playing}/{datum.maxPlayers}";

            placeId = HttpUtility.UrlDecode(Program.la.PlaceLauncherUrl).Split('&')[2].Split('=')[1];
        }
    }

    string loadingSufix = "Starting Roblox";

    int dots = 1;
    private void SuspendTimer_Tick(object sender, EventArgs e)
    {
        progressBar1.Value = phase;

        if (dots == 4) dots = 0;
        dots++;

        label1.Text = loadingSufix + " " + String.Concat(Enumerable.Repeat(".", dots));
    }

    string placeId = "0";

    private void LauncherWindow_Load(object sender, EventArgs e)
    {
        label2.Text = "Roblox Game"; // label2.Text = "Loading Game Information";
        label3.Text = "0/0";

        //Opacity = 0.9f;

        // update label
        label1.Text = loadingSufix + " " + String.Concat(Enumerable.Repeat(".", dots));
    }

    private void formBackground_Click(object sender, EventArgs e)
    {

    }

    private void label2_Click(object sender, EventArgs e)
    {

    }

    private void label1_Click(object sender, EventArgs e)
    {

    }
}