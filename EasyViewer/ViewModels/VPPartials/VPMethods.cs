// ReSharper disable once CheckNamespace

namespace EasyViewer.ViewModels
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;
    using Caliburn.Micro;
    using LibVLCSharp.Shared;
    using Meta.Vlc.Interop.Core.Event;
    using static Helpers.DbMethods;
    using static Helpers.SystemVariables;


    public partial class VideoPlayerViewModel : Screen
    {
        /// <summary>
        ///     Начальные действия джамперов
        /// </summary>
        private async void StartJumperActions()
        {
            if (CurrentJumper.IsWorking) return;
            CurrentJumper.IsWorking = true;
            switch (CurrentJumper.JumperMode)
            {
                case JumperMode.Skip:
                    var time = (long) CurrentJumper.EndTime.TotalMilliseconds;
                    await Task.Run(() => MediaPlayer.Time = time);
                    break;
                case JumperMode.Mute:
                    await Task.Run(() => MediaPlayer.Mute = !MediaPlayer.Mute);
                    break;
                case JumperMode.IncreaseVolume:
                case JumperMode.LowerVolume:
                    await Task.Run(() => MediaPlayer.Volume = CurrentJumper.StandardVolumeValue);
                    break;
            }

            var currentIndex = Jumpers.IndexOf(CurrentJumper);
            CurrentJumper = currentIndex == Jumpers.Count - 1 ? null : Jumpers[currentIndex + 1];
        }

        /// <summary>
        /// Начать проигрывание эпизода
        /// </summary>
        private void PlayEpisode()
        {
            if (WatchingEpisodesCount <= 0)
            {
                _mmvm.IsViewingEnded = true;
                HotReg?.UnregisterHotkeys();
                _mmvm.CloseVideoPlayer();
            }
            else
            {
                CurrentEpisode = CheckedEpisodes[++CurrentEpisodeIndex];
                CurrentEpisodeTime = new TimeSpan();
                PlayVideo();
                if (WindowState == WindowState.Normal) SetFullScreen();
                CurrentEpisode.LastDateViewed = DateTime.Now;
                UpdateDbCollection(CurrentEpisode);
                if (IsEpisodeSkipped is false) WatchingEpisodesCount--;
                _mmvm.AvailableEpisodesCount--;
            }
        }

       private async void PlayVideo()
        {
            if (_mediaPlayer != null) await Task.Run(() => _mediaPlayer.Stop());

            if (_mediaPlayer == null)
            {
                _mediaPlayer = new MediaPlayer(_libVlc);
                _mediaPlayer.TimeChanged += MediaPlayerOnTimeChanged;
                _mediaPlayer.Playing += MediaPlayerOnPlaying;
                _mediaPlayer.Paused += MediaPlayerOnPaused;
                _mediaPlayer.EndReached += MediaPlayerOnEndReached;
            }

            var media = new Media(_libVlc, CurrentAddressInfo.Link);
            await Task.Run(() => _mediaPlayer.Volume = 100);

            NotifyOfPropertyChange(() => MediaPlayer);

            await Task.Run(() => _mediaPlayer.Play(media));
            
        }
    }

    
}