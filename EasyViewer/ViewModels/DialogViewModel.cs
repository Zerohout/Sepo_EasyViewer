namespace EasyViewer.ViewModels
{
    using System;
    using System.IO;
    using System.Windows;
    using Caliburn.Micro;
    using static Helpers.SystemVariables;

    public class DialogViewModel : Screen
    {
        private readonly string _dialogTitle;
        private readonly string _message;
        private readonly Exception _exception;
        private DialogType _currentType;

        public DialogViewModel()
        {

        }

        public DialogViewModel(string message, DialogType currentType,
            Exception exception = null, string dialogTitle = null)
        {
            CurrentType = currentType;
            _message = message;
            _dialogTitle = dialogTitle;
            _exception = exception;
        }

        /// <summary>
        /// Тип диалогового окна
        /// </summary>
        public DialogType CurrentType
        {
            get => _currentType;
            set
            {
                _currentType = value;
                NotifyOfPropertyChange(() => CurrentType);
                NotifyOfPropertyChange(() => YesVisibility);
                NotifyOfPropertyChange(() => NoVisibility);
                NotifyOfPropertyChange(() => CancelVisibility);
                NotifyOfPropertyChange(() => OkVisibility);
            }
        }

        public DialogResult DialogResult { get; set; }

        /// <summary>
        /// Заголовок диалогового окна
        /// </summary>
        public string DialogTitle
        {
            get
            {
                if (_dialogTitle != null) return _dialogTitle;

                switch (_currentType)
                {
                    case DialogType.SAVE_CHANGES:
                        return "Сохранить изменения?";
                    case DialogType.CANCEL_CHANGES:
                        return "Отменить изменения?";
                    case DialogType.REMOVE_OBJECT:
                        return "Удалить объект?";
                    case DialogType.OVERWRITE_FILE:
                        return "Перезаписать файл?";
                    case DialogType.QUESTION:
                        return "Вы хотите это сделать?";
                    case DialogType.INFO:
                        return "Информация.";
                    case DialogType.ERROR:
                        return "Ошибка";
                    default:
                        throw new Exception("Некорректный DialogType");
                }
            }
        }
        /// <summary>
        /// Сообщение внутри диалогового окна
        /// </summary>
        public string Message
        {
            get
            {
                if (_message != null) return _message;

                switch (_currentType)
                {
                    case DialogType.SAVE_CHANGES:
                        return "Сохранить ваши изменения?";
                    case DialogType.CANCEL_CHANGES:
                        return "Отменить ваши изменения?";
                    case DialogType.REMOVE_OBJECT:
                        return $"Вы действительно хотите удалить выбранный объект?";
                    case DialogType.OVERWRITE_FILE:
                        return "Файл уже существует, перезаписать его?";
                    case DialogType.QUESTION:
                        return "Да или нет?";
                    case DialogType.INFO:
                        return "Информация для пользователя.";
                    case DialogType.ERROR:
                        return "Произошла ошибка.";
                    default:
                        throw new Exception("Некорректный DialogType");
                }
            }
        }

        /// <summary>
        /// Видимость кнопки Yes
        /// </summary>
        public Visibility YesVisibility =>
            CurrentType == DialogType.SAVE_CHANGES ||
            CurrentType == DialogType.CANCEL_CHANGES ||
            CurrentType == DialogType.REMOVE_OBJECT ||
            CurrentType == DialogType.QUESTION ||
            CurrentType == DialogType.OVERWRITE_FILE ||
            CurrentType == DialogType.ERROR
                ? Visibility.Visible
                : Visibility.Hidden;
        /// <summary>
        /// Видимость кнопки No
        /// </summary>
        public Visibility NoVisibility =>
            CurrentType == DialogType.SAVE_CHANGES ||
            CurrentType == DialogType.CANCEL_CHANGES ||
            CurrentType == DialogType.REMOVE_OBJECT ||
            CurrentType == DialogType.QUESTION ||
            CurrentType == DialogType.OVERWRITE_FILE ||
            CurrentType == DialogType.ERROR
                ? Visibility.Visible
                : Visibility.Hidden;

        /// <summary>
        /// Видимость кнопки Cancel
        /// </summary>
        public Visibility CancelVisibility =>
            CurrentType == DialogType.SAVE_CHANGES
                ? Visibility.Visible
                : Visibility.Hidden;
        /// <summary>
        /// Видимость кнопки Ok
        /// </summary>
        public Visibility OkVisibility =>
            CurrentType == DialogType.INFO ||
            CurrentType == DialogType.ERROR
                ? Visibility.Visible
                : Visibility.Hidden;

        /// <summary>
        /// Действие положительного ответа
        /// </summary>
        public void YesAction()
        {
            if (CurrentType == DialogType.ERROR)
            {
                Clipboard.SetText(_message);
                var dvm = new DialogViewModel("Сохранить информацию ошибки в файл?", DialogType.QUESTION);
                WinMan.ShowDialog(dvm);

                if (dvm.DialogResult == DialogResult.YES_ACTION)
                {
                    CreateErrorLog();
                }

                TryClose();
                return;
            }

            DialogResult = DialogResult.YES_ACTION;
            TryClose();
        }

        /// <summary>
        /// Действие отрицания
        /// </summary>
        public void NoAction()
        {
            if (CurrentType == DialogType.ERROR)
            {
                CreateErrorLog();
                TryClose();
                return;

            }

            DialogResult = DialogResult.NO_ACTION;
            TryClose();
        }

        
        /// <summary>
        /// Действие отмены
        /// </summary>
        public void CancelAction()
        {
            DialogResult = DialogResult.CANCEL_ACTION;
            TryClose();
        }

        /// <summary>
        /// Создать файл с данными об ошибке
        /// </summary>
        private void CreateErrorLog()
        {
            var filePath = $"{AppPath}\\{ErrorLogsFolderName}\\{ErrorLogName}";

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                fs.Dispose();
            }

            using (var sw = new StreamWriter(filePath))
            {
                LoggingException(sw, _exception);
                sw.Dispose();
            }

            WinMan.ShowDialog(new DialogViewModel("Данные об ошибке сохранены в папке ErrorLogs в папке приложения",
                                                  DialogType.INFO));
        }

        /// <summary>
        /// Запись данных об ошибке в файл.
        /// </summary>
        /// <param name="sw">Экземпляр StreamWriter</param>
        /// <param name="exception">Возникшая ошибка</param>
        private void LoggingException(StreamWriter sw, Exception exception)
        {
            if (exception == null) return;

            sw.WriteLine("Ошибка:");
            sw.WriteLine(exception.ToString());
            sw.WriteLine(string.Empty);
            sw.WriteLine("Текст ошибки:");
            sw.WriteLine(exception.Message);
            sw.WriteLine(string.Empty);
            sw.WriteLine("Источник ошибки:");
            sw.WriteLine(exception.Source);
            sw.WriteLine(string.Empty);
            sw.WriteLine("Подробный StackTrace ошибки:");
            sw.WriteLine(exception.StackTrace);
            sw.WriteLine(string.Empty);
            sw.WriteLine("Возможное решение проблемы:");
            sw.WriteLine(exception.HelpLink);
            sw.WriteLine(string.Empty);
            sw.WriteLine("*************************************");

            LoggingException(sw, exception.InnerException);
        }
    }
}
