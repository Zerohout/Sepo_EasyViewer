namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using Caliburn.Micro;

    public interface ISettingsViewModel : IScreen
    {
        //TODO: Дополнить интерфейс общими методами. 
        //TODO: Переименовать в IFilmEditorSetting
        bool HasChanges { get; }
        //void LoadData();
        void SaveChanges();
    }
}
