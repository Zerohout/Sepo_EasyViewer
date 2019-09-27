// ReSharper disable once CheckNamespace
namespace EasyViewer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Caliburn.Micro;
    using Models.FilmModels;
    using Vlc.DotNet.Wpf;
    using static Helpers.SystemVariables;
    using static Helpers.GlobalMethods;


    public partial class VideoPlayerViewModel : Screen
    {
        /// <summary>
        /// Создать видеоплеер
        /// </summary>
        private void CreatePlayer()
        {
            try
            {
                Vlc = new VlcControl();
                Vlc.SourceProvider.CreatePlayer(VlcDataPath);
                if (VlcPlayer == null)
                {
                    throw new Exception("Не удалось создать видеоплеер");
                }
                VlcPlayer.Playing += VideoPlayer_Playing;
                VlcPlayer.Paused += VideoPlayer_Paused;
                SetFullScreen();
                VlcPlayer.Play(new Uri(CurrentEpisode.SelectedAddress));

                while (VlcPlayer.IsPlaying() is false)
                {
                    Thread.Sleep(50);
                    if (VlcPlayer.CouldPlay is false)
                    {
                        throw new Exception("Видео недоступно. Проверьте ссылку " +
                                            $"({CurrentEpisode.SelectedAddress}) или интернет соединение");
                    }
                }
            }
            catch (Exception e)
            {
                WinMan.ShowDialog(new DialogViewModel("Ошибка при создании или проигрывании серии", DialogType.ERROR, e));
                MMVM.CloseVideoPlayer();
            }
        }
        
        /// <summary>
        /// Начать просмотр
        /// </summary>
        private void StartViewing()
        {
            CreateList();

            CurrentEpisode = CheckedEpisodes[CurrentEpisodeIndex];
            CurrentJumperIndex = Jumpers.Count > 0 ? 0 : -1;
            MainTimer.Start();
            CreatePlayer();
            CurrentEpisode.LastDateViewed = DateTime.Now;
            UpdateDbEpisode(CurrentEpisode);

            WatchingEpisodesCount--;
            MMVM.AvailableEpisodesCount--;
        }

        #region Методы джамперов

        /// <summary>
        /// Начальные действия таймеров
        /// </summary>
        private void StartJumperActions()
        {
            switch (CurrentJumper.JumperMode)
            {
                case JumperMode.Skip:
                    VlcPlayer.Time = (long)CurrentJumper.EndTime.TotalMilliseconds;
                    CurrentJumperIndex++;
                    return;
                case JumperMode.Mute:
                    VlcPlayer.Audio.ToggleMute();
                    break;
                case JumperMode.IncreaseVolume:
                    VlcPlayer.Audio.Volume = CurrentJumper.IncreasedVolumeValue;
                    break;
                case JumperMode.LowerVolume:
                    VlcPlayer.Audio.Volume = CurrentJumper.LoweredVolumeValue;
                    break;
            }

            CurrentJumper.IsWorking = true;
        }

        /// <summary>
        /// Конечные действия таймеров
        /// </summary>
        private void EndJumperActions()
        {
            switch (CurrentJumper.JumperMode)
            {
                case JumperMode.Mute:
                    VlcPlayer.Audio.ToggleMute();
                    break;
                case JumperMode.IncreaseVolume:
                case JumperMode.LowerVolume:
                    VlcPlayer.Audio.Volume = CurrentJumper.StandartVolumeValue;
                    break;
                default:
                    return;
            }

            CurrentJumper.IsWorking = false;
            CurrentJumperIndex++;
        }

        #endregion
        
        /// <summary>
        /// Воспроизвести следующий эпизод
        /// </summary>
        private void PlayNextEpisode()
        {
            if (WatchingEpisodesCount <= 0)
            {
                MMVM.IsViewingEnded = true;
                MainTimer.Stop();
                HotReg?.UnregisterHotkeys();
                MMVM.CloseVideoPlayer();
            }

            if (IsEpisodeSkipped is false)
            {
                WatchingEpisodesCount--;
            }

            MMVM.AvailableEpisodesCount--;

            CurrentEpisode = CheckedEpisodes[++CurrentEpisodeIndex];
            CurrentJumperIndex = Jumpers.Count > 0 ? 0 : -1;

            VlcPlayer.Play(new Uri(CurrentEpisode.SelectedAddress));
            CurrentEpisode.LastDateViewed = DateTime.Now;
            UpdateDbEpisode(CurrentEpisode);
        }



        /// <summary>
        /// Создает список (по порядку, перемешанный (с цепочкой эпизодов)).
        /// </summary>
        private void CreateList()
        {
            if (WS.RandomWatching)
            {
                CheckedEpisodes = ShuffleEpisodes(CheckedEpisodes.ToList(), WS.RandomMixCount ?? 1);

                if (WS.IsEpisodeChainActive)
                {
                    CheckedEpisodes = CreateEpisodeChains(CheckedEpisodes.ToList());
                }
            }
            else
            {
                CheckedEpisodes = CheckedEpisodes.OrderBy(ce => ce.FullNumber).ToList();
            }
        }

        /// <summary>
        /// Начать предпросмотр
        /// </summary>
        private void StartPreview()
        {
            CurrentJumperIndex = 0;
            CreatePlayer();
        }


        #region Creating chain of episodes

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

        #endregion

        #region Дополнительные методы

        /// <summary>
        /// Перемешать список эпизодов
        /// </summary>
        /// <param name="episodes">Список эпизодов</param>
        /// <param name="count">Количество перемешиваний</param>
        /// <returns></returns>
        private List<Episode> ShuffleEpisodes(List<Episode> episodes, int count)
        {
            rnd = new Random();
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
