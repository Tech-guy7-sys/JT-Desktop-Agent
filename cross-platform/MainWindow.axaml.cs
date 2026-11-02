using System.Security.Cryptography;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace JTDesktopAgent.CrossPlatform;

public sealed partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private async void ReviewFile_Click(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Choose a file for local review",
            AllowMultiple = false
        });
        var file = files.FirstOrDefault();
        if (file is null) return;

        try
        {
            await using var stream = await file.OpenReadAsync();
            var hash = await SHA256.HashDataAsync(stream);
            var properties = await file.GetBasicPropertiesAsync();
            ActivityText.Text =
                $"LOCAL FILE REVIEW\nName: {file.Name}\nSize: {properties.Size?.ToString("N0") ?? "Unknown"} bytes\n" +
                $"SHA-256: {Convert.ToHexString(hash)}\n\nNo file contents were uploaded or modified. " +
                "A hash identifies the bytes; it is not a malware verdict.";
        }
        catch (Exception ex)
        {
            ActivityText.Text = $"File review failed: {ex.Message}";
        }
    }

    private void Stop_Click(object? sender, RoutedEventArgs e)
    {
        ActivityText.Text = "Emergency stop is active. No desktop actions are enabled in this shell.";
        StopButton.IsEnabled = false;
    }
}
