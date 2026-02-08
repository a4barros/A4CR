using System;
using System.IO;
using System.Threading.Tasks;
using a4crypt;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace GUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public string selectedFilePath;

        private async void SelectFileButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open Text File",
                AllowMultiple = false
            });
            
            selectedFilePath = files[0].Path.ToString();
        }
        private void EncryptButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string[] x = selectedFilePath.Split(".");
            if (x.Length != 2)
            {
                throw new Exception("Invalid file path");
            }
            string beforeExtension = x[0];
            A4CryptCore.Encrypt(selectedFilePath, $"{beforeExtension}.a4cr", "123");
        }
    }
}