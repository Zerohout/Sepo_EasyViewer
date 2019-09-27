// ReSharper disable once CheckNamespace
// ReSharper disable CompareOfFloatsByEqualityOperator
namespace EasyViewer.ViewModels
{
    using System;
    using System.Timers;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Vlc.DotNet.Core;
    using static Helpers.GlobalMethods;
    using static Helpers.SystemVariables;
    using KeyEventArgs = System.Windows.Input.KeyEventArgs;
    using Screen = Caliburn.Micro.Screen;

    public partial class VideoPlayerViewModel : Screen
    {
        #region VideoPlayer events

        /// <summary>
        /// Проиграть/Поставить на паузу эпизод
        /// </summary>
        public void Play()
        {
            if (VlcPlayer.IsPlaying()) VlcPlayer.Pause();
            else VlcPlayer.Play();
        }

        /// <summary>
        /// Действие при выставлении фильма на паузу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VideoPlayer_Paused(object sender, VlcMediaPlayerPausedEventArgs e)
        {
            if (Topmost is true) Topmost = false;
            NotifyOfPropertyChange(() => IsPlaying);
        }

        /// <summary>
        /// Действие при проигрывании фильма
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VideoPlayer_Playing(object sender, VlcMediaPlayerPlayingEventArgs e)
        {
            if (Topmost is false) Topmost = true;
            NotifyOfPropertyChange(() => IsPlaying);
        }

        #endregion

        /// <summary>
        /// Действия при каждом тике таймера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (VlcPlayer == null) return;

            if (VlcPlayer.CouldPlay is false)
            {
                Vlc.Dispose();

                MMVM.CloseVideoPlayer();
            }

            if (VlcPlayer.IsPlaying() is false) return;

            CurrentEpisodeTime = TimeSpan.FromMilliseconds(VlcPlayer.Time);

            if (CurrentEpisodeTime >= CurrentEpisode.EpisodeEndTime)
            {
                PlayNextEpisode();
            }

            if (CurrentJumper == null) return;

            if (CurrentEpisodeTime >= CurrentJumper.StartTime && CurrentJumper.IsWorking is false)
            {
                StartJumperActions();
            }

            if (CurrentEpisodeTime >= CurrentJumper.EndTime && CurrentJumper.IsWorking is true)
            {
                EndJumperActions();
            }
        }

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
        public void KeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    MMVM.CloseVideoPlayer();
                    break;
                case Key.F:
                    SetFullScreen();
                    break;
                case Key.Right:
                    if (VlcPlayer.Time > CurrentEpisode.EpisodeEndTime.TotalMilliseconds - 5000)
                    {
                        VlcPlayer.Stop();
                    }
                    else
                    {
                        VlcPlayer.Time += 5000;
                    }
                    e.Handled = true;
                    break;
                case Key.Left:
                    if (VlcPlayer.Time < 5000)
                    {
                        VlcPlayer.Time = 0;
                    }
                    else
                    {
                        VlcPlayer.Time -= 5000;
                    }

                    e.Handled = true;
                    break;
            }
        }

        /// <summary>
        /// Перемещение видеоплеера
        /// </summary>
        public void MoveWindow()
        {
            (GetView() as Window)?.DragMove();
        }

        /// <summary>
        /// Показать/Скрыть панель управления видеоплеера
        /// </summary>
        public void ShowHideControl()
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
                var view = ((Window)GetView());
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
                UpdateDbCollection(obj: AppVal.WS);
            }
        }

        #endregion

    }
}
