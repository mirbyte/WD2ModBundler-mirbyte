using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualBasic.Logging;
using Microsoft.Win32; // For OpenFileDialog
using WD2ModBundler.Helpers;
using WD2ModBundler.Services;

namespace WD2ModBundler
{
    public partial class MainWindow : Window
    {
        private MusicHelper? _musicHelper;
        private Action<string> _log;
        private bool _has7Zip;
        private bool _hasModFolder;
        private bool _hasPatchTools;


        //Method for MainWindow.Loaded event
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Start music automatically after window is fully rendered
            _musicHelper?.Play();
            RestoreSavedPaths();
        }

        private void RestoreSavedPaths()
        {
            PathSettings settings = PathSettings.Load();

            if (!string.IsNullOrWhiteSpace(settings.SevenZipPath) && File.Exists(settings.SevenZipPath))
            {
                ArchiveHelper.Set7ZipPath(settings.SevenZipPath);
                SevenZipPathTextBlock.Text = settings.SevenZipPath;
                SevenZipPathTextBlock.Foreground = Brushes.LightGreen;
                _log($"7-Zip restored: {settings.SevenZipPath}");
                _has7Zip = true;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(settings.SevenZipPath))
                    _log($"Saved 7-Zip path is missing: {settings.SevenZipPath}");

                string? sevenZip = ArchiveHelper.TryFind7Zip();
                if (!string.IsNullOrEmpty(sevenZip))
                {
                    SevenZipPathTextBlock.Text = $"Found: {sevenZip}";
                    SevenZipPathTextBlock.Foreground = Brushes.LightGreen;
                    _log($"7-Zip found: {sevenZip}");
                    _has7Zip = true;
                }
            }

            if (!string.IsNullOrWhiteSpace(settings.ModFolderPath) && Directory.Exists(settings.ModFolderPath))
            {
                ModFolderHelper.SetModFolderPath(settings.ModFolderPath);
                FolderPathTextBlock.Text = settings.ModFolderPath;
                FolderPathTextBlock.Foreground = Brushes.LightGreen;
                _log($"Mod folder restored: {settings.ModFolderPath}");
                _hasModFolder = true;
            }
            else if (!string.IsNullOrWhiteSpace(settings.ModFolderPath))
            {
                _log($"Saved mod folder is missing: {settings.ModFolderPath}");
            }

            if (!string.IsNullOrWhiteSpace(settings.PatchToolsPath))
            {
                try
                {
                    PatchToolsHelper.SetPatchToolsPath(settings.PatchToolsPath);
                    PatchToolsPath.Text = settings.PatchToolsPath;
                    PatchToolsPath.Foreground = Brushes.LightGreen;
                    _log($"Patch tools restored: {settings.PatchToolsPath}");
                    _hasPatchTools = true;
                }
                catch (Exception ex)
                {
                    _log($"Saved patch tools folder skipped: {ex.Message}");
                }
            }

            UpdateCombineButton();
        }

        private void Remember(Action<PathSettings> update)
        {
            try
            {
                PathSettings.Update(update);
            }
            catch (Exception ex)
            {
                _log($"Could not save settings: {ex.Message}");
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            // Dispatcher bellow is needed because:
            //    MainWindow constructor starts
            //   |
            //   | --> InitializeComponent()
            //   | (TextBox created but not fully rendered yet)
            //   |
            //   | --> MusicHelper constructor
            //         |
            //         | --> _player.Load()(blocking, synchronous)
            //         |
            //         | --> _log("WAV loaded") called
            //               |
            //               | --> Dispatcher.BeginInvoke queues action
            //               | (does NOT run yet)
            //   |
            //   | --> Constructor finishes, WPF finishes first render
            //         |
            //         | --> Dispatcher executes queued lambda
            //               mhlog?.Invoke("WAV loaded") runs on UI thread
            //               TextBox shows the message


            _log = message =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    Log(message);
                });
            };

            try
            {
                // Embedded WAV resource path (set Build Action = Embedded Resource)
                string wavResource = "WD2ModBundler.Assets.startup.wav";
                _log($"Loading WAV music from: {wavResource}");

                // Pass _log so MusicHelper can report its events
                _musicHelper = new MusicHelper(wavResource, _log);

                _log("MusicHelper initialized successfully!");

                // ASCII splash
                string resourcePath = "/WD2ModBundler;component/Assets/logo.png";
                string asciiArt = AsciiArtHelper.ConvertToAsciiFromResource(resourcePath, 80);
                LogTextBox.Text = asciiArt;
                LogTextBox.ScrollToEnd();

                // Subscribing MainWindow_Loaded method to MainWindow.Loaded event
                this.Loaded += MainWindow_Loaded;
            }
            catch (Exception ex)
            {
                _log($"Error in MainWindow constructor: {ex.Message}");
            }
        }



        #region File / Folder Selection Buttons
        //Bellow are different clickers
        private void Select7Zip_Click(object sender, RoutedEventArgs e)
        {
            string message = "Most common install paths are:\n" +
                             @"C:\Program Files\7-Zip" + "\n" +
                             @"C:\Program Files (x86)\7-Zip";

            MessageBox.Show(message, "7-Zip Info", MessageBoxButton.OK, MessageBoxImage.Information);

            string initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string? savedSevenZip = PathSettings.Load().SevenZipPath;
            if (!string.IsNullOrWhiteSpace(savedSevenZip))
            {
                string? savedDir = Path.GetDirectoryName(savedSevenZip);
                if (!string.IsNullOrEmpty(savedDir) && Directory.Exists(savedDir))
                    initialDirectory = savedDir;
            }

            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Select 7z.exe",
                Filter = "7-Zip Executable|7z.exe",
                InitialDirectory = initialDirectory
            };

            if (ofd.ShowDialog() == true)
            {
                string selectedPath = ofd.FileName;
                ArchiveHelper.Set7ZipPath(selectedPath);
                SevenZipPathTextBlock.Text = selectedPath;
                SevenZipPathTextBlock.Foreground = Brushes.LightGreen;
                _has7Zip = true;
                UpdateCombineButton();
                Remember(s => s.SevenZipPath = selectedPath);
            }
        }

        private void SelectModFolder_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Please select the folder containing mod archives.\nDo NOT extract the archives before selecting.",
                "Mod Folder Info", MessageBoxButton.OK, MessageBoxImage.Information);

            string? savedModFolder = PathSettings.Load().ModFolderPath;
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select folder with mod archives",
                SelectedPath = !string.IsNullOrWhiteSpace(savedModFolder) && Directory.Exists(savedModFolder) ? savedModFolder : ""
            })
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedPath = dialog.SelectedPath;
                    ModFolderHelper.SetModFolderPath(selectedPath);
                    FolderPathTextBlock.Text = selectedPath;
                    FolderPathTextBlock.Foreground = Brushes.LightGreen;
                    _hasModFolder = true;
                    UpdateCombineButton();
                    Remember(s => s.ModFolderPath = selectedPath);
                }
            }
        }

        private void SelectPatchTools_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Please select the folder containing WD2 Patch Tools.\nFolder MUST contain WD2Extract.exe and WD2Pack.exe.",
                "WD2 Patch Tools Info", MessageBoxButton.OK, MessageBoxImage.Information);

            string? savedPatchTools = PathSettings.Load().PatchToolsPath;
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select folder with WD2 Patch Tools",
                SelectedPath = !string.IsNullOrWhiteSpace(savedPatchTools) && Directory.Exists(savedPatchTools) ? savedPatchTools : ""
            })
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedPath = dialog.SelectedPath;
                    PatchToolsHelper.SetPatchToolsPath(selectedPath);
                    PatchToolsPath.Text = selectedPath;
                    PatchToolsPath.Foreground = Brushes.LightGreen;
                    _hasPatchTools = true;
                    UpdateCombineButton();
                    Remember(s => s.PatchToolsPath = selectedPath);
                }
            }
        }

        #endregion

        #region Music Controls

        private void PlayMusic_Click(object sender, RoutedEventArgs e)
        {
            if (_musicHelper != null)
                _musicHelper.Play();
            else
                _log("MusicHelper is null! Cannot play.");
        }

        private void StopMusic_Click(object sender, RoutedEventArgs e)
        {
            _musicHelper?.Stop();
        }

        #endregion

        #region Combine Mods

        private void UpdateCombineButton()
        {
            bool ready = _has7Zip && _hasModFolder && _hasPatchTools;
            CombineModsButton.Visibility = ready ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void CombineMods_Click(object sender, RoutedEventArgs e)  //Async ensures that UI does not stop
        {
            try
            {
                string modFolderPath = ModFolderHelper.GetModFolderPath();
                var service = new ModBundleService();

                await Task.Run(() =>
                {
                    service.CombineMods(
                        modFolderPath,
                        message => Dispatcher.BeginInvoke(() => _log(message)),
                        percent => Dispatcher.BeginInvoke(() => ProgressBar.Value = percent));
                });

                //Inform about CombineMods method finishing successfully. Executed on UI thread, no need to keep it in the ModBundleService.cs
                MessageBox.Show("Mods combined to MyModsBundle folder!", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, 
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region Logging
        //Logging method
        public void Log(string message)
        {
            string line = $"$ {message}{Environment.NewLine}";
            LogTextBox.AppendText(line);
            LogTextBox.CaretIndex = LogTextBox.Text.Length;
            LogTextBox.ScrollToEnd();
        }

        #endregion
    }
}