using System.IO;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Directory = System.IO.Directory;
using MessageBox = System.Windows.Forms.MessageBox;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;

namespace MySpyImageResizer.UserControls.Home;

public partial class HomeTab : UserControl
{
    internal OpenFolderDialog SourceFolderDialog { get; set; }
    internal OpenFolderDialog OutputFolderDialog { get; set; }
    internal string[] Extensions { get; set; } = [".jpeg", ".png", ".bmp", ".gif", ".webp", ".tiff", ".pdf"];
    internal int DefaultImageHeight { get; set; } = 800;
    internal int DefaultImageWidth { get; set; } = 600;

    public HomeTab()
    {
        InitializeComponent();

        SourceFolderDialog = new OpenFolderDialog
        {
            Title = "Select Folder",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        OutputFolderDialog = new OpenFolderDialog
        {
            Title = "Select Folder",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        HeightTextBox.Value = DefaultImageHeight;
        WidthTextBox.Value = DefaultImageWidth;
        OutputFormatComboBox.ItemsSource = (ImageFormat[])Enum.GetValues(typeof(ImageFormat));
        OutputFormatComboBox.SelectedIndex = 0;
    }

    private void BrowseBtn_Click(object sender, RoutedEventArgs e) => HandleFolderSelection(SourceFolderDialog, SourceFolderTextBox);

    private void ChangeOutputBtn_Click(object sender, RoutedEventArgs e) => HandleFolderSelection(OutputFolderDialog, OutputFolderTextBox);
    
    private void StartResizeBtn_Click(object sender, RoutedEventArgs e)
    {
        var sourceFolderName = SourceFolderDialog.FolderName.Split(@"\").Last();
        if (string.IsNullOrEmpty(sourceFolderName))
        {
            MessageBox.Show("Source Folder is not selected.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (string.IsNullOrEmpty(OutputFolderDialog.FolderName))
            OutputFolderDialog.FolderName = Path.Combine(SourceFolderDialog.FolderName, "resized");

        var outputFormat = (ImageFormat)OutputFormatComboBox.SelectedItem;
        var keepAspectRation = KeepAspectRationCheckBox.IsChecked ?? false;
        var height = (int?)HeightTextBox.Value ?? DefaultImageHeight;
        var width = (int?)WidthTextBox.Value ?? DefaultImageWidth;

        List<string> files = [];
        foreach (var ext in Extensions)
            foreach (var filePath in Directory.GetFiles(SourceFolderDialog.FolderName, $"*{ext}"))
                files.Add(filePath);

        if (files.Count is 0)
        {
            MessageBox.Show("No images found in the selected source folder.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        LinearProgressBar.IsIndeterminate = true;
        StatusImage.Source = new BitmapImage(new Uri("/Images/etc/InProgressProcess.png", UriKind.RelativeOrAbsolute));

        if (!Directory.Exists(OutputFolderDialog.FolderName))
            Directory.CreateDirectory(OutputFolderDialog.FolderName);

        foreach (var filePath in files)
        {
            //var outputFilePath = Path.Combine(OutputFolderDialog.FolderName, $"{Path.GetFileName(filePath)}");
            ImageHelper.ResizeImage(filePath, OutputFolderDialog.FolderName, width, height, outputFormat, keepAspectRation);
        }

        LinearProgressBar.IsIndeterminate = false;
        StatusImage.Source = new BitmapImage(new Uri("/Images/etc/CompletedProcess.png", UriKind.RelativeOrAbsolute));
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
