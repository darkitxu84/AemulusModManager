﻿using AemulusModManager.Utilities.Windows;
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
    public partial class ConfigWindowP4GVita : Window
    {
        private MainWindow main;
        private bool handled;

        public ConfigWindowP4GVita(MainWindow _main)
        {
            main = _main;
            InitializeComponent();

            OutputTextbox.Text = main.modPath ?? "";

            BuildFinishedBox.IsChecked = main.config.p4gVitaConfig.buildFinished;
            BuildWarningBox.IsChecked = main.config.p4gVitaConfig.buildWarning;
            ChangelogBox.IsChecked = main.config.p4gVitaConfig.updateChangelog;
            DeleteBox.IsChecked = main.config.p4gVitaConfig.deleteOldVersions;
            UpdateAllBox.IsChecked = main.config.p4gVitaConfig.updateAll;
            UpdateBox.IsChecked = main.config.p4gVitaConfig.updatesEnabled;

            switch (main.config.p4gVitaConfig.cpkName)
            {
                case "mod.cpk":
                    CPKBox.SelectedIndex = 0;
                    break;
                case "m0.cpk":
                    CPKBox.SelectedIndex = 1;
                    break;
                case "m1.cpk":
                    CPKBox.SelectedIndex = 2;
                    break;
                case "m2.cpk":
                    CPKBox.SelectedIndex = 3;
                    break;
                case "m3.cpk":
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
            main.config.p4gVitaConfig.modDir = directory;
            main.modPath = directory;
            main.updateConfig();

            main.MergeButton.IsHitTestVisible = true;
            main.MergeButton.Foreground = new SolidColorBrush(Color.FromRgb(0xf5, 0xa8, 0x3d));
            OutputTextbox.Text = directory;
        }

        private void BuildWarningChecked(object sender, RoutedEventArgs e)
        {
            main.buildWarning = true;
            main.config.p4gVitaConfig.buildWarning = true;
            main.updateConfig();
        }

        private void BuildWarningUnchecked(object sender, RoutedEventArgs e)
        {
            main.buildWarning = false;
            main.config.p4gVitaConfig.buildWarning = false;
            main.updateConfig();
        }

        private void BuildFinishedChecked(object sender, RoutedEventArgs e)
        {
            main.buildFinished = true;
            main.config.p4gVitaConfig.buildFinished = true;
            main.updateConfig();
        }

        private void BuildFinishedUnchecked(object sender, RoutedEventArgs e)
        {
            main.buildFinished = false;
            main.config.p4gVitaConfig.buildFinished = false;
            main.updateConfig();
        }

        private void ChangelogChecked(object sender, RoutedEventArgs e)
        {
            main.updateChangelog = true;
            main.config.p4gVitaConfig.updateChangelog = true;
            main.updateConfig();
        }

        private void ChangelogUnchecked(object sender, RoutedEventArgs e)
        {
            main.updateChangelog = false;
            main.config.p4gVitaConfig.updateChangelog = false;
            main.updateConfig();
        }

        private void UpdateAllChecked(object sender, RoutedEventArgs e)
        {
            main.updateAll = true;
            main.config.p4gVitaConfig.updateAll = true;
            main.updateConfig();
        }

        private void UpdateAllUnchecked(object sender, RoutedEventArgs e)
        {
            main.updateAll = false;
            main.config.p4gVitaConfig.updateAll = false;
            main.updateConfig();
        }

        private void UpdateChecked(object sender, RoutedEventArgs e)
        {
            main.updatesEnabled = true;
            main.config.p4gVitaConfig.updatesEnabled = true;
            main.updateConfig();
            UpdateAllBox.IsEnabled = true;
        }

        private void UpdateUnchecked(object sender, RoutedEventArgs e)
        {
            main.updatesEnabled = false;
            main.config.p4gVitaConfig.updatesEnabled = false;
            main.updateConfig();
            UpdateAllBox.IsChecked = false;
            UpdateAllBox.IsEnabled = false;
        }

        private void DeleteChecked(object sender, RoutedEventArgs e)
        {
            main.deleteOldVersions = true;
            main.config.p4gVitaConfig.deleteOldVersions = true;
            main.updateConfig();
        }

        private void DeleteUnchecked(object sender, RoutedEventArgs e)
        {
            main.deleteOldVersions = false;
            main.config.p4gVitaConfig.deleteOldVersions = false;
            main.updateConfig();
        }

        private async void UnpackPacsClick(object sender, RoutedEventArgs e)
        {
            string selectedPath = FilePicker.SelectFile("Select P4G Vita data.cpk to unpack", Extensions.Cpk);
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
                if (main.config.p4gVitaConfig.cpkName != cpkName)
                {
                    Utilities.ParallelLogger.Log($"[INFO] Output Cpk changed to {cpkName}");
                    main.config.p4gVitaConfig.cpkName = cpkName;
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
