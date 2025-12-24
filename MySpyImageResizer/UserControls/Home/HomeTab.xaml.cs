using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cursors = System.Windows.Input.Cursors;
using Directory = System.IO.Directory;
using MessageBox = System.Windows.Forms.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace MySpyImageResizer.UserControls.Home;

public partial class HomeTab
{
    internal OpenFolderDialog SourceFolderDialog { get; set; }
    internal OpenFolderDialog OutputFolderDialog { get; set; }
    internal int ResizedFilesCount { get; set; }
    internal string[] Extensions { get; set; } = [".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".tiff", ".pdf"];
    internal int DefaultImageHeight { get; set; } = 800;
    internal int DefaultImageWidth { get; set; } = 600;

    private CancellationTokenSource? _cts;

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

    private void BrowseBtn_Click(object sender, RoutedEventArgs e)
    {
        if (_cts?.IsCancellationRequested == false)
            return;

        ResetUi();
        HandleFolderSelection(SourceFolderDialog, SourceFolderTextBox);
        if (string.IsNullOrEmpty(SourceFolderDialog.FolderName))
            return;

        var files = GetFiles(SourceFolderDialog.FolderName, Extensions);
        NumberOfImagesDetectedLabel.Content = $"{files.Count}";
        ImagesProcessedLabel.Content = $"0/{files.Count} Images processed    •    0% complete";
    }

    private void ChangeOutputBtn_Click(object sender, RoutedEventArgs e)
    {
        if (_cts?.IsCancellationRequested == false)
            return;

        //ResetUi();
        HandleFolderSelection(OutputFolderDialog, OutputFolderTextBox);
    }

    private void StartResizeBtn_Click(object sender, RoutedEventArgs e)
    {
        if (_cts?.IsCancellationRequested == false)
            return;

        if (string.IsNullOrEmpty(SourceFolderDialog.FolderName.Split(@"\").Last()))
        {
            MessageBox.Show("Source Folder is not selected.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var files = GetFiles(SourceFolderDialog.FolderName, Extensions);

        if (files.Count is 0)
        {
            MessageBox.Show("No images found in the selected source folder.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (string.IsNullOrEmpty(OutputFolderDialog.FolderName) || OutputFolderTextBox.Text.Equals("Auto-Generate", StringComparison.InvariantCultureIgnoreCase))
            OutputFolderDialog.FolderName = Path.Combine(SourceFolderDialog.FolderName, "resized");

        if (!Directory.Exists(OutputFolderDialog.FolderName))
            Directory.CreateDirectory(OutputFolderDialog.FolderName);

        ReadyToProcessLabel.Content = "Process in progress";
        StatusImage.Source = new BitmapImage(new Uri("/Images/etc/InProgressProcess.png", UriKind.RelativeOrAbsolute));
        LinearProgressBar.ProgressColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6B69D6"));
        LinearProgressBar.Progress = 0;
        StartResizeBtn.Cursor = Cursors.No;
        BrowseBtn.Cursor = Cursors.No;
        ChangeOutputBtn.Cursor = Cursors.No;
        ResizedFilesCount = 0;

        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        // update UI asynchronously
        Task.Run(() => UpdateUiAsync(files, token), token);

        // perform resizing asynchronously
        var outputFormat = (ImageFormat)OutputFormatComboBox.SelectedItem;
        var keepAspectRation = KeepAspectRationCheckBox.IsChecked ?? false;
        var keepDimensions = ConvertFormatOnlyCheckBox.IsChecked ?? false;
        var height = (int?)HeightTextBox.Value ?? DefaultImageHeight;
        var width = (int?)WidthTextBox.Value ?? DefaultImageWidth;

        Task.Run(async () =>
        {
            await ResizeImagesAsync(files, outputFormat, keepAspectRation, keepDimensions, height, width, token);
        }, token);
    }

    private async Task UpdateUiAsync(List<string> files, CancellationToken ct)
    {
        try
        {
            var filesCount = files.Count;

            while (!ct.IsCancellationRequested)
            {
                _ = Dispatcher.InvokeAsync(() =>
                {
                    var percent = (double)ResizedFilesCount / filesCount * 100;

                    ImagesProcessedLabel.Content = $"{ResizedFilesCount}/{filesCount} Images processed    •    {(int)percent}% complete";
                    LinearProgressBar.Progress = percent;
                });

                if (ResizedFilesCount == filesCount)
                {
                    _ = Dispatcher.InvokeAsync(() =>
                    {
                        LinearProgressBar.ProgressColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF19C07B"));
                        StatusImage.Source = new BitmapImage(new Uri("/Images/etc/CompletedProcess.png", UriKind.RelativeOrAbsolute));
                        ReadyToProcessLabel.Content = "Completed process";
                    });


                    var dResult = MessageBox.Show(
                        "Would you like to open the output folder?",
                        "Information",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );

                    if (dResult == DialogResult.Yes)
                        if (!OpenFolderInExplorer(OutputFolderDialog.FolderName))
                            MessageBox.Show("Output folder not found.");

                    _cts?.CancelAsync();
                }

                await Task.Delay(10, ct);
            }
        }
        catch (OperationCanceledException)
        {
            //...
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex}");
        }
        finally
        {
            _ = Dispatcher.InvokeAsync(() =>
            {
                BrowseBtn.Cursor = Cursors.Hand;
                ChangeOutputBtn.Cursor = Cursors.Hand;
                StartResizeBtn.Cursor = Cursors.Hand;
            });
        }
    }

    private async Task ResizeImagesAsync(List<string> files, ImageFormat outputFormat, bool keepAspectRation, bool keepDimensions, int height, int width, CancellationToken ct)
    {
        if (!ct.IsCancellationRequested)
        {
            try
            {
                foreach (var filePath in files)
                {
                    ImageHelper.ResizeImage(filePath, OutputFolderDialog.FolderName, width, height, outputFormat, keepAspectRation, keepDimensions);
                    ResizedFilesCount++;

                    await Task.Delay(1, ct);
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex}");
            }

            await Task.Delay(10, ct);
        }
    }

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
    {
        _cts?.Cancel();
        ResetUi();
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

    private List<string> GetFiles(string path, params string[] extensions)
    {
        List<string> files = [];

        if (extensions.Length is 0)
            return Directory.EnumerateFiles(path).ToList();

        foreach (var ext in extensions)
            foreach (var filePath in Directory.EnumerateFiles(path, $"*{ext}"))
                files.Add(filePath);

        return files;
    }

    private void ConvertFormatOnlyCheckBox_Click(object sender, RoutedEventArgs e)
    {
        var isChecked = ConvertFormatOnlyCheckBox.IsChecked ?? false;
        var keepIsChecked = KeepAspectRationCheckBox.IsChecked ?? false;

        WidthTextBox.IsEnabled = !isChecked;

        if (!keepIsChecked)
            HeightTextBox.IsEnabled = !isChecked;
    }

    private void KeepAspectRationCheckBox_Click(object sender, RoutedEventArgs e)
    {
        var isChecked = KeepAspectRationCheckBox.IsChecked ?? false;
        var convertIsChecked = ConvertFormatOnlyCheckBox.IsChecked ?? false;

        if (!convertIsChecked)
            HeightTextBox.IsEnabled = !isChecked;

        if (isChecked)
            MessageBox.Show("The image is resized proportionally based on its width, preserving the original width-to-height ratio.",
                "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ResetUi()
    {
        BrowseBtn.Cursor = Cursors.Hand;
        ChangeOutputBtn.Cursor = Cursors.Hand;
        StartResizeBtn.Cursor = Cursors.Hand;

        ReadyToProcessLabel.Content = "Ready to process";
        ImagesProcessedLabel.Content = "0/0 Images processed    •    0% complete";
        StatusImage.Source = new BitmapImage(new Uri("/Images/etc/InProgressProcess.png", UriKind.RelativeOrAbsolute));
        NumberOfImagesDetectedLabel.Content = "0";

        LinearProgressBar.Progress = 0;
        LinearProgressBar.ProgressColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6B69D6"));
    }

    private static bool OpenFolderInExplorer(string path)
    {
        if (!Directory.Exists(path))
            return false;

        Process.Start(new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        });

        return true;
    }

}
