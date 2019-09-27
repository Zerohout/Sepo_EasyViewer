// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.WatchingSettingsFolder.ViewModels
{
    using Caliburn.Micro;
    using Models.FilmModels;
    using static Helpers.GlobalMethods;

    public partial class WatchingSettingsViewModel : Screen
    {
        /// <summary>
        /// Загрузка данных
        /// </summary>
        private void LoadData()
        {
            LoadWS();
            Films = new BindableCollection<Film>(GetDbCollection<Film>()) { new Film { Name = "всех" } };
            NotifyOfPropertyChange(() => CheckedEpisodes);
        }

        /// <summary>
        /// Загрузить настройки просмотра
        /// </summary>
        private void LoadWS()
        {
            WatchingSettings = LoadOrCreateWatchingSettings();
            TempWS = LoadOrCreateWatchingSettings();
            NotifyButtons();
        }

        private void NotifyButtons()
        {
            NotifyOfPropertyChange(() => HasChanges);
            NotifyOfPropertyChange(() => CanSaveChanges);
            NotifyOfPropertyChange(() => CanResetLastDateViewed);
            NotifyOfPropertyChange(() => CanCancelChanges);
            NotifyOfPropertyChange(() => CanSetDefaultValues);
        }
    }
}
