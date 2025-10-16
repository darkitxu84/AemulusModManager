using AemulusModManager.Utilities.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AemulusModManager
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindowP5RSwitch : Window
    {
        private MainWindow main;
        private bool language_handled;

        public ConfigWindowP5RSwitch(MainWindow _main)
        {
            main = _main;
            InitializeComponent();

            OutputTextbox.Text = main.modPath ?? "";
            ROMTextbox.Text = main.gamePath ?? "";
            EmulatorTextbox.Text = main.launcherPath ?? "";

            BuildFinishedBox.IsChecked = main.config.p5rSwitchConfig.buildFinished;
            BuildWarningBox.IsChecked = main.config.p5rSwitchConfig.buildWarning;
            ChangelogBox.IsChecked = main.config.p5rSwitchConfig.updateChangelog;
            DeleteBox.IsChecked = main.config.p5rSwitchConfig.deleteOldVersions;
            UpdateAllBox.IsChecked = main.config.p5rSwitchConfig.updateAll;
            UpdateBox.IsChecked = main.config.p5rSwitchConfig.updatesEnabled;

            switch (main.config.p5rSwitchConfig.language)
            {
                case "English":
                    LanguageBox.SelectedIndex = 0;
                    break;
                case "French":
                    LanguageBox.SelectedIndex = 1;
                    break;
                case "Italian":
                    LanguageBox.SelectedIndex = 2;
                    break;
                case "German":
                    LanguageBox.SelectedIndex = 3;
                    break;
                case "Spanish":
                    LanguageBox.SelectedIndex = 4;
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
            main.config.p5rSwitchConfig.modDir = directory;
            main.modPath = directory;
            main.updateConfig();

            main.MergeButton.IsHitTestVisible = true;
            main.MergeButton.Foreground = new SolidColorBrush(Color.FromRgb(0xf7, 0x64, 0x84));
            OutputTextbox.Text = directory;
        }

        private void SetupROMShortcut(object sender, RoutedEventArgs e)
        {
            string p5rRom = FilePicker.SelectFile("Select Persona 5 Royal (Switch) ROM", Extensions.SwitchRom);
            if (p5rRom == null)
                return;

            main.gamePath = p5rRom;
            main.config.p5rSwitchConfig.gamePath = p5rRom;
            main.updateConfig();
            ROMTextbox.Text = p5rRom;
        }

        private void SetupEmulatorShortcut(object sender, RoutedEventArgs e)
        {
            string[] supportedEmulatorsExes =
            {
                "yuzu.exe",
                "ryujinx.exe"
            };
            string emulatorExe = FilePicker.SelectFile("Select Exectuable for Emulator", Extensions.Exe, supportedEmulatorsExes);
            if (emulatorExe == null)
                return;

            main.launcherPath = emulatorExe;
            main.config.p5rSwitchConfig.launcherPath = emulatorExe;
            main.updateConfig();
            EmulatorTextbox.Text = emulatorExe;
        }

        private void BuildWarningChecked(object sender, RoutedEventArgs e)
        {
            main.buildWarning = true;
            main.config.p5rSwitchConfig.buildWarning = true;
            main.updateConfig();
        }

        private void BuildWarningUnchecked(object sender, RoutedEventArgs e)
        {
            main.buildWarning = false;
            main.config.p5rSwitchConfig.buildWarning = false;
            main.updateConfig();
        }

        private void BuildFinishedChecked(object sender, RoutedEventArgs e)
        {
            main.buildFinished = true;
            main.config.p5rSwitchConfig.buildFinished = true;
            main.updateConfig();
        }

        private void BuildFinishedUnchecked(object sender, RoutedEventArgs e)
        {
            main.buildFinished = false;
            main.config.p5rSwitchConfig.buildFinished = false;
            main.updateConfig();
        }

        private void ChangelogChecked(object sender, RoutedEventArgs e)
        {
            main.updateChangelog = true;
            main.config.p5rSwitchConfig.updateChangelog = true;
            main.updateConfig();
        }

        private void ChangelogUnchecked(object sender, RoutedEventArgs e)
        {
            main.updateChangelog = false;
            main.config.p5rSwitchConfig.updateChangelog = false;
            main.updateConfig();
        }

        private void UpdateAllChecked(object sender, RoutedEventArgs e)
        {
            main.updateAll = true;
            main.config.p5rSwitchConfig.updateAll = true;
            main.updateConfig();
        }

        private void UpdateAllUnchecked(object sender, RoutedEventArgs e)
        {
            main.updateAll = false;
            main.config.p5rSwitchConfig.updateAll = false;
            main.updateConfig();
        }

        private void UpdateChecked(object sender, RoutedEventArgs e)
        {
            main.updatesEnabled = true;
            main.config.p5rSwitchConfig.updatesEnabled = true;
            main.updateConfig();
            UpdateAllBox.IsEnabled = true;
        }

        private void UpdateUnchecked(object sender, RoutedEventArgs e)
        {
            main.updatesEnabled = false;
            main.config.p5rSwitchConfig.updatesEnabled = false;
            main.updateConfig();
            UpdateAllBox.IsChecked = false;
            UpdateAllBox.IsEnabled = false;
        }

        private void DeleteChecked(object sender, RoutedEventArgs e)
        {
            main.deleteOldVersions = true;
            main.config.p5rSwitchConfig.deleteOldVersions = true;
            main.updateConfig();
        }

        private void DeleteUnchecked(object sender, RoutedEventArgs e)
        {
            main.deleteOldVersions = false;
            main.config.p5rSwitchConfig.deleteOldVersions = false;
            main.updateConfig();
        }

        private async void UnpackPacsClick(object sender, RoutedEventArgs e)
        {
            string selectedPath = FilePicker.SelectFolder("Select folder with P5R cpks");
            if (selectedPath == null)
            {
                Utilities.ParallelLogger.Log("[ERROR] No folder chosen");
                return;

            }

            var cpksNeeded = new List<string>()
            {
                "ALL_USEU.CPK",
                "PATCH1.CPK"
            };
            var cpks = Directory.GetFiles(selectedPath, "*.cpk", SearchOption.TopDirectoryOnly);

            if (cpksNeeded.Except(cpks.Select(x => Path.GetFileName(x))).Any())
            {
                Utilities.ParallelLogger.Log($"[ERROR] Not all cpks needed (ALL_USEU.CPK and PATCH1.CPK) are found in top directory of {selectedPath}");
                return;
            }

            main.ModGrid.IsHitTestVisible = false;
            UnpackButton.IsHitTestVisible = false;
            foreach (var button in main.buttons)
            {
                button.IsHitTestVisible = false;
                button.Foreground = new SolidColorBrush(Colors.Gray);
            }
            main.GameBox.IsHitTestVisible = false;
            await main.pacUnpack(selectedPath);
            UnpackButton.IsHitTestVisible = true;
        }

        // Stops the user from changing the displayed "Notifications" text to the names of one of the combo boxes
        private void NotifBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            NotifBox.SelectedIndex = 0;
        }

        private void LanguageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;
            language_handled = true;
        }

        private void LanguageBox_DropDownClosed(object sender, EventArgs e)
        {
            if (language_handled)
            {
                var language = (LanguageBox.SelectedValue as ComboBoxItem).Content as String;
                if (main.config.p5rSwitchConfig.language != language)
                {
                    Utilities.ParallelLogger.Log($"[INFO] Language changed to {language}");
                    main.config.p5rSwitchConfig.language = language;
                    main.updateConfig();
                }
                language_handled = false;
            }
        }

        private void OnClose(object sender, CancelEventArgs e)
        {
            Utilities.ParallelLogger.Log("[INFO] Config closed");
        }
    }
}
