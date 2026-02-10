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
        LoadSelections();
    }

    private void OKButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (HashTypeArgon2id.IsChecked == true)
            W.SelectedKeyType = G.KeyTypes.Argon2id;
        else if (HashTypePBKDF2.IsChecked == true)
            W.SelectedKeyType = G.KeyTypes.PBKDF2;

        if (KeyStrengthLow.IsChecked == true)
            W.SelectedKeyStrength = G.KeyStrengths.Low;
        else if (KeyStrengthMedium.IsChecked == true)
            W.SelectedKeyStrength = G.KeyStrengths.Medium;
        else if (KeyStrengthHigh.IsChecked == true)
            W.SelectedKeyStrength = G.KeyStrengths.High;
        else if (KeyStrengthUltra.IsChecked == true)
            W.SelectedKeyStrength = G.KeyStrengths.Ultra;

        Close();
    }

    private void LoadSelections()
    {
        switch (W.SelectedKeyType)
        {
            case G.KeyTypes.Argon2id:
                HashTypeArgon2id.IsChecked = true;
                break;

            case G.KeyTypes.PBKDF2:
                HashTypePBKDF2.IsChecked = true;
                break;
        }

        switch (W.SelectedKeyStrength)
        {
            case G.KeyStrengths.Low:
                KeyStrengthLow.IsChecked = true;
                break;

            case G.KeyStrengths.Medium:
                KeyStrengthMedium.IsChecked = true;
                break;

            case G.KeyStrengths.High:
                KeyStrengthHigh.IsChecked = true;
                break;

            case G.KeyStrengths.Ultra:
                KeyStrengthUltra.IsChecked = true;
                break;
        }
    }

}