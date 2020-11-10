// ReSharper disable CheckNamespace
namespace EasyViewer.Settings.WatchingSettingsFolder.ViewModels
{
    using Caliburn.Micro;
    using Models.FilmModels;
    using static Helpers.DbMethods;

    public partial class WatchingSettingsViewModel : Screen
    {
        /// <summary>
        /// Загрузка данных
        /// </summary>
        private void LoadData()
        {
            LoadWS();
            SetNonRepeatTimeType();
            Films = new BindableCollection<Film>(GetDbCollection<Film>()) { new Film { Name = "всех" } };
            NotifyOfPropertyChange(() => CheckedEpisodes);
        }

        private void SetNonRepeatTimeType()
        {
            switch (WatchingSettings.NonRepeatDaysInterval)
            {
                case 0:
                    SelectedNoneRepeatTimeType = "всегда";
                    break;
                case 365:
                    case 366:
                    SelectedNoneRepeatTimeType = "никогда";
                    break;
                default:
                    SelectedNoneRepeatTimeType = "дней";
                   break;
            }
        }

        /// <summary>
        /// Загрузить настройки просмотра
        /// </summary>
        private void LoadWS()
        {
            WatchingSettings = LoadOrCreateWatchingSettings();
            TempWS = LoadOrCreateWatchingSettings();
            NotifyButtons();
        }

        private void NotifyButtons()
        {
            NotifyOfPropertyChange(() => HasChanges);
            NotifyOfPropertyChange(() => CanSaveChanges);
            NotifyOfPropertyChange(() => CanResetLastDateViewed);
            NotifyOfPropertyChange(() => CanCancelChanges);
            NotifyOfPropertyChange(() => CanSetDefaultValues);
        }

        /// <summary>
        /// Установить значение времени, в течение которого просмотренный эпизод не будет повторяться
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int? SetNonRepeatTime(int? value)
        {
            if(value == null ||
               value <= 0)
            {
                SelectedNoneRepeatTimeType = "всегда";

                return 1;
            }

            switch(_selectedNoneRepeatTimeType)
            {
                case "дней":
                    if(value > 36_525)
                    {
                        SelectedNoneRepeatTimeType = "никогда";
                        return 36_525;
                    }
                    break;
            }

            return value;
        }
        /// <summary>
        /// Конвертация значений NonRepeatTime в зависимости от выбранного типа времени
        /// </summary>
        /// <param name="tempTypeValue"></param>
        /// <returns></returns>
        private int? ConvertNonRepeatTime(string tempTypeValue)
        {
            var timeValue = _noneRepeatTimeCount;
            var typeValue = _selectedNoneRepeatTimeType;

            if(tempTypeValue == null)
                return null;

            switch(typeValue)
            {
                case "никогда":
                    return null;
                case "дней":
                    switch(tempTypeValue)
                    {
                        case "никогда":
                            break;
                        case "всегда":
                            return null;
                    }
                    break;
                case "всегда":
                    return null;
            }

            return timeValue;
        }
    }
}
