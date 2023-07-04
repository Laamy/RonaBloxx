using System;
using System.Runtime.InteropServices;

public class User32
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int SetWindowText(IntPtr hWnd, string lpString);
}