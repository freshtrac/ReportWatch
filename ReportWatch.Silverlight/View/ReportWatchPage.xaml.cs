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


            PriceHistoryChart.AxesX[0].OnZoom += new EventHandler<AxisZoomEventArgs>(PriceHistoryChart_OnZoom);
            PercentChangeChart.AxesX[0].OnZoom += new EventHandler<AxisZoomEventArgs>(PercentChangeChart_OnZoom);

            PriceHistoryChart.AxesX[0].Scroll += new EventHandler<AxisScrollEventArgs>(PriceHistoryChart_Scroll);
            PercentChangeChart.AxesX[0].Scroll += new EventHandler<AxisScrollEventArgs>(PercentChangeChart_Scroll);
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button!=null)
            {
                ((ReportWatchViewModel)this.DataContext).SymbolName = button.CommandParameter.ToString();
            }
        }

        void PriceHistoryChart_Scroll(object sender, AxisScrollEventArgs e)
        {
            Axis axis = sender as Axis;
            PercentChangeChart.AxesX[0].ScrollBarOffset = axis.ScrollBarOffset;
        }

        void PercentChangeChart_Scroll(object sender, AxisScrollEventArgs e)
        {
            Axis axis = sender as Axis;
            PriceHistoryChart.AxesX[0].ScrollBarOffset = axis.ScrollBarOffset;
        }

        void PriceHistoryChart_OnZoom(object sender, AxisZoomEventArgs e)
        {
            Axis axis = sender as Axis;
            PercentChangeChart.AxesX[0].Zoom(e.MaxValue, e.MinValue);
            PercentChangeChart.AxesX[0].ScrollBarOffset = axis.ScrollBarOffset;
        }

        void PercentChangeChart_OnZoom(object sender, AxisZoomEventArgs e)
        {
            Axis axis = sender as Axis;
            PriceHistoryChart.AxesX[0].Zoom(e.MaxValue, e.MinValue);
            PriceHistoryChart.AxesX[0].ScrollBarOffset = axis.ScrollBarOffset;
        }

    }
}
