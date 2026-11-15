using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AgentJeff;

public sealed record AppShortcut(string Name, string Path);

public partial class AppLauncherWindow : Window
{
    private readonly List<AppShortcut> _allApps = new();
    private readonly ObservableCollection<AppShortcut> _visibleApps = new();

    public event Action<string>? AppLaunched;

    public AppLauncherWindow()
    {
        InitializeComponent();
        AppList.ItemsSource = _visibleApps;
        LoadShortcuts();
        SearchBox.Focus();
    }

    private void LoadShortcuts()
    {
        var roots = new[]
        {
            Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu)
        }.Where(path => !string.IsNullOrWhiteSpace(path) && Directory.Exists(path)).Distinct(StringComparer.OrdinalIgnoreCase);

        foreach (var root in roots)
        {
            try
            {
                foreach (var shortcut in Directory.EnumerateFiles(root, "*.lnk", SearchOption.AllDirectories).Take(600))
                {
                    var name = Path.GetFileNameWithoutExtension(shortcut);
                    if (string.IsNullOrWhiteSpace(name) || name.Equals("Uninstall", StringComparison.OrdinalIgnoreCase)) continue;
                    _allApps.Add(new AppShortcut(name, shortcut));
                }
            }
            catch (UnauthorizedAccessException) { /* Skip folders the current user cannot enumerate. */ }
            catch (IOException) { /* Start Menu shortcuts are best-effort discovery only. */ }
        }

        var unique = _allApps
            .GroupBy(app => app.Name, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(app => app.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        _allApps.Clear();
        _allApps.AddRange(unique);
        FilterApps();
        StatusText.Text = _allApps.Count == 0
            ? "No Start Menu shortcuts were found. You can still use Windows search."
            : $"{_allApps.Count} Start Menu shortcuts found · double-click or select Open";
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => FilterApps();

    private void FilterApps()
    {
        var query = SearchBox.Text.Trim();
        var matches = _allApps
            .Where(app => app.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Take(100)
            .ToArray();
        _visibleApps.Clear();
        foreach (var app in matches) _visibleApps.Add(app);
        if (_visibleApps.Count > 0) AppList.SelectedIndex = 0;
        if (StatusText is not null && !string.IsNullOrWhiteSpace(query))
            StatusText.Text = $"{_visibleApps.Count} matching shortcuts";
    }

    private void AppList_MouseDoubleClick(object sender, MouseButtonEventArgs e) => OpenSelected();

    private void OpenSelected_Click(object sender, RoutedEventArgs e) => OpenSelected();

    private void OpenSelected()
    {
        if (AppList.SelectedItem is not AppShortcut selected) return;
        try
        {
            Process.Start(new ProcessStartInfo { FileName = selected.Path, UseShellExecute = true });
            StatusText.Text = $"Opened {selected.Name}";
            AppLaunched?.Invoke(selected.Name);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not open app shortcut", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
