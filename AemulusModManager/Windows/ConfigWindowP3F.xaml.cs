using AemulusModManager.Utilities.Windows;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace AemulusModManager
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindowP3F : Window
    {
        private MainWindow main;

        public ConfigWindowP3F(MainWindow _main)
        {
            main = _main;
            InitializeComponent();

            OutputTextbox.Text = main.modPath ?? "";
            ISOTextbox.Text = main.gamePath ?? "";
            PCSX2Textbox.Text = main.launcherPath ?? "";
            ELFTextbox.Text = main.elfPath ?? "";
            CheatsTextbox.Text = main.config.p3fConfig.cheatsPath ?? "";
            CheatsWSTextbox.Text = main.config.p3fConfig.cheatsWSPath ?? "";
            TexturesTextbox.Text = main.config.p3fConfig.texturesPath ?? "";

            AdvancedLaunchOptions.IsChecked = main.config.p3fConfig.advancedLaunchOptions;
            BuildFinishedBox.IsChecked = main.config.p3fConfig.buildFinished;
            BuildWarningBox.IsChecked = main.config.p3fConfig.buildWarning;
            ChangelogBox.IsChecked = main.config.p3fConfig.updateChangelog;
            DeleteBox.IsChecked = main.config.p3fConfig.deleteOldVersions;
            UpdateAllBox.IsChecked = main.config.p3fConfig.updateAll;
            UpdateBox.IsChecked = main.config.p3fConfig.updatesEnabled;
            UsePnachNewFormatBox.IsChecked = main.config.p3fConfig.usePnachNewFormat;
            Utilities.ParallelLogger.Log("[INFO] Config launched");
        }

        private void ModDirectoryClick(object sender, RoutedEventArgs e)
        {
            var directory = FilePicker.SelectFolder("Select output folder");
            if (directory == null)
                return;

            Utilities.ParallelLogger.Log($"[INFO] Setting output folder to {directory}");
            main.config.p3fConfig.modDir = directory;
            main.modPath = directory;
            main.updateConfig();

            main.MergeButton.IsHitTestVisible = true;
            main.MergeButton.Foreground = new SolidColorBrush(Color.FromRgb(0x6e, 0xb0, 0xf7));
            OutputTextbox.Text = directory;

        }

        private void CheatsDirectoryClick(object sender, RoutedEventArgs e)
        {
            var directory = FilePicker.SelectFolder("Select cheats folder");
            if (directory == null)
                return;

            Utilities.ParallelLogger.Log($"[INFO] Setting cheats folder to {directory}");
            main.config.p3fConfig.cheatsPath = directory;
            main.updateConfig();
            CheatsTextbox.Text = directory;
        }

        private void CheatsWSDirectoryClick(object sender, RoutedEventArgs e)
        {
            var directory = FilePicker.SelectFolder("Select cheats_ws folder");
            if (directory == null)
                return;

            Utilities.ParallelLogger.Log($"[INFO] Setting cheats_ws folder to {directory}");
            main.config.p3fConfig.cheatsWSPath = directory;
            main.updateConfig();
            CheatsWSTextbox.Text = directory;

        }

        private void TexturesDirectoryClick(object sender, RoutedEventArgs e)
        {
            var directory = FilePicker.SelectFolder("Select textures folder");
            if (directory == null)
                return;

            Utilities.ParallelLogger.Log($"[INFO] Setting textures folder to {directory}");
            main.config.p3fConfig.texturesPath = directory;
            main.updateConfig();
            TexturesTextbox.Text = directory;
        }

        private void BuildWarningChecked(object sender, RoutedEventArgs e)
        {
            main.buildWarning = true;
            main.config.p3fConfig.buildWarning = true;
            main.updateConfig();
        }

        private void BuildWarningUnchecked(object sender, RoutedEventArgs e)
        {
            main.buildWarning = false;
            main.config.p3fConfig.buildWarning = false;
            main.updateConfig();
        }

        private void BuildFinishedChecked(object sender, RoutedEventArgs e)
        {
            main.buildFinished = true;
            main.config.p3fConfig.buildFinished = true;
            main.updateConfig();
        }

        private void BuildFinishedUnchecked(object sender, RoutedEventArgs e)
        {
            main.buildFinished = false;
            main.config.p3fConfig.buildFinished = false;
            main.updateConfig();
        }

        private void ChangelogChecked(object sender, RoutedEventArgs e)
        {
            main.updateChangelog = true;
            main.config.p3fConfig.updateChangelog = true;
            main.updateConfig();
        }

        private void ChangelogUnchecked(object sender, RoutedEventArgs e)
        {
            main.updateChangelog = false;
            main.config.p3fConfig.updateChangelog = false;
            main.updateConfig();
        }

        private void UpdateAllChecked(object sender, RoutedEventArgs e)
        {
            main.updateAll = true;
            main.config.p3fConfig.updateAll = true;
            main.updateConfig();
        }

        private void UpdateAllUnchecked(object sender, RoutedEventArgs e)
        {
            main.updateAll = false;
            main.config.p3fConfig.updateAll = false;
            main.updateConfig();
        }

        private void UpdateChecked(object sender, RoutedEventArgs e)
        {
            main.updatesEnabled = true;
            main.config.p3fConfig.updatesEnabled = true;
            main.updateConfig();
            UpdateAllBox.IsEnabled = true;
        }

        private void UpdateUnchecked(object sender, RoutedEventArgs e)
        {
            main.updatesEnabled = false;
            main.config.p3fConfig.updatesEnabled = false;
            main.updateConfig();
            UpdateAllBox.IsChecked = false;
            UpdateAllBox.IsEnabled = false;
        }

        private void DeleteChecked(object sender, RoutedEventArgs e)
        {
            main.deleteOldVersions = true;
            main.config.p3fConfig.deleteOldVersions = true;
            main.updateConfig();
        }

        private void DeleteUnchecked(object sender, RoutedEventArgs e)
        {
            main.deleteOldVersions = false;
            main.config.p3fConfig.deleteOldVersions = false;
            main.updateConfig();
        }

        private void AdvancedLaunchOptionsChecked(object sender, RoutedEventArgs e)
        {
            main.p3fConfig.advancedLaunchOptions = true;
            main.updateConfig();
        }

        private void AdvancedLaunchOptionsUnchecked(object sender, RoutedEventArgs e)
        {
            main.p3fConfig.advancedLaunchOptions = false;
            main.updateConfig();
        }

        private void UsePnachNewFormatChecked(object sender, RoutedEventArgs e)
        {
            main.p3fConfig.usePnachNewFormat = true;
            main.updateConfig();
        }

        private void UsePnachNewFormatUnchecked(object sender, RoutedEventArgs e)
        {
            main.p3fConfig.usePnachNewFormat = false;
            main.updateConfig();
        }

        private void SetupISOShortcut(object sender, RoutedEventArgs e)
        {
            string p3fIso = FilePicker.SelectFile("Select Persona 3 FES ISO", Extensions.Ps2Iso);
            if (p3fIso == null)
                return;

            main.gamePath = p3fIso;
            main.config.p3fConfig.isoPath = p3fIso;
            main.updateConfig();
            ISOTextbox.Text = p3fIso;
        }

        private void SetupPCSX2Shortcut(object sender, RoutedEventArgs e)
        {
            string pcsx2Exe = FilePicker.SelectFile("Select pcsx2.exe", Extensions.Exe, mustContain: "pcsx2");
            if (pcsx2Exe == null)
                return;

            main.launcherPath = pcsx2Exe;
            main.config.p3fConfig.launcherPath = pcsx2Exe;
            main.updateConfig();
            PCSX2Textbox.Text = pcsx2Exe;
        }

        private void SetupELFShortcut(object sender, RoutedEventArgs e)
        {
            string elfFile = FilePicker.SelectFile("Select ELF/SLUS", Extensions.Elf);
            if (elfFile == null)
                return;

            try
            {
                // Read the first four bytes, verify that they end in "ELF"
                using BinaryReader reader = new BinaryReader(new FileStream(elfFile, FileMode.Open));
                string magic = Encoding.ASCII.GetString(reader.ReadBytes(4));

                if (!magic.EndsWith("ELF"))
                {
                    Utilities.ParallelLogger.Log("[ERROR] Invalid ELF/SLUS. The first 4 bytes must be \".ELF\" in ASCII");
                    return;
                }
            }

            catch (Exception ex)
            {
                Utilities.ParallelLogger.Log($"[ERROR] An exception occurred while trying to read the specified ELF/SLUS file: {ex.Message}");
                return;
            }

            main.elfPath = elfFile;
            main.config.p3fConfig.elfPath = elfFile;
            main.updateConfig();
            ELFTextbox.Text = elfFile;
        }

        // Use 7zip on iso
        private async void UnpackPacsClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(main.gamePath))
            {
                string selectedPath = FilePicker.SelectFile("Select P3F's iso to unpack", Extensions.Ps2Iso);
                if (selectedPath == null)
                {
                    Utilities.ParallelLogger.Log("[ERROR] Incorrect file chosen for unpacking.");
                    return;
                }

                main.gamePath = selectedPath;
                main.config.p3fConfig.isoPath = main.gamePath;
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

        private void OnClose(object sender, CancelEventArgs e)
        {
            Utilities.ParallelLogger.Log("[INFO] Config closed");
        }

    }
}
