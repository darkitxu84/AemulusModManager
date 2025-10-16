using AemulusModManager.Utilities.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace AemulusModManager
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindowP5 : Window
    {
        private MainWindow main;

        public ConfigWindowP5(MainWindow _main)
        {
            main = _main;
            InitializeComponent();

            OutputTextbox.Text = main.modPath ?? "";
            EBOOTTextbox.Text = main.gamePath ?? "";
            RPCS3Textbox.Text = main.launcherPath ?? "";
            CpkNameTextbox.Text = main.config.p5Config.cpkName ?? "";

            BuildFinishedBox.IsChecked = main.config.p5Config.buildFinished;
            BuildWarningBox.IsChecked = main.config.p5Config.buildWarning;
            ChangelogBox.IsChecked = main.config.p5Config.updateChangelog;
            DeleteBox.IsChecked = main.config.p5Config.deleteOldVersions;
            UpdateAllBox.IsChecked = main.config.p5Config.updateAll;
            UpdateBox.IsChecked = main.config.p5Config.updatesEnabled;
            Utilities.ParallelLogger.Log("[INFO] Config launched");
        }

        private void ModDirectoryClick(object sender, RoutedEventArgs e)
        {
            var directory = FilePicker.SelectFolder("Select output folder");
            if (directory == null)
                return;

            Utilities.ParallelLogger.Log($"[INFO] Setting output folder to {directory}");
            main.config.p5Config.modDir = directory;
            main.modPath = directory;
            main.updateConfig();

            main.MergeButton.IsHitTestVisible = true;
            main.MergeButton.Foreground = new SolidColorBrush(Color.FromRgb(0xfb, 0x51, 0x51));
            OutputTextbox.Text = directory;
        }

        private void BuildWarningChecked(object sender, RoutedEventArgs e)
        {
            main.buildWarning = true;
            main.config.p5Config.buildWarning = true;
            main.updateConfig();
        }

        private void BuildWarningUnchecked(object sender, RoutedEventArgs e)
        {
            main.buildWarning = false;
            main.config.p5Config.buildWarning = false;
            main.updateConfig();
        }

        private void BuildFinishedChecked(object sender, RoutedEventArgs e)
        {
            main.buildFinished = true;
            main.config.p5Config.buildFinished = true;
            main.updateConfig();
        }

        private void BuildFinishedUnchecked(object sender, RoutedEventArgs e)
        {
            main.buildFinished = false;
            main.config.p5Config.buildFinished = false;
            main.updateConfig();
        }

        private void ChangelogChecked(object sender, RoutedEventArgs e)
        {
            main.updateChangelog = true;
            main.config.p5Config.updateChangelog = true;
            main.updateConfig();
        }

        private void ChangelogUnchecked(object sender, RoutedEventArgs e)
        {
            main.updateChangelog = false;
            main.config.p5Config.updateChangelog = false;
            main.updateConfig();
        }

        private void UpdateAllChecked(object sender, RoutedEventArgs e)
        {
            main.updateAll = true;
            main.config.p5Config.updateAll = true;
            main.updateConfig();
        }

        private void UpdateAllUnchecked(object sender, RoutedEventArgs e)
        {
            main.updateAll = false;
            main.config.p5Config.updateAll = false;
            main.updateConfig();
        }

        private void UpdateChecked(object sender, RoutedEventArgs e)
        {
            main.updatesEnabled = true;
            main.config.p5Config.updatesEnabled = true;
            main.updateConfig();
            UpdateAllBox.IsEnabled = true;
        }

        private void UpdateUnchecked(object sender, RoutedEventArgs e)
        {
            main.updatesEnabled = false;
            main.config.p5Config.updatesEnabled = false;
            main.updateConfig();
            UpdateAllBox.IsChecked = false;
            UpdateAllBox.IsEnabled = false;
        }

        private void DeleteChecked(object sender, RoutedEventArgs e)
        {
            main.deleteOldVersions = true;
            main.config.p5Config.deleteOldVersions = true;
            main.updateConfig();
        }

        private void DeleteUnchecked(object sender, RoutedEventArgs e)
        {
            main.deleteOldVersions = false;
            main.config.p5Config.deleteOldVersions = false;
            main.updateConfig();
        }

        private void SetupEBOOTShortcut(object sender, RoutedEventArgs e)
        {
            string p5Eboot = selectExe("Select Persona 5 EBOOT.BIN", ".bin");
            if (p5Eboot != null && Path.GetFileName(p5Eboot).ToLower() == "eboot.bin")
            {
                main.gamePath = p5Eboot;
                main.config.p5Config.gamePath = p5Eboot;
                main.updateConfig();
                EBOOTTextbox.Text = p5Eboot;
            }
            else
            {
                Utilities.ParallelLogger.Log("[ERROR] Invalid EBOOT.BIN.");
            }
        }

        private void SetupRPCS3Shortcut(object sender, RoutedEventArgs e)
        {
            string rpcs3Exe = selectExe("Select rpcs3.exe", ".exe");
            if (Path.GetFileName(rpcs3Exe) == "rpcs3.exe")
            {
                main.launcherPath = rpcs3Exe;
                main.config.p5Config.launcherPath = rpcs3Exe;
                main.updateConfig();
                RPCS3Textbox.Text = rpcs3Exe;
            }
            else
            {
                Utilities.ParallelLogger.Log("[ERROR] Invalid exe.");
            }
        }

        private string selectExe(string title, string extension)
        {
            string type = "Application";
            if (extension == ".bin")
                type = "EBOOT";
            var openExe = new CommonOpenFileDialog();
            openExe.Filters.Add(new CommonFileDialogFilter(type, $"*{extension}"));
            openExe.EnsurePathExists = true;
            openExe.EnsureValidNames = true;
            openExe.Title = title;
            if (openExe.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return openExe.FileName;
            }
            return null;
        }

        // Use 7zip on iso
        private async void UnpackPacsClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(main.gamePath))
            {
                string selectedPath = FilePicker.SelectFile("Select P5's EBOOT.BIN to unpack", Extensions.Ps3Eboot, exactMatch: "EBOOT.BIN");
                if (selectedPath == null)
                {
                    Utilities.ParallelLogger.Log("[ERROR] Incorrect file chosen for unpacking.");
                    return;
                }

                main.gamePath = selectedPath;
                main.config.p5Config.gamePath = main.gamePath;
                main.updateConfig();
            }

            main.ModGrid.IsHitTestVisible = false;
            UnpackButton.IsHitTestVisible = false;
            foreach (var button in main.buttons)
            {
                button.IsHitTestVisible = false;
                button.Foreground = new SolidColorBrush(Colors.Gray);
            }
            main.GameBox.IsHitTestVisible = false;
            await main.pacUnpack(Path.GetDirectoryName(main.gamePath));
            UnpackButton.IsHitTestVisible = true;
        }

        // Stops the user from changing the displayed "Notifications" text to the names of one of the combo boxes
        private void NotifBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            NotifBox.SelectedIndex = 0;
        }

        private void OnClose(object sender, CancelEventArgs e)
        {
            if (main.config.p5Config.cpkName != CpkNameTextbox.Text)
            {
                Utilities.ParallelLogger.Log($"[INFO] Output Cpk changed to {CpkNameTextbox.Text}.cpk");
                main.config.p5Config.cpkName = CpkNameTextbox.Text;
                main.updateConfig();
            }
            Utilities.ParallelLogger.Log("[INFO] Config closed");
        }
    }
}
