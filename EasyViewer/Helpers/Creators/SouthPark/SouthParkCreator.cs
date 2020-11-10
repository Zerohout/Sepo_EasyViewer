namespace EasyViewer.Helpers.Creators.SouthPark
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using LibVLCSharp.Shared;
    using Models.FilmModels;
    using ViewModels;
    using Vlc.DotNet.Wpf;
    using static SystemVariables;
    using static SPOnline;
    using static SPFreehat;
    using static DbMethods;
    using static CreatorMethods;
    using Application = System.Windows.Application;

    public static class SouthParkCreator
    {
        public static readonly int SeasonCount = 24;

        private static LibVLC _libVlc;

        public static async Task CreateSP(WaitViewModel wvm)
        {
            //Core.Initialize(VlcDataPath.ToString());
            _libVlc = new LibVLC();
            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    Vlc = new VlcControl();
            //    Vlc.SourceProvider.CreatePlayer(VlcDataPath);
            //});

            var films = GetDbCollection<Film>();

            if (films.All(f => f.Name != SP))
            {
                var result = new Film
                {
                    Name = SP,
                    Description = "«Южный парк» (он же South Park и Саус Парк) — фильм картинками, " +
                                  "который отличается от других мультиков тем, что градус треша и угара в нем зашкаливает, " +
                                  "сферически воплощаясь в каждом из жителей маленького и нифига не тихого городка в Колорадо, " +
                                  "которые окружают главных героев: Эрик Картман — маленький расист с зашкаливающим чувством величия " +
                                  "и неуемной жаждой действий. Стэн Марш — обладатель критичного ума, офигевающий над своим папой " +
                                  "и креативным долбозвоном в одном лице, Рэнди Маршем. Кайл Брофловски — рыжеволосый и ироничный " +
                                  "персонаж, чье здравомыслие с лихвой уравнивает общую шизофрению происходящего (постоянный предмет " +
                                  "нападок жирдяя) и главный убиванец фильма, Кенни МакКормик — бичеватый индивид с капюшоном на " +
                                  "все лицо и пошловатой логикой микроскопического эротомана. Не менее колоритен ныне заменивший " +
                                  "вечно подыхающего коротыша другой мегаломаньяк — Профессор Хаос, ненавистник пиписек и " +
                                  "латентный гомосексуалист — Баттерс Стотч.",
                    FilmType = FilmType.Мультсериал
                };

                InsertEntityToDb(result);
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var film = GetDbCollection<Film>().Last();
            await AsyncCreateSpSeasons(film, wvm, stopwatch);
            await CreateSPEpisodes(film, wvm, stopwatch);
            await CreateSpAddresses(film, wvm, stopwatch);
            await AddSpAddressesDurations(film, wvm, stopwatch);
            await SetDefaultAddressInfoToEpisode(film);
        }

        /// <summary>
        /// Создать общий список сезонов в фильме (Южный Парк)
        /// </summary>
        /// <param name="film">Фильм (Южный Парк)</param>
        /// <param name="wvm">Индикатор прогресса загрузки</param>
        /// <param name="stopwatch">Таймер</param>
        private static async Task AsyncCreateSpSeasons(Film film, WaitViewModel wvm, Stopwatch stopwatch)
        {
            wvm.CurrentLoadingStatus = LoadingStatus.CreatingSeasons;
            var seasons = new List<Season>();
            if (await GetCancelCreateSeasonsResult(seasons)) return;

            var filmSeasons = await GetSeasonsByFilm(film, wvm, stopwatch);

            for (var i = 0; i < SeasonCount; i++)
            {
                if (await GetCancelCreateSeasonsResult(seasons)) return;
                var num = i + 1;
                wvm.CurrentSeasonValue++;
                wvm.CurrentPercentValue++;

                if (filmSeasons.Any(s => s.Number == num)) continue;

                var newSeason = new Season
                {
                    Number = num,
                    Description = $"Description of Season_{num}",
                    Film = film
                };

                seasons.Add(newSeason);
                UpdateOperationTime(wvm, stopwatch);
            }

            if (seasons.Count > 0) await Task.Run(() => InsertEntityListToDb(seasons));
        }

        private static Task<bool> GetCancelCreateSeasonsResult(List<Season> seasons)
        {
            return Task.Run(() =>
            {
                if (AddingFilmToken.IsCancellationRequested is false) return false;

                if (seasons.Count > 0) InsertEntityListToDb(seasons);
                return true;
            });
        }

        /// <summary>
        /// Создать общий список эпизодов в каждом сезоне фильма (Южный Парк)
        /// </summary>
        /// <param name="film">Фильм</param>
        /// <param name="wvm">Индикатор прогресса загрузки</param>
        /// <param name="stopwatch">Таймер</param>
        private static async Task CreateSPEpisodes(Film film, WaitViewModel wvm, Stopwatch stopwatch)
        {
            var episodes = new List<Episode>();
            wvm.CurrentLoadingStatus = LoadingStatus.CreatingEpisodes;
            if (await GetCancelCreateEpisodesResult(episodes)) return;
            var seasons = await GetSeasonsByFilm(film, wvm, stopwatch);
            var seasonEpisodes = await Task.Run(() => seasons.SelectMany(s => s.Episodes).ToList());
            var totalEpisodesCount = await Task.Run(() => seasons.Sum(season => GetSPEpisodesCount(season.Number)));
            wvm.MaxPercentValue += totalEpisodesCount;
            UpdateOperationTime(wvm, stopwatch);

            foreach (var season in seasons)
            {
                if (await GetCancelCreateEpisodesResult(episodes)) return;

                wvm.CurrentSeasonValue++;
                wvm.CurrentPercentValue++;

                var episodesCount = GetSPEpisodesCount(season.Number);
                wvm.ResetCurrentLoadingData(true);
                wvm.MaxEpisodeValue = episodesCount;

                for (var j = 0; j < episodesCount; j++)
                {
                    if (await GetCancelCreateEpisodesResult(episodes)) return;

                    wvm.CurrentEpisodeValue++;
                    wvm.CurrentPercentValue++;
                    var epNum = j + 1;
                    var epFullNum = season.Number * 100 + epNum;

                    if (seasonEpisodes.Any(e => e.FullNumber == epFullNum)) continue;

                    var newEpisode = new Episode
                    {
                        Name = $"Episode {epNum} of {season.Number} season",
                        Description = $"Description of {epNum} episode of {season.Number} season",
                        Number = epNum,
                        Season = season,
                        Film = film
                    };

                    episodes.Add(newEpisode);
                    UpdateOperationTime(wvm, stopwatch);
                }
            }

            if (episodes.Count > 0) await Task.Run(() => InsertEntityListToDb(episodes));
        }

        private static Task<bool> GetCancelCreateEpisodesResult(List<Episode> episodes)
        {
            return Task.Run(() =>
            {
                if (AddingFilmToken.IsCancellationRequested is false) return false;

                if (episodes.Count > 0) InsertEntityListToDb(episodes);
                return true;
            });
        }

        private static Task<List<Season>> GetSeasonsByFilm(Film film, WaitViewModel wvm, Stopwatch stopwatch)
        {
            return Task.Run(() =>
            {
                var seasons = GetSeasonListFromDbByFilmId(film.Id);
                wvm.MaxSeasonValue = SeasonCount;
                wvm.MaxPercentValue = SeasonCount;
                UpdateOperationTime(wvm, stopwatch);
                return seasons;
            });
        }

        private static Task<List<Episode>> GetEpisodesBySeasons(List<Season> seasons, WaitViewModel wvm,
            Stopwatch stopwatch)
        {
            return Task.Run(() =>
            {
                var episodes = seasons.SelectMany(s => s.Episodes).ToList();
                wvm.MaxPercentValue += episodes.Count;
                UpdateOperationTime(wvm, stopwatch);
                return episodes;
            });
        }

        /// <summary>
        /// Создать общий список адресов (без длительности) эпизодов в каждом сезоне фильма (Южный Парк)
        /// </summary>
        /// <param name="film">Фильм (Южный Парк)</param>
        /// <param name="wvm">Индикатор прогресса загрузки</param>
        /// <param name="stopwatch">Таймер</param>
        private static async Task CreateSpAddresses(Film film, WaitViewModel wvm, Stopwatch stopwatch)
        {
            var addressInfoList = new List<AddressInfo>();
            wvm.CurrentLoadingStatus = LoadingStatus.CreatingAddresses;
            if (await GetCancelCreateAddressesResult(addressInfoList)) return;
            var seasons = await GetSeasonsByFilm(film, wvm, stopwatch);
            var totalEpisodes = await GetEpisodesBySeasons(seasons, wvm, stopwatch);

            var totalAddressInfoCount =
                await Task.Run(() => totalEpisodes.Sum(episode => episode.FullNumber == 1503 ? 1 : 2));
            var totalAddressInfoList = await Task.Run(() => totalEpisodes.SelectMany(e => e.AddressInfoList).ToList());
            wvm.MaxPercentValue += totalAddressInfoCount;
            UpdateOperationTime(wvm, stopwatch);

            foreach (var season in seasons)
            {
                if (await GetCancelCreateAddressesResult(addressInfoList)) return;
                var episodes = totalEpisodes.Where(e => e.SeasonNumber == season.Number).ToList();

                wvm.ResetCurrentLoadingData(true, true);
                wvm.MaxEpisodeValue = episodes.Count;
                wvm.CurrentSeasonValue++;
                wvm.CurrentPercentValue++;

                foreach (var episode in episodes)
                {
                    if (await GetCancelCreateAddressesResult(addressInfoList)) return;
                    wvm.CurrentPercentValue++;
                    var addressesCount = episode.FullNumber == 1503 ? 1 : 2;

                    wvm.MaxAddressNumber = addressesCount;
                    wvm.CurrentEpisodeValue++;
                    wvm.ResetCurrentLoadingData(false, true);

                    wvm.CurrentAddressNumber++;
                    wvm.CurrentPercentValue++;
                    var link = GetSPEpAddressSPOnline(season.Number, episode.Number);
                    if (totalAddressInfoList.Any(a => a.Link == link)) continue;
                    var voice = GetSpVoiceNameSpOnline(season.Number, episode.Number);
                    var addressInfo = new AddressInfo
                    {
                        Name = $"online-south-park.ru_{voice}",
                        Link = link,
                        VoiceOver = voice,
                        Film = film,
                        Season = season,
                        Episode = episode
                    };

                    addressInfoList.Add(addressInfo);
                    UpdateOperationTime(wvm, stopwatch);

                    if (addressesCount == 1) continue;

                    wvm.CurrentAddressNumber++;
                    wvm.CurrentPercentValue++;

                    link = GetSpAddressFreehat(season.Number, episode.Number);
                    if (totalAddressInfoList.Any(a => a.Link == link)) continue;
                    voice = GetSpVoiceNameFreehat(season.Number, episode.Number);
                    addressInfo = new AddressInfo
                    {
                        Name = $"sp.freehat.cc_{voice}",
                        Link = link,
                        VoiceOver = voice,
                        Film = film,
                        Season = season,
                        Episode = episode
                    };

                    addressInfoList.Add(addressInfo);
                    UpdateOperationTime(wvm, stopwatch);
                }

                if (addressInfoList.Count == 0) continue;
                await Task.Run(() => InsertEntityListToDb(addressInfoList));
                addressInfoList.Clear();
            }
        }

        private static void UpdateOperationTime(WaitViewModel wvm, Stopwatch stopwatch)
        {
            wvm.ElapsedTime = stopwatch.Elapsed;
            wvm.RemainingTime = stopwatch.Elapsed;
        }

        private static async Task SetDefaultAddressInfoToEpisode(Film film)
        {
            var episodes = await Task.Run(() => film.Episodes);

            foreach (var episode in episodes)
            {
                if(episode.AddressInfo != null) continue;
                var addresses = await Task.Run(() => episode.AddressInfoList);
                if (episode.FullNumber == 1503)
                {
                    episode.AddressInfo = addresses.First();
                    continue;
                }

                episode.AddressInfo = addresses[GetDefaultAddressInfoNumber(episode.SeasonNumber, episode.Number)];
            }

            await Task.Run(() => UpdateDbCollection<Episode>(episodes));
        }

        private static int GetDefaultAddressInfoNumber(int seasonNum, int episodeNum)
        {
            switch (seasonNum)
            {
                case 2:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 11:
                case 12:
                case 13:
                case 24: return 1;
                case 19:
                    switch (episodeNum)
                    {
                        case 8:
                        case 9:
                        case 10: return 1;
                        default: return 0;
                    }
                default: return 0;
            }
        }

        private static Task<bool> GetCancelCreateAddressesResult(List<AddressInfo> addressInfoList)
        {
            return Task.Run(() =>
            {
                if (AddingFilmToken.IsCancellationRequested is false) return false;

                if (addressInfoList.Count > 0) InsertEntityListToDb(addressInfoList);
                return true;
            });
        }

        /// <summary>
        /// Добавить длительности эпизодов к каждому адресу
        /// </summary>
        /// <param name="film">Фильм (Южный Парк)</param>
        /// <param name="wvm">Индикатор прогресса загрузки</param>
        /// <param name="stopwatch">Таймер</param>
        private static async Task AddSpAddressesDurations(Film film, WaitViewModel wvm, Stopwatch stopwatch)
        {
            wvm.CurrentLoadingStatus = LoadingStatus.AddingDurations;
            if (AddingFilmToken.IsCancellationRequested) return;
            wvm.MaxSeasonValue = SeasonCount;
            var addressInfoList = await Task.Run(() => film.AddressInfoList);
            wvm.MaxPercentValue = addressInfoList.Count;

            foreach (var addressInfo in addressInfoList)
            {
                if (AddingFilmToken.IsCancellationRequested) return;
                wvm.CurrentSeasonValue = addressInfo.Season.Number;
                wvm.MaxEpisodeValue = GetSPEpisodesCount(addressInfo.Season.Number);
                wvm.CurrentEpisodeValue = addressInfo.Episode.Number;
                wvm.MaxAddressNumber = addressInfo.Episode.FullNumber == 1503 ? 1 : 2;
                if (wvm.CurrentAddressNumber >= wvm.MaxAddressNumber) wvm.CurrentAddressNumber = 0;
                wvm.CurrentAddressNumber++;
                wvm.CurrentPercentValue++;

                if (addressInfo.TotalDuration > new TimeSpan()) continue;
                var duration = await GetEpisodeDuration(addressInfo.Link);
                addressInfo.TotalDuration = duration;
                addressInfo.FilmEndTime = duration;

                UpdateDbCollection(addressInfo);
                UpdateOperationTime(wvm, stopwatch);
            }
        }

        private static async Task<TimeSpan> GetEpisodeDuration(Uri link)
        {
            return await Task.Run(async () =>
            {
                var media = new Media(_libVlc, link);
                await media.Parse(MediaParseOptions.ParseNetwork);
                var time = media.Duration;
                media.Dispose();
                return time == -1 ? new TimeSpan() : TimeSpan.FromMilliseconds(time);
            });
        }
    }
}