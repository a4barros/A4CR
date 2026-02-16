using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            string[] paths = files
                .Select(x => x.TryGetLocalPath() ?? "")
                .Where(p => p != null)
                .ToArray();
            
            ResetControls();
            await HandleFiles(paths);
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
        private async Task HandleFiles(IEnumerable<string> filePaths)
        {
            bool? firstState = null;

            foreach (var path in filePaths)
            {
                var isEncrypted = A4CryptFile.IsFileEncrypted(path);

                if (firstState == null)
                {
                    firstState = isEncrypted;
                    W.IsSelectedFilesEncrypted = isEncrypted;
                }
                else if (firstState != isEncrypted)
                {
                    var error = new ErrorDialog("You selected both encrypted and unencrypted files");
                    await error.ShowDialog(this);
                    ResetControls();
                    return;
                }

                W.selectedFilePathList.Add(path);
                FilesList.Items.Add(Path.GetFileName(path));
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

        private async void Drop(object? sender, DragEventArgs e)
        {
            if (!e.DataTransfer.Contains(DataFormat.File))
                return;

            var files = e.DataTransfer.TryGetFiles();

            if (files == null)
                return;

            string[] paths = files
                .Select(x => x.TryGetLocalPath() ?? "")
                .Where(p => p != null)
                .ToArray();

            await HandleFiles(paths);
        }

        private void DragOver(object? sender, DragEventArgs e)
        {
            if (e.DataTransfer.Contains(DataFormat.File))
            {
                e.DragEffects = DragDropEffects.Copy;
            }
            else
            {
                e.DragEffects = DragDropEffects.None;
            }
        }
    }
}