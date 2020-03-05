// ReSharper disable once CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
	using System;
	using System.Linq;
	using System.Threading;
	using Caliburn.Micro;
	using EasyViewer.ViewModels;
	using Helpers;
	using Vlc.DotNet.Wpf;
	using static Helpers.SystemVariables;

	public partial class AddressEditingViewModel : Screen
	{
		/// <summary>
		/// Проверка адреса на доступность
		/// </summary>
		/// <param name="address">Адрес</param>
		/// <returns></returns>
		private bool CheckAddress(Uri address)
		{
			Vlc = new VlcControl();
			Vlc.SourceProvider.CreatePlayer(VlcDataPath);

			VlcPlayer.Play(address);

			while (VlcPlayer.IsPlaying() is false)
			{
				Thread.Sleep(100);
				Thread.Yield();

				if (VlcPlayer.CouldPlay) continue;

				return false;
			}

			var duration = TimeSpan.FromMilliseconds(VlcPlayer.Length);
			//Vlc = null;
			Vlc.Dispose();
			CurrentAddress.TotalDuration = duration;
			CurrentAddress.FilmEndTime = duration;

			return true;
		}

		/// <summary>
		/// ПРоерить номер джампера на корректность
		/// </summary>
		/// <param name="number">Номер джампера</param>
		/// <returns></returns>
		public (bool status, string reason) CheckJumperNumber(int? number)
		{
			if (number == null) return (false, "недопустимый номер");

			if (Jumpers.Any(j => j.Number == number))
			{
				var dvm = new DialogViewModel("Номер уже существует, хотите ли вы поменять их местами?", DialogType.QUESTION);

				WinMan.ShowDialog(dvm);

				switch (dvm.DialogResult)
				{
					case DialogResult.YES_ACTION:
						Jumpers.First(j => j.Number == number).Number = SelectedJumper.Number;
						return (true, "Операция успешно завершена");
					case DialogResult.NO_ACTION:
						return (false, "Данный номер уже существует");
					default:
						return (false, "Операция отменена");
				}
			}

			return number <= Jumpers.Count
				? (false, "Все номера должны быть по порядку без пропусков")
				: (true, "Операция успешно завершена");
		}

		public void NotifyChanges()
		{
			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => CanCancelChanges);
		}
	}
}