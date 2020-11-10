// ReSharper disable once CheckNamespace

namespace EasyViewer.Settings.FilmEditorFolder.ViewModels
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Forms;
    using Caliburn.Micro;
    using EasyViewer.ViewModels;
    using Helpers;
    using LibVLCSharp.Shared;
    using Models.FilmModels;
    using Newtonsoft.Json;
    using Vlc.DotNet.Wpf;
    using static Helpers.SystemVariables;
    using static Helpers.DbMethods;
    using Clipboard = System.Windows.Clipboard;
    using ComboBox = System.Windows.Controls.ComboBox;
    using KeyEventArgs = System.Windows.Input.KeyEventArgs;
    using ListBox = System.Windows.Controls.ListBox;
    using Screen = Caliburn.Micro.Screen;
    using TextBox = System.Windows.Controls.TextBox;

    public partial class AddressEditingViewModel : Screen
    {
        #region Общие действия

        /// <summary>
        ///     Двойной клик по текстовому полю
        /// </summary>
        /// <param name="source"></param>
        public void TBoxDoubleClick(TextBox source)
        {
            source.SelectAll();
        }


        /// <summary>
        ///     Действие при изменении текста в TextBox'е
        /// </summary>
        public void TextChanged(TextBox sender, TextChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => CurrentAddressInfo);
            NotifyOfPropertyChange(() => CanSetAddress);
            NotifyOfPropertyChange(() => CanSetJumperNumber);
            NotifyOfPropertyChange(() => SelectedJumper);
            NotifyChanges();
        }

        /// <summary>
        ///     Проверка является ли введенный сивол числом
        /// </summary>
        /// <param name="e"></param>
        public void NumericValidation(KeyEventArgs e)
        {
            if (e == null) return;
            e.Handled = (e.Key.GetHashCode() >= 34 && e.Key.GetHashCode() <= 43 ||
                         e.Key.GetHashCode() >= 74 && e.Key.GetHashCode() <= 83) is false;
            NotifyOfPropertyChange(() => CanSetJumperNumber);
            NotifyChanges();
        }

        public void SelectionChanged(ListBox sender)
        {
            SelectedJumpers = new BindableCollection<Jumper>(sender.SelectedItems.Cast<Jumper>());
            if (SelectedJumpers.Count == 1) sender.ScrollIntoView(sender.SelectedItem);
        }

        /// <summary>
        ///     Сохранить изменения
        /// </summary>
        public void SaveChanges()
        {
            if (CanSaveChanges is false) return;

            if (HasAddressChanges)
            {
                var episode = GetEpisodeFromDbById(CurrentAddressInfo.Episode.Id);
                var changeDefaultAddressInfo = Equals(episode.AddressInfo.Id, CurrentAddressInfo.Id);
                UpdateDbCollection(CurrentAddressInfo);
                AddressSnapshot = JsonConvert.SerializeObject(CurrentAddressInfo);
                if (changeDefaultAddressInfo)
                {
                    episode.AddressInfo = CurrentAddressInfo;
                    UpdateDbCollection(episode);
                }
            }

            if (HasJumperChanges)
            {
                UpdateDbCollection<Jumper>(Jumpers);
                Jumpers = new BindableCollection<Jumper>(CurrentAddressInfo.Jumpers);
                CancelSelection();
                RefreshJumpersConfig();
            }


            WinMan.ShowDialog(new DialogViewModel("Изменения успешно сохранены", DialogType.Info));
            if (IsInterfaceUnlocked is false) IsInterfaceUnlocked = true;

            NotifyChanges();
            NotifyOfPropertyChange(() => CanPlay);
            NotifyOfPropertyChange(() => CurrentEpisodeTime);
            NotifyOfPropertyChange(() => CurrentAddressInfo);
            NotifyOfPropertyChange(() => CanDecreaseVideoRate);
            NotifyOfPropertyChange(() => CanIncreaseVideoRate);
            NotifyOfPropertyChange(() => CanDefaultVideoRate);
            NotifyOfPropertyChange(() => CanAddFirstJumper);
            NotifyOfPropertyChange(() => CanAddVolumeJumper);
            NotifyOfPropertyChange(() => EEVM.CanSetDefaultAddress);
        }

        public bool CanSaveChanges => HasAddressChanges || HasJumperChanges;

        /// <summary>
        ///     Отменить изменения
        /// </summary>
        public void CancelChanges()
        {
            if (CanCancelChanges is false) return;

            if (HasAddressChanges)
            {
                CurrentAddressInfo = JsonConvert.DeserializeObject<AddressInfo>(AddressSnapshot);
            }
            else
            {
                SelectedJumper = JsonConvert.DeserializeObject<Jumper>(JumperSnapshot);
                CancelSelection();
            }

            if (IsInterfaceUnlocked is false) IsInterfaceUnlocked = true;
            NotifyChanges();
        }

        public bool CanCancelChanges => HasAddressChanges || HasJumperChanges;

        public override void TryClose(bool? dialogResult = null)
        {
            CancelChanges();
            var toDispose = _mediaPlayer;
            _mediaPlayer = null;
            Task.Run(() => { toDispose?.Dispose(); });
            _libVlc.Dispose();
            base.TryClose(dialogResult);
        }

        #endregion

        #region Действия адреса

        /// <summary>
        ///     Установить адрес
        /// </summary>
        public async void SetAddress()
        {
            if (CanSetAddress is false) return;

            if (await CheckAddress(NewAddress))
            {
                WinMan.ShowDialog(new DialogViewModel("Адрес успешно обновлен", DialogType.Info));
                CurrentAddressInfo.Link = NewAddress;
            }
            else
            {
                WinMan.ShowDialog(new DialogViewModel("Адрес некорректный или отсутствует интернет соединение",
                    DialogType.Info));
            }

            NotifyChanges();
            NotifyOfPropertyChange(() => CanSetAddress);
        }

        public void PasteFilmEndTime()
        {
            if (CanPasteFilmEndTime is false) return;
            if (int.TryParse(Clipboard.GetText(), out var time))
            {
                CurrentAddressInfo.FilmEndTime = TimeSpan.FromMilliseconds(time);
                NotifyOfPropertyChange(() => CurrentAddressInfo);
                NotifyChanges();
            }
        }

        public bool CanPasteFilmEndTime => Clipboard.ContainsText();

        public bool CanSetAddress => CurrentAddressInfo.Link != NewAddress;

        #endregion

        #region Действия видеоплеера

        public void FastBackward()
        {
            if (CanFastBackward is false) return;
           
            TempCurrentEpisodeTime = MediaPlayer.Time - RewindStep;
            CurrentEpisodeTime = TimeSpan.FromMilliseconds(MediaPlayer.Time);
            
        }

        public bool CanFastBackward => MediaPlayer.Media != null && MediaPlayer.Time > RewindStep;

        public void FastForward()
        {
            if (CanFastForward is false) return;

            TempCurrentEpisodeTime = MediaPlayer.Time + RewindStep;
            CurrentEpisodeTime = TimeSpan.FromMilliseconds(MediaPlayer.Time);
        }

        public bool CanFastForward => MediaPlayer.Media != null &&
                                      MediaPlayer.Time < CurrentAddressInfo.TotalDuration.TotalMilliseconds -
                                      RewindStep;

        public void Play()
        {
            if (CanPlay is false) return;

            if (MediaPlayer.Media == null)
            {
                MediaPlayer.Play(new Media(_libVlc, CurrentAddressInfo.Link));

                NotifyOfPropertyChange(() => CanFastBackward);
                NotifyOfPropertyChange(() => CanFastForward);
                NotifyOfPropertyChange(() => CanCopyCurrentTime);
                NotifyOfPropertyChange(() => MediaPlayer);
                NotifyOfPropertyChange(() => CanPlay);
                return;
            }

            if (MediaPlayer.IsPlaying is false) MediaPlayer.Play();
            else MediaPlayer.Pause();
            NotifyOfPropertyChange(() => MediaPlayer);
            NotifyOfPropertyChange(() => CanPlay);
        }


        public bool CanPlay => CurrentAddressInfo.TotalDuration > new TimeSpan() &&
                               MediaPlayer.Time < CurrentAddressInfo.TotalDuration.TotalMilliseconds - 5000;


        public void CopyCurrentTime()
        {
            Clipboard.SetText(MediaPlayer.Time.ToString());
            NotifyOfPropertyChange(() => CanPasteFilmEndTime);
            NotifyOfPropertyChange(() => CanPasteEndTime);
            NotifyOfPropertyChange(() => CanPasteStartTime);
            NotifyOfPropertyChange(() => CanAddFirstJumper);
        }

        public bool CanCopyCurrentTime => MediaPlayer.Media != null;

        public async void VolumeUp()
        {
            if (CanVolumeUp is false) return;
            await Task.Run(() =>
            {
                if (MediaPlayer.Volume > 95) MediaPlayer.Volume = 100;
                else MediaPlayer.Volume += 5;
            });
            NotifyOfPropertyChange(() => CanVolumeUp);
            NotifyOfPropertyChange(() => CanVolumeDown);
            NotifyOfPropertyChange(() => MediaPlayer);
        }

        public bool CanVolumeUp => MediaPlayer.Volume < 100;

        public async void VolumeDown()
        {
            if (CanVolumeDown is false) return;

            await Task.Run(() =>
            {
                if (MediaPlayer.Volume < 5) MediaPlayer.Volume = 0;
                else MediaPlayer.Volume -= 5;
            });
            
            NotifyOfPropertyChange(() => CanVolumeDown);
            NotifyOfPropertyChange(() => CanVolumeUp);
            NotifyOfPropertyChange(() => MediaPlayer);
        }

        public bool CanVolumeDown => MediaPlayer.Volume > 0;

        public void Mute()
        {
            MediaPlayer.Mute = !MediaPlayer.Mute;
            NotifyOfPropertyChange(() => MediaPlayer);
        }

        private void MediaPlayerOnTimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            if(MediaPlayer == null) return;
            if (CanPlay is false)
            {
                MediaPlayer.Pause();
            }

            CurrentEpisodeTime = TimeSpan.FromMilliseconds(e.Time);
            NotifyOfPropertyChange(() => CanFastBackward);
            NotifyOfPropertyChange(() => CanFastForward);
            NotifyOfPropertyChange(() => CanPlay);
            NotifyOfPropertyChange(() => MediaPlayer);
        }

        public void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            if (MediaPlayer.IsPlaying is true) MediaPlayer.Pause();
            NotifyOfPropertyChange(() => MediaPlayer);
            NotifyOfPropertyChange(() => CanPlay);
        }

        public void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var slider = sender as Slider;
            CurrentEpisodeTime = TimeSpan.FromMilliseconds((long) slider.Value);
        }

        public void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            var slider = sender as Slider;
            TempCurrentEpisodeTime = (long) slider.Value;
            if (MediaPlayer.IsPlaying is false) MediaPlayer.Play();
            NotifyOfPropertyChange(() => MediaPlayer);
            NotifyOfPropertyChange(() => CanPlay);
        }

        #endregion

        #region Действия джампера

        

        /// <summary>
        ///     Добавить джампер
        /// </summary>
        public void AddJumper()
        {
            var startTime = Jumpers.Count == 0 ? new TimeSpan() : Jumpers.Last().EndTime + new TimeSpan(1000);
            var jumper = new Jumper
            {
                JumperMode = JumperModes.First(),
                Number = Jumpers.Count + 1,
                StartTime = startTime,
                EndTime = startTime + new TimeSpan(1000),
                Film = EEVM.ESVM.SelectedFilm,
                Season = EEVM.ESVM.SelectedSeason,
                Episode = EEVM.CurrentEpisode,
                AddressInfo = EEVM.SelectedAddressInfo
            };
            InsertEntityToDb(jumper);
            Jumpers = new BindableCollection<Jumper>(CurrentAddressInfo.Jumpers);
            SelectedJumper = Jumpers.LastOrDefault();
        }

        /// <summary>
        ///     Удалить джампер
        /// </summary>
        public void RemoveJumper()
        {
            if (CanRemoveJumper is false) return;
            for (var i = 0; i < SelectedJumpers.Count; i++)
            {
                DeleteEntityFromDb<Jumper>(SelectedJumpers[i].Id);
            }

            Jumpers = new BindableCollection<Jumper>(CurrentAddressInfo.Jumpers);
            NotifyChanges();
            
        }

        public bool CanRemoveJumper => SelectedJumpers.Count > 0;

        public void AddFirstJumper()
        {
            if(CanAddFirstJumper is false) return;
            if(int.TryParse(Clipboard.GetText(), out var time) is false) return;
            var jumper = new Jumper
            {
                JumperMode = JumperMode.Skip,
                Number = Jumpers.Count + 1,
                StartTime = new TimeSpan(),
                EndTime = TimeSpan.FromMilliseconds(time),
                Film = EEVM.ESVM.SelectedFilm,
                Season = EEVM.ESVM.SelectedSeason,
                Episode = EEVM.CurrentEpisode,
                AddressInfo = EEVM.SelectedAddressInfo
            };

            InsertEntityToDb(jumper);
            Jumpers = new BindableCollection<Jumper>(CurrentAddressInfo.Jumpers);
            RefreshJumpersConfig();
            NotifyOfPropertyChange(() => CanAddFirstJumper);
        }

        public bool CanAddFirstJumper => Clipboard.ContainsText() &&
                                         Jumpers.All(j =>
                                             j.StartTime == new TimeSpan() && j.JumperMode != JumperMode.Skip);

        public void AddVolumeJumper()
        {
            if(CanAddVolumeJumper is false) return;
            var jumper = new Jumper
            {
                JumperMode = JumperMode.LowerVolume,
                Number = Jumpers.Count + 1,
                StartTime = new TimeSpan(),
                StandardVolumeValue = MediaPlayer.Volume,
                Film = EEVM.ESVM.SelectedFilm,
                Season = EEVM.ESVM.SelectedSeason,
                Episode = EEVM.CurrentEpisode,
                AddressInfo = EEVM.SelectedAddressInfo
            };
            InsertEntityToDb(jumper);
            Jumpers = new BindableCollection<Jumper>(CurrentAddressInfo.Jumpers);
            RefreshJumpersConfig();
            NotifyOfPropertyChange(() => CanAddVolumeJumper);
        }

        public bool CanAddVolumeJumper => Jumpers.All(j => j.JumperMode != JumperMode.IncreaseVolume);

        public void RefreshJumpersConfig()
        {
            var jumpers = Jumpers.OrderBy(j =>
                {
                    if (j.StandardVolumeValue > 0)
                    {
                        if (j.StartTime == new TimeSpan())
                        {
                            return new TimeSpan();
                        }

                        return j.StartTime;
                    }

                    return j.StartTime + new TimeSpan(1L);
                }
            ).ToList();
            for (var i = 0; i < jumpers.Count(); i++)
            {
                var num = i + 1;
                jumpers[i].Number = num;
            }

            UpdateDbCollection<Jumper>(jumpers);
            Jumpers = new BindableCollection<Jumper>(CurrentAddressInfo.Jumpers);
        }

        public bool CanRefreshJumpersConfig => Jumpers.Count > 0;

        /// <summary>
        ///     Отменить выбор джампера
        /// </summary>
        public void CancelSelection()
        {
            if (CanCancelSelection is false) return;
            SelectedJumper = null;
        }

        public bool CanCancelSelection => SelectedJumpers.Count > 0;

        /// <summary>
        ///     Установить номер джампера
        /// </summary>
        public void SetJumperNumber()
        {
            if (CanSetJumperNumber is false) return;

            var (status, reason) = CheckJumperNumber(NewJumperNumber);

            WinMan.ShowDialog(new DialogViewModel(reason, DialogType.Info));
            if (status)
            {
                SelectedJumper.Number = NewJumperNumber ?? 0;
                UpdateDbCollection(SelectedJumper);
                JumperSnapshot = JsonConvert.SerializeObject(SelectedJumper);
                Jumpers = new BindableCollection<Jumper>(CurrentAddressInfo.Jumpers);
            }

            NotifyChanges();
        }

        public bool CanSetJumperNumber => SelectedJumper?.Number != NewJumperNumber;

        public void CbSelectionChanged(ComboBox sender, SelectionChangedEventArgs e)
        {
            SetJumperFieldsVisibility(e.AddedItems.Cast<JumperMode>().FirstOrDefault());
            NotifyChanges();
        }

        public void PasteStartTime()
        {
            if (CanPasteStartTime is false) return;
            if (int.TryParse(Clipboard.GetText(), out var time))
            {
                SelectedJumper.StartTime = TimeSpan.FromMilliseconds(time);
                NotifyOfPropertyChange(() => SelectedJumper);
                NotifyChanges();
            }
        }

        public bool CanPasteStartTime => Clipboard.ContainsText();

        public void PasteEndTime()
        {
            if (CanPasteEndTime is false) return;
            if (int.TryParse(Clipboard.GetText(), out var time))
            {
                SelectedJumper.EndTime = TimeSpan.FromMilliseconds(time);
                NotifyOfPropertyChange(() => SelectedJumper);
                NotifyChanges();
            }
        }

        public bool CanPasteEndTime => Clipboard.ContainsText();

        public void IncreaseVideoRate()
        {
            var rate = MediaPlayer.Rate;
            MediaPlayer.SetRate((float) (rate + 0.25));
        }

        public bool CanIncreaseVideoRate => CanPlay;

        public void DecreaseVideoRate()
        {
            var rate = MediaPlayer.Rate;
            MediaPlayer.SetRate((float) (rate - 0.25));
        }

        public bool CanDecreaseVideoRate => CanPlay;

        public void DefaultVideoRate()
        {
            MediaPlayer.SetRate(1);
        }

        public bool CanDefaultVideoRate => CanPlay;

        #endregion
    }
}