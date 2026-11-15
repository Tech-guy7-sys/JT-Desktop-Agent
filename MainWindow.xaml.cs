using System.Globalization;
using System.Collections.ObjectModel;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace AgentJeff;

public partial class MainWindow : Window
{
    private readonly SpeechSynthesizer _synth = new();
    private SpeechRecognitionEngine? _recognizer;
    private bool _listening;
    private readonly List<InstalledVoice> _voices = new();
    private readonly ObservableCollection<string> _activity = new();
    private VoiceCompanionWindow? _companion;
    private SecurityCenterWindow? _securityCenter;
    private readonly string[] _commands = { "Review a file", "Start or stop voice input", "Open Security Center", "Show floating Jeff", "Read transcript aloud", "Stop speaking", "Copy transcript", "Save transcript", "Clear transcript", "Clear recent activity" };

    public MainWindow()
    {
        InitializeComponent();
        ActivityList.ItemsSource = _activity;
        LoadRecentActivity();
        LoadVoices();
        LoadRecognizer();
        Closed += (_, _) => { StopListening(); _synth.Dispose(); _recognizer?.Dispose(); };
    }

    private void LoadVoices()
    {
        try
        {
            foreach (var voice in _synth.GetInstalledVoices().Where(v => v.Enabled))
            {
                _voices.Add(voice);
                VoicePicker.Items.Add($"{voice.VoiceInfo.Name} · {voice.VoiceInfo.Culture.Name}");
            }
            if (VoicePicker.Items.Count > 0) VoicePicker.SelectedIndex = 0;
            else SetStatus("NO WINDOWS TTS VOICE FOUND");
        }
        catch (Exception ex) { SetStatus("VOICE ERROR"); TranscriptBox.Text = ex.Message; }
    }

    private void LoadRecognizer()
    {
        try
        {
            var installed = SpeechRecognitionEngine.InstalledRecognizers();
            var recognizerInfo = installed.FirstOrDefault(r => r.Culture.Name.Equals("en-NG", StringComparison.OrdinalIgnoreCase))
                ?? installed.FirstOrDefault(r => r.Culture.Name.Equals("en-US", StringComparison.OrdinalIgnoreCase))
                ?? installed.FirstOrDefault(r => r.Culture.TwoLetterISOLanguageName == "en");
            if (recognizerInfo is null)
            {
                ListenButton.IsEnabled = false;
                SetStatus("WINDOWS SPEECH RECOGNIZER NOT INSTALLED");
                TranscriptBox.Text = "Windows has no compatible installed speech recognizer. Install a supported Windows speech language in Settings, then reopen JT Desktop Agent.";
                return;
            }
            _recognizer = new SpeechRecognitionEngine(recognizerInfo);
            _recognizer.LoadGrammar(new DictationGrammar());
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.SpeechRecognized += (_, e) =>
            {
                if (e.Result.Confidence < 0.45 || string.IsNullOrWhiteSpace(e.Result.Text)) return;
                Dispatcher.Invoke(() =>
                {
                    var existing = TranscriptBox.Text.Trim();
                    if (existing.StartsWith("Your recognized speech")) existing = "";
                    TranscriptBox.Text = string.IsNullOrEmpty(existing) ? e.Result.Text : existing + " " + e.Result.Text;
                    TranscriptBox.ScrollToEnd();
                });
            };
            _recognizer.RecognizeCompleted += (_, _) => Dispatcher.Invoke(() =>
            {
                if (_listening) { _listening = false; ListenButton.Content = "Start listening"; SetStatus("READY · MICROPHONE OFF"); }
            });
        }
        catch (Exception ex)
        {
            ListenButton.IsEnabled = false;
            SetStatus("SPEECH INITIALIZATION FAILED");
            TranscriptBox.Text = $"Could not initialize Windows speech recognition: {ex.Message}";
        }
    }

    private void Listen_Click(object sender, RoutedEventArgs e)
    {
        if (_listening) { StopListening(); return; }
        if (_recognizer is null) { SetStatus("SPEECH RECOGNITION UNAVAILABLE"); return; }
        try
        {
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            _listening = true;
            LogActivity("Voice input started");
            ListenButton.Content = "Listening… (click to stop)";
            SetStatus("LISTENING · MICROPHONE ACTIVE");
        }
        catch (Exception ex) { SetStatus("MICROPHONE ERROR"); MessageBox.Show(this, ex.Message, "JT Desktop Agent", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private void Stop_Click(object sender, RoutedEventArgs e) => StopListening();
    private void StopListening()
    {
        if (!_listening) return;
        _listening = false;
        LogActivity("Voice input stopped");
        try { _recognizer?.RecognizeAsyncCancel(); } catch { }
        ListenButton.Content = "Start listening";
        SetStatus("READY · MICROPHONE OFF");
    }

    private void VoicePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var index = VoicePicker.SelectedIndex;
        if (index >= 0 && index < _voices.Count)
        {
            try { _synth.SelectVoice(_voices[index].VoiceInfo.Name); } catch { }
        }
    }

    private void PreviewVoice_Click(object sender, RoutedEventArgs e) => Speak("Hello. I'm Jeff, your desktop assistant from JT Digital World.");
    private void SpeakTranscript_Click(object sender, RoutedEventArgs e)
    {
        var text = TranscriptBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(text) || text.StartsWith("Your recognized speech")) return;
        Speak(text);
    }
    private void Speak(string text)
    {
        try { _synth.SpeakAsyncCancelAll(); _synth.SpeakAsync(text); SetStatus("SPEAKING"); LogActivity("Read-aloud started"); }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Text-to-speech error", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }
    private void StopSpeaking_Click(object sender, RoutedEventArgs e) { _synth.SpeakAsyncCancelAll(); LogActivity("Read-aloud stopped"); SetStatus(_listening ? "LISTENING · MICROPHONE ACTIVE" : "READY · MICROPHONE OFF"); }
    private void Clear_Click(object sender, RoutedEventArgs e) => TranscriptBox.Clear();

    private void ShowCompanion_Click(object sender, RoutedEventArgs e)
    {
        if (_companion is null || !_companion.IsVisible)
        {
            _companion = new VoiceCompanionWindow();
            _companion.Closed += (_, _) => _companion = null;
            _companion.Show();
            LogActivity("Desktop companion opened");
        }
        else
        {
            _companion.Activate();
            LogActivity("Desktop companion focused");
        }
    }
    private void OpenSecurity_Click(object sender, RoutedEventArgs e)
    {
        if (_securityCenter is null || !_securityCenter.IsVisible)
        {
            _securityCenter = new SecurityCenterWindow { Owner = this };
            _securityCenter.Closed += (_, _) => _securityCenter = null;
            _securityCenter.Show();
            LogActivity("Security Center opened");
        }
        else _securityCenter.Activate();
        LogActivity("Security Center opened");
    }

    private async void ReviewFile_Click(object sender, RoutedEventArgs e)
    {
        var picker = new OpenFileDialog
        {
            Title = "Choose a file for local review",
            CheckFileExists = true,
            Multiselect = false
        };
        if (picker.ShowDialog(this) != true) return;
        LogActivity("Selected file review requested");
        try
        {
            SetStatus("REVIEWING SELECTED FILE");
            var result = await FileReviewService.ReviewAsync(picker.FileName);
            TranscriptBox.Text = result.ToDisplayText();
            SetStatus("FILE REVIEW COMPLETE · LOCAL ONLY");
        }
        catch (Exception ex)
        {
            SetStatus("FILE REVIEW FAILED");
            MessageBox.Show(this, ex.Message, "JT Desktop Agent · File review", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private static string ActivityFilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "JTechDigitalWorld", "JTDesktopAgent", "activity.log");

    private void LoadRecentActivity()
    {
        try
        {
            if (!File.Exists(ActivityFilePath)) return;
            foreach (var entry in File.ReadAllLines(ActivityFilePath).TakeLast(10).Reverse()) _activity.Add(entry);
        }
        catch { /* Activity history is optional; the agent remains usable if storage is unavailable. */ }
    }

    private void LogActivity(string action)
    {
        var entry = $"{DateTime.Now:yyyy-MM-dd HH:mm} · {action}";
        _activity.Insert(0, entry);
        while (_activity.Count > 10) _activity.RemoveAt(_activity.Count - 1);
        try
        {
            var directory = Path.GetDirectoryName(ActivityFilePath);
            if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);
            File.AppendAllText(ActivityFilePath, entry + Environment.NewLine);
            var lines = File.ReadAllLines(ActivityFilePath);
            if (lines.Length > 100) File.WriteAllLines(ActivityFilePath, lines.TakeLast(100));
        }
        catch { /* Logging must never block a user-requested action. */ }
    }

    private void ClearActivity_Click(object sender, RoutedEventArgs e)
    {
        var answer = MessageBox.Show(this, "Clear the recent action labels stored on this device?", "Clear activity", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (answer != MessageBoxResult.Yes) return;
        _activity.Clear();
        try { if (File.Exists(ActivityFilePath)) File.Delete(ActivityFilePath); }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not clear activity", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private void OpenCommandPalette_Click(object sender, RoutedEventArgs e) => OpenCommandPalette();

    private void OpenCommandPalette()
    {
        CommandPaletteOverlay.Visibility = Visibility.Visible;
        CommandSearchBox.Text = string.Empty;
        CommandResults.ItemsSource = _commands;
        if (CommandResults.Items.Count > 0) CommandResults.SelectedIndex = 0;
        CommandSearchBox.Focus();
    }

    private void CloseCommandPalette_Click(object sender, RoutedEventArgs e) => CloseCommandPalette();

    private void CloseCommandPalette()
    {
        CommandPaletteOverlay.Visibility = Visibility.Collapsed;
        Focus();
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.K && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            OpenCommandPalette();
            e.Handled = true;
            return;
        }
        if (CommandPaletteOverlay.Visibility != Visibility.Visible) return;
        if (e.Key == Key.Escape)
        {
            CloseCommandPalette();
            e.Handled = true;
        }
        else if (e.Key == Key.Enter)
        {
            RunSelectedCommand();
            e.Handled = true;
        }
    }

    private void CommandSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (CommandResults is null) return;
        var query = CommandSearchBox.Text.Trim();
        var matches = _commands.Where(command => command.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
        CommandResults.ItemsSource = matches;
        if (CommandResults.Items.Count > 0) CommandResults.SelectedIndex = 0;
    }

    private void CommandResults_MouseDoubleClick(object sender, MouseButtonEventArgs e) => RunSelectedCommand();

    private void CommandPaletteBackdrop_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (ReferenceEquals(e.OriginalSource, CommandPaletteOverlay)) CloseCommandPalette();
    }

    private void CommandPalettePanel_MouseDown(object sender, MouseButtonEventArgs e) => e.Handled = true;

    private void RunSelectedCommand()
    {
        if (CommandResults.SelectedItem is not string command) return;
        CloseCommandPalette();
        switch (command)
        {
            case "Review a file": ReviewFile_Click(this, new RoutedEventArgs()); break;
            case "Start or stop voice input": Listen_Click(this, new RoutedEventArgs()); break;
            case "Open Security Center": OpenSecurity_Click(this, new RoutedEventArgs()); break;
            case "Show floating Jeff": ShowCompanion_Click(this, new RoutedEventArgs()); break;
            case "Read transcript aloud": SpeakTranscript_Click(this, new RoutedEventArgs()); break;
            case "Stop speaking": StopSpeaking_Click(this, new RoutedEventArgs()); break;
            case "Copy transcript": CopyTranscript_Click(this, new RoutedEventArgs()); break;
            case "Save transcript": SaveTranscript_Click(this, new RoutedEventArgs()); break;
            case "Clear transcript": Clear_Click(this, new RoutedEventArgs()); break;
            case "Clear recent activity": ClearActivity_Click(this, new RoutedEventArgs()); break;
        }
    }

    private void CopyTranscript_Click(object sender, RoutedEventArgs e)
    {
        var text = TranscriptBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(text) || text.StartsWith("Your recognized speech", StringComparison.Ordinal)) return;
        try { Clipboard.SetText(text); SetStatus("TRANSCRIPT COPIED"); LogActivity("Transcript copied to clipboard"); }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Clipboard unavailable", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private void SaveTranscript_Click(object sender, RoutedEventArgs e)
    {
        var text = TranscriptBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(text) || text.StartsWith("Your recognized speech", StringComparison.Ordinal)) return;
        var dialog = new SaveFileDialog { Title = "Save reviewed transcript", FileName = "JT-Jeff-Transcript.txt", Filter = "Text files (*.txt)|*.txt|Markdown files (*.md)|*.md", AddExtension = true, OverwritePrompt = true };
        if (dialog.ShowDialog(this) != true) return;
        try
        {
            File.WriteAllText(dialog.FileName, text);
            SetStatus("TRANSCRIPT SAVED");
            LogActivity("Transcript saved by user");
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Could not save transcript", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private void SetStatus(string status) => StatusText.Text = status.ToUpperInvariant();
}