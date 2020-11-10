// ReSharper disable once CheckNamespace

namespace EasyViewer.MainMenu.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.Micro;
    using EasyViewer.ViewModels;
    using Models.FilmModels;
    using static Helpers.GlobalMethods;
    using static Helpers.SystemVariables;
    using static Helpers.DbMethods;

    public partial class MainMenuViewModel : Screen
    {
        private void CollapseMainWindow()
        {
            var mainView = (MainViewModel) Parent;
            if (mainView.WindowState == WindowState.Normal)
            {
                mainView.WindowState = WindowState.Minimized;
            }
            else
            {
                mainView.WindowState = WindowState.Normal;
            }
        }

        /// <summary>
        /// Загрузка списка фильмов из базы данных
        /// </summary>
        private void LoadFilms()
        {
            Films = new BindableCollection<Film>(GetDbCollection<Film>());
        }

        /// <summary>
        /// Установить значения эпизодов
        /// </summary>
        private void SetEpisodesValues()
        {
            if (CheckedEpisodes.Count <= 0)
            {
                AvailableEpisodesCount = 0;
                WatchingEpisodesCount = 0;
            }
            else
            {
                AvailableEpisodesCount = CheckedEpisodes.Count;
                WatchingEpisodesCount = AppVal.WS.DefaultEpCount;
            }
        }

        /// <summary>
        /// Загрузка данных
        /// </summary>
        private Task LoadData()
        {
            return Task.Run(() =>
            {
                AppVal.WS = LoadOrCreateWatchingSettings();
                Films = new BindableCollection<Film>(GetDbCollection<Film>());
                _checkedFilmsCount = _films.Count(f => f.Checked);
                CheckedEpisodes = new List<Episode>(Films
                    .Where(f => f.Checked)
                    .SelectMany(f => f.CheckedEpisodes));
                CreateList();
            });
        }

        #region Просмотр серий

        /// <summary>
        /// Закрытие видеоплеера
        /// </summary>
        public void CloseVideoPlayer()
        {
            VideoPlayer.TryClose();
            VideoPlayer = null;
            NotifyOfPropertyChange(() => CanStart);
            NotifyOfPropertyChange(() => EpisodesCountRemainingString);
            if (Tray.Visible)
            {
                ActivateDeactivateTray((Window) ((MainViewModel) Parent).GetView());
            }

            if (AppVal.WS.NightHelperShutdown)
            {
                if (AppVal.WS.NightHelperStartTime <= DateTime.Now.Hour ||
                    AppVal.WS.NightHelperEndTime >= DateTime.Now.Hour)
                {
                    if (IsShutdownComp is false) IsShutdownComp = true;
                }
            }

            if (IsViewingEnded && IsShutdownComp)
            {
                var psi = new ProcessStartInfo("shutdown", "/s /t 10")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                Process.Start(psi);
                Exit();
            }
        }

        private Task IncreaseWatchingEpisodes()
        {
            return Task.Run(() =>
            {
                if (WatchingEpisodesCount < AvailableEpisodesCount) WatchingEpisodesCount++;
            });
        }

        private Task DecreaseWatchingEpisodes()
        {
            return Task.Run(() =>
            {
                if (WatchingEpisodesCount > 0) WatchingEpisodesCount--;
            });
        }

        #endregion

        #region Обработка выбранных эпизодов

        /// <summary>
        /// Создает список (по порядку, перемешанный (с цепочкой эпизодов)).
        /// </summary>
        private void CreateList()
        {
            if (AppVal.WS.RandomWatching)
            {
                _checkedEpisodes = ShuffleEpisodes(_checkedEpisodes.ToList(), AppVal.WS.RandomMixCount ?? 1);

                if (AppVal.WS.IsEpisodeChainActive)
                {
                    _checkedEpisodes = CreateEpisodeChains(CheckedEpisodes.ToList());
                }
            }
            else
            {
                _checkedEpisodes = CheckedEpisodes.OrderBy(ce => ce.FullNumber).ToList();
            }

            NotifyOfPropertyChange(() => CheckedEpisodes);
        }

        /// <summary>
        /// Создание цепочки эпизодов
        /// </summary>
        /// <param name="episodes"></param>
        /// <returns></returns>
        private List<Episode> CreateEpisodeChains(List<Episode> episodes)
        {
            var count = episodes.Count;

            while (CheckChain(episodes))
            {
                for (var i = 0; i < count; i++)
                {
                    var num = i;
                    var prevChainLink = episodes[num].PrevChainLink;

                    if (prevChainLink <= 0) continue;

                    var chain = FindChain(episodes, prevChainLink);

                    if (chain == null) continue;

                    if (num + chain.Count >= count)
                    {
                        num = count - chain.Count;
                    }

                    foreach (var index in chain.Select(link => episodes.IndexOf(link)))
                    {
                        var temp = episodes[num];
                        episodes[num] = episodes[index];
                        episodes[index] = temp;
                        num++;
                    }

                    i = num - 1;
                }
            }

            return episodes;
        }

        /// <summary>
        /// Нахождение и составление звеньев
        /// </summary>
        /// <param name="episodes">Список эпизодов</param>
        /// <param name="prevNum">Номер предыдущего эпизода в цепочке</param>
        /// <returns></returns>
        private List<Episode> FindChain(List<Episode> episodes, int prevNum)
        {
            var prevEpisode = episodes.FirstOrDefault(e => e.FullNumber == prevNum);

            if (prevEpisode == null) return null;

            var elem = episodes[episodes.IndexOf(prevEpisode)];

            if (elem.PrevChainLink > 0)
            {
                FindChain(episodes, elem.PrevChainLink);
            }

            var result = new List<Episode>();
            while (true)
            {
                result.Add(elem);
                elem = episodes.FirstOrDefault(e => e.FullNumber == elem.NextChainLink);

                if (elem == null) break;
            }

            return result;
        }

        /// <summary>
        /// Проверка сборки цепочки эпизодов
        /// </summary>
        /// <param name="episodes">Список эпизодов</param>
        /// <returns></returns>
        private bool CheckChain(List<Episode> episodes)
        {
            var count = episodes.Count;

            for (var i = 0; i < count; i++)
            {
                var num = i;
                var prev = episodes[num].PrevChainLink;
                var next = episodes[num].NextChainLink;

                if (prev == 0 && next == 0) continue;

                if (prev > 0)
                {
                    if (episodes.FirstOrDefault(e => e.FullNumber == prev) == null) continue;
                    if (num == 0 || prev != episodes[num - 1].FullNumber) return true;
                }

                if (next > 0)
                {
                    if (episodes.FirstOrDefault(e => e.FullNumber == next) == null) continue;
                    if (num == count - 1 || next != episodes[num + 1].FullNumber) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Перемешать список эпизодов
        /// </summary>
        /// <param name="episodes">Список эпизодов</param>
        /// <param name="count">Количество перемешиваний</param>
        /// <returns></returns>
        private List<Episode> ShuffleEpisodes(List<Episode> episodes, int count)
        {
            var rnd = new Random();
            for (var i = 0; i < count; i++)
            {
                for (var j = episodes.Count - 1; j >= 1; j--)
                {
                    var k = rnd.Next(j + 1);
                    var temp = episodes[k];
                    episodes[k] = episodes[j];
                    episodes[j] = temp;
                }
            }

            return episodes;
        }

        #endregion
    }
}