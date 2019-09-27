namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using Caliburn.Micro;

    public partial class EditorSelectorViewModel : Conductor<Screen>.Collection.OneActive
    {
        public EditorSelectorViewModel()
        {
            NotifyOfPropertyChange(() => SelectedFilmType);
        }

    }
}
