using System;
using System.IO;
using System.Net;
using Ionic.Zip;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;

namespace GameLauncher
{
    public partial class MainWindow : Window
    {
        private Button launchButton;
        private const string GameExecutable = "Wow.exe";
        private readonly string gamePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameFiles");
        private readonly string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameFiles", "Data");
        private readonly string enUSPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameFiles", "Data", "enUS");
        private readonly string tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");

        private readonly string gameLink = "https://drive.google.com/uc?export=download&id=1meP_47sEZo3iPuF4OlnIjKIjJv2jn6PV";
        private readonly string dataLink = "https://drive.usercontent.google.com/download?id=1aFw1cV3VBDKW8qUb0gJ1cwuGLFo4a7VT&export=download&authuser=0&confirm=t&uuid=aa9129f1-b197-4d0e-b9f9-730f7bd76646&at=APZUnTVP0FemiP3PB0XZcAKYUXOd%3A171882398569";
        private readonly string enUSLink = "https://drive.usercontent.google.com/download?id=1Vc79r-dcziBj_IW2qjmhS2GFejNqXyCs&export=download&authuser=0&confirm=t&uuid=74f09dee-93e0-4f59-a3c4-a7a19257b9f8&at=APZUnTU95XIprguL4IuNvHzmd2Kv%3A1718764791066D";
        private readonly string newsLink = "https://drive.google.com/uc?export=download&id=16NU-Ad2CgPZs2qMwla3QLSFFrK5tvi5M";

        public MainWindow()
        {
            InitializeComponent();
            launchButton = (Button)this.FindName("LaunchButton");
            CheckGameInstallation();
            LoadNews();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (launchButton.Content.ToString() == "Download")
            {
                DownloadAndInstallGame();
            }
            else if (launchButton.Content.ToString() == "Launch Game")
            {
                PlayGame();
            }
        }

        private void CheckGameInstallation()
        {
            if (File.Exists(Path.Combine(gamePath, GameExecutable)))
            {
                launchButton.Content = "Launch Game";
            }
            else
            {
                launchButton.Content = "Download";
            }
        }

        private void DownloadAndInstallGame()
        {
            try
            {
                CreateDirectories();

                string zipFilePath = DownloadZipFile(gameLink, tempPath);
                ExtractZipFile(zipFilePath, gamePath);

                string dataZipFilePath = DownloadZipFile(dataLink, tempPath);
                ExtractZipFile(dataZipFilePath, dataPath);

                string enUSZipFilePath = DownloadZipFile(enUSLink, tempPath);
                ExtractZipFile(enUSZipFilePath, enUSPath);

                if (File.Exists(Path.Combine(gamePath, GameExecutable)))
                {
                    launchButton.Content = "Launch Game";
                }
                else
                {
                    MessageBox.Show("Game executable not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during download and installation: {ex.Message}");
            }
        }

        private void PlayGame()
        {
            try
            {
                string gameExecutablePath = Path.Combine(gamePath, GameExecutable);
                if (File.Exists(gameExecutablePath))
                {
                    Process.Start(gameExecutablePath);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Game executable not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during game launch: {ex.Message}");
            }
        }

        private string DownloadZipFile(string url, string downloadPath)
        {
            using (WebClient client = new WebClient())
            {
                string fileName = Path.GetFileName(new Uri(url).LocalPath);
                string filePath = Path.Combine(downloadPath, fileName);
                client.DownloadFile(url, filePath);
                return filePath;
            }
        }

        private void ExtractZipFile(string zipFilePath, string extractPath)
        {
            if (File.Exists(zipFilePath))
            {
                using (Ionic.Zip.ZipFile archive = Ionic.Zip.ZipFile.Read(zipFilePath))
                {
                    archive.ExtractAll(extractPath, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                }
            }
            else
            {
                MessageBox.Show("Zip file not found.");
            }
        }

        private void CreateDirectories()
        {
            Directory.CreateDirectory(tempPath);
            Directory.CreateDirectory(gamePath);
            Directory.CreateDirectory(dataPath);
            Directory.CreateDirectory(enUSPath);
        }

        private void LoadNews()
        {
            string newsFilePath = Path.Combine(tempPath, "news.html");
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(newsLink, newsFilePath);
                }
                if (File.Exists(newsFilePath))
                {
                    NewsBrowser.Navigate(new Uri(newsFilePath));
                }
                else
                {
                    MessageBox.Show("News file not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading news: {ex.Message}");
            }
        }
    }
}
