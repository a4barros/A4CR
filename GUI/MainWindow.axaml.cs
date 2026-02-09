using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using a4crypt;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;

namespace GUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SelectFileButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ResetControls();
            var topLevel = GetTopLevel(this);

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open File",
                AllowMultiple = false,
            });

            W.selectedFilePath = files[0].Path.AbsolutePath;
            W.IsSelectedFileEncrypted = A4CryptFile.IsFileEncrypted(W.selectedFilePath);

            EncDecButton.IsEnabled = true;
            if (W.IsSelectedFileEncrypted)
            {
                EncDecButton.Content = "Decrypt";
                var fileHeaders = A4CryptFile.OpenHeadersOnly(W.selectedFilePath);
                EncryptionStatus.Content = $"{fileHeaders.KeyType} {fileHeaders.KeyStrength}";
            }
            else
            {
                EncDecButton.Content = "Encrypt";
                OptionsButton.IsEnabled = true;
            }
            FileLabel.Content = W.selectedFilePath.Split("/")[^1];
        }
        private async void EncryptButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var passwordDialog = new PasswordDialog();
            var password = await passwordDialog.ShowDialog<string>(this);

            if (password is null)
            {
                return;
            }

            try
            {
                if (W.IsSelectedFileEncrypted)
                {
                    A4CryptCore.Decrypt(
                        W.selectedFilePath,
                        W.selectedFilePath.Replace(".a4cr", ""),
                        password
                    );
                }
                else
                {
                    A4CryptCore.Encrypt(
                        W.selectedFilePath,
                        $"{W.selectedFilePath}.a4cr",
                        password,
                        W.SelectedKeyType,
                        W.SelectedKeyStrength);
                }
            }
            catch (Exception ex)
            {
                var error = new ErrorDialog(ex.Message);
                await error.ShowDialog(this);
            }
            finally
            {
                W.selectedFilePath = "";
                ResetControls();
            }

        }

        private async void Window_Drop(object? sender, DragEventArgs e)
        {
            var files = e.DataTransfer.TryGetFiles();
            if (files != null)
            {
                if (files.Length > 1)
                {
                    var error = new ErrorDialog("You can drop a single file only");
                    await error.ShowDialog(this);
                }
                Console.WriteLine(files[0].Path.AbsolutePath);
            }
        }

        private async void OptionsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var options = new OptionsWindow();
            await options.ShowDialog(this);
        }

        void ResetControls()
        {
            EncDecButton.IsEnabled = false;
            OptionsButton.IsEnabled = false;
            EncDecButton.Content = "Encrypt/Decrypt";
            FileLabel.Content = "(no file)";
            EncryptionStatus.Content = "";
        }
    }
}