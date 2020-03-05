// ReSharper disable once CheckNamespace
// ReSharper disable CompareOfFloatsByEqualityOperator
namespace EasyViewer.ViewModels
{
	using System;
	using System.Timers;
	using System.Windows;
	using System.Windows.Input;
	using Helpers.Creators;
	using Models.FilmModels;
	using Vlc.DotNet.Core;
	using static Helpers.GlobalMethods;
	using static Helpers.SystemVariables;
	using KeyEventArgs = System.Windows.Input.KeyEventArgs;
	using Screen = Caliburn.Micro.Screen;

	public partial class VideoPlayerViewModel : Screen
	{

		#region VideoPlayer events

		/// <summary>
		/// Проиграть/Поставить на паузу эпизод
		/// </summary>
		public void Play()
		{
			if (VlcPlayer.IsPlaying() is true) VlcPlayer.Pause();
			else VlcPlayer.Play();
		}
		/// <summary>
		/// Перемотка эпизода назад на величину FFStep
		/// </summary>
		public void FastForwardLeft()
		{
			if (CanFastForwardLeft is false || VlcPlayer.Time <= 0) return;

			if (VlcPlayer.Time <= FFStep)
			{
				VlcPlayer.Time = 0;
			}
			else
			{
				VlcPlayer.Time -= FFStep;
			}
		}

		public bool CanFastForwardLeft => VlcPlayer.IsPlaying();

		/// <summary>
		/// Перемотка эпизода вперёд на величину FFStep
		/// </summary>
		public void FastForwardRight()
		{
			if (CanFastForwardRight is false || VlcPlayer.Time == VlcPlayer.Length) return;

			if (VlcPlayer.Time >= VlcPlayer.Length - FFStep)
			{
				VlcPlayer.Time = FFStep;
			}
			else
			{
				VlcPlayer.Time += FFStep;
			}
		}
		public bool CanFastForwardRight => VlcPlayer.IsPlaying();

		/// <summary>
		/// Действие при выставлении фильма на паузу
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void VideoPlayer_Paused(object sender, VlcMediaPlayerPausedEventArgs e)
		{
			if (Topmost is true) Topmost = false;
			NotifyOfPropertyChange(() => IsPlaying);
		}

		/// <summary>
		/// Действие при проигрывании фильма
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void VideoPlayer_Playing(object sender, VlcMediaPlayerPlayingEventArgs e)
		{
			if (Topmost is false) Topmost = true;
			NotifyOfPropertyChange(() => IsPlaying);
		}

		#endregion

		/// <summary>
		/// Действия при каждом тике таймера
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (VlcPlayer == null) return;

			if (VlcPlayer.CouldPlay is false)
			{
				CreatorMethods.LogError(CurrentEpisode.Address.Address);
				Vlc.Dispose();

				MMVM.CloseVideoPlayer();
			}

			if (CurrentMode == VideoPlayerMode.Preview
			    && (VlcPlayer.IsPlaying() is false
			        || VlcPlayer.Time <= FFStep))
			{
				NotifyOfPropertyChange(() => CanFastForwardLeft);
				NotifyOfPropertyChange(() => CanFastForwardRight);
			}
			
			CurrentEpisodeTime = TimeSpan.FromMilliseconds(VlcPlayer.Time);

			if (CurrentEpisodeTime >= CurrentEpisode.Address.FilmEndTime) PlayNextEpisode();
			
			JumperTimerActions(CurrentJumper);
		}

		

		#region Window events

		/// <summary>
		/// Установить полноэкранный режим
		/// </summary>
		public void SetFullScreen()
		{
			if (WindowState == WindowState.Normal)
			{
				WindowVisibility = Visibility.Collapsed;
				WindowState = WindowState.Maximized;
				WindowVisibility = Visibility.Visible;
			}
			else
			{
				WindowState = WindowState.Normal;
			}
		}

		/// <summary>
		/// Действие при нажатии клавиши клавиатуры
		/// </summary>
		/// <param name="e"></param>
		public void KeyDown(KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Escape:
					MMVM.CloseVideoPlayer();
					break;
				case Key.F:
					SetFullScreen();
					break;
				case Key.Right:
					if (VlcPlayer.Time > CurrentEpisode.Address.FilmEndTime.TotalMilliseconds - 5000)
					{
						VlcPlayer.Stop();
					}
					else
					{
						VlcPlayer.Time += 5000;
					}
					e.Handled = true;
					break;
				case Key.Left:
					if (VlcPlayer.Time < 5000)
					{
						VlcPlayer.Time = 0;
					}
					else
					{
						VlcPlayer.Time -= 5000;
					}

					e.Handled = true;
					break;
			}
		}

		/// <summary>
		/// Перемещение видеоплеера
		/// </summary>
		public void MoveWindow()
		{
			(GetView() as Window)?.DragMove();
		}

		/// <summary>
		/// Показать/Скрыть панель управления видеоплеера
		/// </summary>
		public void ShowHideControl()
		{
			ControlVisibility = ControlVisibility == Visibility.Collapsed
				? Visibility.Visible
				: Visibility.Collapsed;
		}

		/// <summary>
		/// Сохранить изменения
		/// </summary>
		public void SaveChanges()
		{
			var hasChanges = false;
			if (WindowState == WindowState.Normal)
			{
				var view = ((Window)GetView());
				var winLoc = new Point(view.Left, view.Top);

				if (AppVal.WS.VPStartupPos != winLoc)
				{
					AppVal.WS.VPStartupPos = winLoc;
					hasChanges = true;
				}

				if (AppVal.WS.VPSize.X != WindowWidth ||
					AppVal.WS.VPSize.Y != WindowHeight)
				{
					AppVal.WS.VPSize = new Point(WindowWidth, WindowHeight);
					hasChanges = true;
				}
			}

			if (hasChanges)
			{
				UpdateDbCollection(obj: AppVal.WS);
			}
		}

		#endregion

	}
}
