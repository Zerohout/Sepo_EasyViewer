// ReSharper disable once CheckNamespace
namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
	using System;
	using System.Linq;
	using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.Micro;
	using EasyViewer.ViewModels;
	using Helpers;
    using LibVLCSharp.Shared;
    using Vlc.DotNet.Wpf;
	using static Helpers.SystemVariables;

	public partial class AddressEditingViewModel : Screen
	{
		/// <summary>
		/// Проверка адреса на доступность
		/// </summary>
		/// <param name="address">Адрес</param>
		/// <returns></returns>
		private async Task<bool> CheckAddress(Uri address)
		{
			var libVlc = new LibVLC();
			var media = new Media(_libVlc, address);
            await media.Parse(MediaParseOptions.ParseNetwork);

            if (media.ParsedStatus == MediaParsedStatus.Failed)
            {
				media.Dispose();
				libVlc.Dispose();
                return false;
            }
			var duration = media.Duration;
			CurrentAddressInfo.TotalDuration = TimeSpan.FromMilliseconds(duration);
			CurrentAddressInfo.FilmEndTime = TimeSpan.FromMilliseconds(duration);
            media.Dispose();
            libVlc.Dispose();
			return true;
		}

		/// <summary>
		/// Проверить номер джампера на корректность
		/// </summary>
		/// <param name="number">Номер джампера</param>
		/// <returns></returns>
		public (bool status, string reason) CheckJumperNumber(int? number)
		{
			if (number == null) return (false, "недопустимый номер");

			if (Jumpers.Any(j => j.Number == number))
			{
				var dvm = new DialogViewModel("Номер уже существует, хотите ли вы поменять их местами?", DialogType.Question);

				WinMan.ShowDialog(dvm);

				switch (dvm.DialogResult)
				{
					case DialogResult.YesAction:
                        var jumper = Jumpers.First(j => j.Number == number);
						jumper.Number = SelectedJumper.Number;
						DbMethods.UpdateDbCollection(jumper);
						return (true, "Операция успешно завершена");
					case DialogResult.NoAction:
						return (false, "Данный номер уже существует");
					default:
						return (false, "Операция отменена");
				}
			}

			return number > Jumpers.Count
				? (false, "Все номера должны быть по порядку без пропусков")
				: (true, "Операция успешно завершена");
		}

        private void SetJumperFieldsVisibility(JumperMode mode)
        {
            switch (mode)
            {
                case JumperMode.Skip:
                case JumperMode.Mute:
                    VolumeValueVisibility = Visibility.Hidden;
                    EndValueVisibility = Visibility.Visible;
                    break;
                case JumperMode.IncreaseVolume:
                case JumperMode.LowerVolume:
                    VolumeValueVisibility = Visibility.Visible;
                    EndValueVisibility = Visibility.Hidden;
                    break;
            }
        }

		public void NotifyChanges()
		{
			NotifyOfPropertyChange(() => CanSaveChanges);
			NotifyOfPropertyChange(() => CanCancelChanges);
		}
	}
}