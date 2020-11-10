// ReSharper disable once CheckNamespace

namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Models.FilmModels;
    using Newtonsoft.Json;
    using static Helpers.GlobalMethods;
    using static Helpers.DbMethods;

    public partial class SeasonsEditingViewModel : Screen
    {
        #region Season actions

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        public void SaveChanges()
        {
            if (CanSaveChanges is false) return;

            CurrentSeason.Description = SeasonDescription;
            UpdateDbCollection(CurrentSeason);

            ESVM.ResetSelectedSeason(CurrentSeason);
            NotifyChanges();
        }

        public bool CanSaveChanges => HasChanges;

        /// <summary>
        /// Отменить изменения сезона
        /// </summary>
        public void CancelChanges()
        {
            if (CanCancelChanges is false) return;
            SeasonDescription = CurrentSeason.Description;
            NotifyChanges();
        }

        public bool CanCancelChanges => HasChanges;

        #endregion

        #region Episodes actions

        /// <summary>
        /// Выбрать первый в списке эпизод (проблема с неактивным ListBox'ом)
        /// </summary>
        public void SelectEpisode()
        {
            if (CanSelectEpisode is false) return;
            SelectedEpisode = Episodes.First();
        }

        public bool CanSelectEpisode => Episodes.Count > 0 &&
                                        SelectedEpisode == null;

        /// <summary>
        /// Добавить эпизоды
        /// </summary>
        public void AddEpisodes()
        {
            var number = Episodes.Count;
            var lastNumber = Episodes.LastOrDefault()?.Number ?? 0;
            var episodes = new List<Episode>();
            var name = $"{TranslateFileName(ESVM.SelectedFilm.Name)}_S{CurrentSeason.Number}_E";
            for (var i = 0; i < AddingEpisodeValue; i++)
            {
                if (number > 0 && number != lastNumber)
                {
                    for (var j = 1; j <= lastNumber; j++)
                    {
                        if (Episodes.Any(e => e.Number == j)
                            || episodes.Any(e => e.Number == j)) continue;
                        episodes.Add(new Episode
                        {
                            Name = name + j,
                            Number = j,
                            Season = CurrentSeason,
                            Film = ESVM.SelectedFilm
                        });
                        lastNumber++;
                        break;
                    }

                    continue;
                }

                episodes.Add(new Episode
                {
                    Name = name + ++number,
                    Number = number,
                    Season = CurrentSeason,
                    Film = ESVM.SelectedFilm,
                });
                lastNumber++;
            }

            InsertEntityListToDb(episodes);
            Episodes = new BindableCollection<Episode>(CurrentSeason.Episodes);
            SelectedEpisode = Episodes.LastOrDefault();
        }

        /// <summary>
        /// Редактировать выбранный эпизод
        /// </summary>
        public void EditEpisode()
        {
            if (CanEditEpisode is false) return;
            ESVM.ResetSelectedEpisode(SelectedEpisode);
        }

        public bool CanEditEpisode => SelectedEpisodes.Count == 1;

        /// <summary>
        /// Удалить выбранный эпизод
        /// </summary>
        public void DeleteSelectedEpisodes()
        {
            if (CanDeleteSelectedEpisodes is false) return;

            for (var i = 0; i < SelectedEpisodes.Count; i++)
            {
                FullyEpisodeDeleting(SelectedEpisodes[i]);
            }

            Episodes = new BindableCollection<Episode>(CurrentSeason.Episodes);
            SelectedEpisode = Episodes.LastOrDefault();
        }

        public bool CanDeleteSelectedEpisodes => SelectedEpisodes.Count > 0;

        /// <summary>
        /// Сбросить выбор эпизода
        /// </summary>
        public void CancelSelection()
        {
            if (CanCancelSelection is false) return;
            SelectedEpisode = null;
        }

        public bool CanCancelSelection => SelectedEpisodes.Count > 0;

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
            SelectedEpisodes = new BindableCollection<Episode>(sender.SelectedItems.Cast<Episode>());
            if (SelectedEpisodes.Count == 1) sender.ScrollIntoView(sender.SelectedItem);
        }

       
        /// <summary>
        /// Действие при изменении текста
        /// </summary>
        public void TextChanged()
        {
            NotifyChanges();
        }

        #endregion
    }
}