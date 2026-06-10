using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GUI;

public partial class PasswordDialog : Window
{
    public PasswordDialog(bool IsConfirmDialog=false)
    {
        InitializeComponent();
        if (IsConfirmDialog)
        {
            PasswordLabel.Content = "Please enter the password again to confirm.\n\n" +
                "WARNING: If you lose the password, there will be no way to recover this \ndocument.";
        }
    }

    private void ButtonOK_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(PasswordTextbox.Text);
    }
}