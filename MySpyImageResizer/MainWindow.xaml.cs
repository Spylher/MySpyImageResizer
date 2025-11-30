using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using MySpyImageResizer.UserControls.Home;

namespace MySpyImageResizer;

using static App;

public partial class MainWindow : Window
{
    public IntPtr Handle => new WindowInteropHelper(this).Handle;
    public HomeTab HomeTab { get; set; } = new();

    public MainWindow()
    {
        InitializeComponent();
        InitializeTheme(this);

        var folderDialog = new OpenFolderDialog
        {
            Title = "Select Folder2",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
        };
        //folderDialog.ShowDialog();
        //if (folderDialog.ShowDialog() == true)
        //{
        //    var folderName = folderDialog.FolderName;
        //    MessageBox.Show($"You picked ${folderName}!");
        //}
        Container.Content = HomeTab;
    }

    private void TopPanel_MouseDown(object sender, MouseButtonEventArgs e) => ReleaseCapture_MouseDown(Handle, null!);

    private void CloseApp_Click(object sender, RoutedEventArgs e) => Close();

    private void MenuItem_Exit_Click(object sender, RoutedEventArgs e) => Close(); // Icon menu exit

    private void MinimizeApp_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    public static void ReleaseCapture_MouseDown(IntPtr hwnd, MouseEventArgs e)
    {
        ReleaseCapture();
        SendMessage(hwnd, 0x112, 0xf012, 0);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool ReleaseCapture();

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool SendMessage(IntPtr hWnd, int message, int wParam, int lParam);
}