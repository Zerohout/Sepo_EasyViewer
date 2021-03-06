﻿namespace EasyViewer.Helpers.Creators
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Threading;
	using System.Threading.Tasks;
	using ViewModels;
	using static SystemVariables;

	public static class CreatorMethods
	{
		/// <summary>
		/// Запись ошибки в файл
		/// </summary>
		/// <param name="address">Некорректный адрес</param>
		/// <param name="filmName">Название фильма</param>
		/// <param name="fullNumber">Полный номер эпизода</param>
		public static void LogError(Uri address, int? fullNumber = null, string filmName = null)
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

		/// <summary>
		/// Получить количество эпизодов в сезоне фильма Южный Парк
		/// </summary>
		/// <param name="seasonNum">Номер сезона</param>
		/// <returns></returns>
		public static int GetSPEpisodesCount(int seasonNum)
		{
			switch (seasonNum)
			{
				case 1:
					return 13;
				case 2:
					return 18;
				case 3:
				case 4:
				case 6:
					return 17;
				case 7:
					return 15;
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
					return 10;
				case 24:
                    return 1;
				default:
					return 14;
			}
		}
	}
}
