using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Threading;

namespace ReportWatch.Silverlight
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Dispatcher _Dispatcher;

        public void UseDispatcher(Dispatcher dispatcher)
        {
            this._Dispatcher = dispatcher;
        }

        protected void NotifyPropertyChanged(String propertyName)
        {
            if (this._Dispatcher != null)
            {
                this._Dispatcher.BeginInvoke(delegate()
                {
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                });
            }
            else
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void ForcePropertyChanged(String propertyName)
        {
            NotifyPropertyChanged(propertyName);
        }

    }
}
