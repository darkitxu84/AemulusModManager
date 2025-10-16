using AemulusModManager.Utilities.Windows;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AemulusModManager
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindowP3P : Window
    {
        private MainWindow main;
        private bool handled;

        public ConfigWindowP3P(MainWindow _main)
        {
            main = _main;
            InitializeComponent();

            OutputTextbox.Text = main.modPath ?? "";
            ISOTextbox.Text = main.gamePath ?? "";
            PPSSPPTextbox.Text = main.launcherPath ?? "";
            TexturesTextbox.Text = main.config.p3pConfig.texturesPath ?? "";
            CheatsTextbox.Text = main.config.p3pConfig.cheatsPath ?? "";

            BuildFinishedBox.IsChecked = main.config.p3pConfig.buildFinished;
            BuildWarningBox.IsChecked = main.config.p3pConfig.buildWarning;
            ChangelogBox.IsChecked = main.config.p3pConfig.updateChangelog;
            DeleteBox.IsChecked = main.config.p3pConfig.deleteOldVersions;
            UpdateAllBox.IsChecked = main.config.p3pConfig.updateAll;
            UpdateBox.IsChecked = main.config.p3pConfig.updatesEnabled;

            switch (main.config.p3pConfig.cpkName)
            {
                case "bind":
                    CPKBox.SelectedIndex = 0;
                    break;
                case "mod.cpk":
                    CPKBox.SelectedIndex = 1;
                    break;
                case "mod1.cpk":
                    CPKBox.SelectedIndex = 2;
                    break;
                case "mod2.cpk":
                    CPKBox.SelectedIndex = 3;
                    break;
                case "mod3.cpk":
                    CPKBox.SelectedIndex = 4;
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
            main.config.p3pConfig.modDir = directory;
            main.modPath = directory;
            main.updateConfig();

            main.MergeButton.IsHitTestVisible = true;
            main.MergeButton.Foreground = new SolidColorBrush(Color.FromRgb(0xfc, 0x83, 0xe3));
            OutputTextbox.Text = directory;
        }

        private void TextureDirectoryClick(object sender, RoutedEventArgs e)
        {
            var directory = FilePicker.SelectFolder("Select textures folder");
            if (directory == null)
                return;

            Utilities.ParallelLogger.Log($"[INFO] Setting textures folder to {directory}");
            main.config.p3pConfig.texturesPath = directory;
            main.updateConfig();
            TexturesTextbox.Text = directory;

        }

        private void CheatDirectoryClick(object sender, RoutedEventArgs e)
        {
            var iniFile = FilePicker.SelectFile("Select the P3P cheats ini (ULUS10512.ini)", Extensions.PpssppCheat);
            if (iniFile == null)
                return;

            Utilities.ParallelLogger.Log($"[INFO] Setting cheats ini to {iniFile}");
            main.config.p3pConfig.cheatsPath = iniFile;
            main.updateConfig();
            CheatsTextbox.Text = iniFile;
        }

        private void BuildWarningChecked(object sender, RoutedEventArgs e)
        {
            main.buildWarning = true;
            main.config.p3pConfig.buildWarning = true;
            main.updateConfig();
        }

        private void BuildWarningUnchecked(object sender, RoutedEventArgs e)
        {
            main.buildWarning = false;
            main.config.p3pConfig.buildWarning = false;
            main.updateConfig();
        }

        private void BuildFinishedChecked(object sender, RoutedEventArgs e)
        {
            main.buildFinished = true;
            main.config.p3pConfig.buildFinished = true;
            main.updateConfig();
        }

        private void BuildFinishedUnchecked(object sender, RoutedEventArgs e)
        {
            main.buildFinished = false;
            main.config.p3pConfig.buildFinished = false;
            main.updateConfig();
        }

        private void ChangelogChecked(object sender, RoutedEventArgs e)
        {
            main.updateChangelog = true;
            main.config.p3pConfig.updateChangelog = true;
            main.updateConfig();
        }

        private void ChangelogUnchecked(object sender, RoutedEventArgs e)
        {
            main.updateChangelog = false;
            main.config.p3pConfig.updateChangelog = false;
            main.updateConfig();
        }

        private void UpdateAllChecked(object sender, RoutedEventArgs e)
        {
            main.updateAll = true;
            main.config.p3pConfig.updateAll = true;
            main.updateConfig();
        }

        private void UpdateAllUnchecked(object sender, RoutedEventArgs e)
        {
            main.updateAll = false;
            main.config.p3pConfig.updateAll = false;
            main.updateConfig();
        }

        private void UpdateChecked(object sender, RoutedEventArgs e)
        {
            main.updatesEnabled = true;
            main.config.p3pConfig.updatesEnabled = true;
            main.updateConfig();
            UpdateAllBox.IsEnabled = true;
        }

        private void UpdateUnchecked(object sender, RoutedEventArgs e)
        {
            main.updatesEnabled = false;
            main.config.p3pConfig.updatesEnabled = false;
            main.updateConfig();
            UpdateAllBox.IsChecked = false;
            UpdateAllBox.IsEnabled = false;
        }

        private void DeleteChecked(object sender, RoutedEventArgs e)
        {
            main.deleteOldVersions = true;
            main.config.p3pConfig.deleteOldVersions = true;
            main.updateConfig();
        }

        private void DeleteUnchecked(object sender, RoutedEventArgs e)
        {
            main.deleteOldVersions = false;
            main.config.p3pConfig.deleteOldVersions = false;
            main.updateConfig();
        }

        private void SetupISOShortcut(object sender, RoutedEventArgs e)
        {
            string p3pISO = FilePicker.SelectFile("Select Persona 3 Portable ISO", Extensions.PspIso);
            if (p3pISO == null)
                return;

            main.gamePath = p3pISO;
            main.config.p3pConfig.isoPath = p3pISO;
            main.updateConfig();
            ISOTextbox.Text = p3pISO;
        }

        private void SetupPPSSPPShortcut(object sender, RoutedEventArgs e)
        {
            string[] ppssppExeNames =
            {
                "PPSSPPWindows.exe",
                "PPSSPPWindows64.exe"
            };
            string ppssppExe = FilePicker.SelectFile("Select PPSSPPWindows.exe/PPSSPPWindows64.exe", Extensions.Exe, ppssppExeNames);
            if (ppssppExe == null)
                return;

            main.launcherPath = ppssppExe;
            main.config.p3pConfig.launcherPath = ppssppExe;
            main.updateConfig();
            PPSSPPTextbox.Text = ppssppExe;
        }

        // Use 7zip on iso
        private async void UnpackPacsClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(main.gamePath))
            {
                string selectedPath = FilePicker.SelectFile("Select P3P ISO to unpack", Extensions.PspIso);
                if (selectedPath == null)
                {
                    Utilities.ParallelLogger.Log("[ERROR] Incorrect file chosen for unpacking.");
                    return;
                }

                main.gamePath = selectedPath;
                main.config.p3pConfig.isoPath = main.gamePath;
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
            await main.pacUnpack(main.gamePath);
            UnpackButton.IsHitTestVisible = true;
        }

        // Stops the user from changing the displayed "Notifications" text to the names of one of the combo boxes
        private void NotifBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            NotifBox.SelectedIndex = 0;
        }

        private void CPKBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;
            handled = true;
        }

        private void CPKBox_DropDownClosed(object sender, EventArgs e)
        {
            if (handled)
            {
                var cpkName = (CPKBox.SelectedValue as ComboBoxItem).Content as String;
                if (main.config.p3pConfig.cpkName != cpkName)
                {
                    Utilities.ParallelLogger.Log($"[INFO] Output changed to {cpkName}");
                    main.config.p3pConfig.cpkName = cpkName;
                    main.updateConfig();
                }
                handled = false;
            }
        }

        private void OnClose(object sender, CancelEventArgs e)
        {
            Utilities.ParallelLogger.Log("[INFO] Config closed");
        }
    }
}
