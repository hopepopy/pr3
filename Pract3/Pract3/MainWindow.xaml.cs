using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Pract3
{
    public partial class MainWindow : Window
    {
        List<string> songs;
        int current;
        bool isPause;
        bool isRepeat;
        bool isShuffled;
        public MainWindow()
        {
            InitializeComponent();
            songs = new List<string>();
            current = 0;
            isPause = false;
            isRepeat = false;
        }
        private void LoadSongs(string path)
        {
            string[] files = Directory.GetFiles(path);
            songs = new List<string>();
            List<string> names = new List<string>();
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                if (fileInfo.Extension == ".mp3" || fileInfo.Extension == ".wav" || fileInfo.Extension == ".m4a")
                {
                    songs.Add(file);
                    names.Add(fileInfo.Name);
                }
            }
            SongsListBox.ItemsSource = names.ToArray();
        }
        private void SelectSong(int index)
        {
            if (index > songs.Count - 1)
            {
                current = 0;
            }
            else if (index < 0)
            {
                current = songs.Count - 1;
            }
            else
            {
                current = index;
            }
            SongsListBox.SelectedIndex = current;
        }
        private void PlaySong()
        {
            PlayerElement.Source = new Uri(songs[current]);
            if (!isPause)
            {
                PlayerElement.Play();
            }
        }
        private void Pause()
        {
            if (isPause)
            {
                PlayerElement.Play();
                isPause = false;
            }
            else
            {
                PlayerElement.Pause();
                isPause = true;
            }
        }
        private void Shuffle()
        {
            if (isShuffled)
            {
                string song = songs[0];
                FileInfo file = new FileInfo(song);
                LoadSongs(file.Directory.FullName);
                isShuffled = false;
            }
            else
            {
                List<string> newSongs = new List<string>();
                List<string> newNames = new List<string>();

                while (songs.Count > 0)
                {
                    Random r = new Random();
                    int index = r.Next(songs.Count);
                    FileInfo file = new FileInfo(songs[index]);
                    newSongs.Add(songs[index]);
                    newNames.Add(file.Name);
                    songs.RemoveAt(index);
                }
                songs = newSongs;
                SongsListBox.ItemsSource = newNames.ToArray();
                isShuffled = true;
            }
            SelectSong(0);
            PlaySong();
        }
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog() { IsFolderPicker = true };
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                LoadSongs(dialog.FileName);
            }
            isRepeat = false;
            isPause = false;
            isShuffled = false;
            if (songs.Count > 0)
            {
                SelectSong(0);
                PlaySong();
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (songs.Count > 0)
            {
                SelectSong(current - 1);
                PlaySong();
            }
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (songs.Count > 0)
            {
                SelectSong(current + 1);
                PlaySong();
            }
        }
        private void PlayerElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            TimeSpan time = PlayerElement.NaturalDuration.TimeSpan;
            TimeSlider.Maximum = time.Ticks;
            TimeSlider.Value = 0;

            CurrentTimeText.Text = "0:00";
            string maxSeconds = time.Seconds >= 10 ? $"{time.Seconds}" : $"0{time.Seconds}";
            MaxTimeText.Text = $"{time.Minutes}:{maxSeconds}";
        }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (songs.Count > 0)
            {
                Pause();
            }
        }
        private void SongsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (songs.Count > 0)
            {
                SelectSong(SongsListBox.SelectedIndex);
                PlaySong();
            }
        }
        private void TimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimeSpan time = new TimeSpan(Convert.ToInt64(TimeSlider.Value));
            PlayerElement.Position = time;
            string seconds = time.Seconds >= 10 ? $"{time.Seconds}" : $"0{time.Seconds}";
            CurrentTimeText.Text = $"{time.Minutes}:{seconds}";
        }
        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (songs.Count > 0)
            {
                isRepeat = !isRepeat;
            }
        }
        private void PlayerElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (songs.Count > 0)
            {
                if (!isRepeat)
                {
                    SelectSong(current + 1);
                }
                PlaySong();
            }
        }
        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            if (songs.Count > 0)
            {
                Shuffle();
            }
        }
    }
}
