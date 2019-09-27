// ReSharper disable once CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Helpers;
    using Microsoft.Win32;
    using Models.FilmModels;
    using Newtonsoft.Json;
    using static Helpers.GlobalMethods;

    public partial class SeasonsEditingViewModel : Screen
    {
        #region Season actions

        /// <summary>
        /// Кнопка "Изменить изображение"
        /// </summary>
        public void ChangeImage()
        {
            var ofd = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = $"Изображения(*.jpg, *.png, *.bmp)|*.jpg;*.png; *.bmp;"
            };

            if (ofd.ShowDialog() is false) return;

            CurrentSeason.ImageBytes = ImageToByteArray(new Bitmap(ofd.FileName)).ToArray();

            NotifyOfPropertyChange(() => Logo);
            NotifyOfPropertyChange(() => CanSaveChanges);
        }
        /// <summary>
        /// Сохранить изменения
        /// </summary>
        public void SaveChanges()
        {
            if (CanSaveChanges is false) return;

            var film = GetDbCollection<Film>().First(f => f.Name == ESVM.SelectedFilm.Name);
            film.Seasons[film.Seasons.FindIndex(s => s.Number == CurrentSeason.Number)] = CurrentSeason;

            UpdateDbCollection(obj: film);

            SeasonSnapshot = JsonConvert.SerializeObject(CurrentSeason);

            ESVM.ResetSelectedSeason(CurrentSeason);
            NotifyOfPropertyChange(() => CanSaveChanges);
        }

        public bool CanSaveChanges => HasChanges;

        #endregion

        #region Episodes actions

        public void SelectEpisode()
        {
            if (CanSelectEpisode is false) return;
            SelectedEpisode = Episodes.First();
        }

        public bool CanSelectEpisode => Episodes.Count > 0 &&
                                        SelectedEpisode == null;

        public void AddEpisode()
        {
            var count = IsDefSettingsEnabled
                ? DefaultAddingEpisodeValue
                : 1;

            for (var i = 0; i < count; i++)
            {
                var episode = new Episode
                {
                    SeasonNumber = CurrentSeason.Number,
                    Number = Episodes.Count + 1
                };

                if (IsFirstJumperEnabled)
                {
                    episode.Jumpers.Add(new Jumper
                    {
                        JumperMode = SystemVariables.JumperMode.Skip,
                        StartTime = TimeSpan.FromSeconds(JumperStartTime ?? 0),
                        EndTime = TimeSpan.FromSeconds(JumperEndTime ?? 1)
                    });
                }

                Episodes.Add(episode);
            }

            var film = GetDbCollection<Film>().First(f => f.Name == ESVM.SelectedFilm.Name);
            film.Seasons[film.Seasons.FindIndex(s => s.Number == CurrentSeason.Number)] = CurrentSeason;
            UpdateDbCollection(obj: film);

            NotifyOfPropertyChange(() => Episodes);
            NotifyOfPropertyChange(() => CanRemoveEpisode);
            NotifyOfPropertyChange(() => CanSelectEpisode);
        }

        public void EditEpisode()
        {
            ESVM.ResetSelectedEpisode(SelectedEpisode);
        }

        public bool CanEditEpisode => SelectedEpisode != null;

        public void RemoveEpisode()
        {

        }

        public bool CanRemoveEpisode => true;

        public void CancelSelection()
        {

        }

        public bool CanCancelSelection => true;

        public void EnableDefSettings()
        {
            IsDefSettingsEnabled = !IsDefSettingsEnabled;
            NotifyOfPropertyChange(() => DefSettingsVisibility);
        }

        #endregion

        #region Total actions

        /// <summary>
        /// Действие при нажатии на клавиши
        /// </summary>
        /// <param name="eventArgs"></param>
        public void KeyDown(object eventArgs)
        {

        }

        /// <summary>
        /// Проверка на ввод числовых данных
        /// </summary>
        /// <param name="e"></param>
        public void NumericValidation(KeyEventArgs e)
        {
            e.Handled = (e.Key.GetHashCode() >= 34 && e.Key.GetHashCode() <= 43 ||
                         e.Key.GetHashCode() >= 74 && e.Key.GetHashCode() <= 83) is false;
        }
        /// <summary>
        /// Двойной клик по текстовому полю
        /// </summary>
        /// <param name="sender"></param>
        public void TBoxDoubleClick(TextBox sender)
        {
            sender.SelectAll();
        }

        /// <summary>
        /// Изменение выбора в ListBox
        /// </summary>
        /// <param name="sender"></param>
        public void SelectionChanged(ListBox sender)
        {
            sender.ScrollIntoView(sender.SelectedItem);
            NotifyOfPropertyChange(() => CanCancelSelection);
        }
        /// <summary>
        /// Действие при изменении текста
        /// </summary>
        public void TextChanged()
        {
            NotifyOfPropertyChange(() => CanSaveChanges);
        }

        #endregion




    }
}