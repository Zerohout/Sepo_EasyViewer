// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using Caliburn.Micro;

    public partial class EditorSelectorViewModel : Conductor<Screen>.Collection.OneActive
    {
        /// <summary>
        /// Отменить выбор типа фильма
        /// </summary>
        public void CancelFilmTypeSelection() { SelectedFilmType = null; }
        public bool CanCancelFilmTypeSelection => SelectedFilmType != null;

        /// <summary>
        /// Отменить выбор фильма
        /// </summary>
        public void CancelFilmSelection() { SelectedFilm = null; }
        public bool CanCancelFilmSelection => SelectedFilm != null;

        /// <summary>
        /// Отменить выбор сезона
        /// </summary>
        public void CancelSeasonSelection() { SelectedSeason = null; }
        public bool CanCancelSeasonSelection => SelectedSeason != null;

        /// <summary>
        /// Отменить выбор эпизода
        /// </summary>
        public void CancelEpisodeSelection() { SelectedEpisode = null; }
        public bool CanCancelEpisodeSelection => SelectedEpisode != null;
    }
}
