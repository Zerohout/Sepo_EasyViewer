namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using Caliburn.Micro;

    public partial class EpisodesEditingViewModel : Screen
	{
		public EpisodesEditingViewModel()
		{

		}

		protected override void OnInitialize()
		{
			DisplayName = "Редактор эпизодов";
			
			base.OnInitialize();
		}
	}
}
