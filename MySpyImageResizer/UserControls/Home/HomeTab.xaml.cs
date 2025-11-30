using Microsoft.Win32;
using Syncfusion.CompoundFile.DocIO.Net;
using Syncfusion.XPS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = System.Windows.Forms.MessageBox;
using System;
using Directory = System.IO.Directory;

namespace MySpyImageResizer.UserControls.Home;

public partial class HomeTab : UserControl
{
    public OpenFolderDialog SourceFolder { get; set; }
    public OpenFolderDialog OutputFolder { get; set; }

    public HomeTab()
    {
        InitializeComponent();

        SourceFolder = new OpenFolderDialog
        {
            Title = "Select Folder",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        OutputFolder = new OpenFolderDialog
        {
            Title = "Select Folder",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

    }

    private void BrowseBtn_Click(object sender, RoutedEventArgs e) => HandleFolderSelection(SourceFolder, SourceFolderTextBox);

    private void ChangeOutputBtn_Click(object sender, RoutedEventArgs e) => HandleFolderSelection(OutputFolder, OutputFolderTextBox);


    private void StartResizeBtn_Click(object sender, RoutedEventArgs e)
    {
        LinearProgressBar.IsIndeterminate = true;
    }

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
    {
        LinearProgressBar.IsIndeterminate = false;
    }

    private void HandleFolderSelection(OpenFolderDialog folderDialog, TextBox textBox)
    {
        if (!string.IsNullOrEmpty(folderDialog.FolderName))
            folderDialog.InitialDirectory = Directory.GetParent(folderDialog.FolderName)!.FullName;

        folderDialog.ShowDialog();

        var folderName = folderDialog.FolderName.Split(@"\").Last();
        if (string.IsNullOrEmpty(folderName))
            return;

        textBox.Foreground = Brushes.WhiteSmoke;
        textBox.Padding = new Thickness(0, 8, 0, 0);
        textBox.Text = folderName[..Math.Min(23, folderName.Length)];
    }

}
