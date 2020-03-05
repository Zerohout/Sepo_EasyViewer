// ReSharper disable once CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
	using System.Linq;
	using System.Windows.Controls;
	using System.Windows.Input;
	using Caliburn.Micro;
	using Models.FilmModels;
	using Newtonsoft.Json;
	using static Helpers.GlobalMethods;

	public partial class SeasonsEditingViewModel : Screen
	{
		#region Season actions

		/// <summary>
		/// Сохранить изменения
		/// </summary>
		public void SaveChanges()
		{
			if (CanSaveChanges is false) return;

			CurrentSeason.Description = SeasonDescription.ToString();
			UpdateDbSeason(ESVM.SelectedFilm.Name, CurrentSeason);

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
			SeasonDescription = CurrentSeason.Description.ToString();
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
			var num = Episodes.Count;
			for (var i = 0; i < AddingEpisodeValue; i++)
			{
				var number = ++num;
				var name = $"{TranslateFileName(ESVM.SelectedFilm.Name)}_S{CurrentSeason.Number}_E{number}";
				CurrentSeason.Episodes.Add(new Episode
				{
					Name = name,
					SeasonNumber = CurrentSeason.Number,
					Number = number
				});
			}

			UpdateDbSeason(ESVM.SelectedFilm.Name,CurrentSeason);
			SelectedEpisode = Episodes.LastOrDefault();
			NotifyOfPropertyChange(() => Episodes);
		}
		/// <summary>
		/// Редактировать выбранный эпизод
		/// </summary>
		public void EditEpisode()
		{
			if (CanEditEpisode is false) return;
			ESVM.ResetSelectedEpisode(SelectedEpisode);
		}

		public bool CanEditEpisode => SelectedEpisode != null;
		/// <summary>
		/// Удалить последний эпизод
		/// </summary>
		public void RemoveEpisode()
		{
			if (CanRemoveEpisode is false) return;

			CurrentSeason.Episodes.Remove(Episodes.Last());
			UpdateDbSeason(ESVM.SelectedFilm.Name,CurrentSeason);
			SelectedEpisode = Episodes.LastOrDefault();
			NotifyOfPropertyChange(() => Episodes);
		}

		public bool CanRemoveEpisode => Episodes.Count > 0;
		/// <summary>
		/// Сбросить выбор эпизода
		/// </summary>
		public void CancelSelection()
		{
			if (CanCancelSelection is false) return;
			SelectedEpisode = null;
		}

		public bool CanCancelSelection => SelectedEpisode != null;

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
			sender.ScrollIntoView(sender.SelectedItem);
			NotifyOfPropertyChange(() => CanCancelSelection);
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