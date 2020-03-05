// ReSharper disable once CheckNamespace
namespace EasyViewer.MainMenu.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using Caliburn.Micro;
    using EasyViewer.ViewModels;
    using LiteDB;
    using Models.FilmModels;
    using static Helpers.GlobalMethods;
    using static Helpers.SystemVariables;

    public partial class MainMenuViewModel : Screen
    {
        /// <summary>
        /// Загрузка списка фильмов из базы данных
        /// </summary>
        public void LoadFilms()
        {
            using (var db = new LiteDatabase(DBPath))
            {
                var films = db.GetCollection<Film>();

                Films = new BindableCollection<Film>(films.FindAll());
            }
        }

        /// <summary>
        /// Установить значения эпизодов
        /// </summary>
        public void SetEpisodesValues()
        {
            if (_checkedEpisodes.Count <= 0)
            {
                AvailableEpisodesCount = 0;
                WatchingEpisodesCount = 0;
            }
            else
            {
                if (AvailableEpisodesCount == 0)
                {
                    WatchingEpisodesCount = AppVal.WS.DefaultEpCount ?? 0;
                }

                AvailableEpisodesCount = CheckedEpisodes.Count;
            }
        }

        /// <summary>
        /// Отфильтровать серии от просмотренных и не выбранных
        /// </summary>
        public void SetCheckedEpisodes()
        {
            CheckedEpisodes =
                new List<Episode>(Films
                                  .Where(s => s.Checked)
                                  .SelectMany(s => s
                                                  .GetEpisodes(true, AppVal.WS.NonRepeatDaysInterval ?? -1)));
        }
               
        #region Просмотр серий

        /// <summary>
        /// Закрытие видеоплеера
        /// </summary>
        public void CloseVideoPlayer()
        {
            VideoPlayer.SaveChanges();
            MainTimer.Stop();
            VideoPlayer.Topmost = false;
            Vlc.Dispose();
            VideoPlayer.TryClose();
            VideoPlayer = null;
            NotifyOfPropertyChange(() => CanStart);
            NotifyOfPropertyChange(() => EpisodesCountRemainingString);
            if (Tray.Visible)
            {
                ActivateDeactivateTray((Window)((MainViewModel)Parent).GetView());
            }
            if (IsViewingEnded)
            {
                if (IsShutdownComp)
                {
                    var psi = new ProcessStartInfo("shutdown", "/s /t 5")
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };

                    Process.Start(psi);

                    Exit();
                }
            }
        }


        public TimeSpan IntellectualShutdownTimer { get; set; }

        /// <summary>
        /// Метод начала просмотра серий
        /// </summary>
        private void StartWatch()
        {
            IntellectualShutdownTimer = new TimeSpan();

            //if (GeneralSettings.WatchingInRow is true)
            //{

            //    var number = GeneralSettings.LastWatchedEpisodeInRowFullNumber;
            //    var tempEpisode = CheckedEpisodes.First(ce => ce.FullNumber == number);
            //    CurrentEpisodeIndex = CheckedEpisodes.IndexOf(tempEpisode) + 1;

            //}
            //else
            //{
            //    CheckedEpisodes = ShuffleEpisode(CheckedEpisodes, GeneralSettings.RandomMixCount ?? 1);
            //    CurrentEpisodeIndex = 0;
            //}

            //while (GeneralSettings.EpisodesCount > 0 && GeneralSettings.AvailableEpisodesCount > 0)
            //{

            //    GeneralSettings.EpisodesCount--;
            //    NotifyOfPropertyChange(() => GeneralSettings);

            //    //цикл для переключения серии без потерь в количестве указанных просмотров
            //    do
            //    {
            //        IsSwitchEpisode = false;
            //        GeneralSettings.AvailableEpisodesCount--;
            //        TotalEpisodeTime = new TimeSpan();
            //        PlayEpisode(CheckedEpisodes[CurrentEpisodeIndex++]);

            //    } while (IsSwitchEpisode && GeneralSettings.AvailableEpisodesCount > 0);
            //}

            if (IsShutdownComp)
            {
                var psi = new ProcessStartInfo("shutdown", "/s /t 5")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                Process.Start(psi);
            }

            Exit();
        }



        //private void PlayEpisode(Episode episode)
        //{

        //    CurrentDuration = option.Duration;
        //    option.LastDateViewed = DateTime.Now;
        //    Jumpers = new List<Jumper>(option.Jumpers);
        //    CurrentJumper = Jumpers[jumperCount++];

        //    StartVideoPlayer();

        //    Helper.Timer.Restart();
        //    LaunchMonitoring();
        //    if (GeneralSettings.WatchingInRow)
        //    {
        //        GeneralSettings.LastWatchedEpisodeInRowFullNumber = episode.FullNumber;
        //    }
        //}



        ///// <summary>
        ///// Запуск серии в видео проигрывателе
        ///// </summary>
        //private void StartVideoPlayer()
        //{

        //    if (CurrentJumper.StartTime > new TimeSpan())
        //    {
        //        //IsDelayedSkip = true;
        //        return;
        //    }

        //    CurrentJumper = jumperCount < Jumpers.Count
        //        ? Jumpers[jumperCount++]
        //        : null;
        //}

        #endregion

        #region Мониторинг

        ///// <summary>
        ///// Запуск таймера мониторинга
        ///// </summary>
        //private void LaunchMonitoring()
        //{
        //    var autoEvent = new AutoResetEvent(false);

        //    var stateTimer = new Timer(MonitoringAction, autoEvent,
        //                               new TimeSpan(0, 0, 0),
        //                               new TimeSpan(0, 0, 1));
        //    autoEvent.WaitOne();
        //    autoEvent.Dispose();
        //    stateTimer.Dispose();
        //}

        ///// <summary>
        ///// Действия мониторинга
        ///// </summary>
        ///// <param name="stateInfo"></param>
        //private void MonitoringAction(object stateInfo)
        //{
        //    var autoEvent = (AutoResetEvent)stateInfo;

        //    NotifyOfPropertyChange(() => GeneralSettings);
        //    NotifyOfPropertyChange(() => EndDate);
        //    NotifyOfPropertyChange(() => EndTime);

        //    PauseMonitoring();

        //    if (Helper.Timer.IsRunning)
        //    {
        //        if (CurrentJumper != null)
        //        {
        //            if (TotalEpisodeTime >= CurrentJumper.StartTime &&
        //                TotalEpisodeTime < CurrentJumper.EndTime)
        //            {
        //                DelayedSkipMonitoring();
        //            }
        //        }

        //        // Активация выключения компьютера, при включенном ночном помощнике
        //        if (GeneralSettings.NightHelperShutdown is true && IsShutdownComp is false)
        //        {
        //            if (IntellectualShutdownTimer >= GeneralSettings.NightHelperShutdownTimeSpan &&
        //                GeneralSettings.NightHelperShutdownReachedTime > DateTime.Now.TimeOfDay)
        //            {
        //                IsShutdownComp = true;
        //            }

        //            IntellectualShutdownTimer += new TimeSpan(0, 0, 1);
        //        }

        //        TotalEpisodeTime += new TimeSpan(0, 0, 1);
        //    }

        //    //Таймер превысил длительность серии
        //    if (ElapsedTime > CurrentDuration || IsSwitchEpisode)
        //    {
        //        if (IsPaused is true)
        //        {
        //            IsPaused = false;
        //            IsNowPause = false;
        //        }

        //        jumperCount = 0;
        //        Helper.Timer.Reset();
        //        Helper.Msg.PressKey(VK_ESCAPE);
        //        autoEvent.Set();
        //    }

        //    TotalEpisodeTime += new TimeSpan(0, 0, 1);
        //}

        ///// <summary>
        ///// Действия после отложенного запуска
        ///// </summary>
        //private void DelayedSkipMonitoring()
        //{
        //    TotalEpisodeTime += new TimeSpan(0, 0, CurrentJumper.SkipCount * 5);
        //    Helper.Timer.Stop();

        //    CurrentJumper = jumperCount < Jumpers.Count
        //        ? Jumpers[jumperCount++]
        //        : null;

        //    Helper.Timer.Start();
        //}


        ///// <summary>
        ///// Мониторинг состояния паузы
        ///// </summary>
        //private void PauseMonitoring()
        //{
        //    //Серия поставлена на паузу
        //    if (IsPaused && Helper.Timer.IsRunning)
        //    {
        //        IsNowPause = true;
        //        Helper.Timer.Stop();
        //        ((MainViewModel)Parent).WindowState = WindowState.Normal;
        //        return;
        //    }

        //    //Серия снята с паузы
        //    if (!IsPaused && !Helper.Timer.IsRunning && IsNowPause)
        //    {
        //        Helper.Timer.Start();
        //        ((MainViewModel)Parent).WindowState = WindowState.Minimized;
        //        IsNowPause = false;
        //    }
        //}

        #endregion

        #region Дополнительные методы

        /// <summary>
        /// Перемешать эпизоды в списке
        /// </summary>
        /// <param name="list">Список эпизодов</param>
        /// <param name="count">Количество перемешиваний</param>
        /// <returns></returns>
        private List<Episode> ShuffleEpisodes(List<Episode> list, int count)
        {
            for (var i = 0; i < count; i++)
            {
                for (var j = list.Count - 1; j >= 1; j--)
                {
                    var k = rnd.Next(j + 1);
                    // обменять значения data[j] и data[i]
                    var temp = list[k];
                    list[k] = list[j];
                    list[j] = temp;
                }
            }

            return list;
        }

        #endregion
    }
}
