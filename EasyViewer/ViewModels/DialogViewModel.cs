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
                    case DialogType.SaveChanges:
                        return "Сохранить изменения?";
                    case DialogType.CancelChanges:
                        return "Отменить изменения?";
                    case DialogType.RemoveObject:
                        return "Удалить объект?";
                    case DialogType.OverwriteFile:
                        return "Перезаписать файл?";
                    case DialogType.Question:
                        return "Вы хотите это сделать?";
                    case DialogType.Info:
                        return "Информация.";
                    case DialogType.Error:
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
                    case DialogType.SaveChanges:
                        return "Сохранить ваши изменения?";
                    case DialogType.CancelChanges:
                        return "Отменить ваши изменения?";
                    case DialogType.RemoveObject:
                        return $"Вы действительно хотите удалить выбранный объект?";
                    case DialogType.OverwriteFile:
                        return "Файл уже существует, перезаписать его?";
                    case DialogType.Question:
                        return "Да или нет?";
                    case DialogType.Info:
                        return "Информация для пользователя.";
                    case DialogType.Error:
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
            CurrentType == DialogType.SaveChanges ||
            CurrentType == DialogType.CancelChanges ||
            CurrentType == DialogType.RemoveObject ||
            CurrentType == DialogType.Question ||
            CurrentType == DialogType.OverwriteFile ||
            CurrentType == DialogType.Error
                ? Visibility.Visible
                : Visibility.Hidden;
        /// <summary>
        /// Видимость кнопки No
        /// </summary>
        public Visibility NoVisibility =>
            CurrentType == DialogType.SaveChanges ||
            CurrentType == DialogType.CancelChanges ||
            CurrentType == DialogType.RemoveObject ||
            CurrentType == DialogType.Question ||
            CurrentType == DialogType.OverwriteFile ||
            CurrentType == DialogType.Error
                ? Visibility.Visible
                : Visibility.Hidden;

        /// <summary>
        /// Видимость кнопки Cancel
        /// </summary>
        public Visibility CancelVisibility =>
            CurrentType == DialogType.SaveChanges
                ? Visibility.Visible
                : Visibility.Hidden;
        /// <summary>
        /// Видимость кнопки Ok
        /// </summary>
        public Visibility OkVisibility =>
            CurrentType == DialogType.Info ||
            CurrentType == DialogType.Error
                ? Visibility.Visible
                : Visibility.Hidden;

        /// <summary>
        /// Действие положительного ответа
        /// </summary>
        public void YesAction()
        {
            if (CurrentType == DialogType.Error)
            {
                Clipboard.SetText(_message);
                var dvm = new DialogViewModel("Сохранить информацию ошибки в файл?", DialogType.Question);
                WinMan.ShowDialog(dvm);

                if (dvm.DialogResult == DialogResult.YesAction)
                {
                    CreateErrorLog();
                }

                TryClose();
                return;
            }

            DialogResult = DialogResult.YesAction;
            TryClose();
        }

        /// <summary>
        /// Действие отрицания
        /// </summary>
        public void NoAction()
        {
            if (CurrentType == DialogType.Error)
            {
                CreateErrorLog();
                TryClose();
                return;

            }

            DialogResult = DialogResult.NoAction;
            TryClose();
        }

        
        /// <summary>
        /// Действие отмены
        /// </summary>
        public void CancelAction()
        {
            DialogResult = DialogResult.CancelAction;
            TryClose();
        }

        /// <summary>
        /// Создать файл с данными об ошибке
        /// </summary>
        private void CreateErrorLog()
        {
            var filePath = $"{AppPath}\\{ErrorLogsFolderName}\\{LogName}";

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
                                                  DialogType.Info));
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
