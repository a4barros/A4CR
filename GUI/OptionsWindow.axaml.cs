using a4crypt;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GUI;

public partial class OptionsWindow : Window
{
    public OptionsWindow()
    {
        InitializeComponent();
    }

    private void OKButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (HashTypeArgon2id.IsChecked == true)
        {
            W.SelectedKeyType = G.KeyTypes.Argon2id;
        }
        else if (HashTypePBKDF2.IsChecked == true)
        {
            W.SelectedKeyType = G.KeyTypes.PBKDF2;
        }
        if (KeyStrengthLow.IsChecked == true)
        {
            W.SelectedKeyStrength = G.KeyStrengths.Low;
        }
        else if (KeyStrengthMedium.IsChecked == true)
        {
            W.SelectedKeyStrength = G.KeyStrengths.Medium;
        }
        else if (KeyStrengthMedium.IsChecked == true)
        {
            W.SelectedKeyStrength = G.KeyStrengths.High;
        }
        else if (KeyStrengthHigh.IsChecked == true)
        {
            W.SelectedKeyStrength = G.KeyStrengths.Ultra;
        }
    }
}