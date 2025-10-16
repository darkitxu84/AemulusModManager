using AemulusModManager.Utilities.Windows;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace AemulusModManager
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindowP1PSP : Window
    {
        private MainWindow main;

        public ConfigWindowP1PSP(MainWindow _main)
        {
            main = _main;
            InitializeComponent();

            OutputTextbox.Text = main.modPath ?? "";
            ISOTextbox.Text = main.gamePath ?? "";
            PPSSPPTextbox.Text = main.launcherPath ?? "";
            TexturesTextbox.Text = main.config.p1pspConfig.texturesPath ?? "";
            CheatsTextbox.Text = main.config.p1pspConfig.cheatsPath ?? "";

            BuildFinishedBox.IsChecked = main.config.p1pspConfig.buildFinished;
            BuildWarningBox.IsChecked = main.config.p1pspConfig.buildWarning;
            ChangelogBox.IsChecked = main.config.p1pspConfig.updateChangelog;
            DeleteBox.IsChecked = main.config.p1pspConfig.deleteOldVersions;
            CreateIsoBox.IsChecked = main.config.p1pspConfig.createIso;
            UpdateAllBox.IsChecked = main.config.p1pspConfig.updateAll;
            UpdateBox.IsChecked = main.config.p1pspConfig.updatesEnabled;

            Utilities.ParallelLogger.Log("[INFO] Config launched");
        }

        private void ModDirectoryClick(object sender, RoutedEventArgs e)
        {
            var directory = FilePicker.SelectFolder("Select output folder");
            if (directory == null)
                return;

            Utilities.ParallelLogger.Log($"[INFO] Setting output folder to {directory}");
            main.config.p1pspConfig.modDir = directory;
            main.modPath = directory;
            main.updateConfig();

            main.MergeButton.IsHitTestVisible = true;
            main.MergeButton.Foreground = new SolidColorBrush(Color.FromRgb(0xb6, 0x83, 0xfc));
            OutputTextbox.Text = directory;
        }

        private void TextureDirectoryClick(object sender, RoutedEventArgs e)
        {
            var directory = FilePicker.SelectFolder("Select textures folder");
            if (directory == null)
                return;

            Utilities.ParallelLogger.Log($"[INFO] Setting textures folder to {directory}");
            main.config.p1pspConfig.texturesPath = directory;
            main.updateConfig();
            TexturesTextbox.Text = directory;
        }

        private void CheatDirectoryClick(object sender, RoutedEventArgs e)
        {
            var file = FilePicker.SelectFile("Select the P1PSP cheats ini (ULUS10432.ini)", Extensions.PpssppCheat);
            if (file == null)
                return;

            Utilities.ParallelLogger.Log($"[INFO] Setting cheats ini to {file}");
            main.config.p1pspConfig.cheatsPath = file;
            main.updateConfig();
            CheatsTextbox.Text = file;
        }

        private void BuildWarningChecked(object sender, RoutedEventArgs e)
        {
            main.buildWarning = true;
            main.config.p1pspConfig.buildWarning = true;
            main.updateConfig();
        }

        private void BuildWarningUnchecked(object sender, RoutedEventArgs e)
        {
            main.buildWarning = false;
            main.config.p1pspConfig.buildWarning = false;
            main.updateConfig();
        }

        private void BuildFinishedChecked(object sender, RoutedEventArgs e)
        {
            main.buildFinished = true;
            main.config.p1pspConfig.buildFinished = true;
            main.updateConfig();
        }

        private void BuildFinishedUnchecked(object sender, RoutedEventArgs e)
        {
            main.buildFinished = false;
            main.config.p1pspConfig.buildFinished = false;
            main.updateConfig();
        }

        private void ChangelogChecked(object sender, RoutedEventArgs e)
        {
            main.updateChangelog = true;
            main.config.p1pspConfig.updateChangelog = true;
            main.updateConfig();
        }

        private void ChangelogUnchecked(object sender, RoutedEventArgs e)
        {
            main.updateChangelog = false;
            main.config.p1pspConfig.updateChangelog = false;
            main.updateConfig();
        }

        private void UpdateAllChecked(object sender, RoutedEventArgs e)
        {
            main.updateAll = true;
            main.config.p1pspConfig.updateAll = true;
            main.updateConfig();
        }

        private void UpdateAllUnchecked(object sender, RoutedEventArgs e)
        {
            main.updateAll = false;
            main.config.p1pspConfig.updateAll = false;
            main.updateConfig();
        }

        private void UpdateChecked(object sender, RoutedEventArgs e)
        {
            main.updatesEnabled = true;
            main.config.p1pspConfig.updatesEnabled = true;
            main.updateConfig();
            UpdateAllBox.IsEnabled = true;
        }

        private void UpdateUnchecked(object sender, RoutedEventArgs e)
        {
            main.updatesEnabled = false;
            main.config.p1pspConfig.updatesEnabled = false;
            main.updateConfig();
            UpdateAllBox.IsChecked = false;
            UpdateAllBox.IsEnabled = false;
        }

        private void DeleteChecked(object sender, RoutedEventArgs e)
        {
            main.deleteOldVersions = true;
            main.config.p1pspConfig.deleteOldVersions = true;
            main.updateConfig();
        }

        private void DeleteUnchecked(object sender, RoutedEventArgs e)
        {
            main.deleteOldVersions = false;
            main.config.p1pspConfig.deleteOldVersions = false;
            main.updateConfig();
        }

        private void SetupISOShortcut(object sender, RoutedEventArgs e)
        {
            string p1pspISO = FilePicker.SelectFile("Select Persona 1 (PSP) ISO", Extensions.PspIso);
            if (p1pspISO == null)
                return;

            Utilities.ParallelLogger.Log($"[INFO] Setting ISO file to {p1pspISO}");
            main.gamePath = p1pspISO;
            main.config.p1pspConfig.isoPath = p1pspISO;
            main.updateConfig();
            ISOTextbox.Text = p1pspISO;
        }

        private void SetupPPSSPPShortcut(object sender, RoutedEventArgs e)
        {
            string ppssppExe = FilePicker.SelectFile("Select PPSSPPWindows.exe/PPSSPPWindows64.exe", Extensions.Exe, mustContain: "ppssppwindows");
            if (ppssppExe == null)
                return;

            Utilities.ParallelLogger.Log($"[INFO] Setting PPSSPP exe to {ppssppExe}");
            main.launcherPath = ppssppExe;
            main.config.p1pspConfig.launcherPath = ppssppExe;
            main.updateConfig();
            PPSSPPTextbox.Text = ppssppExe;
        }

        // Use 7zip on iso
        private async void UnpackPacsClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(main.gamePath))
            {
                string selectedPath = FilePicker.SelectFile("Select P1PSP ISO to unpack", Extensions.PspIso);
                if (selectedPath == null)
                {
                    Utilities.ParallelLogger.Log("[ERROR] Incorrect file chosen for unpacking.");
                    return;
                }

                main.gamePath = selectedPath;
                main.config.p1pspConfig.isoPath = main.gamePath;
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

        private void CreateIsoBox_Checked(object sender, RoutedEventArgs e)
        {
            main.createIso = true;
            main.config.p1pspConfig.createIso = true;
            main.updateConfig();
        }

        private void CreateIsoBox_Unchecked(object sender, RoutedEventArgs e)
        {
            main.createIso = false;
            main.config.p1pspConfig.createIso = false;
            main.updateConfig();
        }

        private void OnClose(object sender, CancelEventArgs e)
        {
            Utilities.ParallelLogger.Log("[INFO] Config closed");
        }
    }
}
