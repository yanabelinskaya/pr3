using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Media;
using System.Windows.Controls;
using System.Runtime.ConstrainedExecution;

namespace пр3
{
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer;
        private List<string> playlist;
        private int currentTrackIndex;
        private bool isPlaying;
        private bool isRepeating;
        private bool isShuffling;
        private Random random;
        private Thread positionThread;
        private Thread durationThread;
        private List<int> listenedTracksIndexes;

        public MainWindow()
        {
            InitializeComponent();
            mediaPlayer = new MediaPlayer();
            listenedTracksIndexes = new List<int>();
            playlist = new List<string>();
            isPlaying = false;
            isRepeating = false;
            isShuffling = false;
            random = new Random();
            positionThread = new Thread(UpdatePosition);
            durationThread = new Thread(UpdateDuration);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            var result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                string[] files = Directory.GetFiles(dialog.FileName);
                foreach (string file in files)
                {
                    FileTxt.Items.Add(file);
                    playlist.Add(file);
                }
                if (playlist.Count > 0)
                {
                    PlayTrack(0);
                }
            }
        }

        private void PlayTrack(int index)
        {
            if (playlist.Count == 0) return;
            currentTrackIndex = index;
            mediaPlayer.Open(new Uri(playlist[currentTrackIndex]));
            mediaPlayer.Play();
            isPlaying = true;
            positionThread =new Thread(UpdatePosition);
            durationThread = new Thread(UpdateDuration);
            Thread currenTimeThread = new Thread(UpdateCurrentTime);
            Thread remainingTimeThread = new Thread(UpdateRemainingTime);
            positionThread.Start();
            durationThread.Start();
            currenTimeThread.Start();
            remainingTimeThread.Start();
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

            listenedTracksIndexes.Add(currentTrackIndex);
        }

        private void UpdateRemainingTime(object? obj)
        {
            while (isPlaying)
            {
                Thread.Sleep(1000);
                Dispatcher.Invoke(() =>
                {
                    TimeSpan remainingTime = mediaPlayer.NaturalDuration.TimeSpan - mediaPlayer.Position;
                    remainingTimeTextBlock.Text = "-" + remainingTime.ToString(@"mm\:ss");
                });
            }
        }

        private void UpdateCurrentTime(object? obj)
        {
            while(isPlaying)
            {
                Thread.Sleep(1000);
                Dispatcher.Invoke(() =>
                {
                    currentTimeTextBlock.Text = mediaPlayer.Position.ToString(@"mm\:ss");
                });
            }
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            if (currentTrackIndex < playlist.Count - 1)
            {
                currentTrackIndex++;
                PlayTrack(currentTrackIndex);
            }
            else
            {
                if (isRepeating)
                {
                    PlayTrack(currentTrackIndex);
                }
            }
        }

        private void UpdatePosition()
        {
            while (isPlaying)
            {
                Thread.Sleep(1000);
                Dispatcher.Invoke(() =>
                {
                    longPositionSlider.Value = mediaPlayer.Position.TotalSeconds;
                });
            }
        }

        private void UpdateDuration()
        {
            while (isPlaying)
            {
                Thread.Sleep(1000);
                Dispatcher.Invoke(() =>
                {
                    longPositionSlider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                    longPositionSlider.Value = mediaPlayer.Position.TotalSeconds;
                });
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (currentTrackIndex > 0)
            {
                currentTrackIndex--;
                PlayTrack(currentTrackIndex);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                mediaPlayer.Pause();
                isPlaying = false;
            }
            else
            {
                mediaPlayer.Play();
                isPlaying = true;
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (currentTrackIndex < playlist.Count - 1)
            {
                currentTrackIndex++;
                PlayTrack(currentTrackIndex);
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            isRepeating = !isRepeating;
            if(isRepeating)
            {
                PlayTrack(currentTrackIndex);
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            isShuffling = !isShuffling;
            if (isShuffling)
            {
                ShufflePlaylist();
                PlayRandomTrack();
            }
            else
            {
                playlist.Sort();
            }
        }


        private void PlayRandomTrack()
        {
            if (playlist.Count > 0)
            {
                int randomIndex = random.Next(playlist.Count);
                PlayTrack(randomIndex);
            }

        }

        private void ShufflePlaylist()
        {
            int n = playlist.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                string value = playlist[k];
                playlist[k] = playlist[n];
                playlist[n] = value;
            }
        }

        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = volumeSlider.Value;
        }

        private void longPositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (longPositionSlider.IsMouseCaptureWithin)
            {
                mediaPlayer.Position = TimeSpan.FromSeconds(longPositionSlider.Value);
            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Открыть окно с историей прослушивания
            List<string> listenedTracks = new List<string>();
            foreach(int index in listenedTracksIndexes)
            {
                listenedTracks.Add(playlist[index]);
            }
            Window1 Window1 = new Window1(playlist);
            Window1.Owner = this;
            Window1.ShowDialog();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FileTxt.SelectedItem != null)
            {
                int index = FileTxt.SelectedIndex;
                PlayTrack(index);
            }
        }

    }
}