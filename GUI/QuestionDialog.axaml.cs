using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GUI;

public partial class QuestionDialog : Window
{
    public QuestionDialog(string question, string yesText = "Yes")
    {
        InitializeComponent();
        QuestionLabel.Content = question;
        ButtonYes.Content = yesText;
    }

    private void Button_Yes(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(true);
    }

    private void Button_No(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close(false);
    }
}