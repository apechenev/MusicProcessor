using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows;
using System.Windows.Input;

namespace MusicProcessor.Commands
{
    class ShowDialogCommand : ICommand
    {
        private readonly Func<CommonFileDialog> _setupAction;
        private readonly Action<string> _valueSetAction;
        private readonly Func<bool> _canExecute;

        public ShowDialogCommand(Func<CommonFileDialog> setupAction, Action<string> valueSetAction, Func<bool> canExecute)
        {
            _setupAction = setupAction;
            _valueSetAction = valueSetAction;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public void Execute(object parameter)
        {
            var dlg = _setupAction();
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                _valueSetAction(dlg.FileName);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            Application.Current.Dispatcher.Invoke(() => CanExecuteChanged?.Invoke(this, new EventArgs()));
        }
    }
}
