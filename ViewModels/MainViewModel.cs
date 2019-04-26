using Microsoft.WindowsAPICodePack.Dialogs;
using MusicProcessor.API;
using MusicProcessor.Commands;
using MusicProcessor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace MusicProcessor.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged, IProgressHandler
    {
        private readonly ShowDialogCommand _showFolderDialogCmd;
        private readonly ShowDialogCommand _showFreqFileSelectorCmd;
        private readonly ShowDialogCommand _saveProgramFileCmd;
        private readonly CreateProgramCommand _createProgramCmd;
        private readonly Config _config;

        private ISamplesLibrary _samplesLibrary;

        private int _progressValue = 0;
        private int _maxProgressValue = 100;

        private bool _programCreationStart;

        public MainViewModel(Config config, ISamplesLibrary samplesLibrary)
        {
            _config = config;
            _samplesLibrary = samplesLibrary;
            _samplesLibrary.Load(_config.LibraryFolder);
            _createProgramCmd = new CreateProgramCommand(this);

            _showFolderDialogCmd = new ShowDialogCommand(() => new CommonOpenFileDialog()
            {
                Title = "Выберите папку с библиотекой частот",
                IsFolderPicker = true,
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true
            }, (value) => LibraryFolder = value, () => !_programCreationStart);

            _showFreqFileSelectorCmd = new ShowDialogCommand(() => new CommonOpenFileDialog()
            {
                Title = "Выберите файл с частотной программой",
                IsFolderPicker = false,
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true
            }, (value) => FreqProblemFile = value, () => !_programCreationStart);
            _saveProgramFileCmd = new ShowDialogCommand(() => new CommonSaveFileDialog()
            {
                Title = "Выбрать место файл для сохранения программы",
                AddToMostRecentlyUsedList = false,
                EnsureFileExists = false,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                ShowPlacesList = true,
                DefaultExtension = "wav",
                DefaultFileName = "output.wav",
                AlwaysAppendDefaultExtension = true,
            }, (value) => SaveProgramFile = value, () => !_programCreationStart);
        }

        public ICommand ShowFolderDialogCmd => _showFolderDialogCmd;
        public ICommand ShowFreqFileSelectorCmd => _showFreqFileSelectorCmd;
        public ICommand SaveProgramFileCmd => _saveProgramFileCmd;
        public ICommand CreateProgramCmd => _createProgramCmd;

        public int SampleLength
        {
            get => _config.SampleLength;
            set => _config.SampleLength = value;
        }

        public ISamplesLibrary Library
        {
            get
            {
                return _samplesLibrary;
            }
        }

        public IEnumerable<Tuple<string, string>> LibraryEntries
        {
            get
            {
                return Library.Entries;
            }
        }

        public string LibraryFolder
        {
            get
            {
                return _config.LibraryFolder;
            }
            set
            {
                _config.LibraryFolder = value;
                _samplesLibrary.Load(_config.LibraryFolder);
                NotifyPropertyChanged("LibraryFolder");
                NotifyPropertyChanged("LibraryEntries");
            }
        }

        public string FreqProblemFile
        {
            get
            {
                return _config.FreqProblemFile;
            }
            set
            {
                _config.FreqProblemFile = value;
                NotifyPropertyChanged("FreqProblemFile");
            }
        }

        public string SaveProgramFile
        {
            get
            {
                return _config.SaveProgramFile;
            }
            set
            {
                _config.SaveProgramFile = value;
                NotifyPropertyChanged("SaveProgramFile");
            }
        }

        public int ProgressValue
        {
            get
            {
                return _progressValue;
            }
            set
            {
                _progressValue = value;
                NotifyPropertyChanged("ProgressValue");
            }
        }

        public int MaxProgressValue
        {
            get
            {
                return _maxProgressValue;
            }
            set
            {
                _maxProgressValue = value;
                NotifyPropertyChanged("MaxProgressValue");
            }
        }

        public bool ProgramCreationOver
        {
            get => !_programCreationStart;
        }

        public bool ProgramCreationStart
        {
            get => _programCreationStart; set
            {
                if (!value)
                {
                    _config.Save();
                }

                _programCreationStart = value;
                _showFreqFileSelectorCmd.RaiseCanExecuteChanged();
                _showFolderDialogCmd.RaiseCanExecuteChanged();
                _saveProgramFileCmd.RaiseCanExecuteChanged();
                NotifyPropertyChanged("ProgramCreationStart");
                NotifyPropertyChanged("ProgramCreationOver");
            }
        }

        protected void NotifyPropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public void Reset()
        {
            ProgressValue = 0;
        }

        public void SetMax(int value)
        {
            MaxProgressValue = value;
        }

        public void Report(int value)
        {
            ProgressValue = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;


    }
}
