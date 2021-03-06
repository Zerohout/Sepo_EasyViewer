﻿// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.WatchingSettingsFolder.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Caliburn.Micro;
    using EasyViewer.ViewModels;
    using Helpers;
    using Models.FilmModels;
    using Models.SettingModels;
    using static Helpers.SystemVariables;
    using static Helpers.DbMethods;

    public partial class WatchingSettingsViewModel : Screen
    {
        /// <summary>
        /// Сброс даты последнего просмотра у всех эпизодов
        /// </summary>
        public void TotalEpisodesReset()
        {
            var dvm = new DialogViewModel(
                "Внимание, данное действие сбросит дату последнего просмотра у ВСЕХ эпизодов.\nВы хотите продолжить?",
                DialogType.Question);
            WinMan.ShowDialog(dvm);

            if (dvm.DialogResult == DialogResult.YesAction)
            {
                var films = GetDbCollection<Film>();

                films.SelectMany(f => f.Episodes).ToList().ForEach(e => e.LastDateViewed = AppVal.ResetTime);

                UpdateDbCollection(films.Where(f => f.Name != "всех"));
            }
        }

        /// <summary>
        /// Установить все значения по умолчанию
        /// </summary>
        public void SetDefaultValues()
        {
            if (CanSetDefaultValues is false)
            {
                NotifyOfPropertyChange(() => CanSetDefaultValues);
                return;
            }

            var dvm = new DialogViewModel("Данная операция безвозвратна. Вы действительно хотите установить настройки по умолчанию?",
                                          DialogType.Question);
            WinMan.ShowDialog(dvm);

            if (dvm.DialogResult == DialogResult.NoAction)
            {
                return;
            }

            WatchingSettings = new WatchingSettings { Id = WatchingSettings.Id };
            UpdateDbCollection(entity: WatchingSettings);
            //SaveChanges();
            LoadWS();
            NotifyButtons();

            WinMan.ShowDialog(new DialogViewModel("Данные успешно сброшены по умоланию", DialogType.Info));
        }

        public bool CanSetDefaultValues => GlobalMethods.IsEquals(TempWS, new WatchingSettings()) is false;

        /// <summary>
        /// Отмена изменений
        /// </summary>
        public void CancelChanges()
        {
            if (CanCancelChanges is false)
            {
                NotifyOfPropertyChange(() => CanCancelChanges);
                return;
            }

            var dvm = new DialogViewModel("Данная операция безвозвратна. Вы действительно хотите отменить изменения?",
                                          DialogType.Question);
            WinMan.ShowDialog(dvm);

            if (dvm.DialogResult == DialogResult.NoAction)
            {
                return;
            }

            WatchingSettings = TempWS;
            SaveChanges();
            LoadWS();
            NotifyButtons();
        }

        public bool CanCancelChanges => HasChanges;

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        public void SaveChanges()
        {
            if (CanSaveChanges is false)
            {
                NotifyOfPropertyChange(() => CanSaveChanges);
                return;
            }

            UpdateDbCollection(entity: WatchingSettings);
            LoadWS();
            NotifyButtons();

            WinMan.ShowDialog(new DialogViewModel("Изменения успешно сохранены", DialogType.Info));
        }

        public bool CanSaveChanges => HasChanges;


        /// <summary>
        /// Сбросить последнюю дату просмотра у эпизодов
        /// </summary>
        public void ResetLastDateViewed()
        {
            if (CanResetLastDateViewed is false)
            {
                NotifyOfPropertyChange(() => CanResetLastDateViewed);
            }

            DialogViewModel dvm;
            var filmName = SelectedGlobalResetFilm.Name;

            if (filmName == "всех")
            {
                dvm = new DialogViewModel($"Вы уверены что хотите сбросить даты последнего просмотра " +
                                          $"ВСЕХ эпизодов? Эта операция необратима.", DialogType.Question);
            }
            else
            {
                dvm = new DialogViewModel($"Вы уверены что хотите сбросить даты последнего просмотра " +
                                          $"всех эпизодов \"{filmName}\"? " +
                                          "Эта операция необратима.", DialogType.Question);
            }

            WinMan.ShowDialog(dvm);

            if (dvm.DialogResult == DialogResult.YesAction)
            {
                var resetEpisodes = new List<Episode>();
                if (filmName == "всех")
                {
                    Films.ToList().ForEach(s => resetEpisodes.AddRange(s.Episodes));
                }
                else
                {
                    resetEpisodes.AddRange(Films.First(f => f.Name == filmName).Episodes);
                }

                if (resetEpisodes.All(re => re.LastDateViewed == AppVal.ResetTime))
                {
                    WinMan.ShowDialog(new DialogViewModel("Дата уже сброшена.", DialogType.Info));
                    return;
                }

                foreach (var ep in resetEpisodes)
                {
                    ep.LastDateViewed = AppVal.ResetTime;
                }

                UpdateDbCollection(objCollection: resetEpisodes);
                WinMan.ShowDialog(new DialogViewModel("Дата успешно сброшена.", DialogType.Info));
            }
            NotifyOfPropertyChange(() => CanResetLastDateViewed);
        }

        public bool CanResetLastDateViewed => SelectedGlobalResetFilm != null;




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
        /// действие при переключении флагов
        /// </summary>
        public void CheckedStatusChanged()
        {
            NotifyOfPropertyChange(() => NightHelperSettingsVisibility);
            NotifyOfPropertyChange(() => WatchingSettings);
            NotifyOfPropertyChange(() => RandomEnabledVisibility);
            NotifyOfPropertyChange(() => WatchingInRowVisibility);
            NotifyButtons();
        }

        /// <summary>
        /// Действие при изменении текста
        /// </summary>
        public void TextChanged()
        {
            NotifyOfPropertyChange(() => WatchingSettings);
            NotifyOfPropertyChange(() => DefaultEpisodesActualDuration);
            NotifyButtons();
        }

        public void ImportSettingsToFile()
        {

        }

        //public bool CanImportSettingsToFile => IsEquals(new WatchingSettings(), WatchingSettings) is false;

        public void ExportSettingsFromFile()
        {

        }

        public bool CanExportSettingsFromFile => true;

        public void SelectionChanged()
        {
            switch (SelectedNoneRepeatTimeType)
            {
                case "никогда":
                    WatchingSettings.NonRepeatDaysInterval = 366;
                    break;
                case "всегда":
                    WatchingSettings.NonRepeatDaysInterval = null;
                    break;
            }
            NotifyButtons();
        }

        public void NonRepeatIntervalChanged()
        {
            NotifyButtons();
        }
    }
}
