using System.Windows;
using System.Windows.Input;
using System;

namespace CIV
{
    /// <summary>
    /// Shows the main window.
    /// </summary>
    public class DashboardShowCommand : CommandBase<DashboardShowCommand>
    {
        public override void Execute(object parameter)
        {

            Dashboard form = Application.Current.MainWindow as Dashboard;
            if (form != null)
            {

                if (form.IsVisible)
                    form.Hide();
                else
                {
                    form.Show();
                    form.WindowState = WindowState.Normal;
                    form.Activate();
                    form.RefreshAccount(false);
                    //form.InvalidateVisual();
                    //Size size = new Size(Application.Current.MainWindow.ActualWidth, Application.Current.MainWindow.ActualHeight);
                    //Application.Current.MainWindow.Measure(size);
                    //Application.Current.MainWindow.Arrange(new Rect(size));


                }

                form.ShowInTaskbar = form.IsVisible;
            }
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        //public event EventHandler CanExecuteChanged;
    }
}