using MusicProcessor.API;
using MusicProcessor.Processor;
using MusicProcessor.ViewModels;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace MusicProcessor.Commands
{
    class CreateProgramCommand : ICommand
    {
        private readonly MainViewModel _mainViewModel;

        public event EventHandler CanExecuteChanged;


        public CreateProgramCommand(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return !_mainViewModel.ProgramCreationStart;
        }

        public void Execute(object parameter)
        {
            if (_mainViewModel.ProgramCreationStart)
            {
                return;
            }

            if (string.IsNullOrEmpty(_mainViewModel.LibraryFolder))
            {
                MessageBox.Show("Перед началом создания программы необходимо выбрать папку библиотеки частот.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (string.IsNullOrEmpty(_mainViewModel.FreqProblemFile))
            {
                MessageBox.Show("Перед началом создания программы необходимо выбрать исходный файл с частотной программой.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (string.IsNullOrEmpty(_mainViewModel.SaveProgramFile))
            {
                MessageBox.Show("Перед началом создания программы необходимо выбрать выходной файл с результатом обработки частотной программы.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (_mainViewModel.UseCustomSampleLength && _mainViewModel.SampleLength <= 0)
            {
                MessageBox.Show("Установленная длина сэмпла должна быть больше 0 мс.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _mainViewModel.ProgramCreationStart = true;
            RaiseOnCanExecuteChanged();

            Task.Run(() =>
            {
                try
                {
                    IDraftProblemParser draftProblemParser = new DraftProblemParser();
                    var result = draftProblemParser.Parse(_mainViewModel.FreqProblemFile, _mainViewModel.SampleLength);


                    StringBuilder sb = new StringBuilder();
                    foreach (var missedSample in _mainViewModel.Library.Validate(result))
                    {
                        sb.Append(missedSample).Append("; ");
                    }

                    if (sb.Length > 0)
                    {
                        MessageBox.Show("Следующие сэмплы не найдены в библиотеке сэмплов:\r\n\r\n" + sb.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    AudioPipeline audioPipeline = new AudioPipeline(_mainViewModel.Library, result, _mainViewModel, _mainViewModel.UseCustomSampleLength);
                    audioPipeline.Process(_mainViewModel.SaveProgramFile);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    _mainViewModel.ProgramCreationStart = false;
                    RaiseOnCanExecuteChanged();
                }
            });
        }

        private void RaiseOnCanExecuteChanged()
        {
            Application.Current.Dispatcher.Invoke(() => CanExecuteChanged?.Invoke(this, new EventArgs()));
        }
    }
}
