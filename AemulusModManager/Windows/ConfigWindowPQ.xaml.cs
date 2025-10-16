using AemulusModManager.Utilities.Windows;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace AemulusModManager
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindowPQ : Window
    {
        private MainWindow main;

        public ConfigWindowPQ(MainWindow _main)
        {
            main = _main;
            InitializeComponent();

            OutputTextbox.Text = main.modPath ?? "";
            ROMTextbox.Text = main.gamePath ?? "";
            CitraTextbox.Text = main.launcherPath ?? "";

            BuildFinishedBox.IsChecked = main.config.pqConfig.buildFinished;
            BuildWarningBox.IsChecked = main.config.pqConfig.buildWarning;
            ChangelogBox.IsChecked = main.config.pqConfig.updateChangelog;
            DeleteBox.IsChecked = main.config.pqConfig.deleteOldVersions;
            UpdateAllBox.IsChecked = main.config.pqConfig.updateAll;
            UpdateBox.IsChecked = main.config.pqConfig.updatesEnabled;
            Utilities.ParallelLogger.Log("[INFO] Config launched");
        }

        private void ModDirectoryClick(object sender, RoutedEventArgs e)
        {
            var directory = FilePicker.SelectFolder("Select output folder");
            if (directory == null)
                return;

            Utilities.ParallelLogger.Log($"[INFO] Setting output folder to {directory}");
            main.config.pqConfig.modDir = directory;
            main.modPath = directory;
            main.updateConfig();

            main.MergeButton.IsHitTestVisible = true;
            main.MergeButton.Foreground = new SolidColorBrush(Color.FromRgb(0xfb, 0x84, 0x6a));
            OutputTextbox.Text = directory;
        }

        private void BuildWarningChecked(object sender, RoutedEventArgs e)
        {
            main.buildWarning = true;
            main.config.pqConfig.buildWarning = true;
            main.updateConfig();
        }

        private void BuildWarningUnchecked(object sender, RoutedEventArgs e)
        {
            main.buildWarning = false;
            main.config.pqConfig.buildWarning = false;
            main.updateConfig();
        }

        private void BuildFinishedChecked(object sender, RoutedEventArgs e)
        {
            main.buildFinished = true;
            main.config.pqConfig.buildFinished = true;
            main.updateConfig();
        }

        private void BuildFinishedUnchecked(object sender, RoutedEventArgs e)
        {
            main.buildFinished = false;
            main.config.pqConfig.buildFinished = false;
            main.updateConfig();
        }

        private void ChangelogChecked(object sender, RoutedEventArgs e)
        {
            main.updateChangelog = true;
            main.config.pqConfig.updateChangelog = true;
            main.updateConfig();
        }

        private void ChangelogUnchecked(object sender, RoutedEventArgs e)
        {
            main.updateChangelog = false;
            main.config.pqConfig.updateChangelog = false;
            main.updateConfig();
        }

        private void UpdateAllChecked(object sender, RoutedEventArgs e)
        {
            main.updateAll = true;
            main.config.pqConfig.updateAll = true;
            main.updateConfig();
        }

        private void UpdateAllUnchecked(object sender, RoutedEventArgs e)
        {
            main.updateAll = false;
            main.config.pqConfig.updateAll = false;
            main.updateConfig();
        }

        private void UpdateChecked(object sender, RoutedEventArgs e)
        {
            main.updatesEnabled = true;
            main.config.pqConfig.updatesEnabled = true;
            main.updateConfig();
            UpdateAllBox.IsEnabled = true;
        }

        private void UpdateUnchecked(object sender, RoutedEventArgs e)
        {
            main.updatesEnabled = false;
            main.config.pqConfig.updatesEnabled = false;
            main.updateConfig();
            UpdateAllBox.IsChecked = false;
            UpdateAllBox.IsEnabled = false;
        }

        private void DeleteChecked(object sender, RoutedEventArgs e)
        {
            main.deleteOldVersions = true;
            main.config.pqConfig.deleteOldVersions = true;
            main.updateConfig();
        }

        private void DeleteUnchecked(object sender, RoutedEventArgs e)
        {
            main.deleteOldVersions = false;
            main.config.pqConfig.deleteOldVersions = false;
            main.updateConfig();
        }

        private void SetupROMShortcut(object sender, RoutedEventArgs e)
        {
            string pqRom = FilePicker.SelectFile("Select Persona Q ROM", Extensions.N3dsRom);
            if (pqRom == null)
                return;

            main.gamePath = pqRom;
            main.config.pqConfig.ROMPath = pqRom;
            main.updateConfig();
            ROMTextbox.Text = pqRom;
        }

        private void SetupCitraShortcut(object sender, RoutedEventArgs e)
        {
            string[] ctrEmus =
            {
                "citra-qt.exe",
                "lime-qt.exe",
                "lime3ds-gui.exe",
                "lime3ds.exe",
                "azahar.exe"
            };
            string emuExe = FilePicker.SelectFile("Select Exectuable for Emulator", Extensions.Exe, ctrEmus);
            if (emuExe == null)
                return;

            main.launcherPath = emuExe;
            main.config.pqConfig.launcherPath = emuExe;
            main.updateConfig();
            CitraTextbox.Text = emuExe;
        }

        private async void UnpackPacsClick(object sender, RoutedEventArgs e)
        {
            string selectedPath = FilePicker.SelectFile("Select PQ data.cpk to unpack", Extensions.Cpk);
            if (selectedPath == null)
            {
                Utilities.ParallelLogger.Log("[ERROR] Incorrect file chosen for unpacking.");
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

        private void OnClose(object sender, CancelEventArgs e)
        {
            Utilities.ParallelLogger.Log("[INFO] Config closed");
        }
    }
}
