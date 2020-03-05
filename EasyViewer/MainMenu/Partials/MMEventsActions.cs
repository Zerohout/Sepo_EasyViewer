// ReSharper disable once CheckNamespace
namespace EasyViewer.MainMenu.ViewModels
{
	using System.Collections.Generic;
	using System.IO;
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows;
	using Caliburn.Micro;
	using EasyViewer.ViewModels;
	using LiteDB;
	using Models.FilmModels;
	using EasyViewer.Settings.SettingsFolder.ViewModels;
	using static Helpers.GlobalMethods;
	using static Helpers.Creators.SouthPark.SPCreator;
	using static Helpers.SystemVariables;
	using KeyEventArgs = System.Windows.Input.KeyEventArgs;
	using Screen = Caliburn.Micro.Screen;

	public partial class MainMenuViewModel : Screen
	{
		#region Window actions

		/// <summary>
		/// Действие при изменении текста
		/// </summary>
		public void TextChanged()
		{
		}

		/// <summary>
		/// Проверка является ливведенный сивол числом
		/// </summary>
		/// <param name="e"></param>
		public void NumericValidation(KeyEventArgs e)
		{
			e.Handled = (e.Key.GetHashCode() >= 34 && e.Key.GetHashCode() <= 43 ||
						 e.Key.GetHashCode() >= 74 && e.Key.GetHashCode() <= 83) is false;

		}

		/// <summary>
		/// Попадание курсора на кнопку выхода
		/// </summary>
		public void CursorOnExit()
		{
			Background = new Uri(MMBackgroundOnExitUri, UriKind.Relative);

		}

		/// <summary>
		/// Выход курсора за пределы кнопки выхода
		/// </summary>
		public void CursorOutsideExit()
		{
			Background = new Uri(MMBackgroundUri, UriKind.Relative);
		}

		/// <summary>
		/// Действие при выходе
		/// </summary>
		public void Exit()
		{
			VideoPlayer?.TryClose();
			HotReg?.UnregisterHotkeys();
			((MainViewModel)Parent).Exit();
		}

		#endregion

		/// <summary>
		/// Действие при старте просмотра
		/// </summary>
		public void Start()
		{
			if (!CanStart) return;

			ActivateDeactivateTray((Window)((MainViewModel)Parent).GetView());

			VideoPlayer = new VideoPlayerViewModel(this, VideoPlayerMode.Viewing,
												   WatchingEpisodesCount, CheckedEpisodes)
			{
				MainView = (Window)((MainViewModel)Parent).GetView()
			};

			WinMan.ShowWindow(VideoPlayer);
			NotifyOfPropertyChange(() => EpisodesCountRemainingString);
			NotifyOfPropertyChange(() => CanStart);
		}
		/// <summary>
		/// Статус кнопку Start
		/// </summary>
		public bool CanStart => WatchingEpisodesCount > 0 && (VlcPlayer == null);



		/// <summary>
		/// Действие при выборе/снятия выбора с фильма
		/// (Обновление списка фильмов и выбранных эпизодов)
		/// </summary>
		public void CheckedValidation()
		{
			UpdateDbCollection(objCollection: Films);
			SetCheckedEpisodes();
		}

		/// <summary>
		/// Действие при нажатии клавиши клавиатуры
		/// </summary>
		/// <param name="e"></param>
		public void KeyDown(KeyEventArgs e)
		{

		}

		/// <summary>
		/// Перейти к настройкам
		/// </summary>
		public void GoToSettings()
		{
			((MainViewModel)Parent).ChangeActiveItem(new SettingsViewModel());
		}

		/// <summary>
		/// Действие при нажатии на клавишу PauseBreak
		/// </summary>
		public void PauseButtonAction()
		{
			if (VideoPlayer == null) return;

			if (VlcPlayer == null)
			{
				if (VideoPlayer.Topmost)
				{
					if (VideoPlayer.WindowState == WindowState.Maximized)
					{
						VideoPlayer.WindowState = WindowState.Normal;
					}

					VideoPlayer.Topmost = false;
				}
			}
			else
			{
				VideoPlayer.Topmost = false;
				VideoPlayer.Play();
			}

			NotifyOfPropertyChange(() => CanStart);
			ActivateDeactivateTray((Window)((MainViewModel)Parent).GetView());
		}


		#region Меню разработчика

		/// <summary>
		/// Открыть меню разработчика
		/// </summary>
		public void SecretAction()
		{
			if (SecretVisibility == Visibility.Visible)
			{
				SecretVisibility = Visibility.Collapsed;
				return;
			}

			if (WinMan.ShowDialog(new SecretViewModel()) is true)
			{
				SecretVisibility = Visibility.Visible;
			}
		}

		/// <summary>
		/// Добавление эпизодов
		/// </summary>
		public async Task<bool> AddSouthPark()
		{
			if (CanAddAddSouthPark is false) return false;
			AddingFilmCancellationTokenSource = new CancellationTokenSource();
			AddingFilmToken = AddingFilmCancellationTokenSource.Token;
			using (var db = new LiteDatabase(DBPath))
			{
				var films = db.GetCollection<Film>("Film");

				try
				{
					var wvm = new WaitViewModel();
					WinMan.ShowWindow(wvm);

					((Window)((MainViewModel)Parent).GetView()).IsEnabled = false;

					await Task.Run(() =>
					{
						var film = CreateSP(wvm);
						if (AddingFilmToken.IsCancellationRequested is false)
						{
							films.Insert(film);
						}
					});
					
					((Window)((MainViewModel)Parent).GetView()).IsEnabled = true;
					wvm.TryClose();
				}
				catch (Exception e)
				{
					WinMan.ShowWindow(new DialogViewModel(e.ToString(), DialogType.ERROR, e));
					return false;
				}
				Films = new BindableCollection<Film>(films.FindAll());
				CheckedValidation();
			}

			return AddingFilmToken.IsCancellationRequested is false;
		}

		public bool CanAddAddSouthPark => Films.Count < 1;

		/// <summary>
		/// Выполнить команду
		/// </summary>
		public async void StartCommand()
		{
			if (string.IsNullOrWhiteSpace(SelectedCommand)) return;
			bool result;
			switch (SelectedCommand)
			{
				case "Добавить \"Южный Парк\"":
					result = await AddSouthPark();
					break;
				case "Проверить длительности эпизодов":
					result = ValidateEpisodesDuration();
					break;
				case "Удалить все фильмы":
					result = RemoveFilms();
					break;
				default:
					WinMan.ShowDialog(new DialogViewModel("Неизвестная команда", DialogType.INFO));
					return;
			}
			WinMan.ShowDialog(new DialogViewModel(result ? "Команда успешно выполнена" : "Команда не выполнена", DialogType.INFO));
		}

		/// <summary>
		/// Удалить все эпизоды
		/// </summary>
		public bool RemoveFilms()
		{
			if (CanRemoveFilms is false) return false;
			using (var db = new LiteDatabase(DBPath))
			{
				db.DropCollection("Film");
				Films = new BindableCollection<Film>(db.GetCollection<Film>().FindAll());
				CheckedValidation();
			}

			return true;
		}

		public bool CanRemoveFilms => Films.Count >= 1;

		public bool ValidateEpisodesDuration()
		{
			if (CanValidateEpisodesDuration is false) return false;

			var result = new List<string>();
			var count = 1;


			foreach (var film in Films)
			{
				foreach (var episode in film.Episodes)
				{
					result.Add($"{count:000})\t{film.Name} - {episode.FullNumber}, {episode.Address.Address} - {episode.Address.TotalDuration}");
					count++;
				}
			}

			var filePath = $"{AppPath}\\{DevelopmentDataFolderName}\\{LogName}";

			using (var fs = new FileStream(filePath, System.IO.FileMode.Create))
			{
				fs.Dispose();
			}

			using (var sw = new StreamWriter(filePath))
			{
				foreach (var str in result)
				{
					sw.WriteLine(str);
				}
				sw.Dispose();
			}

			return true;
		}

		public bool CanValidateEpisodesDuration => Films.Count >= 1;

		#endregion

	}
}
