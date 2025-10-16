using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AemulusModManager
{
    /// <summary>
    /// Interaction logic for DownloadWindow.xaml
    /// </summary>
    public partial class DownloadWindow : Window
    {
        public bool YesNo = false;
        public DownloadWindow(GameBananaAPIV4 record)
        {
            InitializeComponent();
            DownloadText.Text = $"{record.Title}\nSubmitted by {record.Owner.Name}";
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = record.Image;
            bitmap.EndInit();
            Preview.Source = bitmap;
        }
        public DownloadWindow(GameBananaRecord record)
        {
            InitializeComponent();
            DownloadText.Text = $"{record.Title}\nSubmitted by {record.Owner.Name}";
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = record.Image;
            bitmap.EndInit();
            Preview.Source = bitmap;
        }
        public DownloadWindow(string name, string author, Uri image = null)
        {
            InitializeComponent();
            if (image != null)
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = image;
                bitmap.EndInit();
                Preview.Source = bitmap;
            }
            DownloadText.Text = $"{name}\nSubmitted by {author}";
        }
        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            YesNo = true;

            Close();
        }
        private void No_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
