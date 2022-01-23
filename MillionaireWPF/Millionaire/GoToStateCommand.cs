using System;
using System.Windows.Input;

namespace Millionaire
{
    public class GoToStateCommand : ICommand
    {
        private ControllerWindow _parent;

        public event EventHandler CanExecuteChanged;

        public GoToStateCommand(ControllerWindow parent)
        {
            _parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            int state = (int)parameter;
            _parent.GoToScene(state);
        }
    }
}
