// ReSharper disable once CheckNamespace
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace EasyViewer.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using Helpers.Creators;
    using LibVLCSharp.Shared;
    using LibVLCSharp.WPF;
    using Models.FilmModels;
    using Vlc.DotNet.Core;
    using Vlc.DotNet.Wpf;
    using static Helpers.DbMethods;
    using static Helpers.SystemVariables;
    using KeyEventArgs = System.Windows.Input.KeyEventArgs;
    using Screen = Caliburn.Micro.Screen;

    public partial class VideoPlayerViewModel : Screen
    {
        #region VideoPlayer events

        public void VideoViewLoaded(VideoView videoView)
        {
            PlayEpisode();
        }

        /// <summary>
        /// Проиграть/Поставить на паузу эпизод
        /// </summary>
        public async void PlayPause()
        {
            if (MediaPlayer.IsPlaying is true) await Task.Run(() =>MediaPlayer.Pause());
            else await Task.Run(() =>MediaPlayer.Play());
        }

        /// <summary>
        /// Перемотка эпизода назад на величину FFStep
        /// </summary>
        public async void FastForwardLeft()
        {
            await Task.Run(() =>
            {
                MediaPlayer.Time = MediaPlayer.Time <= RewindStep
                    ? 0
                    : MediaPlayer.Time - RewindStep;
            });
        }

        public bool CanFastForwardLeft => true;

        /// <summary>
        /// Перемотка эпизода вперёд на величину FFStep
        /// </summary>
        public async void FastForwardRight()
        {
            if (CanFastForwardRight is false) return;
            await Task.Run(() => MediaPlayer.Time += RewindStep);
        }

        public bool CanFastForwardRight => MediaPlayer?.Time < MediaPlayer?.Length - RewindStep;


        public async void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            if (MediaPlayer.IsPlaying is true) await Task.Run(() => MediaPlayer.Pause());
            var slider = sender as Slider;
        }

        public void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var slider = sender as Slider;
            CurrentEpisodeTime = TimeSpan.FromMilliseconds((long) slider.Value);
        }

        public async void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            var slider = sender as Slider;
            var value = slider.Value;
            await Task.Run(() => MediaPlayer.Time = (long) value);
            if (MediaPlayer.IsPlaying is false) MediaPlayer.Play();
        }

        private void MediaPlayerOnPaused(object sender, EventArgs e)
        {
            if (Topmost is true) Topmost = false;
            NotifyOfPropertyChange(() => IsPlaying);
        }

        private void MediaPlayerOnPlaying(object sender, EventArgs e)
        {
            if (Topmost is false) Topmost = true;
            NotifyOfPropertyChange(() => IsPlaying);
        }

        private  void MediaPlayerOnTimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            CurrentEpisodeTime = TimeSpan.FromMilliseconds(e.Time);
            if (CurrentJumper != null && CurrentEpisodeTime >= CurrentJumper.StartTime) StartJumperActions();
            if (CurrentEpisodeTime >= CurrentEpisode.AddressInfo.FilmEndTime) PlayEpisode();
        }

        private void MediaPlayerOnEndReached(object sender, EventArgs e)
        {
            PlayEpisode();
        }

        #endregion

        #region Window events

        /// <summary>
        /// Установить полноэкранный режим
        /// </summary>
        public void SetFullScreen()
        {
            if (WindowState == WindowState.Normal)
            {
                WindowVisibility = Visibility.Collapsed;
                WindowState = WindowState.Maximized;
                WindowVisibility = Visibility.Visible;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        /// <summary>
        /// Действие при нажатии клавиши клавиатуры
        /// </summary>
        /// <param name="e"></param>
        public void KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    _mmvm.CloseVideoPlayer();
                    break;
                case Key.F:
                    SetFullScreen();
                    break;
                case Key.Space:
                    PlayPause();
                    break;
                case Key.Z:
                    ShowHideControl(null, null);
                    break;
                case Key.Right:
                    FastForwardRight();
                    e.Handled = true;
                    break;
                case Key.Left:
                    FastForwardLeft();
                    e.Handled = true;
                    break;
                default: return;
            }
        }

        /// <summary>
        /// Перемещение видеоплеера
        /// </summary>
        public void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            (GetView() as Window)?.DragMove();
        }

        /// <summary>
        /// Показать/Скрыть панель управления видеоплеера
        /// </summary>
        public void ShowHideControl(object sender, MouseButtonEventArgs e)
        {
            ControlVisibility = ControlVisibility == Visibility.Collapsed
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        public void SaveChanges()
        {
            var hasChanges = false;
            if (WindowState == WindowState.Normal)
            {
                var view = ((Window) GetView());
                var winLoc = new Point(view.Left, view.Top);

                if (AppVal.WS.VPStartupPos != winLoc)
                {
                    AppVal.WS.VPStartupPos = winLoc;
                    hasChanges = true;
                }

                if (AppVal.WS.VPSize.X != WindowWidth ||
                    AppVal.WS.VPSize.Y != WindowHeight)
                {
                    AppVal.WS.VPSize = new Point(WindowWidth, WindowHeight);
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                UpdateDbCollection(entity: AppVal.WS);
            }
        }

        public override async void TryClose(bool? dialogResult = null)
        {
            SaveChanges();
            Topmost = false;
            await Task.Run(() => _mediaPlayer.Stop());
            await Task.Run(() => _mediaPlayer.Dispose());
            await Task.Run(() => _libVlc.Dispose());
            base.TryClose(dialogResult);
        }

        #endregion
    }
}