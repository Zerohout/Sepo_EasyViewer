// ReSharper disable once CheckNamespace

namespace EasyViewer.MainMenu.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Caliburn.Micro;
    using EasyViewer.ViewModels;
    using Helpers.Creators;
    using Models.FilmModels;
    using Settings.SettingsFolder.ViewModels;
    using static Helpers.GlobalMethods;
    using static Helpers.Creators.SouthPark.SouthParkCreator;
    using static Helpers.SystemVariables;
    using static Helpers.DbMethods;

    public partial class MainMenuViewModel : Screen
    {
        private int _checkedFilmsCount;

        /// <summary>
        ///     Действие при старте просмотра
        /// </summary>
        public void Start()
        {
            if (CanStart is false) return;

            CreateList();
            ActivateDeactivateTray((Window) ((MainViewModel) Parent).GetView());
            VideoPlayer = new VideoPlayerViewModel(this,
                WatchingEpisodesCount, CheckedEpisodes) {MainView = (Window) ((MainViewModel) Parent).GetView()};
            WinMan.ShowWindow(VideoPlayer);
            NotifyOfPropertyChange(() => EpisodesCountRemainingString);
            NotifyOfPropertyChange(() => CanStart);
        }

        public bool CanStart => _watchingEpisodesCount > 0 && _checkedFilmsCount  == 1;

        /// <summary>
        ///     Действие при выборе/снятия выбора с фильма
        ///     (Обновление списка фильмов и выбранных эпизодов)
        /// </summary>
        public async void CheckedValidation(Film film)
        {
            UpdateDbCollection(film);
            await LoadData();
            
        }

        /// <summary>
        ///     Действие при нажатии клавиши клавиатуры
        /// </summary>
        /// <param name="e"></param>
        public void KeyDown(KeyEventArgs e)
        {
        }

        /// <summary>
        ///     Перейти к настройкам
        /// </summary>
        public void GoToSettings()
        {
            ((MainViewModel) Parent).ChangeActiveItem(new SettingsViewModel());
        }

        /// <summary>
        ///     Действие при нажатии на клавишу PauseBreak
        /// </summary>
        public void PauseButtonAction()
        {
                if (VideoPlayer == null) return;

                if (VideoPlayer.Topmost)
                {
                    if (VideoPlayer.WindowState == WindowState.Maximized) VideoPlayer.WindowState = WindowState.Normal;
                    VideoPlayer.Topmost = false;
                }
                else
                {
                    if (VideoPlayer.WindowState == WindowState.Normal) VideoPlayer.WindowState = WindowState.Maximized;
                    VideoPlayer.Topmost = false;
                }

                VideoPlayer.PlayPause();
                NotifyOfPropertyChange(() => CanStart);
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    ActivateDeactivateTray((Window) ((MainViewModel) Parent).GetView());
                });
        }

        #region Window actions

        /// <summary>
        ///     Действие при изменении текста
        /// </summary>
        public void TextChanged()
        {
        }

        /// <summary>
        ///     Проверка является ливведенный сивол числом
        /// </summary>
        /// <param name="e"></param>
        public void NumericValidation(KeyEventArgs e)
        {
            e.Handled = (e.Key.GetHashCode() >= 34 && e.Key.GetHashCode() <= 43 ||
                         e.Key.GetHashCode() >= 74 && e.Key.GetHashCode() <= 83) is false;
        }

        /// <summary>
        ///     Попадание курсора на кнопку выхода
        /// </summary>
        public void CursorOnExit()
        {
            Background = new Uri(MMBackgroundOnExitUri, UriKind.Relative);
        }

        /// <summary>
        ///     Выход курсора за пределы кнопки выхода
        /// </summary>
        public void CursorOutsideExit()
        {
            Background = new Uri(MMBackgroundUri, UriKind.Relative);
        }

        /// <summary>
        ///     Действие при выходе
        /// </summary>
        public void Exit()
        {
            VideoPlayer?.TryClose();
            HotReg?.UnregisterHotkeys();
            ((MainViewModel) Parent).Exit();
        }

        #endregion


        #region Меню разработчика

        /// <summary>
        ///     Открыть меню разработчика
        /// </summary>
        public void SecretAction()
        {
            if (SecretVisibility == Visibility.Visible)
            {
                SecretVisibility = Visibility.Collapsed;
                return;
            }

            if (WinMan.ShowDialog(new SecretViewModel()) is true) SecretVisibility = Visibility.Visible;
        }

        /// <summary>
        ///     Добавление эпизодов
        /// </summary>
        public async Task<bool> AddSouthPark()
        {
            if (CanAddAddSouthPark is false) return false;
            AddingFilmCancellationTokenSource = new CancellationTokenSource();
            AddingFilmToken = AddingFilmCancellationTokenSource.Token;

            try
            {
                var wvm = new WaitViewModel();
                WinMan.ShowWindow(wvm);

                ((Window) ((MainViewModel) Parent).GetView()).IsEnabled = false;

                await CreateSP(wvm);

                ((Window) ((MainViewModel) Parent).GetView()).IsEnabled = true;
                wvm.TryClose();
            }
            catch (Exception e)
            {
                WinMan.ShowWindow(new DialogViewModel(e.ToString(), DialogType.Error, e));
                return false;
            }

            Films = new BindableCollection<Film>(GetDbCollection<Film>());
            return AddingFilmToken.IsCancellationRequested is false;
        }

        public bool CanAddAddSouthPark => true;

        /// <summary>
        ///     Выполнить команду
        /// </summary>
        public async void StartCommand()
        {
            if (string.IsNullOrWhiteSpace(SelectedCommand)) return;
            bool result;
            switch (SelectedCommand)
            {
                case "Добавить \"Южный Парк\"":
                    result = await AddSouthPark();
                    break;
                case "Проверить длительности эпизодов":
                    result = ValidateEpisodesDuration();
                    break;
                case "Проверить содержимое фильма \"Южный Парк\"":
                    result = await CheckSP();
                    break;
                case "Удалить все фильмы":
                    result = RemoveFilms();
                    break;
                default:
                    WinMan.ShowDialog(new DialogViewModel("Неизвестная команда", DialogType.Info));
                    return;
            }

            WinMan.ShowDialog(new DialogViewModel(result ? "Команда успешно выполнена" : "Команда не выполнена",
                DialogType.Info));
        }

        /// <summary>
        ///     Удалить выбранный фильм
        /// </summary>
        public void RemoveSelectedFilm()
        {
            if (CanRemoveSelectedFilm is false) return;

            FullyFilmDeleting(SelectedRemovingFilm);
            LoadFilms();
        }

        public bool CanRemoveSelectedFilm => SelectedRemovingFilm != null;

        /// <summary>
        ///     Удалить все фильмы
        /// </summary>
        public bool RemoveFilms()
        {
            if (CanRemoveFilms is false) return false;
            DropDbCollection<Jumper>();
            DropDbCollection<AddressInfo>();
            DropDbCollection<Episode>();
            DropDbCollection<Season>();
            DropDbCollection<Film>();
            Films = new BindableCollection<Film>(GetDbCollection<Film>());
            return true;
        }

        public bool CanRemoveFilms => Films.Count >= 1;

        public bool ValidateEpisodesDuration()
        {
            if (CanValidateEpisodesDuration is false) return false;

            var result = new List<string>();
            var count = 1;

            foreach (var film in Films)
            {
                var episodes = film.Episodes;

                foreach (var episode in episodes)
                {
                    result.Add(
                        $"{count:000})\t{film.Name} - {episode.FullNumber}, {episode.AddressInfo.Link} - {episode.AddressInfo.TotalDuration}");
                    count++;
                }
            }

            var filePath = $"{AppPath}\\{DevelopmentDataFolderName}\\DurationValidate-{LogName}";

            new FileStream(filePath, FileMode.Create).Dispose();

            using (var sw = new StreamWriter(filePath))
            {
                foreach (var str in result) sw.WriteLine(str);

                sw.Dispose();
            }

            return true;
        }

        public bool CanValidateEpisodesDuration => Films.Count >= 1;

        private int GetTotalEpisodesCount()
        {
            var episodesCount = 0;
            for (var i = 1; i <= SeasonCount; i++)
            {
                episodesCount += CreatorMethods.GetSPEpisodesCount(i);
            }

            return episodesCount;
        }

        public async Task<bool> CheckSP()
        {
            return await Task.Run(async () =>
            {
                if (CanCheckSP is false) return false;

                var result = new List<string>();

                var film = Films.First(f => f.Name == "Южный парк");
                var seasons = await Task.Run(() => film.Seasons);

                result.Add($"Общая информация:\n{film.FilmType} {film.Name}\n" +
                           $"Сезонов - {seasons.Count}, Эпизодов - {await Task.Run(() => film.Episodes.Count)}\n\n" +
                           "\tИнформация по Сезонам и эпизодам:\n\n");


                foreach (var season in seasons)
                {
                    var episodes = await Task.Run(() => season.Episodes);
                    result.Add($"\tСезон №{season.Number}, количество эпизодов - {episodes.Count}\n");

                    foreach (var episode in episodes)
                    {
                        var addressInfoList = await Task.Run(() => episode.AddressInfoList);
                        result.Add(
                            $"\t\tЭпизод №{episode.Number}, полный номер - {episode.FullNumber}, количество адресов - {addressInfoList.Count}\n");

                        foreach (var address in addressInfoList)
                        {
                            var jumpers = await Task.Run(() => address.Jumpers);
                            result.Add($"\t\t\tНазвание адреса - {address.Name}, адрес - {address.Link}, " +
                                       $"озвучка - {address.VoiceOver}, длительность - {address.TotalDuration}, джамперов - {jumpers.Count}");

                            foreach (var jumper in jumpers)
                                result.Add(
                                    $"\t\t\t\tДжампер №{jumper.Number}, режим джампера - {jumper.JumperMode}, время начала - {jumper.StartTime}, " +
                                    $"время конца - {jumper.EndTime}, длительность - {jumper.Duration}");
                        }

                        result.Add("\n\n");
                    }
                }

                var logName =
                    $"{DateTime.Today:d}_{DateTime.Now.Hour:00}.{DateTime.Now.Minute:00}.{DateTime.Now.Second:00}.txt";
                var filePath = $"{AppPath}\\{DevelopmentDataFolderName}\\CheckSP-{logName}";

                new FileStream(filePath, FileMode.Create).Dispose();

                using (var sw = new StreamWriter(filePath))
                {
                    foreach (var str in result) await sw.WriteLineAsync(str);

                    sw.Dispose();
                }

                return true;
            });
        }

        public bool CanCheckSP => Films.Any(f => f.Name == "Южный парк");

        #endregion
    }
}