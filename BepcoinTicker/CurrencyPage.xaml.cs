using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace BepcoinTicker
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class CurrencyPage : Page
    {
        public Currency Currency;

        public CurrencyPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Currency = (Currency) e.Parameter;
            UpdateGraph();
        }

        private void UpdateGraph()
        {
            if (Currency == null || Currency.code == "") return;
            LineChart.DataContext = NbpApi
                .GetExchangesForSingleCurrencyInRange(DateFrom.Date.Date, DateTo.Date.Date, Currency.code).Select(exchange =>
                    new KeyValuePair<DateTime, double>(exchange.EffectiveDate, exchange.mid)).ToArray();

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage),"back");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void DateFrom_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            UpdateGraph();
        }

        private void DateTo_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            UpdateGraph();
        }
    }
}
