namespace EasyViewer.Helpers.Creators
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Threading;
	using ViewModels;
	using static SystemVariables;

	public static class CreatorMethods
	{
		/// <summary>
		/// Получение длительноси фильма
		/// </summary>
		/// <param name="filmName">Название фильма</param>
		/// <param name="address">Адрес фильма</param>
		/// <param name="fullNumber">Полный номер эпизода (для логирования ошибки)</param>
		/// <returns></returns>
		public static TimeSpan GetEpisodeDuration(string filmName, Uri address, string fullNumber, WaitViewModel wvm, Stopwatch stopwatch)
		{
			VlcPlayer.Play(address);

			while (!VlcPlayer.IsPlaying())
			{
				Thread.Sleep(50);
				Thread.Yield();
				wvm.RemainingTime = stopwatch.Elapsed;
				if (VlcPlayer.CouldPlay) continue;

				LogError(address, filmName, fullNumber);

				VlcPlayer.Stop();

				return new TimeSpan();
			}

			var result = TimeSpan.FromMilliseconds(VlcPlayer.Length);

			VlcPlayer.Stop();
			Thread.Yield();

			return result;
		}
		/// <summary>
		/// Запись ошибки в файл
		/// </summary>
		/// <param name="address">Некорректный адрес</param>
		/// <param name="filmName">Название фильма</param>
		/// <param name="fullNumber">Полный номер эпизода</param>
		public static void LogError(Uri address, string fullNumber = null, string filmName = null)
		{
			string error;

			error = fullNumber == null || filmName == null
				? $"Адрес {address} возможно не существует, либо нет интернет соединения"
				: $"Адрес {address} возможно не существует, либо нет интернет соединения.\nПолный номер недоступного эпизода {fullNumber} в фильме \"{filmName}\"";
			var filePath = $"{AppPath}\\{ErrorLogsFolderName}\\{LogName}";

			using (var fs = new FileStream(filePath, FileMode.Create))
			{
				fs.Dispose();
			}

			using (var sw = new StreamWriter(filePath))
			{
				sw.WriteLine(error);
				sw.Dispose();
			}
		}
	}
}
