using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GUI;

public partial class PasswordDialog : Window
{
    public PasswordDialog()
    {
        InitializeComponent();
    }

    private void ButtonOK_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(PasswordTextbox.Text);
    }
}