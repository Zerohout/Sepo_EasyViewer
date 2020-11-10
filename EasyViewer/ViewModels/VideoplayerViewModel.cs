namespace EasyViewer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;
    using LibVLCSharp.Shared;
    using LibVLCSharp.WPF;
    using MainMenu.ViewModels;
    using Models.FilmModels;
    using Newtonsoft.Json;
    using Views;
    using static Helpers.SystemVariables;
    using Screen = Caliburn.Micro.Screen;

    public partial class VideoPlayerViewModel : Screen
    {
        public VideoPlayerViewModel()
        {
        }

        public VideoPlayerViewModel(MainMenuViewModel mmvm, int? epCount,
            List<Episode> checkedEpisodes)
        {
            _libVlc = new LibVLC();
            _mmvm = mmvm;
            WatchingEpisodesCount = epCount;
            CheckedEpisodes = checkedEpisodes;
        }

        protected override void OnInitialize()
        {
            HotReg.RegisterGlobalHotkey(SkipEpisode, Keys.Right, ModifierKeys.Control);
            HotReg.RegisterGlobalHotkey(WriteNumberEpisodeToFile, Keys.Insert, ModifierKeys.Control);
            HotReg.RegisterGlobalHotkey(WriteNumberEpisodeToFile, Keys.Oemtilde, ModifierKeys.Alt);
            base.OnInitialize();
        }

        private void SkipEpisode()
        {
            IsEpisodeSkipped = true;
            PlayEpisode();
        }

        private async void WriteNumberEpisodeToFile()
        {
            var path = $@"{AppDataPath}\EpisodesToEditing.txt";
            if (File.Exists(path) is false)
            {
               File.Create(path).Dispose();
            }
            var msg = $"Фильм - {CurrentEpisode.Film.Name}, Сезон - {CurrentEpisode.Season.Number},\n" +
                      $"Эпизод - {CurrentEpisode.Number} ({CurrentEpisode.FullNumber}), Адрес - {CurrentAddressInfo.Link},\n" +
                      $"****************************************************";
            using var tw = File.AppendText(path);
            await tw.WriteLineAsync(msg);
        }

        protected override void OnViewLoaded(object view)
        {
            if (view is VideoPlayerView playerView)
            {
                playerView.Left = AppVal.WS.VPStartupPos.X;
                playerView.Top = AppVal.WS.VPStartupPos.Y;
            }

            base.OnViewLoaded(view);
        }
    }
}