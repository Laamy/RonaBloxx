﻿using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using System;

public partial class LauncherWindow : Form
{
    public LauncherWindow() => InitializeComponent();

    private void RobloxTimer_Tick(object sender, EventArgs e)
    {
        foreach (Process proc in Process.GetProcesses())
        {
            if (proc.MainWindowTitle == "Roblox")
            {
                RobloxClient.ExitApp();
            }
        }

        if (RobloxProcess.universe != null)
        {
            Datum datum = RobloxProcess.universe.data.First();
            label2.Text = $"{datum.name} ({datum.playing}/{datum.maxPlayers})";
            //label3.Text = datum.description;

            placeId = HttpUtility.UrlDecode(Program.la.PlaceLauncherUrl).Split('&')[2].Split('=')[1];
        }
    }

    string loadingSufix = "Starting Roblox";

    int dots = 1;
    private void SuspendTimer_Tick(object sender, EventArgs e)
    {
        if (dots == 4) dots = 0;
        dots++;

        label1.Text = loadingSufix + " " + String.Concat(Enumerable.Repeat(".", dots));
    }

    string placeId = "0";

    private void LauncherWindow_Load(object sender, EventArgs e)
    {
        label2.Text = ""; // label2.Text = "Loading Game Information";
        //label3.Text = "..";

        Opacity = 0.9f;

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