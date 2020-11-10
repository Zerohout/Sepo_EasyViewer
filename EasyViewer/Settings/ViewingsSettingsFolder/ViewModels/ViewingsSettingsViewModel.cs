namespace EasyViewer.Settings.ViewingsSettingsFolder.ViewModels
{
    using Caliburn.Micro;
    using Helpers;
    using Models.FilmModels;

    public partial class ViewingsSettingsViewModel : Screen
	{
		public ViewingsSettingsViewModel()
		{
			
		}

		protected override void OnInitialize()
		{
            Films = new BindableCollection<Film>(DbMethods.GetDbCollection<Film>());
			base.OnInitialize();
		}
	}
}
