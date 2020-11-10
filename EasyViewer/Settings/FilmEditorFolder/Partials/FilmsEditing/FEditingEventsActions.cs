// ReSharper disable CheckNamespace

namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Forms.VisualStyles;
    using System.Windows.Input;
    using System.Windows.Markup;
    using Caliburn.Micro;
    using Models.FilmModels;
    using Newtonsoft.Json;
    using static Helpers.DbMethods;
    using static Helpers.SystemVariables;

    public partial class FilmsEditingViewModel : Screen
    {
        #region Film actions

        /// <summary>
        ///     Создать фильм
        /// </summary>
        public void CreateFilm()
        {
            if (CanCreateFilm is false) return;
            InsertEntityToDb(CurrentFilm);
            ESVM.ResetSelectedFilm(CurrentFilm, CurrentFilm.FilmType != ESVM.SelectedFilmType);
        }

        public bool CanCreateFilm => string.IsNullOrWhiteSpace(CurrentFilm.Name) is false &&
                                     CurrentFilm.Name != NewFilmName;

        /// <summary>
        ///     Сохранить изменения
        /// </summary>
        public void SaveChanges()
        {
            if (CanSaveChanges is false) return;

            UpdateDbCollection(CurrentFilm);
            OriginalFilm = GetFilmFromDbById(CurrentFilm.Id);
            ESVM.ResetSelectedFilm(CurrentFilm, CurrentFilm.FilmType != ESVM.SelectedFilmType);
            NotifyChanges();
        }

        public bool CanSaveChanges => HasChanges;

        public void CancelChanges()
        {
            if (CanCancelChanges is false) return;

            CurrentFilm = GetFilmFromDbById(CurrentFilm.Id);
            NotifyChanges();
        }

        public bool CanCancelChanges => HasChanges;

        /// <summary>
        ///     Удалить фильм
        /// </summary>
        public void RemoveFilm()
        {
            FullyFilmDeleting(CurrentFilm);
            ESVM.SetFilms(ESVM.SelectedFilmType);
        }

        #endregion

        #region Seasons actions

        /// <summary>
        ///     Выбрать первый сезон в списке (решается баг с потерей фокуса курсора)
        /// </summary>
        
        public void SelectSeason()
        {
            if (CanSelectSeason is false) return;

            SelectedSeason = Seasons.First();
        }
        public bool CanSelectSeason => SelectedSeasons.Count > 0;

        /// <summary>
        ///     Добавить сезоны
        /// </summary>
        public void AddSeasons()
        {
            var number = Seasons.Count;
            var lastNumber = Seasons.LastOrDefault()?.Number ?? 0;
            var seasons = new List<Season>();
            for (var i = 0; i < AddingSeasonValue; i++)
            {
                if (number > 0 && number != lastNumber)
                {
                    for (var j = 1; j <= lastNumber; j++)
                    {
                        if (Seasons.Any(s => s.Number == j)
                            || seasons.Any(s => s.Number == j)) continue;

                        seasons.Add(new Season
                        {
                            Number = j,
                            Film = CurrentFilm
                        });
                        lastNumber++;
                        break;
                    }

                    continue;
                }

                seasons.Add(new Season
                {
                    Number = ++number,
                    Film = CurrentFilm
                });

                lastNumber++;
            }

            InsertEntityListToDb(seasons);
            Seasons = new BindableCollection<Season>(CurrentFilm.Seasons);
            SelectedSeason = Seasons.Last();
        }

        /// <summary>
        ///     Редактировать выбранный сезон
        /// </summary>
        public void ModifySeason()
        {
            if (CanModifySeason is false) return;
            ESVM.ResetSelectedSeason(SelectedSeason);
        }

        public bool CanModifySeason => SelectedSeasons.Count == 1;

        /// <summary>
        ///     Удалить выбранные сезоны
        /// </summary>
        public void DeleteSelectedSeasons()
        {
            if (CanDeleteSelectedSeasons is false) return;

            for (var i = 0; i < SelectedSeasons.Count; i++)
            {
                FullySeasonDeleting(SelectedSeasons[i]);
            }
            Seasons = new BindableCollection<Season>(CurrentFilm.Seasons);
            SelectedSeason = Seasons.LastOrDefault();
        }

        public bool CanDeleteSelectedSeasons => SelectedSeasons.Count > 0;

        /// <summary>
        ///     Убрать выделение с сезона
        /// </summary>
        public void CancelSelection()
        {
            if (CanCancelSelection is false) return;
            SelectedSeason = null;
        }
        public bool CanCancelSelection => SelectedSeasons.Count > 0;

        #endregion

        #region Total actions

        /// <summary>
        ///     Двойной клик по текстовому полю
        /// </summary>
        /// <param name="source"></param>
        public void TBoxDoubleClick(TextBox source)
        {
            source.SelectAll();
        }

        /// <summary>
        ///     Действие при изменении текста в TextBox'е
        /// </summary>
        public void TextChanged()
        {
            NotifyOfPropertyChange(() => CanCreateFilm);
            NotifyChanges();
        }

        /// <summary>
        ///     Действия при нажатии клавиши
        /// </summary>
        /// <param name="eventArgs"></param>
        public void KeyDown(object eventArgs)
        {
        }

        /// <summary>
        ///     Проверка на ввод числовых данных
        /// </summary>
        /// <param name="e"></param>
        public void NumericValidation(KeyEventArgs e)
        {
            e.Handled = (e.Key.GetHashCode() >= 34 && e.Key.GetHashCode() <= 43 ||
                         e.Key.GetHashCode() >= 74 && e.Key.GetHashCode() <= 83) is false;
        }

        public void IsEnabledChangedAction(ListBox sender)
        {
            Console.WriteLine("IsEnabledChangedAction");
        }

        /// <summary>
        ///     Изменение выбора в ListBox
        /// </summary>
        /// <param name="sender"></param>
        public void SelectionChanged(ListBox sender)
        {
            SelectedSeasons = new BindableCollection<Season>(sender.SelectedItems.Cast<Season>());
            if(SelectedSeasons.Count == 1) sender.ScrollIntoView(sender.SelectedItem);
        }

        #endregion
    }
}