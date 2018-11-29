using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RGBHistogrammWPF
{
  public class DelegateAction : ICommand
  {
    private Action _action;

    public DelegateAction(Action action)
    {
      _action = action ?? throw new ArgumentNullException(nameof(action));
    }
   
    #region CanExecute

#   pragma warning disable 67 // interface implementation
    public event EventHandler CanExecuteChanged;
#   pragma warning restore 67

    public bool CanExecute(object parameter) => true;

    #endregion

    public void Execute(object parameter)
    {
      _action();
    }
  }
}
