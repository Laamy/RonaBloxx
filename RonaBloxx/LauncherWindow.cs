using System.Diagnostics;
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
            label2.Text = RobloxProcess.universe.data.First().name;

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
        label2.Text = "";

        Opacity = 0.9f;

        // update label
        label1.Text = loadingSufix + " " + String.Concat(Enumerable.Repeat(".", dots));
    }

    private void formBackground_Click(object sender, EventArgs e)
    {

    }
}