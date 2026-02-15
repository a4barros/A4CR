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
            var topLevel = GetTopLevel(this);

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open File",
                AllowMultiple = true,
            });

            if (files is null || files.Count < 1)
            {
                return;
            }

            ResetControls();

            var firstFile = files[0];
            W.IsSelectedFilesEncrypted = A4CryptFile.IsFileEncrypted(firstFile.TryGetLocalPath());

            foreach (var file in files)
            {
                var filePath = file.TryGetLocalPath();
                W.selectedFilePathList.Add(filePath);
                if (W.IsSelectedFilesEncrypted != A4CryptFile.IsFileEncrypted(filePath))
                {
                    var error = new ErrorDialog("You selected both encrypted and unencrypted files");
                    await error.ShowDialog(this);
                    ResetControls();
                }
                FilesList.Items.Add(filePath.Split("\\")[^1]);
            }

            EncDecButton.IsEnabled = true;
            if (W.IsSelectedFilesEncrypted)
            {
                EncDecButton.Content = "Decrypt";
            }
            else
            {
                EncDecButton.Content = "Encrypt";
                OptionsButton.IsEnabled = true;
            }
        }
        private async void EncryptButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string password, passwordConfirmation;
  
            var passwordDialog = new PasswordDialog();
            password = await passwordDialog.ShowDialog<string>(this);

            if (password is null)
            {
                return;
            }

            if (W.IsSelectedFilesEncrypted == false)
            {
                var passwordDialogConfirmation = new PasswordDialog(IsConfirmDialog: true);
                passwordConfirmation = await passwordDialogConfirmation.ShowDialog<string>(this);
                if (password != passwordConfirmation)
                {
                    var error = new ErrorDialog("The two passwords do not match");
                    await error.ShowDialog(this);
                    return;
                }
                if (passwordConfirmation is null)
                {
                    return;
                }
            }

            try
            {
                if (W.IsSelectedFilesEncrypted)
                {
                    foreach (var filePath in W.selectedFilePathList)
                    {
                        A4CryptCore.Decrypt(
                        filePath,
                        filePath.Replace(".a4cr", ""),
                        password
                        );
                    }
                    foreach (var filePath in W.selectedFilePathList)
                    {
                        File.Delete(filePath);
                    }
                }
                else
                {
                    foreach (var filePath in W.selectedFilePathList)
                    {
                        A4CryptCore.Encrypt(
                        filePath,
                        $"{filePath}.a4cr",
                        password,
                        W.SelectedKeyType,
                        W.SelectedKeyStrength);
                    }
                    var question = new QuestionDialog("Delete original file(s)?", "DELETE");
                    var deleteFiles = await question.ShowDialog<bool>(this);
                    if (deleteFiles)
                    {
                        foreach (var filePath in W.selectedFilePathList)
                        {
                            File.Delete(filePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var error = new ErrorDialog(ex.Message);
                await error.ShowDialog(this);
            }
            finally
            {
                ResetControls();
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
            W.selectedFilePathList.Clear();
            FilesList.Items.Clear();
        }
    }
}