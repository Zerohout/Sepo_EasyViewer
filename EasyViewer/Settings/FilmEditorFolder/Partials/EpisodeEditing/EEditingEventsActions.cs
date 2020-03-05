// ReSharper disable once CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
	using System.Linq;
	using System.Windows.Controls;
	using Caliburn.Micro;
	using Models.FilmModels;
	using Newtonsoft.Json;
	using static Helpers.GlobalMethods;

	public partial class EpisodeEditingViewModel : Screen
	{

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
			NotifyChanges();
		}

		public void SaveChanges()
		{
			if (CanSaveChanges is false) return;
			UpdateDbEpisode(CurrentEpisode);
			EpisodeSnapshot = JsonConvert.SerializeObject(CurrentEpisode);
			NotifyChanges();
		}

		public bool CanSaveChanges => HasChanges;

		public void CancelChanges()
		{
			if (CanCancelChanges is false) return;
			CurrentEpisode = JsonConvert.DeserializeObject<Episode>(EpisodeSnapshot);
			NotifyChanges();
		}

		public bool CanCancelChanges => HasChanges;

	}
}