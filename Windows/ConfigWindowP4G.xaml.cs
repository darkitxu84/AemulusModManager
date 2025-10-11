using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using AemulusModManager.Utilities.Windows;
using System.Collections.Generic;

namespace AemulusModManager
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindowP4G : Window
    {
        private MainWindow main;

        public ConfigWindowP4G(MainWindow _main)
        {
            main = _main;
            InitializeComponent();

            OutputTextbox.Text = main.modPath ?? "";
            P4GTextbox.Text = main.gamePath ?? "";
            ReloadedTextbox.Text = main.launcherPath ?? "";

            KeepSND.IsChecked = main.emptySND;
            CpkBox.IsChecked = main.useCpk;
            BuildFinishedBox.IsChecked = main.config.p4gConfig.buildFinished;
            BuildWarningBox.IsChecked = main.config.p4gConfig.buildWarning;
            ChangelogBox.IsChecked = main.config.p4gConfig.updateChangelog;
            DeleteBox.IsChecked = main.config.p4gConfig.deleteOldVersions;
            UpdateBox.IsChecked = main.config.p4gConfig.updatesEnabled;
            UpdateAllBox.IsChecked = main.config.p4gConfig.updateAll;

            switch (main.cpkLang)
            {
                case "data_e.cpk":
                    LanguageBox.SelectedIndex = 0;
                    break;
                case "data.cpk":
                    LanguageBox.SelectedIndex = 1;
                    break;
                case "data_c.cpk":
                    LanguageBox.SelectedIndex = 2;
                    break;
                case "data_k.cpk":
                    LanguageBox.SelectedIndex = 3;
                    break;
                default:
                    LanguageBox.SelectedIndex = 0;
                    main.cpkLang = "data_e.cpk";
                    main.config.p4gConfig.cpkLang = "data_e.cpk";
                    main.updateConfig();
                    break;
            }
            Utilities.ParallelLogger.Log("[INFO] Config launched");
        }

        private void ModDirectoryClick(object sender, RoutedEventArgs e)
        {
            var directory = FilePicker.SelectFolder("Select output folder");
            if (directory == null)
                return;

            Utilities.ParallelLogger.Log($"[INFO] Setting output folder to {directory}");
            main.config.p4gConfig.modDir = directory;
            main.modPath = directory;
            main.updateConfig();

            main.MergeButton.IsHitTestVisible = true;
            main.MergeButton.Foreground = new SolidColorBrush(Color.FromRgb(0xf5, 0xe6, 0x3d));
            OutputTextbox.Text = directory;
        }

        private void SndChecked(object sender, RoutedEventArgs e)
        {
            main.emptySND = true;
            main.config.p4gConfig.emptySND = true;
            main.updateConfig();
        }

        private void SndUnchecked(object sender, RoutedEventArgs e)
        {
            main.emptySND = false;
            main.config.p4gConfig.emptySND = false;
            main.updateConfig();
        }

        private void CpkChecked(object sender, RoutedEventArgs e)
        {
            main.useCpk = true;
            main.config.p4gConfig.useCpk = true;
            main.updateConfig();
        }

        private void CpkUnchecked(object sender, RoutedEventArgs e)
        {
            main.useCpk = false;
            main.config.p4gConfig.useCpk = false;
            main.updateConfig();
        }

        private void UpdateChecked(object sender, RoutedEventArgs e)
        {
            main.updatesEnabled = true;
            main.config.p4gConfig.updatesEnabled = true;
            main.updateConfig();
            UpdateAllBox.IsEnabled = true;
        }

        private void UpdateUnchecked(object sender, RoutedEventArgs e)
        {
            main.updatesEnabled = false;
            main.config.p4gConfig.updatesEnabled = false;
            main.updateConfig();
            UpdateAllBox.IsChecked = false;
            UpdateAllBox.IsEnabled = false;
        }

        private void UpdateAllChecked(object sender, RoutedEventArgs e)
        {
            main.updateAll = true;
            main.config.p4gConfig.updateAll = true;
            main.updateConfig();
        }

        private void UpdateAllUnchecked(object sender, RoutedEventArgs e)
        {
            main.updateAll = false;
            main.config.p4gConfig.updateAll = false;
            main.updateConfig();
        }

        private void BuildWarningChecked(object sender, RoutedEventArgs e)
        {
            main.buildWarning = true;
            main.config.p4gConfig.buildWarning = true;
            main.updateConfig();
        }

        private void BuildWarningUnchecked(object sender, RoutedEventArgs e)
        {
            main.buildWarning = false;
            main.config.p4gConfig.buildWarning = false;
            main.updateConfig();
        }

        private void BuildFinishedChecked(object sender, RoutedEventArgs e)
        {
            main.buildFinished = true;
            main.config.p4gConfig.buildFinished = true;
            main.updateConfig();
        }

        private void BuildFinishedUnchecked(object sender, RoutedEventArgs e)
        {
            main.buildFinished = false;
            main.config.p4gConfig.buildFinished = false;
            main.updateConfig();
        }

        private void ChangelogChecked(object sender, RoutedEventArgs e)
        {
            main.updateChangelog = true;
            main.config.p4gConfig.updateChangelog = true;
            main.updateConfig();
        }

        private void ChangelogUnchecked(object sender, RoutedEventArgs e)
        {
            main.updateChangelog = false;
            main.config.p4gConfig.updateChangelog = false;
            main.updateConfig();
        }

        private void DeleteChecked(object sender, RoutedEventArgs e)
        {
            main.deleteOldVersions = true;
            main.config.p4gConfig.deleteOldVersions = true;
            main.updateConfig();
        }

        private void DeleteUnchecked(object sender, RoutedEventArgs e)
        {
            main.deleteOldVersions = false;
            main.config.p4gConfig.deleteOldVersions = false;
            main.updateConfig();
        }

        private void SetupP4GShortcut(object sender, RoutedEventArgs e)
        {
            string p4gExe = FilePicker.SelectFile("Select P4G.exe", Extensions.Exe, exactMatch: "P4G.exe");
            if (p4gExe == null)
                return;

            main.gamePath = p4gExe;
            main.config.p4gConfig.exePath = p4gExe;
            main.updateConfig();
            P4GTextbox.Text = p4gExe;
        }

        private void SetupReloadedShortcut(object sender, RoutedEventArgs e)
        {
            string[] supportedReloadedExes =
            {
                "Reloaded-II.exe",
                "Reloaded-II32.exe"
            };
            string reloadedExe = FilePicker.SelectFile("Select Reloaded-II.exe", Extensions.Exe, supportedReloadedExes);
            if (reloadedExe == null)
                return;

            main.launcherPath = reloadedExe;
            main.config.p4gConfig.reloadedPath = reloadedExe;
            main.updateConfig();
            ReloadedTextbox.Text = reloadedExe;
        }

        private async void UnpackPacsClick(object sender, RoutedEventArgs e)
        {
            string directory;
            if (main.modPath != null && File.Exists($@"{Directory.GetParent(main.modPath)}\data00004.pac"))
                directory = Directory.GetParent(main.modPath).ToString();
            else
                directory = FilePicker.SelectFolder("Select P4G Game Directory");

            if (directory == null)
                return;
            if (!File.Exists($@"{directory}\{main.cpkLang}"))
            {
                Utilities.ParallelLogger.Log($"[ERROR] Invalid folder. Cannot find {main.cpkLang}");
                return;
            }

            UnpackButton.IsHitTestVisible = false;
            main.ModGrid.IsHitTestVisible = false;
            main.GameBox.IsHitTestVisible = false;
            foreach (var button in main.buttons)
            {
                button.Foreground = new SolidColorBrush(Colors.Gray);
                button.IsHitTestVisible = false;
            }
            await main.pacUnpack(directory);
            UnpackButton.IsHitTestVisible = true;
        }
            
        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (LanguageBox.SelectedIndex != -1 && IsLoaded)
            {
                int index = LanguageBox.SelectedIndex;
                string selectedLanguage = null;
                switch (index)
                {
                    case 0:
                        selectedLanguage = "data_e.cpk";
                        break;
                    case 1:
                        selectedLanguage = "data.cpk";
                        break;
                    case 2:
                        selectedLanguage = "data_c.cpk";
                        break;
                    case 3:
                        selectedLanguage = "data_k.cpk";
                        break;
                }
                main.config.p4gConfig.cpkLang = selectedLanguage;
                main.cpkLang = selectedLanguage;
                main.updateConfig();
            }
        }

        // Stops the user from changing the displayed "Notifications" text to the names of one of the combo boxes
        private void NotifBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            NotifBox.SelectedIndex = 0;
        }

        private void OnClose(object sender, CancelEventArgs e)
        {
            Utilities.ParallelLogger.Log("[INFO] Config closed");
        }
    }
}
