using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using ReportWatch.Database;
using System.Data.Services.Client;
using Visifire.Charts;
using System.Windows.Browser;
using Telerik.Windows.Controls;

namespace ReportWatch.Silverlight
{
    public partial class ReportWatchPage : UserControl
    {
        public ReportWatchPage()
        {
            InitializeComponent();
            this.DataContext = new ReportWatchViewModel();
            ((ReportWatchViewModel)this.DataContext).UseDispatcher(this.Dispatcher);
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button!=null)
            {
                ((ReportWatchViewModel)this.DataContext).SymbolName = button.CommandParameter.ToString();
            }
        }
    }
}
