using System.Windows;

namespace AgentJeff;

public partial class VoiceCompanionWindow : Window
{
    public VoiceCompanionWindow()
    {
        InitializeComponent();
    }

    private void Header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) DragMove();
    }

    private void Hide_Click(object sender, RoutedEventArgs e) => Close();
}