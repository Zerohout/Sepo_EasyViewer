// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using System.Drawing;
    using System.Linq;
    using System.Windows.Controls;
    using Microsoft.Win32;
    using Models.FilmModels;
    using Newtonsoft.Json;
    using static Helpers.GlobalMethods;
    using static Helpers.SystemVariables;
    using Screen = Caliburn.Micro.Screen;

    public partial class FilmsEditingViewModel : Screen
    {
        #region Film actions

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

            CurrentFilm.ImageBytes = ImageToByteArray(new Bitmap(ofd.FileName)).ToArray();

            NotifyOfPropertyChange(() => Logo);
            NotifyChanges();
        }

        /// <summary>
        /// Создать фильм
        /// </summary>
        public void CreateFilm()
        {
            if (CanCreateFilm is false) return;
            AddFilmToDb(CurrentFilm);
            ESVM.ResetSelectedFilm(CurrentFilm, CurrentFilm.FilmType != ESVM.SelectedFilmType);
        }

        public bool CanCreateFilm => string.IsNullOrWhiteSpace(CurrentFilm.Name) is false &&
                                     CurrentFilm.Name != NewFilmName;

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        public void SaveChanges()
        {
            if (CanSaveChanges is false) return;

            UpdateDbCollection(obj: CurrentFilm);

            FilmSnapshot = JsonConvert.SerializeObject(CurrentFilm);

            ESVM.ResetSelectedFilm(CurrentFilm, CurrentFilm.FilmType != ESVM.SelectedFilmType);
        }

        public bool CanSaveChanges => HasChanges;

        public void CancelChanges()
        {
            if (CanCancelChanges is false) return;

            CurrentFilm = GetDbCollection<Film>().First(f => f.Id == CurrentFilm.Id);
            NotifyChanges();
        }

        public bool CanCancelChanges => HasChanges;

        /// <summary>
        /// Удалить фильм
        /// </summary>
        public void RemoveFilm()
        {
            RemoveFilmFromDb(CurrentFilm);
            ESVM.SetFilms(ESVM.SelectedFilmType);
        }

        public void SelectionChanged()
        {
            NotifyOfPropertyChange(() => CurrentFilm);
            NotifyChanges();
        }

        #endregion

        #region Seasons actions

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
        /// Выбрать первый сезон в списке (решается баг с потерей фокуса курсора)
        /// </summary>
        public void SelectSeason()
        {
            if (CanSelectSeason is false) return;

            SelectedSeason = CurrentFilm.Seasons.First();
        }

        public bool CanSelectSeason => Seasons.Count > 0 &&
                                       SelectedSeason == null;

        /// <summary>
        /// Добавить сезон
        /// </summary>
        public void AddSeason()
        {
            var number = CurrentFilm.Seasons.Count + 1;
            var imageBytes = ImageToByteArray(new Bitmap($"{AppDataPath}\\{DefaultLogoImageName}"));

            CurrentFilm.Seasons.Add(new Season { Number = number, ImageBytes = imageBytes });
            SelectedSeason = CurrentFilm.Seasons.Last();

            UpdateDbCollection(obj: CurrentFilm);
            NotifyOfPropertyChange(() => Seasons);
            NotifyOfPropertyChange(() => CanRemoveSeason);
        }

        /// <summary>
        /// Редактировать выбранный сезон
        /// </summary>
        public void EditSeason()
        {
            ESVM.ResetSelectedSeason(SelectedSeason);
        }

        public bool CanEditSeason => SelectedSeason != null;

        /// <summary>
        /// Удалить выбранный сезон
        /// </summary>
        public void RemoveSeason()
        {
            if (CanRemoveSeason is false) return;
            CurrentFilm.Seasons.Remove(CurrentFilm.Seasons.Last());
            SelectedSeason = CurrentFilm.Seasons.LastOrDefault();

            UpdateDbCollection(obj: CurrentFilm);
            NotifyOfPropertyChange(() => Seasons);
            NotifyOfPropertyChange(() => CanRemoveSeason);

        }

        public bool CanRemoveSeason => CurrentFilm.Seasons.Count > 0;

        /// <summary>
        /// Убрать выделение с сезона
        /// </summary>
        public void CancelSelection()
        {
            if (CanCancelSelection is false) return;
            SelectedSeason = null;
        }

        public bool CanCancelSelection => SelectedSeason != null;

        #endregion

        #region Total actions

        /// <summary>
        /// Двойной клик по текстовому полю
        /// </summary>
        /// <param name="source"></param>
        public void TBoxDoubleClick(TextBox source)
        {
            source.SelectAll();
        }

        /// <summary>
        /// Действие при изменении текста в TextBox'е
        /// </summary>
        public void TextChanged()
        {
            NotifyOfPropertyChange(() => CanCreateFilm);
            NotifyChanges();
        }

        /// <summary>
        /// Действия при нажатии клавиши
        /// </summary>
        /// <param name="eventArgs"></param>
        public void KeyDown(object eventArgs)
        {

        }

        #endregion

    }
}
