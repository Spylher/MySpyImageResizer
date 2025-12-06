using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        StatusImage.Source = new BitmapImage(new Uri("/Images/etc/InProgressProcess.png", UriKind.RelativeOrAbsolute));
    }

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
    {
        LinearProgressBar.IsIndeterminate = false;
        StatusImage.Source = new BitmapImage(new Uri("/Images/etc/ReadyToProcess.png", UriKind.RelativeOrAbsolute));
        //StatusImage.Source = new BitmapImage(new Uri("/Images/etc/CompletedProcess.png", UriKind.RelativeOrAbsolute));
    }

    private void HandleFolderSelection(OpenFolderDialog folderDialog, TextBox textBox)
    {
        if (!string.IsNullOrEmpty(folderDialog.FolderName))
            folderDialog.InitialDirectory = Directory.GetParent(folderDialog.FolderName)!.FullName;

        if (folderDialog.ShowDialog() is false)
        {
            textBox.Foreground = new SolidColorBrush(Color.FromRgb(98, 98, 106));
            folderDialog.FolderName = "";

            if (textBox.Name.Equals("OutputFolderTextBox", StringComparison.InvariantCultureIgnoreCase))
            {
                textBox.Text = "Auto-Generate";
                textBox.Padding = new Thickness(-110, 8, 0, 0);
            }
            else
            {
                textBox.Text = "No folder selected";
                textBox.Padding = new Thickness(-90, 8, 0, 0);
            }

            return;
        }

        var folderName = folderDialog.FolderName.Split(@"\").Last();
        if (string.IsNullOrEmpty(folderName))
            return;

        textBox.Foreground = Brushes.WhiteSmoke;
        textBox.Padding = new Thickness(0, 8, 0, 0);
        textBox.Text = folderName[..Math.Min(23, folderName.Length)];
    }

}
